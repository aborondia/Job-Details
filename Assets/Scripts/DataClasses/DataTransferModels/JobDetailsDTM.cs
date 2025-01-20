using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class JobDetailsDTM
{
    public string objectId { get; set; }
    public string detailsReportId { get; set; }
    public string createdBy { get; set; }
    public JobDetail content { get; set; }

    public JobDetailsDTM(string createdBy, JobDetail content)
    {
        this.objectId = content.ObjectId;
        this.detailsReportId = content.DetailsReportId;
        this.createdBy = createdBy;
        this.content = content;
    }
}
