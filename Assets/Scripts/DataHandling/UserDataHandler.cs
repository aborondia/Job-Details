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
    private Dictionary<string, Dictionary<string, User>> usersWithRoles = new Dictionary<string, Dictionary<string, User>>();
    public Dictionary<string, Dictionary<string, User>> UsersWithRoles => usersWithRoles;
    private List<User> unverifiedUsers = new List<User>();
    public List<User> UnverifiedUsers => unverifiedUsers;
    private bool rolesObtained = false;
    public bool RolesObtained => rolesObtained;
    public UnityEvent OnCurrentUserPopulatedEvent = new UnityEvent();
    public UnityEvent OnUsersPopulatedEvent = new UnityEvent();

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
        ActionHelper.ExecuteActionWhenTrue(() =>
        {
            SetCurrentUser();
        }, () =>
        {
            return this.rolesObtained;
        });
    }

    private void OnSettingRegularUser()
    {
        // regular user
    }

    private void OnSettingAdvancedUser()
    {
        PopulateUsers();
    }

    #endregion

    #region Getters/Setters

    public void PopulateUsers()
    {
        AppController.Active.ServerCommunicator.GetUsersWithRoles(response =>
        {
            this.usersWithRoles = JSONHelper.GetUsersWithRoles(response);

            AppController.Active.ServerCommunicator.GetUnverifiedUsers(response =>
            {
                this.unverifiedUsers = JSONHelper.GetUnverifiedUsers(response);
                this.OnUsersPopulatedEvent.Invoke();
            });
        });
    }

    private void SetCurrentUser()
    {
        RoleDTM currentUserRole = null;

        AppController.Active.ServerCommunicator.GetUserRole(response =>
        {
            currentUserRole = JSONHelper.GetRole(response);

            this.currentUser = new User(AppController.Active.ServerCommunicator.CurrentUser, currentUserRole);

            this.OnCurrentUserPopulatedEvent.Invoke();

            if (_UserRoleServerName == this.currentUser.RoleDTM.name)
            {
                OnSettingRegularUser();
            }
            else
            {
                OnSettingAdvancedUser();
            }

        }, AppController.Active.ServerCommunicator.CurrentUser.objectId);
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

        this.rolesObtained = true;
    }

    public void OnUserDeleted(User user)
    {
        if (user.DTM.verified)
        {
            this.usersWithRoles[user.RoleDTM.objectId].Remove(user.DTM.objectId);
        }
        else
        {
            this.unverifiedUsers.Remove(user);
        }
    }

    #endregion
}