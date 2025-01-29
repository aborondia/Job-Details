using System.Collections;
using System.Text;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using ResponseDelegateString = ActionHelper.StringDelegate;
using ResponseDelegateBool = ActionHelper.BoolDelegate;
using Cysharp.Threading.Tasks;

public class ServerCommunicator : MonoBehaviour
{
    private const string apiUrl = "https://parseapi.back4app.com";
    [SerializeField] private bool showSuccessLogs = true;
    [SerializeField] private bool showFailureLogs = true;
    public string FunctionsUrl => $"{apiUrl}/functions";
    public string UsersUrl => $"{apiUrl}/users";
    public string RolesUrl => $"{apiUrl}/roles";
    public string ClassesUrl => $"{apiUrl}/classes";
    public string LoginUrl => $"{apiUrl}/login";
    public string LogoutUrl => $"{apiUrl}/logout";
    private bool signedIn;
    public bool SignedIn => signedIn;
    private int serverOperationsInProgress = 0;
    public bool ProcessingRequests => serverOperationsInProgress > 0;
    private UserDTM currentUserDTM;
    public UserDTM CurrentUserDTM => currentUserDTM;
    public UnityEvent OnLoginSuccessEvent;
    public UnityEvent OnLoginFailedEvent;
    public UnityEvent OnLogoutSuccessEvent;
    public UnityEvent OnLogoutFailedEvent;
    public UnityEvent OnRegisterSuccessEvent;
    public UnityEvent OnRegisterFailedEvent;
    public UnityEvent OnRequestStartedEvent;
    public UnityEvent OnRequestCompletedEvent;
    private int instanceId;

    private void Awake()
    {
        this.instanceId = this.GetInstanceID();

        this.OnRequestStartedEvent.AddListener(() =>
        {
            QueryController.Active.BlockInteractions(this.instanceId);
        });

        this.OnRequestCompletedEvent.AddListener(() =>
        {
            QueryController.Active.UnblockInteractions(this.instanceId);
        });
    }

    #region Communication

    #region Users

    public void CreateUser(UserSignupDTM userSignupDTM, ResponseDelegateBool responseDelegateBool)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCreatingUser(userSignupDTM, responseDelegateBool).Forget();
    }

    private async UniTaskVoid StartCreatingUser(UserSignupDTM userSignupDTM, ResponseDelegateBool responseDelegateBool)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest request;

        form.AddField("username", userSignupDTM.username);
        form.AddField("displayName", userSignupDTM.username);
        form.AddField("email", userSignupDTM.email);
        form.AddField("password", userSignupDTM.password);
        form.AddField("verificationUrlBase", apiUrl);

        request = UnityWebRequest.Post($"{this.FunctionsUrl}/registerUser", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartCreatingUser): " + request.downloadHandler.text);
            responseDelegateBool?.Invoke(true);
            this.OnRegisterSuccessEvent.Invoke();
        }
        else
        {
            OnFailure(request, "(StartCreatingUser)");
            responseDelegateBool?.Invoke(false);
            this.OnRegisterFailedEvent.Invoke();
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void CheckRegistrationCredentials(string displayName, string email, ResponseDelegateString returnStringDelegate)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCheckRegistrationCredentials(displayName, email, returnStringDelegate).Forget();
    }

    private async UniTaskVoid StartCheckRegistrationCredentials(string displayName, string email, ResponseDelegateString returnStringDelegate)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", displayName);
        form.AddField("email", email);

        UnityWebRequest request = UnityWebRequest.Post($"{this.FunctionsUrl}/checkRegistrationCredentials", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Revocable-Session", "1");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog($"Response (StartCheckRegistrationCredentials): {request.downloadHandler.text}");

            returnStringDelegate?.Invoke(request.downloadHandler.text);
        }
        else
        {
            OnFailure(request, "(StartCheckRegistrationCredentials)");
            this.OnLoginFailedEvent.Invoke();
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteUser(string id, Action successAction = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartDeletingUser(id, successAction).Forget();
    }

    private async UniTaskVoid StartDeletingUser(string id, Action successAction)
    {
        string url = $"{this.ClassesUrl}/_User/{id}";

        UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartDeletingUser): " + id);
            successAction?.Invoke();
        }
        else
        {
            OnFailure(request, "(StartDeletingUser)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateUser(User user, Action successAction = null)
    {
        if (ReferenceEquals(user, null))
        {
            OnFailure();

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartUpdatingUser(user, successAction).Forget();
    }

    private async UniTaskVoid StartUpdatingUser(User user, Action successAction)
    {
        UserDTM dtm = user.DTM;
        string url = $"{this.ClassesUrl}/_User/{user.DTM.objectId}";
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        UnityWebRequest request = UnityWebRequest.Put(url, bodyRaw);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog($"Response: (StartUpdatingUser) {request.downloadHandler.text}");
            successAction?.Invoke();
        }
        else
        {
            OnFailure(request, "(StartUpdatingUser)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUsersForRegularUser(ResponseDelegateString responseDelegate)
    {
        this.OnRequestStartedEvent.Invoke();

        StartGettingUsersForRegularUser(responseDelegate).Forget();
    }

    private async UniTaskVoid StartGettingUsersForRegularUser(ResponseDelegateString responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/getUsersForRegularUser", "POST");
        byte[] bodyRaw = new UTF8Encoding().GetBytes("{}");

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingUsersForRegularUser): " + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            OnFailure(request, "(StartGettingUsersForRegularUser)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUsersForAdmin(ResponseDelegateString responseDelegate)
    {
        this.OnRequestStartedEvent.Invoke();

        StartGettingUsersForAdmin(responseDelegate).Forget();
    }

    private async UniTaskVoid StartGettingUsersForAdmin(ResponseDelegateString responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/getUsersForAdmin", "POST");
        byte[] bodyRaw = new UTF8Encoding().GetBytes("{}");

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingUsersWithRoles): " + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            OnFailure(request, "(StartGettingUsersWithRoles)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void LogIn(UserSignInDTM userSignInDTM)
    {
        this.OnRequestStartedEvent.Invoke();

        StartLoggingIn(userSignInDTM).Forget();
    }

    private async UniTaskVoid StartLoggingIn(UserSignInDTM userSignInDTM)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", userSignInDTM.userName);
        form.AddField("password", userSignInDTM.password);

        UnityWebRequest request = UnityWebRequest.Post($"{this.FunctionsUrl}/userLogin", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Revocable-Session", "1");

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog($"Response (StartSigningIn): {request.downloadHandler.text}");
            this.currentUserDTM = JSONHelper.GetUserDTM(request.downloadHandler.text);

            this.signedIn = true;
            this.OnLoginSuccessEvent.Invoke();
        }
        else
        {
            OnFailure(request, "(StartSigningIn)");

            this.OnLoginFailedEvent.Invoke();
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetRole(string roleId, ResponseDelegateString responseDelegate)
    {
        this.OnRequestStartedEvent.Invoke();

        StartGettingRole(roleId, responseDelegate).Forget();
    }

    private async UniTaskVoid StartGettingRole(string roleId, ResponseDelegateString responseDelegate)
    {
        WWWForm form = new WWWForm();
        form.AddField("objectId", roleId);
        UnityWebRequest request = UnityWebRequest.Post(this.FunctionsUrl + "/getRole", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            responseDelegate?.Invoke(request.downloadHandler.text);
            ShowSuccessLog("Response (GetRole): " + request.downloadHandler.text);
        }
        else
        {
            OnFailure(request, "(GetRole)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void LogOut(ResponseDelegateBool responseDelegateBool = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null) || !this.signedIn)
        {
            OnFailure();
            responseDelegateBool?.Invoke(false);

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartLoggingOut(responseDelegateBool).Forget();
    }

    private async UniTaskVoid StartLoggingOut(ResponseDelegateBool responseDelegateBool)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/userLogout", "POST");
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog($"Response (StartLoggingOut): Sign out successfull");

            responseDelegateBool?.Invoke(true);
            this.currentUserDTM = null;
            this.signedIn = false;

            this.OnLogoutSuccessEvent.Invoke();
        }
        else
        {
            responseDelegateBool?.Invoke(false);
            OnFailure(request, "(StartLoggingOut)");
            this.OnLogoutFailedEvent.Invoke();
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #region Recovery

    public void SendForgottenUsername(string email, ResponseDelegateBool responseDelegateBool = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartSendingForgottenUsername(email, responseDelegateBool).Forget();
    }

    private async UniTaskVoid StartSendingForgottenUsername(string email, ResponseDelegateBool responseDelegateBool)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest request;

        form.AddField("email", email);

        request = UnityWebRequest.Post($"{this.FunctionsUrl}/forgotUsername", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartSendingForgottenUsername): " + request.downloadHandler.text);
            responseDelegateBool?.Invoke(true);
        }
        else
        {
            OnFailure(request, "(StartSendingForgottenUsername)");
            responseDelegateBool?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void SendForgotPasswordRequest(string email, ResponseDelegateBool responseDelegateBool = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartSendingForgotPasswordRequest(email, responseDelegateBool).Forget();
    }

    private async UniTaskVoid StartSendingForgotPasswordRequest(string email, ResponseDelegateBool responseDelegateBool)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest request;

        form.AddField("email", email);

        request = UnityWebRequest.Post($"{this.FunctionsUrl}/forgotPassword", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartSendingPasswordReset): " + request.downloadHandler.text);
            responseDelegateBool?.Invoke(true);
        }
        else
        {
            OnFailure(request, "(StartSendingPasswordReset)");
            responseDelegateBool?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void SendPasswordResetRequest(string code, string password, ResponseDelegateBool responseDelegateBool = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartSendingResetPasswordRequest(code, password, responseDelegateBool).Forget();
    }

    private async UniTaskVoid StartSendingResetPasswordRequest(string code, string password, ResponseDelegateBool responseDelegateBool)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest request;

        form.AddField("code", code);
        form.AddField("newPassword", password);

        request = UnityWebRequest.Post($"{this.FunctionsUrl}/resetPassword", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartSendingResetPasswordRequest): " + request.downloadHandler.text);
            responseDelegateBool?.Invoke(true);
        }
        else
        {
            OnFailure(request, "(StartSendingResetPasswordRequest)");
            responseDelegateBool?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #region Roles

    public void GetRoles(ResponseDelegateString responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartGettingRoles(responseDelegate).Forget();
    }

    private async UniTaskVoid StartGettingRoles(ResponseDelegateString responseDelegate)
    {
        string url = $"{this.ClassesUrl}/_Role";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingRoles): " + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            OnFailure(request, "(StartGettingRoles)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUserRole(ResponseDelegateString responseDelegate, string userObjectId)
    {
        this.OnRequestStartedEvent.Invoke();

        StartGettingUserRole(responseDelegate, userObjectId).Forget();
    }

    private async UniTaskVoid StartGettingUserRole(ResponseDelegateString responseDelegate, string userObjectId)
    {
        UnityWebRequest request;
        Dictionary<string, object> whereDict = new Dictionary<string, object>
        {
            {
                "users", new Dictionary<string, string>
                {
                    { "__type", "Pointer" },
                    { "className", "_User" },
                    { "objectId", userObjectId }
                }
            }
        };
        string whereJson = JsonConvert.SerializeObject(whereDict);
        string encodedWhere = UnityWebRequest.EscapeURL(whereJson);
        string url = $"{this.ClassesUrl}/_Role?where={encodedWhere}";

        request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingUserRole): " + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            OnFailure(request, "(StartGettingUserRole)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void RemoveUserRole(RoleDTM dtm, Action successAction = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartRemovingUserRole(dtm, successAction).Forget();
    }

    private async UniTaskVoid StartRemovingUserRole(RoleDTM dtm, Action successAction)
    {
        string url = $"{this.RolesUrl}/{dtm.objectId}";
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        UnityWebRequest request = UnityWebRequest.Put(url, bodyRaw);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartUpdatingRole): " + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            OnFailure(request, "(StartUpdatingRole)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateRole(RoleUpdateDTM dtm, Action successAction = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartUpdatingRole(dtm, successAction).Forget();
    }

    private async UniTaskVoid StartUpdatingRole(RoleUpdateDTM dtm, Action successAction)
    {
        string url = $"{this.FunctionsUrl}/updateUserRole";
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartUpdatingRole): " + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            OnFailure(request, "(StartUpdatingRole)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void VerifyUser(string userId, Action successAction = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartVerifyingUser(userId, successAction).Forget();
    }

    private async UniTaskVoid StartVerifyingUser(string userId, Action successAction)
    {
        string url = $"{this.FunctionsUrl}/verifyUser";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartVerifyingUser): " + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            OnFailure(request, "(StartVerifyingUser)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void VerifyRegistration(string confirmationCode, Action successAction = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartVerifyingRegistration(confirmationCode, successAction).Forget();
    }

    private async UniTaskVoid StartVerifyingRegistration(string confirmationCode, Action successAction)
    {
        string url = $"{this.FunctionsUrl}/verifyRegistration";
        WWWForm form = new WWWForm();
        form.AddField("token", confirmationCode);
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartVerifyingUser): " + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            OnFailure(request, "(StartVerifyingUser)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #region Job Details

    #region DetailsReport

    public void CreateDetailsReport(ResponseDelegateString responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            OnFailure();

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCreatingDetailsReport(responseDelegate).Forget();
    }

    private async UniTaskVoid StartCreatingDetailsReport(ResponseDelegateString responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/createDetailsReport", "POST");
        string jsonBody = $"{{\"createdBy\":\"{this.currentUserDTM.objectId}\"}}";
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartCreatingDetailsReport): " + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            OnFailure(request, "Request failed (StartCreatingDetailsReport)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetDetailsReports(ResponseDelegateString responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartGettingDetailsReports(responseDelegate).Forget();
    }

    private async UniTaskVoid StartGettingDetailsReports(ResponseDelegateString responseDelegate)
    {
        string url = $"{this.FunctionsUrl}/retrieveDetailReports";
        WWWForm form = new WWWForm();
        form.AddField("createdBy", this.currentUserDTM.objectId);

        UnityWebRequest request = UnityWebRequest.Post(url, form);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingDetailsReports): " + request.downloadHandler.text);

            responseDelegate?.Invoke(request.downloadHandler.text);
        }
        else
        {
            OnFailure(request, "(StartGettingDetailsReports)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteDetailsReport(string id, ResponseDelegateBool responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartDeletingDetailsReport(id, responseDelegate).Forget();
    }

    private async UniTaskVoid StartDeletingDetailsReport(string id, ResponseDelegateBool responseDelegate)
    {
        string url = $"{this.FunctionsUrl}/deleteDetailReport";
        WWWForm form = new WWWForm();
        form.AddField("objectId", id);

        UnityWebRequest request = UnityWebRequest.Post(url, form);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Success (StartDeletingDetailsReport): " + request.downloadHandler.text);
            responseDelegate?.Invoke(true);
        }
        else
        {
            OnFailure(request, "(StartDeletingDetailsReport)");
            responseDelegate?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #region JobDetails

    public void CreateJobDetails(JobDetail jobDetails, ResponseDelegateBool responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            OnFailure();

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCreatingJobDetails(jobDetails, responseDelegate).Forget();
    }

    private async UniTaskVoid StartCreatingJobDetails(JobDetail jobDetails, ResponseDelegateBool responseDelegate = null)
    {
        string url = $"{this.FunctionsUrl}/createJobDetail";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        JobDetailsDTM dtm = new JobDetailsDTM(this.currentUserDTM.objectId, jobDetails);
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartCreatingJobDetails): " + request.downloadHandler.text);
            responseDelegate?.Invoke(true);
        }
        else
        {
            OnFailure(request, "(StartCreatingJobDetails)");
            responseDelegate?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetJobDetails(string detailsReportObjectId, ResponseDelegateString responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartGettingJobDetails(detailsReportObjectId, responseDelegate).Forget();
    }

    private async UniTaskVoid StartGettingJobDetails(string detailsReportObjectId, ResponseDelegateString responseDelegate)
    {
        string url = $"{this.ClassesUrl}/JobDetail";
        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingJobDetails): " + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            OnFailure(request, "(StartGettingJobDetails)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateJobDetails(JobDetail jobDetails, ResponseDelegateBool responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            OnFailure();

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartUpdatingJobDetails(jobDetails, responseDelegate).Forget();
    }

    private async UniTaskVoid StartUpdatingJobDetails(JobDetail jobDetails, ResponseDelegateBool responseDelegate)
    {
        string url = $"{this.ClassesUrl}/JobDetail/{jobDetails.ObjectId}";
        JobDetailsDTM dtm = new JobDetailsDTM(this.currentUserDTM.objectId, jobDetails);
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        UnityWebRequest request = UnityWebRequest.Put(url, bodyRaw);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartUpdatingJobDetails): " + request.downloadHandler.text);

            responseDelegate?.Invoke(true);
        }
        else
        {
            OnFailure(request, "(StartUpdatingJobDetails)");
            responseDelegate?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteJobDetails(string id, ResponseDelegateBool responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            OnFailure();

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartDeletingJobDetails(id, responseDelegate).Forget();
    }

    private async UniTaskVoid StartDeletingJobDetails(string id, ResponseDelegateBool responseDelegate)
    {
        string url = $"{this.ClassesUrl}/JobDetail/{id}";
        UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Success (StartDeletingJobDetails): " + id);

            responseDelegate?.Invoke(true);
        }
        else
        {
            OnFailure(request, "(StartDeletingJobDetails");
            responseDelegate?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #endregion

    #region Email

    public void SendEmail(CustomMailMessage customMailMessage, ResponseDelegateBool responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartSendingEmail(customMailMessage, responseDelegate).Forget();
    }

    private async UniTaskVoid StartSendingEmail(CustomMailMessage customMailMessage, ResponseDelegateBool responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/sendEmail", "POST");
        string jsonBody = JsonConvert.SerializeObject(customMailMessage);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", AppController.Active.ServerCommunicator.CurrentUserDTM.sessionToken);
        request.SetRequestHeader("Content-Type", "application/json");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            responseDelegate?.Invoke(true);
            ShowSuccessLog("Response (StartSendingEmail): " + request.downloadHandler.text);
        }
        else
        {
            responseDelegate?.Invoke(false);
            OnFailure(request, "(StartSendingEmail)");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #endregion

    #region Debug

    private void ShowSuccessLog(string message)
    {
        if (this.showSuccessLogs)
        {
            LogHelper.Active.Log(message);
        }
    }

    private void OnFailure(UnityWebRequest request, string requestName = "")
    {
        RequestErrorDTM requestErrorDTM = JSONHelper.GetRequestErrorDTM(request.downloadHandler.text);

        if (ReferenceEquals(request.downloadHandler, null))
        {
            QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(() =>
            {
                QueryController.Active.ChangeView(Enumerations.MainView.Login, Enumerations.Subview.Login_EnterCredentials);
            }, request.error
            , false);
        }
        else
        {
            requestErrorDTM = JSONHelper.GetRequestErrorDTM(request.downloadHandler.text);

            switch (requestErrorDTM.code)
            {
                case 209:
                    QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(() =>
                    {
                        QueryController.Active.ChangeView(Enumerations.MainView.Login, Enumerations.Subview.Login_EnterCredentials);
                    }, "Your session is no longer valid. Please login again."
                    , false);
                    break;
                default:
                    QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(() =>
                    {
                        QueryController.Active.ChangeView(Enumerations.MainView.Login, Enumerations.Subview.Login_EnterCredentials);
                    }, requestErrorDTM.error
                    , false);
                    break;
            }
        }

        if (this.showFailureLogs)
        {
            ShowFailureLog($"Request Failed {requestName}: {request.error}");
        }
    }

    private void OnFailure()
    {
        QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(() =>
        {
            QueryController.Active.ChangeView(Enumerations.MainView.Login, Enumerations.Subview.Login_EnterCredentials);
        }, "Something went wrong. Please login again.");
    }

    private void ShowFailureLog(string message)
    {
        if (this.showFailureLogs)
        {
            LogHelper.Active.LogError(message);
        }
    }

    #endregion
}