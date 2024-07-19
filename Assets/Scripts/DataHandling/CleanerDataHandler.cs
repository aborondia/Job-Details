using System.Collections.Generic;
using UnityEngine;

public class CleanerDataHandler : MonoBehaviour
{
    private Dictionary<string, UserNameReferenceDTM> userNameReferences = new Dictionary<string, UserNameReferenceDTM>();
    public Dictionary<string, UserNameReferenceDTM> UserNameReferences => userNameReferences;

    private void Awake()
    {
        AppController.Active.ServerCommunicator.OnSignInSuccessEvent.AddListener(() =>
        {
            AppController.Active.ServerCommunicator.GetUserNameReferences(response =>
            {
                List<UserNameReferenceDTM> userNameReferenceDTMs = JSONHelper.GetUserNameReferenceDTMs(response);
                PopulateUserNameReferences(userNameReferenceDTMs);
            });
        });
    }

    private void PopulateUserNameReferences(List<UserNameReferenceDTM> userNameReferenceDTMs)
    {
        bool currentUserInReferences = false;

        foreach (UserNameReferenceDTM userNameReferenceDTM in userNameReferenceDTMs)
        {
            if (userNameReferenceDTM.userObjectId == AppController.Active.ServerCommunicator.CurrentUser.objectId)
            {
                currentUserInReferences = true;
            }

            if (!this.userNameReferences.ContainsKey(userNameReferenceDTM.userObjectId))
            {
                this.userNameReferences.Add(userNameReferenceDTM.userObjectId, userNameReferenceDTM);
            }
        }

        if (!currentUserInReferences)
        {
            UserDTM currentUser = AppController.Active.ServerCommunicator.CurrentUser;
            UserNameReferenceDTM currentUserReference = new UserNameReferenceDTM();

            currentUserReference.userName = currentUser.username;
            currentUserReference.userObjectId = currentUser.objectId;

            if (!this.userNameReferences.ContainsKey(currentUser.objectId))
            {
                this.userNameReferences.Add(currentUser.objectId, currentUserReference);
                AppController.Active.ServerCommunicator.CreateUserNameReference(currentUserReference);
            }
        }
    }

    public UserNameReferenceDTM GetCurrentUserReference()
    {
        return GetUserReference(AppController.Active.ServerCommunicator.CurrentUser.objectId);
    }

    public UserNameReferenceDTM GetUserReference(string userObjectId)
    {
        if (this.userNameReferences.ContainsKey(userObjectId))
        {
            return this.userNameReferences[userObjectId];
        }
        else
        {
            return null;
        }
    }
}
