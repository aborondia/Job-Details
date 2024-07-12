using System.Collections;
using System.Text;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using ReturnStringDelegate = ActionHelper.ReturnStringDelegate;
using System;
using System.Collections.Generic;
using ResponseDelegate = ActionHelper.ReturnStringDelegate;
public class ServerCommunicator : MonoBehaviour
{
    private string appId;
    public string AppId => appId;
    private string restKey;
    public string RestKey => restKey;
    private const string apiUrl = "https://parseapi.back4app.com";
    public string FunctionsUrl => $"{apiUrl}/functions";
    public string UsersUrl => $"{apiUrl}/users";
    public string ClassesUrl => $"{apiUrl}/classes";
    public string LoginUrl => $"{apiUrl}/login";
    public string LogoutUrl => $"{apiUrl}/logout";
    private bool signedIn;
    public bool SignedIn => signedIn;
    private UserDTM currentUser;
    public UserDTM CurrentUser => currentUser;
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
        });
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
            JSONNode node = JSON.Parse(request.downloadHandler.text);

            this.currentUser = JsonConvert.DeserializeObject<UserDTM>(request.downloadHandler.text);
            LogHelper.Active.Log("Login successful: " + request.downloadHandler.text);

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
        if (ReferenceEquals(this.currentUser, null) || !this.signedIn)
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
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);

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

    #region Job Details

    #region DetailsReport

    public void CreateDetailsReport(ResponseDelegate responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingDetailsReport(responseDelegate));
    }

    private IEnumerator StartCreatingDetailsReport(ResponseDelegate responseDelegate)
    {
        UnityWebRequest request = new UnityWebRequest($"https://parseapi.back4app.com/classes/DetailsReport", "POST");
        string jsonBody = $"{{\"createdBy\":\"{this.currentUser.objectId}\"}}";
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response: " + request.downloadHandler.text);

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

    public void GetDetailsReports(ResponseDelegate responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartGettingDetailsReports(responseDelegate));
    }

    private IEnumerator StartGettingDetailsReports(ResponseDelegate responseDelegate)
    {
        Dictionary<string, object> whereDict = new Dictionary<string, object>
        {
            { "createdBy", this.currentUser.objectId },
        };

        string whereJson = JsonUtility.ToJson(whereDict);
        string encodedWhere = UnityWebRequest.EscapeURL(whereJson);

        string url = $"{this.ClassesUrl}/DetailsReport?where={encodedWhere}";

        UnityWebRequest request = UnityWebRequest.Get(url);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response: " + request.downloadHandler.text);

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
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);

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

    public void CreateJobDetails(JobDetail jobDetails, ResponseDelegate responseDelegate = null)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingJobDetails(jobDetails, responseDelegate));
    }

    private IEnumerator StartCreatingJobDetails(JobDetail jobDetails, ResponseDelegate responseDelegate = null)
    {
        string url = $"https://parseapi.back4app.com/classes/JobDetail";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        JobDetailsDTM dtm = new JobDetailsDTM(this.currentUser.objectId, jobDetails, jobDetails.DetailsReportId);
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response: " + request.downloadHandler.text);
            responseDelegate?.Invoke(request.downloadHandler.text);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void GetJobDetails(string detailsReportObjectId, ResponseDelegate responseDelegate = null)
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartGettingJobDetails(detailsReportObjectId, responseDelegate));
    }

    private IEnumerator StartGettingJobDetails(string detailsReportObjectId, ResponseDelegate responseDelegate)
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
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response: " + request.downloadHandler.text);

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
        if (ReferenceEquals(this.currentUser, null))
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
        JobDetailsDTM dtm = new JobDetailsDTM(this.currentUser.objectId, jobDetails, detailsReportId);
        string jsonBody = JsonConvert.SerializeObject(dtm);
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        UnityWebRequest request = UnityWebRequest.Put(url, bodyRaw);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteJobDetails(string id)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartDeletingJobDetails(id));
    }

    private IEnumerator StartDeletingJobDetails(string id)
    {
        string url = $"{this.ClassesUrl}/JobDetail/{id}";
        UnityWebRequest request = UnityWebRequest.Delete(url);

        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);

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

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            LogHelper.Active.Log("Response: " + request.downloadHandler.text);
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
