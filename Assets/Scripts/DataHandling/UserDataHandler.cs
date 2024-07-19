using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UserDataHandler : MonoBehaviour
{
    private User currentUser;
    public User CurrentUser => currentUser;
    private Dictionary<string, RoleDTM> roles = new Dictionary<string, RoleDTM>();
    public Dictionary<string, RoleDTM> Roles => roles;
    private Dictionary<string, User> users = new Dictionary<string, User>();
    public Dictionary<string, User> Users => users;
    private bool rolesObtained = false;
    public bool RolesObtained => rolesObtained;

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

    private void OnSignInComplete()
    {
        ActionHelper.ExecuteActionWhenTrue(() =>
        {
            foreach (var entry in this.roles)
            {
                AppController.Active.ServerCommunicator.GetUsersWithRoles(response =>
                    {
                        PopulateUsers(JSONHelper.GetUsers(response), entry.Value);
                    }, entry.Key);
            }
        }, () =>
        {
            return this.rolesObtained;
        });
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

    private void PopulateUsers(List<UserDTM> userDTMs, RoleDTM roleDTM)
    {
        foreach (UserDTM dtm in userDTMs)
        {
            if (!this.users.ContainsKey(dtm.objectId))
            {
                User newUser = new User(dtm, roleDTM);

                this.users.Add(dtm.objectId, newUser);

                if (dtm.objectId == AppController.Active.ServerCommunicator.CurrentUser.objectId)
                {
                    this.currentUser = newUser;
                }
            }
        }
    }

    public User GetCurrentUser()
    {
        string currentUserId = AppController.Active.ServerCommunicator.CurrentUser.objectId;

        if (this.users.ContainsKey(currentUserId))
        {
            return this.users[currentUserId];
        }
        else
        {
            return null;
        }
    }

    public User GetUser(string userObjectId)
    {
        if (this.users.ContainsKey(userObjectId))
        {
            return this.users[userObjectId];
        }
        else
        {
            return null;
        }
    }
}