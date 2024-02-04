using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Threading;
using UnityEngine;
using System;
using System.Collections;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;
using sharpPDF;

public class MailSender : MonoBehaviour
{
    [SerializeField] private string smtpServer;
    [SerializeField] private string from;
    [SerializeField] private string to;
    [SerializeField] private string password;
    [SerializeField] private int port = 587;
    private MailMessage mailMessage;
    Attachment attachment;
    SmtpClient smtpClient;
    private string subject = "Job Details";
    private string body = "Job Details";
    private Thread sendMailThread;

    private void Awake()
    {
        Test();
    }

    public void Test()
    {
        this.from = "K-E-JobDetails@outlook.com";
        this.password = "19MadeByAJB85!";
        this.to = "aborondia@gmail.com";
        this.smtpServer = "smtp.outlook.com";
        SetupClient();
        CreateMailMessage();
        SetupMailMessage();
        SendEmail();
    }

    private void OnApplicationQuit()
    {
        StopMailThread();
    }

    private void OnDestroy()
    {
        StopMailThread();
    }

    #region Sending

    public void SendEmail()
    {
        if (!ValidateAll())
        {
            return;
        }

        if (!ReferenceEquals(this.sendMailThread, null))
        {
            Debug.Log("More than one email thread cannot be started!");

            return;
        }

        if (ReferenceEquals(this.mailMessage, null) || ReferenceEquals(this.smtpClient, null))
        {
            DisplayError("MailMessage and/or Client are null!");

            return;
        }

        this.sendMailThread = new Thread(SendMailThread);

        this.sendMailThread.Start();
    }

    private void SendMailThread()
    {
        pdfDocument pdfDocument;
        System.IO.MemoryStream memoryStream;
        ContentType contentType;

        if (ReferenceEquals(this.mailMessage, null) || ReferenceEquals(this.smtpClient, null))
        {
            return;
        }

        SetupMailMessage();

        pdfDocument = DocumentCreator.Active.CreateDocument(new JobDetails.DetailsReport());
        contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
        memoryStream = new System.IO.MemoryStream();

        pdfDocument.createPDF(memoryStream, (BufferedStream bufferedStream) =>
        {
            this.attachment = new System.Net.Mail.Attachment(bufferedStream, contentType);
            this.attachment.ContentDisposition.FileName = "JobDetails.pdf";

            this.mailMessage.Attachments.Add(this.attachment);
            smtpClient.Send(this.mailMessage);
        });

        memoryStream.Close();
    }

    #endregion

    #region Setup

    private void SetupClient()
    {
        if (!ValidateCredentials())
        {
            return;
        }

        this.smtpClient = new SmtpClient(this.smtpServer)
        {
            Port = this.port,
            Credentials = new NetworkCredential(this.from, this.password),
            EnableSsl = true,
        };
    }

    public void CreateMailMessage()
    {
        if (!ValidateCredentials())
        {
            return;
        }

        this.mailMessage = new MailMessage(this.from, this.to);
    }

    private void SetupMailMessage()
    {
        if (ReferenceEquals(this.mailMessage, null))
        {
            DisplayError("Cannot setup mail message!");

            return;
        }

        this.mailMessage.Subject = this.subject;
        this.mailMessage.Body = $"<span style='font-size: 12pt; color: black;'>{this.body}</span>";
    }

    public void SetSubject(string value)
    {
        if (ReferenceEquals(this.mailMessage, null))
        {
            DisplayError("MailMessage is null!");

            return;
        }

        this.subject = value;
    }

    public void SetBody(string value)
    {
        if (ReferenceEquals(this.mailMessage, null))
        {
            DisplayError("MailMessage is null!");

            return;
        }

        this.body = value;
    }

    public void SetAttachment(string filePath)
    {
        if (ReferenceEquals(this.mailMessage, null))
        {
            DisplayError("MailMessage is null!");

            return;
        }

        this.attachment = new Attachment(filePath, MediaTypeNames.Application.Octet);
    }

    #endregion

    #region Validation

    private bool ValidateAll()
    {
        return ValidateMailMessage() && ValidateCredentials() && !ReferenceEquals(this.smtpClient, null);
    }

    private bool ValidateCredentials()
    {
        if (!ValidateEmail())
        {
            return false;
        }

        if (!ValidatePassword())
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
        Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");

        if (!regex.IsMatch(this.from))
        {
            DisplayError("The from email is invalid!");

            return false;
        }

        return true;
    }

    private bool ValidatePassword()
    {
        if (String.IsNullOrEmpty(this.password))
        {
            DisplayError("Password cannot be empty!");

            return false;
        }

        return true;
    }

    #endregion

    private void StopMailThread()
    {
        if (!ReferenceEquals(this.sendMailThread, null))
        {
            this.sendMailThread.Abort();
        }
    }

    private void DisplayError(string value)
    {
        Debug.LogError(value);
    }
}