using System.Collections;
using System.Text;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using ReturnStringDelegate = ActionHelper.StringDelegate;
using System;
using System.Collections.Generic;
using ResponseDelegateString = ActionHelper.StringDelegate;
using ResponseDelegateBool = ActionHelper.BoolDelegate;
using NUnit.Framework.Interfaces;
using Newtonsoft.Json.Linq;

public class ServerCommunicator : MonoBehaviour
{
    private const string apiUrl = "https://parseapi.back4app.com";
    // [SerializeField] private string appId;
    // public string AppId => appId;
    // [SerializeField] private string javaScriptKey;
    // public string JavaScriptKey => javaScriptKey;
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
    public UnityEvent OnSignInSuccessEvent;
    public UnityEvent OnSignInFailedEvent;
    public UnityEvent OnRegisterSuccessEvent;
    public UnityEvent OnRegisterFailedEvent;
    public UnityEvent OnRequestStartedEvent;
    public UnityEvent OnRequestCompletedEvent;

    private IEnumerator CallGetRolesWithUsersFunction()
    {
        UnityWebRequest request = new UnityWebRequest(this.FunctionsUrl + "/getRolesWithUsers", "POST");
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes("{}");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog($"Response (CallGetRolesWithUsersFunction):{request.downloadHandler.text}");
        }
        else
        {
            ShowFailureLog($"Request failed (CallGetRolesWithUsersFunction): {request.error}");
        }
    }

    #region Communication

    #region Users

    public void CreateUser(UserSignupDTM userSignupDTM)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingUser(userSignupDTM));
    }

    private IEnumerator StartCreatingUser(UserSignupDTM userSignupDTM)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.UsersUrl}", "POST");
        string jsonBody = JsonConvert.SerializeObject(userSignupDTM);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Revocable-Session", "1");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("User Created: " + request.downloadHandler.text);
            this.OnRegisterSuccessEvent.Invoke();
        }
        else
        {
            ShowFailureLog("Request failed: " + request.error);
            this.OnRegisterFailedEvent.Invoke();
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteUser(string id, Action successAction = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartDeletingUser(id, successAction));
    }

    private IEnumerator StartDeletingUser(string id, Action successAction)
    {
        string url = $"{this.ClassesUrl}/_User/{id}";

        UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Deleted: " + id);
            successAction?.Invoke();
        }
        else
        {
            ShowFailureLog("Request failed: " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateUser(User user, Action successAction = null)
    {
        if (ReferenceEquals(user, null))
        {
            ShowFailureLog("User is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartUpdatingUser(user, successAction));
    }

    private IEnumerator StartUpdatingUser(User user, Action successAction)
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog($"Response: (StartUpdatingUser) {request.downloadHandler.text}");
            successAction?.Invoke();
        }
        else
        {
            ShowFailureLog($"Request failed (StartUpdatingUser):  {request.error} {request.downloadHandler.text}");
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUserNameReferences(ReturnStringDelegate responseDelegate = null)
    {
        StartCoroutine(StartGettingUserNameReferences(responseDelegate));
    }

    private IEnumerator StartGettingUserNameReferences(ReturnStringDelegate responseDelegate)
    {
        string url = $"{this.ClassesUrl}/UserNameReference";

        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response:  (StartGettingUserNameReferences)" + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            ShowFailureLog("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void CreateUserNameReference(UserNameReferenceDTM currentUserReference)
    {
        StartCoroutine(StartCreatingUserNameReference(currentUserReference));
    }

    private IEnumerator StartCreatingUserNameReference(UserNameReferenceDTM currentUserReference)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.ClassesUrl}/UserNameReference", "POST");

        string jsonBody = JsonConvert.SerializeObject(currentUserReference);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.SetRequestHeader("Content-Type", "application/json");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartCreatingUserNameReference): " + request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartCreatingUserNameReference): " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUsersForRegularUser(ReturnStringDelegate responseDelegate)
    {
        StartCoroutine(StartGettingUsersForRegularUser(responseDelegate));
    }

    private IEnumerator StartGettingUsersForRegularUser(ReturnStringDelegate responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/getUsersForRegularUser", "POST");
        byte[] bodyRaw = new UTF8Encoding().GetBytes("{}");

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingUsersForRegularUser): " + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartGettingUsersForRegularUser): " + request.error);
        }
    }

    public void GetUsersForAdmin(ReturnStringDelegate responseDelegate)
    {
        StartCoroutine(StartGettingUsersForAdmin(responseDelegate));
    }

    private IEnumerator StartGettingUsersForAdmin(ReturnStringDelegate responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/getUsersForAdmin", "POST");
        byte[] bodyRaw = new UTF8Encoding().GetBytes("{}");

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingUsersWithRoles): " + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartGettingUsersWithRoles): " + request.error);
        }
    }

    public void SignIn(UserSignInDTM userSignInDTM)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartSigningIn(userSignInDTM));
    }

    private IEnumerator StartSigningIn(UserSignInDTM userSignInDTM)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", userSignInDTM.userName);
        form.AddField("password", userSignInDTM.password);

        UnityWebRequest request = UnityWebRequest.Post($"{this.FunctionsUrl}/userLogin", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Revocable-Session", "1");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog($"Response (StartSigningIn): {request.downloadHandler.text}");
            this.currentUserDTM = JSONHelper.GetUserDTM(request.downloadHandler.text);

            this.signedIn = true;
            this.OnSignInSuccessEvent.Invoke();
        }
        else
        {
            ShowFailureLog("Request failed (StartSigningIn): " + request.error);
            this.OnSignInFailedEvent.Invoke();
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetRole(string roleId, ReturnStringDelegate responseDelegate)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartGettingRole(roleId, responseDelegate));
    }

    private IEnumerator StartGettingRole(string roleId, ReturnStringDelegate responseDelegate)
    {
        WWWForm form = new WWWForm();
        form.AddField("objectId", roleId);
        UnityWebRequest request = UnityWebRequest.Post(this.FunctionsUrl + "/getRole", form);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            responseDelegate?.Invoke(request.downloadHandler.text);
            ShowSuccessLog("Response (GetRole): " + request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (GetRole): " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    private void SignOut()
    {
        if (ReferenceEquals(this.currentUserDTM, null) || !this.signedIn)
        {
            ShowFailureLog("Not signed in!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartSigningOut());
    }

    private IEnumerator StartSigningOut()
    {
        UnityWebRequest request = new UnityWebRequest(this.LogoutUrl, "POST");

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Logout successful");

            this.signedIn = false;
        }
        else
        {
            ShowFailureLog("Logout failed: " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #region Roles

    public void GetRoles(ReturnStringDelegate responseDelegate = null)
    {
        StartCoroutine(StartGettingRoles(responseDelegate));
    }

    private IEnumerator StartGettingRoles(ReturnStringDelegate responseDelegate)
    {
        string url = $"{this.ClassesUrl}/_Role";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);

        yield return request.SendWebRequest();

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
            ShowFailureLog("Request failed (StartGettingRoles): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUserRole(ReturnStringDelegate responseDelegate, string userObjectId)
    {
        StartCoroutine(StartGettingUserRole(responseDelegate, userObjectId));
    }

    private IEnumerator StartGettingUserRole(ReturnStringDelegate responseDelegate, string userObjectId)
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingUserRole): " + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartGettingUserRole): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void RemoveUserRole(RoleDTM dtm, Action successAction = null)
    {
        StartCoroutine(StartRemovingUserRole(dtm, successAction));
    }

    private IEnumerator StartRemovingUserRole(RoleDTM dtm, Action successAction)
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartUpdatingRole): " + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            ShowFailureLog("Request failed (StartUpdatingRole): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateRole(RoleUpdateDTM dtm, Action successAction = null)
    {
        StartCoroutine(StartUpdatingRole(dtm, successAction));
    }

    private IEnumerator StartUpdatingRole(RoleUpdateDTM dtm, Action successAction)
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartUpdatingRole): " + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            ShowFailureLog("Request failed (StartUpdatingRole): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void VerifyUser(string userId, Action successAction = null)
    {
        StartCoroutine(StartVerifyingUser(userId, successAction));
    }

    private IEnumerator StartVerifyingUser(string userId, Action successAction)
    {
        string url = $"{this.FunctionsUrl}/verifyUser";
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartVerifyingUser): " + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            ShowFailureLog("Request failed (StartVerifyingUser): " + request.error + request.downloadHandler.text);
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
            ShowFailureLog("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingDetailsReport(responseDelegate));
    }

    private IEnumerator StartCreatingDetailsReport(ResponseDelegateString responseDelegate)
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

        yield return request.SendWebRequest();

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
            ShowFailureLog("Request failed (StartCreatingDetailsReport): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetDetailsReports(ResponseDelegateString responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartGettingDetailsReports(responseDelegate));
    }

    private IEnumerator StartGettingDetailsReports(ResponseDelegateString responseDelegate)
    {
        string url = $"{this.FunctionsUrl}/retrieveDetailReports";
        WWWForm form = new WWWForm();
        form.AddField("createdBy", this.currentUserDTM.objectId);

        UnityWebRequest request = UnityWebRequest.Post(url, form);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartGettingDetailsReports): " + request.downloadHandler.text);

            responseDelegate?.Invoke(request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartGettingDetailsReports): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteDetailsReport(string id)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartDeletingDetailsReport(id));
    }

    private IEnumerator StartDeletingDetailsReport(string id)
    {
        string url = $"{this.FunctionsUrl}/deleteDetailReport";
        WWWForm form = new WWWForm();
        form.AddField("objectId", id);
        // string url = $"{this.ClassesUrl}/DetailsReport/{id}";

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        // UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Success (StartDeletingDetailsReport): " + request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartDeletingDetailsReport): " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #region JobDetails

    public void CreateJobDetails(JobDetail jobDetails, ResponseDelegateString responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            ShowFailureLog("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingJobDetails(jobDetails, responseDelegate));
    }

    private IEnumerator StartCreatingJobDetails(JobDetail jobDetails, ResponseDelegateString responseDelegate = null)
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartCreatingJobDetails): " + request.downloadHandler.text);
            responseDelegate?.Invoke(request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartCreatingJobDetails): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetJobDetails(string detailsReportObjectId, ResponseDelegateString responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartGettingJobDetails(detailsReportObjectId, responseDelegate));
    }

    private IEnumerator StartGettingJobDetails(string detailsReportObjectId, ResponseDelegateString responseDelegate)
    {
        string url = $"{this.ClassesUrl}/JobDetail";
        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

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
            ShowFailureLog("Request failed (StartGettingJobDetails): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateJobDetails(JobDetail jobDetails, ResponseDelegateString responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            ShowFailureLog("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartUpdatingJobDetails(jobDetails, responseDelegate));
    }

    private IEnumerator StartUpdatingJobDetails(JobDetail jobDetails, ResponseDelegateString responseDelegate)
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartUpdatingJobDetails): " + request.downloadHandler.text);

            responseDelegate?.Invoke(request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartUpdatingJobDetails): " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteJobDetails(string id, ResponseDelegateBool responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            ShowFailureLog("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartDeletingJobDetails(id, responseDelegate));
    }

    private IEnumerator StartDeletingJobDetails(string id, ResponseDelegateBool responseDelegate)
    {
        string url = $"{this.ClassesUrl}/JobDetail/{id}";
        UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", ServerConfiguration.AppId);
        request.SetRequestHeader("X-Parse-JavaScript-Key", ServerConfiguration.JavaScriptKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Success (StartDeletingJobDetails): " + id);

            responseDelegate?.Invoke(true);
        }
        else
        {
            ShowFailureLog("Request failed (StartDeletingJobDetails): " + request.error);
            responseDelegate?.Invoke(false);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #endregion

    #region Email

    public void SendEmail(CustomMailMessage customMailMessage)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartSendingEmail(customMailMessage));
    }

    private IEnumerator StartSendingEmail(CustomMailMessage customMailMessage)
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

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ShowSuccessLog("Response (StartSendingEmail): " + request.downloadHandler.text);
        }
        else
        {
            ShowFailureLog("Request failed (StartSendingEmail): " + request.error);
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

    private void ShowFailureLog(string message)
    {
        if (this.showFailureLogs)
        {
            LogHelper.Active.LogError(message);
        }
    }

    #endregion
}