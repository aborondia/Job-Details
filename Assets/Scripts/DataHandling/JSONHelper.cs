using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleJSON;

public static class JSONHelper
{
    public static RegistrationValidationDTM GetRegistrationValidationDTM(string response)
    {
        JSONNode resultsNode = JSON.Parse(response)["result"];

        return JsonConvert.DeserializeObject<RegistrationValidationDTM>(resultsNode.ToString());
    }

    public static UserDTM GetUserDTM(string response)
    {
        JSONNode resultsNode = JSON.Parse(response)["result"];
        UserDTM userDTM = JsonConvert.DeserializeObject<UserDTM>(resultsNode["user"].ToString());

        return userDTM;
    }

    public static RequestErrorDTM GetRequestErrorDTM(string response)
    {
        return JsonConvert.DeserializeObject<RequestErrorDTM>(response);
    }

    public static DetailsReport GetDetailsReportFromCreate(string response)
    {
        DetailsReport detailsReport;
        JSONNode result = JSON.Parse(response)["result"];
        DetailsReportDTM dtm = JsonConvert.DeserializeObject<DetailsReportDTM>(result.ToString());

        detailsReport = new DetailsReport(dtm);

        return detailsReport;
    }

    public static List<DetailsReport> GetDetailsReports(string response)
    {
        List<DetailsReport> detailsReports = new List<DetailsReport>();
        JSONNode resultsNode = JSON.Parse(response)["result"];

        foreach (JSONNode node in resultsNode.Values)
        {
            JSONNode reportNode = node["detailsReport"];
            JSONNode detailsNode = node["jobDetails"];
            DetailsReport detailsReport = GetDetailsReport(reportNode);
            List<JobDetailsDTM> jobDetails = GetJobDetailDTMs(detailsNode);
            detailsReport.PopulateJobDetails(jobDetails);

            detailsReports.Add(detailsReport);
        }

        return detailsReports;
    }

    private static DetailsReport GetDetailsReport(JSONNode node)
    {
        string createdBy = node["createdBy"];
        string objectId = node["objectId"];
        DetailsReportDTM detailsReportDTM = JsonConvert.DeserializeObject<DetailsReportDTM>(node.ToString());
        DetailsReport detailsReport = new DetailsReport(detailsReportDTM);

        return detailsReport;
    }

    private static ActionHelper.StringDelegate GetDetailsReponseDelegate(DetailsReport detailsReport, List<JobDetail> jobDetails)
    {
        ActionHelper.StringDelegate responseDelegate = (string response) =>
        {
            jobDetails = GetJobDetails(response);

            foreach (JobDetail jobDetail in jobDetails)
            {
                if (ReferenceEquals(jobDetail, null))
                {
                    continue;
                }

                detailsReport.AddJobDetail(jobDetail);
            }

            detailsReport.OnPopulatingAllJobDetails();
        };

        return responseDelegate;
    }

    private static List<JobDetail> GetJobDetails(string response)
    {
        List<JobDetail> jobDetails = new List<JobDetail>();
        JSONNode result = JSON.Parse(response)["results"];
        List<JobDetailsDTM> jobDetailsDTMs = GetJobDetailDTMs(result);

        foreach (JobDetailsDTM dtm in jobDetailsDTMs)
        {
            if (ReferenceEquals(dtm.content, null))
            {
                continue;
            }

            jobDetails.Add(dtm.content);
        }

        return jobDetails;
    }

    private static List<DetailsReportDTM> GetDetailsReportDTMs(JSONNode nodeWithValues)
    {
        List<DetailsReportDTM> reportDTMs = new List<DetailsReportDTM>();

        foreach (JSONNode node in nodeWithValues.Values)
        {
            DetailsReportDTM dtm = GetDetailsReportDTM(node);

            reportDTMs.Add(dtm);
        }

        return reportDTMs;
    }

    private static DetailsReportDTM GetDetailsReportDTM(JSONNode node)
    {
        DetailsReportDTM dtm = new DetailsReportDTM();

        dtm.createdAt = DateTime.Parse(node["createdAt"]);
        dtm.createdBy = node["createdBy"];
        dtm.objectId = node["objectId"];
        dtm.updatedAt = DateTime.Parse(node["updatedAt"]);

        return dtm;
    }

    public static DetailsReportDTM GetDetailsReportDTM(string createdBy, string result)
    {
        DetailsReportDTM dtm = new DetailsReportDTM();
        JSONNode node = JSON.Parse(result);
        DateTime createdAt;

        DateTime.TryParse(node["createdAt"], out createdAt);
        dtm.createdAt = createdAt;
        dtm.createdBy = createdBy;
        dtm.objectId = node["objectId"];
        dtm.updatedAt = dtm.createdAt;

        return dtm;
    }

    private static List<JobDetailsDTM> GetJobDetailDTMs(JSONNode nodeWithValues)
    {
        List<JobDetailsDTM> dtms = new List<JobDetailsDTM>();

        foreach (JSONNode node in nodeWithValues.Values)
        {
            JobDetailsDTM dtm = GetJobDetailDTM(node);

            dtms.Add(dtm);
        }

        return dtms;
    }

    private static JobDetailsDTM GetJobDetailDTM(JSONNode node)
    {
        JobDetailsDTM dtm = JsonConvert.DeserializeObject<JobDetailsDTM>(node.ToString());
        dtm.content = GetJobDetailsDTMContent(dtm.objectId, node["content"]);

        return dtm;
    }

    private static JobDetail GetJobDetailsDTMContent(string objectId, JSONNode node)
    {
        JobDetail content = new JobDetail();

        int jobTypeIndex = node["JobType"];
        int paymentTypeIndex = node["PaymentType"];
        string detailsReportId = node["DetailsReportId"];
        string clientName = node["ClientName"];
        string clientAddress = node["ClientAddress"];
        DateTime jobDate = node["JobDate"];
        DateTime startTime = node["StartTime"];
        DateTime finishTime = node["FinishTime"];
        Enumerations.JobTypeEnum jobType = (Enumerations.JobTypeEnum)jobTypeIndex;
        List<CleanerJobEntry> cleaners = GetCleaners(node["Cleaners"]);
        Enumerations.PaymentTypeEnum paymentType = (Enumerations.PaymentTypeEnum)paymentTypeIndex;
        string description = node["Description"];

        content.SetJobDetailProperties(
            detailsReportId,
            clientName,
            clientAddress,
            jobDate,
            startTime,
            finishTime,
            jobType,
            cleaners,
            paymentType,
            description,
            objectId);

        return content;
    }

    private static List<CleanerJobEntry> GetCleaners(JSONNode nodeWithValues)
    {
        List<CleanerJobEntry> cleaners = new List<CleanerJobEntry>();

        foreach (JSONNode node in nodeWithValues.Values)
        {
            cleaners.Add(GetCleaner(node));
        }

        return cleaners;
    }

    private static CleanerJobEntry GetCleaner(JSONNode node)
    {
        CleanerJobEntry cleanerJobEntry;
        string cleanerName = node["Name"];
        float cleanerHoursWorked = node["HoursWorked"];

        cleanerJobEntry = new CleanerJobEntry(cleanerName, cleanerHoursWorked);

        return cleanerJobEntry;
    }

    public static List<RoleDTM> GetRoles(string response)
    {
        List<RoleDTM> dtms = new List<RoleDTM>();
        JSONNode result = JSON.Parse(response)["results"];

        foreach (JSONNode node in result.Values)
        {
            dtms.Add(GetRole(node));
        }

        return dtms;
    }

    public static RoleDTM GetRole(JSONNode node)
    {
        string name = node["name"];
        string objectId = node["objectId"];

        return new RoleDTM(name, objectId);
    }

    public static Dictionary<string, User> GetUsers(string response)
    {
        Dictionary<string, User> users = new Dictionary<string, User>();
        JSONNode result = JSON.Parse(response)["result"];
        UserDTM userDTM;
        RoleDTM roleDTM;

        foreach (JSONNode node in result.Values)
        {
            userDTM = new UserDTM();
            userDTM.objectId = node["objectId"];
            userDTM.username = node["username"];
            userDTM.email = node["email"];
            userDTM.verified = node["verified"];
            userDTM.roleId = node["roleId"];

            if (!String.IsNullOrEmpty(userDTM.roleId))
            {
                roleDTM = AppController.Active.UserDataHandler.GetRoleById(userDTM.roleId);
            }
            else
            {
                roleDTM = null;
            }

            User user = new User(userDTM, roleDTM);

            users.Add(user.DTM.username, user);
        }

        return users;
    }

    private static UserDTM GetUserDTM(JSONNode node)
    {
        UserDTM dtm = new UserDTM();

        dtm.objectId = node["objectId"];
        dtm.username = node["username"];
        dtm.email = node["email"];
        dtm.verified = node["verified"];

        return dtm;
    }
}
