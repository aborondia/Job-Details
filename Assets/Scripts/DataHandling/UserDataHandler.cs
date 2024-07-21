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
    private bool rolesObtained = false;
    public bool RolesObtained => rolesObtained;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            QueryController.Active.ChangeView(Enumerations.MainView.Users, Enumerations.Subview.Default);
        }
    }

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
       AppController.Active.ServerCommunicator.GetDetailsReports();
    }

    private void OnSettingAdvancedUser()
    {
        AppController.Active.ServerCommunicator.GetUsersWithRoles(response =>
        {
            this.usersWithRoles = JSONHelper.GetUsersWithRoles(response);
        });
    }

    #endregion

    #region Getters/Setters

    private void SetCurrentUser()
    {
        RoleDTM currentUserRole = null;

        AppController.Active.ServerCommunicator.GetUserRole(response =>
        {
            currentUserRole = JSONHelper.GetRole(response);

            this.currentUser = new User(AppController.Active.ServerCommunicator.CurrentUser, currentUserRole);

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

    // private void PopulateUsers(List<UserDTM> userDTMs, RoleDTM roleDTM)
    // {
    //     Debug.Log(userDTMs.Count);
    //     foreach (UserDTM dtm in userDTMs)
    //     {
    //         if (!this.usersWithRoles.ContainsKey(dtm.objectId))
    //         {
    //             User newUser = new User(dtm, roleDTM);
    //             Debug.Log($"{dtm.username} - {roleDTM.name} ");

    //             this.usersWithRoles.Add(dtm.objectId, newUser);

    //             if (dtm.objectId == AppController.Active.ServerCommunicator.CurrentUser.objectId)
    //             {
    //                 this.currentUser = newUser;
    //             }
    //         }
    //     }
    // }

    // public User GetCurrentUser()
    // {
    //     string currentUserId = AppController.Active.ServerCommunicator.CurrentUser.objectId;

    //     if (this.usersWithRoles.ContainsKey(currentUserId))
    //     {
    //         return this.usersWithRoles[currentUserId];
    //     }
    //     else
    //     {
    //         return null;
    //     }
    // }

    // public User GetUser(string userObjectId)
    // {
    //     if (this.usersWithRoles.ContainsKey(userObjectId))
    //     {
    //         return this.usersWithRoles[userObjectId];
    //     }
    //     else
    //     {
    //         return null;
    //     }
    // }

    #endregion
}