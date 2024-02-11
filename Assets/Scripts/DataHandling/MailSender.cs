using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.Networking;
using sharpPDF;
using Newtonsoft.Json;


public class MailSender : MonoBehaviour
{
    [SerializeField] string appId;
    [SerializeField] string restKey;
    [SerializeField] private string from;
    [SerializeField] private string to;
    [SerializeField] private string subject;
    [SerializeField] private string body;
    private CustomMailMessage mailMessage;
    CustomMailAttachment attachment;
    Regex emailRegex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
    private const string apiUrl = "https://parseapi.back4app.com/functions/sendEmail";

    public void StartSendingEmail()
    {
        CreateEmail();
        StartCoroutine(SendEmail());
    }

    #region Sending

    private IEnumerator SendEmail()
    {
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        string jsonBody = JsonConvert.SerializeObject(this.mailMessage);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);

        Debug.Log(this.mailMessage.To);
        Debug.Log(this.mailMessage.Content);
        Debug.Log(jsonBody);

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

    #region Setup

    public void CreateEmail()
    {
        pdfDocument pdfDocument;
        MemoryStream memoryStream;
        byte[] fileBytes;

        pdfDocument = DocumentCreator.Active.GetDocument(new JobDetails.DetailsReport());
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
        Debug.LogError(value);
    }
}

[System.Serializable]
public class Personalization
{
    public Recipient[] to;
    public string subject;
}

[System.Serializable]
public class Recipient
{
    public string email;
}

[System.Serializable]
public class Content
{
    public string type;
    public string value;
}

[System.Serializable]
public class Attachment
{
    public string content;
    public string filename;
    public string type;
    public string disposition;
}

[System.Serializable]
public class SendGridEmail
{
    public Personalization[] personalizations;
    public Recipient from;
    public Content[] content;
    public Attachment[] attachments;
}