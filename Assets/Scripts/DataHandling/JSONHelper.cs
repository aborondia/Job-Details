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
            detailsReports.Add(new DetailsReport(detailsReportDTM));
        }

        return detailsReports;
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
}
