public class JobDetailsDTM
{
    public string userId { get; set; }
    public string objectId { get; set; }
    public string content { get; set; }

    public JobDetailsDTM(string userId, string content)
    {
        this.userId = userId;
        this.content = content;
    }

    public JobDetailsDTM(string userId, string content, string objectId)
    {
        this.userId = userId;
        this.objectId = objectId;
        this.content = content;
    }
}
