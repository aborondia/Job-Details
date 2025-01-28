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

    public void StartSendingEmail(DetailsReport detailsReport, string recipient, string body = "")
    {
        CreateEmail(detailsReport, recipient, body);
        AppController.Active.ServerCommunicator.SendEmail(this.mailMessage, success =>
        {
            string responseMessage;

            if (success)
            {
                responseMessage = "Email sent successfully.";
            }
            else
            {
                responseMessage = "Email could not be sent.";
            }

            QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null, responseMessage);
        });
    }

    #region Setup

    public void CreateEmail(DetailsReport detailsReport, string recipient, string body)
    {
        pdfDocument pdfDocument;
        MemoryStream memoryStream;
        byte[] fileBytes;
        string from = AppController.Active.UserDataHandler.CurrentUser.DTM.email;
        string to = String.IsNullOrEmpty(recipient) ? this.debugEmail : recipient;
        DateTime startDate = detailsReport.Details.Values.Select(dr => dr.JobDate).Min();
        DateTime endDate = detailsReport.Details.Values.Select(dr => dr.JobDate).Max();
        string subject = $"Details:  {startDate.ToShortDateString()} - {endDate.ToShortDateString()} ({from})";

        pdfDocument = DocumentCreator.Active.GetDocument(detailsReport);
        memoryStream = new MemoryStream();
        fileBytes = new byte[0];

        pdfDocument.createPDF(memoryStream, (BufferedStream bufferedStream) =>
        {
            fileBytes = new byte[bufferedStream.Length];
            bufferedStream.Read(fileBytes, 0, (int)bufferedStream.Length);
        });

        CreateAttachment(Convert.ToBase64String(fileBytes));

        if (String.IsNullOrWhiteSpace(body))
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
}