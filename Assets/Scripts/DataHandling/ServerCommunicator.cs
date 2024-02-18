using System.Collections;
using System.Text;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using ReturnStringDelegate = ActionHelper.ReturnStringDelegate;
using JobDetails;
using System;

public class ServerCommunicator : MonoBehaviour
{
    private string appId;
    public string AppId => appId;
    private string restKey;
    public string RestKey => restKey;
    private const string apiUrl = "https://parseapi.back4app.com";
    public string FunctionsUrl => $"{apiUrl}/functions";
    public string UsersUrl => $"{apiUrl}/users";
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

    public void CreateDetailsReport(DetailsReport detailsReport)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartCreatingDetailsReport(detailsReport));
    }

    private IEnumerator StartCreatingDetailsReport(DetailsReport detailsReport)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/updateJobDetail", "POST");
        string detailsData = JsonConvert.SerializeObject(detailsReport);
        string jsonBody = JsonConvert.SerializeObject(new JobDetailsDTM(this.currentUser.objectId, detailsData));
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
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void UpdateDetailsReport(DetailsReport detailsReport)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartUpdatingDetailsReport(detailsReport));
    }

    private IEnumerator StartUpdatingDetailsReport(DetailsReport detailsReport)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/updateJobDetail", "POST");
        string detailsData = JsonConvert.SerializeObject(detailsReport);
        string jsonBody = JsonConvert.SerializeObject(new JobDetailsDTM(this.currentUser.objectId, detailsData));
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
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

    public void DeleteDetailsReport(DetailsReport detailsReport)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            LogHelper.Active.LogError("Current user is null!");

            return;
        }

        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartDeletingDetailsReport(detailsReport));
    }

    private IEnumerator StartDeletingDetailsReport(DetailsReport detailsReport)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/deleteJobDetail", "POST");
        string detailsData = JsonConvert.SerializeObject(detailsReport);
        string jsonBody = JsonConvert.SerializeObject(new JobDetailsDTM(this.currentUser.objectId, detailsData, detailsReport.ObjectId));
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        LogHelper.Active.Log(jsonBody);
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

    private void RetrieveJobDetails()
    {
        this.OnRequestStartedEvent.Invoke();

        StartCoroutine(StartRetrievingJobDetails());
    }

    private IEnumerator StartRetrievingJobDetails()
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/retrieveJobDetails", "POST");
        string jsonBody = JsonConvert.SerializeObject(new JobDetailsDTM(this.currentUser.objectId, ""));
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
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }

        this.OnRequestCompletedEvent.Invoke();
    }

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
