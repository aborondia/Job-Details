public class CustomMailAttachment
{
    public string Content { get; set; }
    public string FileName { get; set; }
    public string Type { get; set; }
    public string Disposition { get; set; }

    public CustomMailAttachment(string content, string fileName, string type, string dispositon)
    {
        this.Content = content;
        this.FileName = fileName;
        this.Type = type;
        this.Disposition = dispositon;
    }
}
