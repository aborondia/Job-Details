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
using System.Linq;

public class MailSender : MonoBehaviour
{
    [SerializeField] private string debugEmail;
    private CustomMailMessage mailMessage;
    CustomMailAttachment attachment;

    public void StartSendingEmail(DetailsReport detailsReport, User recipient, string body = "")
    {
        CreateEmail(detailsReport, recipient, body);
        AppController.Active.ServerCommunicator.SendEmail(this.mailMessage);
    }

    #region Setup

    // K-E-JobDetails@outlook.com
    // aborondia@gmail.com
    // Default Subject
    // Job details.
    public void CreateEmail(DetailsReport detailsReport, User recipient, string body)
    {
        pdfDocument pdfDocument;
        MemoryStream memoryStream;
        byte[] fileBytes;
        string from = AppController.Active.UserDataHandler.CurrentUser.DTM.email;
        string to = ReferenceEquals(recipient, null) ? this.debugEmail : recipient.DTM.email;
        DateTime startDate = detailsReport.Details.Values.Select(dr => dr.JobDate).Min();
        DateTime endDate = detailsReport.Details.Values.Select(dr => dr.JobDate).Max();
        string subject = $"{from} Details:  {startDate.ToShortDateString()} - {endDate.ToShortDateString()}";

        pdfDocument = DocumentCreator.Active.GetDocument(detailsReport);
        memoryStream = new MemoryStream();
        fileBytes = new byte[0];

        pdfDocument.createPDF(memoryStream, (BufferedStream bufferedStream) =>
        {
            fileBytes = new byte[bufferedStream.Length];
            bufferedStream.Read(fileBytes, 0, (int)bufferedStream.Length);
        });

        CreateAttachment(Convert.ToBase64String(fileBytes));

        if (String.IsNullOrEmpty(body))
        {
            body = "Job details";
        }

        this.mailMessage = new CustomMailMessage(to, from, subject, body, this.attachment);
    }

    public void CreateAttachment(string content)
    {
        this.attachment = new CustomMailAttachment(content, "JobDetails.pdf", "attachment/pdf", "attachment");
    }

    #endregion

    private void DisplayError(string value)
    {
        Debug.LogError(value);
    }
}