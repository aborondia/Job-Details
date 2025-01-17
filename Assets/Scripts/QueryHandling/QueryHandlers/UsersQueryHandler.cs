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
    private List<Enumerations.UserRoleEnum> roleInputDropdownValues = new List<Enumerations.UserRoleEnum>();
    private UnityEvent onUserDataChange = new UnityEvent();

    protected override void Awake()
    {
        base.Awake();

        foreach (Enumerations.UserRoleEnum roleType in Enum.GetValues(typeof(Enumerations.UserRoleEnum)))
        {
            if (roleType == Enumerations.UserRoleEnum.Owner)
            {
                continue;
            }

            this.roleInputDropdownValues.Add(roleType);
        }
    }

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

        foreach (User user in AppController.Active.UserDataHandler.Users.Values)
        {
            AddUserRow(user);
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
        // CustomEnumField userTypeEnumField = baseElement.Q<CustomEnumField>();
        DropdownField userTypeDropdownField = baseElement.Q<DropdownField>();
        VisualElement userTypeLabelContainer = baseElement.Q<VisualElement>("user-role-label-container");
        Action onDataChangeAction = () => this.onUserDataChange.Invoke();
        nameLabel.text = $"{user.DTM.username} - {user.DTM.email}";

        SetupDeleteUserButton(deleteUserButton, user, onDataChangeAction);
        SetupUserRoleInput(userTypeDropdownField, userTypeLabelContainer, user, onDataChangeAction);
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

    private void SetupUserRoleInput(
        DropdownField userTypeDropdownField,
        VisualElement userTypeLabelContainer,
        User user,
        Action onDataChangeAction)
    {
        bool canChangeRoles;
        Enumerations.UserRoleEnum userRole;
        CustomLabel userTypeLabel;

        if (!user.DTM.verified)
        {
            VisualElementHelper.SetElementDisplay(userTypeDropdownField.parent, DisplayStyle.None);
            return;
        }

        switch (user.RoleDTM.name)
        {
            case UserDataHandler._AdminRoleServerName:
                userRole = Enumerations.UserRoleEnum.Admin;
                break;
            case UserDataHandler._OwnerRoleServerName:
                userRole = Enumerations.UserRoleEnum.Owner;
                break;
            case UserDataHandler._UserRoleServerName:
                userRole = Enumerations.UserRoleEnum.RegularUser;
                break;
            default:
                userRole = Enumerations.UserRoleEnum.RegularUser;
                break;
        }

        canChangeRoles = (int)AppController.Active.UserDataHandler.CurrentUser.RoleInHierarchy == (int)Enumerations.UserRoleEnum.Owner;

        if (canChangeRoles && user.RoleDTM.name != UserDataHandler._OwnerRoleServerName)
        {
            userTypeDropdownField.choices = this.roleInputDropdownValues.Select(role => role.ToString()).ToList();
            userTypeDropdownField.value = AppController.Active.UserDataHandler.GetRoleEnum(user.RoleDTM.name).ToString();
            VisualElementHelper.SetElementDisplay(userTypeLabelContainer, DisplayStyle.None);
            VisualElementHelper.SetElementDisplay(userTypeDropdownField, DisplayStyle.Flex);

            userTypeDropdownField.RegisterValueChangedCallback(evt =>
            {
                string newRoleId;
                string newRoleName = evt.newValue;
                RoleUpdateDTM roleUpdateDTM;
                RoleDTM newRoleDTM;

                switch (newRoleName)
                {
                    case UserDataHandler._AdminRoleServerName:
                        newRoleId = AppController.Active.UserDataHandler.Roles
                        .FirstOrDefault(roleEntry => String.Equals(roleEntry.Value.name, newRoleName)).Value.objectId;
                        break;
                    case UserDataHandler._UserRoleServerName:
                        newRoleId = AppController.Active.UserDataHandler.Roles
                        .FirstOrDefault(roleEntry => String.Equals(roleEntry.Value.name, newRoleName)).Value.objectId;
                        break;
                    default:
                        return;
                }

                if (user.RoleDTM.objectId == newRoleId)
                {
                    return;
                }

                roleUpdateDTM = new RoleUpdateDTM(user.DTM.objectId, newRoleId, user.RoleDTM.objectId);
                newRoleDTM = AppController.Active.UserDataHandler.GetRoleById(newRoleId);

                AppController.Active.ServerCommunicator.UpdateRole(roleUpdateDTM, () =>
                {
                    user.UpdateRole(newRoleDTM);
                });
            });
        }
        else
        {
            VisualElementHelper.SetElementDisplay(userTypeLabelContainer, DisplayStyle.Flex);
            VisualElementHelper.SetElementDisplay(userTypeDropdownField, DisplayStyle.None);

            userTypeLabel = userTypeLabelContainer.Q<CustomLabel>();
            userTypeLabel.text = userRole.ToString();
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
                AppController.Active.ServerCommunicator.VerifyUser(user.DTM.objectId, () =>
                {
                    user.DTM.verified = true;
                    user.UpdateRole(AppController.Active.UserDataHandler.GetRoleByName(UserDataHandler._UserRoleServerName));

                    onDataChangeAction?.Invoke();
                });
            });
        }
    }
}
