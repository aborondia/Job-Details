using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
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
    private UserDTM currentUser;
    public UserDTM CurrentUser => currentUser;

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

        // CreateUser(new UserSignupDTM("Andrew", "aborondia@gmail.com", "123"));
    }

    #region Communication

    #region Users

    public void CreateUser(UserSignupDTM userSignupDTM)
    {
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
            Debug.Log("User Created: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
        }
    }

    public void SignIn(UserSignInDTM userSignInDTM)
    {
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
            Debug.Log("Login successful: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Login failed: " + request.error);
        }
    }

    private void SignOut()
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            Debug.LogError("Current user is null!");

            return;
        }
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
            Debug.Log("Logout successful");
        }
        else
        {
            Debug.LogError("Logout failed: " + request.error);
        }
    }

    #endregion

    #region Job Details

    public void CreateDetailsReport(DetailsReport detailsReport)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            Debug.LogError("Current user is null!");

            return;
        }

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
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }
    }

    public void UpdateDetailsReport(DetailsReport detailsReport)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            Debug.LogError("Current user is null!");

            return;
        }

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
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }
    }

    public void DeleteDetailsReport(DetailsReport detailsReport)
    {
        if (ReferenceEquals(this.currentUser, null))
        {
            Debug.LogError("Current user is null!");

            return;
        }

        StartCoroutine(StartDeletingDetailsReport(detailsReport));
    }

    private IEnumerator StartDeletingDetailsReport(DetailsReport detailsReport)
    {
        UnityWebRequest request = new UnityWebRequest($"{this.FunctionsUrl}/deleteJobDetail", "POST");
        string detailsData = JsonConvert.SerializeObject(detailsReport);
        string jsonBody = JsonConvert.SerializeObject(new JobDetailsDTM(this.currentUser.objectId, detailsData, detailsReport.ObjectId));
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);
        Debug.Log(jsonBody);
        request.SetRequestHeader("X-Parse-Application-Id", this.appId);
        request.SetRequestHeader("X-Parse-REST-API-Key", this.restKey);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Parse-Session-Token", this.currentUser.sessionToken);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }
    }

    private void RetrieveJobDetails()
    {
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
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error + request.downloadHandler.text);
        }
    }

    #endregion

    #region Email

    public void SendEmail(CustomMailMessage customMailMessage)
    {
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
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
        }
    }

    #endregion

    #endregion
}
