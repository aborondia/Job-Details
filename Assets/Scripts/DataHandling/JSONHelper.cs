using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

public static class JSONHelper
{
    public static DetailsReport GetDetailsReportFromCreate(string createdBy, string result)
    {
        DetailsReport detailsReport;
        DetailsReportDTM dtm = GetDetailsReportDTM(createdBy, result);

        detailsReport = new DetailsReport(dtm);

        return detailsReport;
    }

    public static List<DetailsReport> GetDetailsReports(string result)
    {
        List<DetailsReportDTM> detailsReportDTMs;
        List<DetailsReport> detailsReports = new List<DetailsReport>();
        JSONNode resultsNode = JSON.Parse(result)["results"];

        detailsReportDTMs = GetDetailsReportDTMs(resultsNode);

        foreach (DetailsReportDTM detailsReportDTM in detailsReportDTMs)
        {
            DetailsReport detailsReport = new DetailsReport(detailsReportDTM);
            List<JobDetail> jobDetails = new List<JobDetail>();

            detailsReports.Add(detailsReport);

            ActionHelper.StringDelegate responseDelegate = GetDetailsReponseDelegate(detailsReport, jobDetails);

            AppController.Active.ServerCommunicator.GetJobDetails(detailsReportDTM.objectId, responseDelegate);
        }

        return detailsReports;
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

        dtm.createdAt = DateTime.Parse(node["createdAt"]);
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
        JobDetailsDTM dtm = new JobDetailsDTM();
        string objectId = node["objectId"];

        dtm.objectId = objectId;
        dtm.jsonFile = GetJsonFile(node["jsonFile"]);
        dtm.createdBy = node["createdBy"];
        dtm.content = GetJobDetailsDTMContent(objectId, node["content"]);
        // dtm.content = node["content"];
        // dtm.reportPointer = node["reportPointer"];

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
        string cleanerName = node["name"];
        string cleanerObjectId = node["cleanerObjectId"];
        float cleanerHoursWorked = node["hours"];

        cleanerJobEntry = new CleanerJobEntry(cleanerObjectId, cleanerName, cleanerHoursWorked);

        return cleanerJobEntry;
    }

    public static List<UserNameReferenceDTM> GetUserNameReferenceDTMs(string response)
    {
        List<UserNameReferenceDTM> dtms = new List<UserNameReferenceDTM>();
        JSONNode result = JSON.Parse(response)["results"];

        foreach (JSONNode node in result.Values)
        {
            dtms.Add(GetUserNameReferenceDTM(node));
        }

        return dtms;
    }

    private static UserNameReferenceDTM GetUserNameReferenceDTM(JSONNode node)
    {
        UserNameReferenceDTM dtm = new UserNameReferenceDTM();

        dtm.userName = node["userName"];
        dtm.userObjectId = node["userObjectId"];

        return dtm;
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
        RoleDTM dtm = new RoleDTM();
        dtm.name = node["name"];
        dtm.objectId = node["objectId"];

        return dtm;
    }

    public static List<UserDTM> GetUsers(string response)
    {
        List<UserDTM> dtms = new List<UserDTM>();
        JSONNode result = JSON.Parse(response)["results"];

        foreach (JSONNode node in result.Values)
        {
            dtms.Add(GetUser(node));
        }

        return dtms;
    }

    public static UserDTM GetUser(JSONNode node)
    {
        UserDTM dtm = new UserDTM();

        dtm.objectId = node["objectId"];
        dtm.username = node["username"];
        dtm.email = node["email"];
        // dtm.createdAt = node["createdAt"];
        // dtm.updatedAt = node["updatedAt"];
        // dtm.emailVerified = node["emailVerified"];

        return dtm;
    }

    private static JobDetailsDTM.JsonFile GetJsonFile(JSONNode node)
    {
        JobDetailsDTM.JsonFile dtm = new JobDetailsDTM.JsonFile();

        dtm.__type = node["__type"];
        dtm.name = node["name"];

        return dtm;
    }
}
