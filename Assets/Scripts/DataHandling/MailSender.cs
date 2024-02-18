using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.Networking;
using sharpPDF;
using Newtonsoft.Json;
using SimpleJSON;
using System.Text;

public class MailSender : MonoBehaviour
{
    [SerializeField] private string from;
    [SerializeField] private string to;
    [SerializeField] private string subject;
    [SerializeField] private string body;
    private CustomMailMessage mailMessage;
    CustomMailAttachment attachment;
    Regex emailRegex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

    public void StartSendingEmail()
    {
        // CreateEmail();
        // StartCoroutine(SendEmail());
        // StartCoroutine(GetUniqueId());
    }

    #region Sending

    private IEnumerator GetUniqueId()
    {
        UnityWebRequest request = new UnityWebRequest($"{AppController.Active.ServerCommunicator.FunctionsUrl}/getUniqueId", "POST");
        string jsonBody = "{}";
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", AppController.Active.ServerCommunicator.AppId);
        request.SetRequestHeader("X-Parse-REST-API-Key", AppController.Active.ServerCommunicator.RestKey);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            JSONNode node = JSON.Parse(request.downloadHandler.text);
            LogHelper.Active.Log("Response: " + request.downloadHandler.text);

            StartCoroutine(CreateJobDetails(node["result"]["objectId"]));
        }
        else
        {
            LogHelper.Active.LogError("Request failed: " + request.error);
        }
    }

    public class Test
    {
        public string CustomId { get; set; }
        public string Content { get; set; }

        public Test(string customId, string content)
        {
            this.CustomId = customId;
            this.Content = content;
        }
    }

    private IEnumerator CreateJobDetails(string id)
    {
        UnityWebRequest request = new UnityWebRequest($"{AppController.Active.ServerCommunicator.FunctionsUrl}/uploadJobDetail", "POST");
        string jsonBody = JsonConvert.SerializeObject(new Test(id, "{data: test}"));
        byte[] bodyRaw = new UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", AppController.Active.ServerCommunicator.AppId);
        request.SetRequestHeader("X-Parse-REST-API-Key", AppController.Active.ServerCommunicator.RestKey);

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
    }

    private IEnumerator SendEmail()
    {
        UnityWebRequest request = new UnityWebRequest($"{AppController.Active.ServerCommunicator.FunctionsUrl}/sendEmail", "POST");
        string jsonBody = JsonConvert.SerializeObject(this.mailMessage);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        request.SetRequestHeader("X-Parse-Application-Id", AppController.Active.ServerCommunicator.AppId);
        request.SetRequestHeader("X-Parse-REST-API-Key", AppController.Active.ServerCommunicator.RestKey);

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
    }

    #endregion

    #region Setup

    public void CreateEmail()
    {
        pdfDocument pdfDocument;
        MemoryStream memoryStream;
        byte[] fileBytes;

        pdfDocument = DocumentCreator.Active.GetDocument(new JobDetails.DetailsReport(AppController.Active.ServerCommunicator.CurrentUser.objectId));
        memoryStream = new System.IO.MemoryStream();
        fileBytes = new byte[0];

        pdfDocument.createPDF(memoryStream, (BufferedStream bufferedStream) =>
        {
            fileBytes = new byte[bufferedStream.Length];
            bufferedStream.Read(fileBytes, 0, (int)bufferedStream.Length);
        });

        CreateAttachment(Convert.ToBase64String(fileBytes));
        this.mailMessage = new CustomMailMessage(this.to, this.from, this.subject, this.body, this.attachment);
    }

    public void CreateAttachment(string content)
    {
        this.attachment = new CustomMailAttachment(content, "JobDetails.pdf", "attachment/pdf", "attachment");
    }

    #endregion

    #region Validation

    private bool ValidateAll()
    {
        return ValidateMailMessage() && ValidateCredentials();
    }

    private bool ValidateCredentials()
    {
        if (!ValidateEmail())
        {
            return false;
        }

        return true;
    }

    private bool ValidateMailMessage()
    {
        if (ReferenceEquals(this.mailMessage, null))
        {
            return false;
        }

        return true;
    }

    private bool ValidateEmail()
    {
        if (!this.emailRegex.IsMatch(this.from))
        {
            DisplayError("The from email is invalid!");

            return false;
        }

        return true;
    }

    #endregion

    private void DisplayError(string value)
    {
        LogHelper.Active.LogError(value);
    }
}