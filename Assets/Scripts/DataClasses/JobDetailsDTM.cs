public class JobDetailsDTM
{
    public string CustomId { get; set; }
    public string Content { get; set; }

    public JobDetailsDTM(string customId, string content)
    {
        this.CustomId = customId;
        this.Content = content;
    }
}
