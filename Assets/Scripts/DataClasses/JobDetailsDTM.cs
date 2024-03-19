using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class JobDetailsDTM
{
    public JsonFile jsonFile;
    public string createdBy;
    public JobDetail content;
    public ReportPointer reportPointer;

    public JobDetailsDTM(string createdBy, JobDetail content, string detailsReportId)
    {
        this.jsonFile = new JsonFile();
        this.createdBy = createdBy;
        this.content = content;
        this.reportPointer = new ReportPointer(detailsReportId);
    }

    public class JsonFile
    {
        public string __type = "File";
        public string name = "JobDetails";
    }

    public class ReportPointer
    {
        public string __type = "Pointer";
        public string className = "DetailsReport";
        public string objectId;

        public ReportPointer(string objectId)
        {
            this.objectId = objectId;
        }
    }

    public string SerializeToJson()
    {
        var json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        });

        return json;
    }
}
