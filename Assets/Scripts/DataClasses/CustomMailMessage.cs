public class CustomMailMessage
{
    public string To { get; set; }
    public string From { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Content { get; set; }
    public string FileName { get; set; }
    public string Type { get; set; }
    public string Disposition { get; set; }

    public CustomMailMessage(string to, string from, string subject, string body, CustomMailAttachment attachment)
    {
        this.To = to;
        this.From = from;
        this.Subject = subject;
        this.Body = body;
        SetAttachment(attachment.Content, attachment.FileName, attachment.Type, attachment.Disposition);
    }

    private void SetAttachment(string content, string fileName, string type, string dispositon)
    {
        this.Content = content;
        this.FileName = fileName;
        this.Type = type;
        this.Disposition = dispositon;
    }
}
