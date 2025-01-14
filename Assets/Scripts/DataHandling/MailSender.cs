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
        CreateEmail();
        AppController.Active.ServerCommunicator.SendEmail(this.mailMessage);
    }

    #region Setup

    public void CreateEmail()
    {
        pdfDocument pdfDocument;
        MemoryStream memoryStream;
        byte[] fileBytes;

        pdfDocument = DocumentCreator.Active.GetDocument(new DetailsReport(new DetailsReportDTM()));
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