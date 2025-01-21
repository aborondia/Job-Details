using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UserDataHandler : MonoBehaviour
{
    public const string _UserRoleServerName = "RegularUser";
    public const string _AdminRoleServerName = "Admin";
    public const string _OwnerRoleServerName = "Owner";
    private User currentUser;
    public User CurrentUser => currentUser;
    private Dictionary<string, RoleDTM> roles = new Dictionary<string, RoleDTM>();
    public Dictionary<string, RoleDTM> Roles => roles;
    private Dictionary<string, User> users = new Dictionary<string, User>();
    public Dictionary<string, User> Users => users;
    private bool rolesObtained = false;
    public bool RolesObtained => rolesObtained;
    public UnityEvent OnCurrentUserPopulatedEvent = new UnityEvent();
    public UnityEvent OnUsersPopulatedEvent = new UnityEvent();
    public UnityEvent OnRolesPopulatedEvent = new UnityEvent();

    #region Initialization

    private void Start()
    {
        AppController.Active.ServerCommunicator.GetRoles(response =>
        {
            PopulateRoles(JSONHelper.GetRoles(response));
        });

        AppController.Active.ServerCommunicator.OnSignInSuccessEvent.AddListener(() =>
        {
            OnSignInComplete();
        });
    }

    #endregion

    #region Results

    private void OnSignInComplete()
    {
        ActionHelper.ExecuteActionWhenTrue(() => SetCurrentUser(), () => this.rolesObtained);
    }

    private void OnSettingUser()
    {
        PopulateUsers();
    }

    #endregion

    #region Getters/Setters

    public RoleDTM GetRoleById(string roleId)
    {
        if (!this.roles.ContainsKey(roleId))
        {
            return null;
        }

        return this.roles[roleId];
    }

    public RoleDTM GetRoleByName(string roleName)
    {
        return this.roles.Values.FirstOrDefault(role => role.name == roleName);
    }

    public Enumerations.UserRoleEnum GetRoleEnum(string roleName)
    {
        switch (roleName)
        {
            case UserDataHandler._AdminRoleServerName:
                return Enumerations.UserRoleEnum.Admin;
            case UserDataHandler._OwnerRoleServerName:
                return Enumerations.UserRoleEnum.Owner;
            default:
                return Enumerations.UserRoleEnum.RegularUser;
        }
    }

    public void PopulateUsers()
    {
        if (this.currentUser.RoleDTM.name == _UserRoleServerName)
        {
            AppController.Active.ServerCommunicator.GetUsersForRegularUser(response =>
            {
                this.users = JSONHelper.GetUsers(response);
                this.OnUsersPopulatedEvent.Invoke();
            });
        }
        else
        {
            AppController.Active.ServerCommunicator.GetUsersForAdmin(response =>
            {
                this.users = JSONHelper.GetUsers(response);
                this.OnUsersPopulatedEvent.Invoke();
            });
        }
    }

    private void SetCurrentUser()
    {
        UserDTM currentUserDTM = AppController.Active.ServerCommunicator.CurrentUserDTM;
        RoleDTM userRole;

        if (!currentUserDTM.verified)
        {
            QueryController.Active.ChangeView(Enumerations.MainView.Login, Enumerations.Subview.Login_UnregisteredUserLogin);

            return;
        }

        userRole = this.roles[currentUserDTM.roleId];
        this.currentUser = new User(currentUserDTM, userRole);

        this.OnCurrentUserPopulatedEvent.Invoke();

        OnSettingUser();
    }

    private void PopulateRoles(List<RoleDTM> roleDTMs)
    {
        foreach (RoleDTM dtm in roleDTMs)
        {
            if (!this.roles.ContainsKey(dtm.objectId))
            {
                this.roles.Add(dtm.objectId, dtm);
            }
        }

        this.OnRolesPopulatedEvent.Invoke();
        this.rolesObtained = true;
    }

    public void OnUserDeleted(User user)
    {
        this.users.Remove(user.DTM.username);
    }

    #endregion
}