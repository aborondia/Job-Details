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

        foreach (var entry in AppController.Active.UserDataHandler.UsersWithRoles)
        {
            foreach (User user in entry.Value.Values)
            {
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
        CustomButton verifyUserButton = baseElement.Q<CustomButton>();
        VisualElement userTypeContainer = baseElement.Q<VisualElement>("user-type-container");
        CustomEnumField userTypeEnumField = baseElement.Q<CustomEnumField>();
        Action onDataChangeAction = () => this.onUserDataChange.Invoke();
        nameLabel.text = $"{user.DTM.username} - {user.DTM.email}";

        if (user.RoleDTM.name == "Owner")
        {
            deleteUserButton.style.display = DisplayStyle.None;
            userTypeEnumField.style.display = DisplayStyle.None;
        }
        else
        {
            deleteUserButton.RegisterCallback<ClickEvent>(evt =>
            {
                AppController.Active.ServerCommunicator.DeleteUser(user.DTM.objectId, onDataChangeAction);
            });

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
                            .FirstOrDefault(role => String.Equals(role.Value.name, roleName)).Value.name;
                            break;
                        case Enumerations.UserRoleEnum.User:
                            roleName = UserDataHandler._UserRoleServerName;
                            newRoleId = AppController.Active.UserDataHandler.Roles
                            .FirstOrDefault(role => String.Equals(role.Value.name, roleName)).Value.name;
                            break;
                        default:
                            return;
                    }

                    if (String.IsNullOrEmpty(roleName) || user.RoleDTM.objectId == newRoleId)
                    {
                        return;
                    }

                    user.RoleDTM.objectId = newRoleId;
                    user.RoleDTM.name = roleName;
                    // user.DTM.role.__op = "RemoveRelation";

                    AppController.Active.ServerCommunicator.UpdateUser(user, () =>
                    {
                        // user.DTM.role = new RoleRelation(newRoleId);
                        AppController.Active.ServerCommunicator.UpdateUser(user, onDataChangeAction);
                    });

                });
            }
            else
            {
                userTypeEnumField.style.display = DisplayStyle.None;
            }
        }

        if (user.DTM.verified)
        {
            verifyUserButtonContainer.style.display = DisplayStyle.None;
        }
        else
        {
            verifyUserButton.RegisterCallback<ClickEvent>(evt =>
            {
                RoleDTM roleDTM = new RoleDTM();

                roleDTM.name = UserDataHandler._UserRoleServerName;
                roleDTM.objectId = AppController.Active.UserDataHandler.Roles
                .First(entry => entry.Value.name == roleDTM.name).Key;
                roleDTM.users = new RelationOperation(new List<string> { user.DTM.objectId });
                user.DTM.verified = true;

                AppController.Active.ServerCommunicator.UpdateRole(roleDTM, onDataChangeAction);
                AppController.Active.ServerCommunicator.UpdateUser(user, onDataChangeAction);
            });
        }
    }
}
