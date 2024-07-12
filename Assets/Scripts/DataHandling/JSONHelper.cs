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

            ActionHelper.ReturnStringDelegate responseDelegate = GetDetailsReponseDelegate(detailsReport, jobDetails);

            AppController.Active.ServerCommunicator.GetJobDetails(detailsReportDTM.objectId, responseDelegate);
        }

        return detailsReports;
    }

    private static ActionHelper.ReturnStringDelegate GetDetailsReponseDelegate(DetailsReport detailsReport, List<JobDetail> jobDetails)
    {
        ActionHelper.ReturnStringDelegate responseDelegate = (string response) =>
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

            Debug.Log(dtm.content.ObjectId);
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
        Debug.Log(node);
        Debug.Log(node.ToString());
        string clientAddress = node["ClientAddress"];
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
        float cleanerHoursWorked = node["hours"];

        cleanerJobEntry = new CleanerJobEntry(cleanerName, cleanerHoursWorked);

        return cleanerJobEntry;
    }

    private static JobDetailsDTM.JsonFile GetJsonFile(JSONNode node)
    {
        JobDetailsDTM.JsonFile dtm = new JobDetailsDTM.JsonFile();

        dtm.__type = node["__type"];
        dtm.name = node["name"];

        return dtm;
    }
}
