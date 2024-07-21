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
    private string appId;
    public string AppId => appId;
    private string restKey;
    public string RestKey => restKey;
    private const string apiUrl = "https://parseapi.back4app.com";
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
    public UserDTM CurrentUser => currentUserDTM;
    public UnityEvent OnSignInSuccessEvent;
    public UnityEvent OnSignInFailedEvent;
    public UnityEvent OnRegisterSuccessEvent;
    public UnityEvent OnRegisterFailedEvent;
    public UnityEvent OnRequestStartedEvent;
    public UnityEvent OnRequestCompletedEvent;

    private void Awake()
    {
        if (String.IsNullOrEmpty(this.appId))
        {
            this.appId = PlayerPrefs.GetString("AppId");
        }
        if (String.IsNullOrEmpty(this.restKey))
        {
            this.restKey = PlayerPrefs.GetString("RestKey");
        }

        this.OnSignInSuccessEvent.AddListener(() =>
        {
            // CreateJobDetails(new JobDetail(), "ANVUxDjPru");
            // CreateDetailsReport();
            // GetJobDetails("bY7VbX6fIP");
            // GetDetailsReports();
            // DeleteDetailsReport("bnfHqJVG5d");
            // UpdateJobDetails("8KRnJSC9BK", "bY7VbX6fIP", new JobDetail());
            // DeleteJobDetails("eKsFih6wE5");
            StartCoroutine(CallGetRolesWithUsersFunction());
        });
    }

    private IEnumerator CallGetRolesWithUsersFunction()
    {
        UnityWebRequest request = new UnityWebRequest(this.FunctionsUrl + "/getRolesWithUsers", "POST");
        request.SetRequestHeader("X-Parse-Application-Id", appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes("{}");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
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

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Revocable-Session", "1");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("User Created: " + request.downloadHandler.text);
            this.OnRegisterSuccessEvent.Invoke();
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error);
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

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Deleted: " + id);
            successAction?.Invoke();
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateUser(User user, Action successAction = null)
    {
        if (ReferenceEquals(user, null))
        {
            LogHelper.Active.LogError("User is null!");

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

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartUpdatingUser)" + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
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

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartGettingUserNameReferences)" + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
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

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.SetRequestHeader("Content-Type", "application/json");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response (StartCreatingUserNameReference): " + request.downloadHandler.text);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUsers(ReturnStringDelegate responseDelegate = null)
    {
        StartCoroutine(StartGettingUsers(responseDelegate));
    }

    private IEnumerator StartGettingUsers(ReturnStringDelegate responseDelegate)
    {
        string url = $"{this.ClassesUrl}/_User";

        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartGettingUsers)" + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetUsersWithRoles(ReturnStringDelegate responseDelegate)
    {
        StartCoroutine(StartGettingUsersWithRoles(responseDelegate));
    }

    private IEnumerator StartGettingUsersWithRoles(ReturnStringDelegate responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/getRolesWithUsers", "POST");
        byte[] bodyRaw = new UTF8Encoding().GetBytes("{}");

        request.SetRequestHeader("X-Parse-Application-Id", appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response (StartGettingUsersWithRoles): " + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
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

        UnityWebRequest request = UnityWebRequest.Post(this.LoginUrl, form);
        request.SetRequestHeader("X-Parse-Application-Id", appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", restKey);
        request.SetRequestHeader("X-Parse-Revocable-Session", "1");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            this.currentUserDTM = JsonConvert.DeserializeObject<UserDTM>(request.downloadHandler.text);
            LogHelper.Active.Log("Response {StartSigningIn}: " + request.downloadHandler.text);

            this.signedIn = true;

            this.OnSignInSuccessEvent.Invoke();
        }
        else
        {
            LogHelper.Active.LogError("Login failed: " + request.error);
            this.OnSignInFailedEvent.Invoke();
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    private void SignOut()
    {
        if (ReferenceEquals(this.currentUserDTM, null) || !this.signedIn)
        {
            LogHelper.Active.LogError("Not signed in!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartSigningOut());
    }

    private IEnumerator StartSigningOut()
    {
        UnityWebRequest request = new UnityWebRequest(this.LogoutUrl, "POST");

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Logout successful");

            this.signedIn = false;
        }
        else
        {
            LogHelper.Active.LogError("Logout failed: " + request.error);
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

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartGettingRoles)" + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
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
        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartGettingUserRole)" + request.downloadHandler.text);

            responseDelegate.Invoke(request.downloadHandler.text);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }



    public void UpdateRole(RoleDTM dtm, Action successAction = null)
    {
        StartCoroutine(StartUpdatingRole(dtm, successAction));
    }

    private IEnumerator StartUpdatingRole(RoleDTM dtm, Action successAction)
    {
        string url = $"{this.RolesUrl}/{dtm.objectId}";
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        UnityWebRequest request = UnityWebRequest.Put(url, bodyRaw);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartUpdatingRole)" + request.downloadHandler.text);
            successAction?.Invoke();
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
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
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingDetailsReport(responseDelegate));
    }

    private IEnumerator StartCreatingDetailsReport(ResponseDelegateString responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"https://parseapi.back4app.com/classes/DetailsReport", "POST");
        string jsonBody = $"{{\"createdBy\":\"{this.currentUserDTM.objectId}\"}}";
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartCreatingDetailsReport)" + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
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
        string url;

        if (AppController.Active.UserDataHandler.CurrentUser.RoleDTM.name == UserDataHandler._UserRoleServerName)
        {
            Dictionary<string, object> whereDict = new Dictionary<string, object>
        {
            { "createdBy", this.currentUserDTM.objectId },
        };

            string whereJson = JsonUtility.ToJson(whereDict);
            string encodedWhere = UnityWebRequest.EscapeURL(whereJson);

            url = $"{this.ClassesUrl}/DetailsReport?where={encodedWhere}";
        }
        else
        {
            url = $"{this.ClassesUrl}/DetailsReport";
        }

        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartGettingDetailsReports)" + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
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
        string url = $"{this.ClassesUrl}/DetailsReport/{id}";

        UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Deleted: " + id);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #region JobDetails

    public void CreateJobDetails(JobDetail jobDetails, ResponseDelegateString responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingJobDetails(jobDetails, responseDelegate));
    }

    private IEnumerator StartCreatingJobDetails(JobDetail jobDetails, ResponseDelegateString responseDelegate = null)
    {
        string url = $"https://parseapi.back4app.com/classes/JobDetail";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        JobDetailsDTM dtm = new JobDetailsDTM(this.currentUserDTM.objectId, jobDetails, jobDetails.DetailsReportId);
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartCreatingJobDetails)" + request.downloadHandler.text);
            responseDelegate?.Invoke(request.downloadHandler.text);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
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
        Dictionary<string, object> whereDict = new Dictionary<string, object>
        {
            { "jsonFile", new { __type = "File", name = "" } },
            { "createdBy", "" },
            { "content", "" },
            { "reportPointer", new { __type = "Pointer", className = "DetailsReport", objectId = detailsReportObjectId } }
        };
        string whereJson = JsonUtility.ToJson(whereDict);
        string encodedWhere = UnityWebRequest.EscapeURL(whereJson);
        string url = $"{this.ClassesUrl}/JobDetail?where={encodedWhere}";
        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartGettingJobDetails)" + request.downloadHandler.text);

            if (!ReferenceEquals(responseDelegate, null))
            {
                responseDelegate.Invoke(request.downloadHandler.text);
            }
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateJobDetails(string jobDetailsId, string detailsReportId, JobDetail jobDetails)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartUpdatingJobDetails(jobDetailsId, detailsReportId, jobDetails));
    }

    private IEnumerator StartUpdatingJobDetails(string jobDetailsId, string detailsReportId, JobDetail jobDetails)
    {
        string url = $"{this.ClassesUrl}/JobDetail/{jobDetailsId}";
        JobDetailsDTM dtm = new JobDetailsDTM(this.currentUserDTM.objectId, jobDetails, detailsReportId);
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        UnityWebRequest request = UnityWebRequest.Put(url, bodyRaw);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartUpdatingJobDetails)" + request.downloadHandler.text);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteJobDetails(string id, ResponseDelegateBool responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUserDTM, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartDeletingJobDetails(id, responseDelegate));
    }

    private IEnumerator StartDeletingJobDetails(string id, ResponseDelegateBool responseDelegate)
    {
        string url = $"{this.ClassesUrl}/JobDetail/{id}";
        UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUserDTM.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Deleted: " + id);

            responseDelegate?.Invoke(true);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error);
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
        UnityWebRequest request = new UnityWebRequest($"{apiUrl}/sendEmail", "POST");
        string jsonBody = JsonConvert.SerializeObject(customMailMessage);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        // request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response:  (StartSendingEmail)" + request.downloadHandler.text);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    #endregion

    #endregion
}