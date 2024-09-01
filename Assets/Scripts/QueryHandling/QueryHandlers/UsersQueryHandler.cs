using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UsersQueryHandler : QueryHandler
{
    [SerializeField] private VisualTreeAsset userRowBase;
    ScrollView usersScrollView;
    VisualElement usersScrollViewContentContainer;
    private UnityEvent onUserDataChange = new UnityEvent();

    protected override void InitializeElements()
    {
        usersScrollView = this.parentElement.Q<ScrollView>();
        this.usersScrollViewContentContainer = this.usersScrollView.contentContainer;

        this.onUserDataChange.AddListener(() => RefreshUsersList());
        AppController.Active.UserDataHandler.OnUsersPopulatedEvent.AddListener(() =>
        {
            if (QueryController.Active.CurrentMainView == Enumerations.MainView.Users)
            {
                RefreshUsersList();
            }
        });
    }

    protected override void SetupButtons()
    {
    }

    protected override void SetupInputs()
    {
    }

    protected override void SetViewElements()
    {
    }

    protected override void OnAnyViewChanged()
    {
        base.OnAnyViewChanged();

        if (QueryController.Active.CurrentMainView == this.mainView)
        {
            RefreshUsersList();
        }
    }

    private void RefreshUsersList()
    {
        this.usersScrollViewContentContainer.Clear();

        foreach (User user in AppController.Active.UserDataHandler.UnverifiedUsers)
        {
            AddUserRow(user);
        }

        foreach (var entry in AppController.Active.UserDataHandler.UsersWithRoles)
        {
            foreach (User user in entry.Value.Values)
            {
                if (user.RoleDTM.name == UserDataHandler._OwnerRoleServerName)
                {
                    continue;
                }

                AddUserRow(user);
            }
        }
    }

    private void AddUserRow(User user)
    {
        TemplateContainer baseElement = this.userRowBase.Instantiate();
        VisualElement nameLabelContainer = baseElement.Q<VisualElement>("name-label-container");
        Label nameLabel = nameLabelContainer.Q<Label>();
        VisualElement deleteUserButtonContainer = baseElement.Q<VisualElement>("delete-user-button-container");
        CustomButton deleteUserButton = baseElement.Q<CustomButton>();
        VisualElement verifyUserButtonContainer = baseElement.Q<VisualElement>("verify-user-button-container");
        CustomButton verifyUserButton = verifyUserButtonContainer.Q<CustomButton>();
        VisualElement userTypeContainer = baseElement.Q<VisualElement>("user-type-container");
        CustomEnumField userTypeEnumField = baseElement.Q<CustomEnumField>();
        Action onDataChangeAction = () => this.onUserDataChange.Invoke();
        nameLabel.text = $"{user.DTM.username} - {user.DTM.email}";

        SetupDeleteUserButton(deleteUserButton, user, onDataChangeAction);
        SetupUserRoleInput(userTypeEnumField, user, onDataChangeAction);
        SetupVerifyUserButton(verifyUserButtonContainer, verifyUserButton, user, onDataChangeAction);

        this.usersScrollViewContentContainer.Add(baseElement);
    }

    private void SetupDeleteUserButton(CustomButton deleteUserButton, User user, Action onDataChangeAction)
    {
        bool canDeleteUser;

        switch (user.RoleDTM.name)
        {
            case UserDataHandler._AdminRoleServerName:
                if (AppController.Active.UserDataHandler.CurrentUser.RoleDTM.name == UserDataHandler._OwnerRoleServerName)
                {
                    deleteUserButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
                    canDeleteUser = true;
                }
                else
                {
                    VisualElementHelper.SetElementDisplay(deleteUserButton.parent, DisplayStyle.None);
                    canDeleteUser = false;
                }
                break;
            case UserDataHandler._OwnerRoleServerName:
                VisualElementHelper.SetElementDisplay(deleteUserButton.parent, DisplayStyle.None);
                canDeleteUser = false;
                break;
            case UserDataHandler._UserRoleServerName:
                deleteUserButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
                canDeleteUser = true;
                break;
            default:
                deleteUserButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
                canDeleteUser = true;
                break;
        }

        if (canDeleteUser)
        {
            deleteUserButton.RegisterCallback<ClickEvent>(evt =>
            {
                onDataChangeAction += () => AppController.Active.UserDataHandler.PopulateUsers();
                AppController.Active.ServerCommunicator.DeleteUser(user.DTM.objectId, onDataChangeAction);
            });
        }
    }

    private void SetupUserRoleInput(CustomEnumField userTypeEnumField, User user, Action onDataChangeAction)
    {
        bool canChangeRoles;

        if (!user.DTM.verified)
        {
            VisualElementHelper.SetElementDisplay(userTypeEnumField.parent, DisplayStyle.None);

            return;
        }

        canChangeRoles = AppController.Active.UserDataHandler.CurrentUser.RoleDTM.name == UserDataHandler._OwnerRoleServerName;

        switch (user.RoleDTM?.name)
        {
            case UserDataHandler._AdminRoleServerName:
                userTypeEnumField.value = Enumerations.UserRoleEnum.Admin;
                userTypeEnumField.SetEnabled(true);
                break;
            case UserDataHandler._UserRoleServerName:
                userTypeEnumField.value = Enumerations.UserRoleEnum.User;
                userTypeEnumField.SetEnabled(true);
                break;
            default:
                userTypeEnumField.value = Enumerations.UserRoleEnum.User;
                userTypeEnumField.SetEnabled(true);
                return;
        }

        userTypeEnumField.SetEnabled(canChangeRoles);

        if (!canChangeRoles)
        {
            return;
        }

        if (user.DTM.verified)
        {
            userTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                string newRoleId;
                string roleName;

                switch (evt.newValue)
                {
                    case Enumerations.UserRoleEnum.Admin:
                        roleName = UserDataHandler._AdminRoleServerName;
                        newRoleId = AppController.Active.UserDataHandler.Roles
                        .FirstOrDefault(role => String.Equals(role.Value.name, roleName)).Value.objectId;
                        break;
                    case Enumerations.UserRoleEnum.User:
                        roleName = UserDataHandler._UserRoleServerName;
                        newRoleId = AppController.Active.UserDataHandler.Roles
                        .FirstOrDefault(role => String.Equals(role.Value.name, roleName)).Value.objectId;
                        break;
                    default:
                        return;
                }

                if (String.IsNullOrEmpty(roleName) || user.RoleDTM.objectId == newRoleId)
                {
                    return;
                }

                RoleDTM oldRoleDTM = new RoleDTM();
                RoleDTM newRoleDTM = new RoleDTM();

                oldRoleDTM.name = user.RoleDTM.name;
                oldRoleDTM.objectId = user.RoleDTM.objectId;
                oldRoleDTM.users = new RelationOperation(new List<string> { user.DTM.objectId }, RelationOperation.RelationOperations.Remove);

                newRoleDTM.name = roleName;
                newRoleDTM.objectId = newRoleId;
                newRoleDTM.users = new RelationOperation(new List<string> { user.DTM.objectId }, RelationOperation.RelationOperations.Add);

                AppController.Active.ServerCommunicator.RemoveUserRole(oldRoleDTM, () =>
                {
                    AppController.Active.ServerCommunicator.UpdateRole(newRoleDTM, () =>
                    {
                        user.UpdateRole(newRoleDTM);
                        AppController.Active.ServerCommunicator.UpdateUser(user, onDataChangeAction);
                    });
                });
            });
        }
        else
        {
            VisualElementHelper.SetElementDisplay(userTypeEnumField, DisplayStyle.None);
        }
    }

    private void SetupVerifyUserButton(
    VisualElement verifyUserButtonContainer,
    CustomButton verifyUserButton,
    User user,
    Action onDataChangeAction)
    {
        if (user.DTM.verified)
        {
            VisualElementHelper.SetElementDisplay(verifyUserButtonContainer, DisplayStyle.None);
        }
        else
        {
            VisualElementHelper.SetElementDisplay(verifyUserButtonContainer, DisplayStyle.Flex);

            verifyUserButton.RegisterCallback<ClickEvent>(evt =>
            {
                RoleDTM roleDTM = new RoleDTM();

                roleDTM.name = UserDataHandler._UserRoleServerName;
                roleDTM.objectId = AppController.Active.UserDataHandler.Roles
                .First(entry => entry.Value.name == roleDTM.name).Key;
                roleDTM.users = new RelationOperation(new List<string> { user.DTM.objectId }, RelationOperation.RelationOperations.Add);
                user.DTM.verified = true;

                AppController.Active.ServerCommunicator.UpdateRole(roleDTM, () =>
                {
                    user.UpdateRole(roleDTM);
                    AppController.Active.ServerCommunicator.UpdateUser(user, onDataChangeAction);
                });
            });
        }
    }
}
