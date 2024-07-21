using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class DetailsReportsHandler : MonoBehaviour
{
    private Dictionary<string, DetailsReport> detailsReports;
    public Dictionary<string, DetailsReport> DetailsReports => detailsReports;
    public UnityEvent OnReportsCollectionChangedEvent = new UnityEvent();

    private void Awake()
    {
        SetReportsOnLogin();
    }

    private void SetReportsOnLogin()
    {
        AppController.Active.ServerCommunicator.OnSignInSuccessEvent.AddListener(() =>
        {
            if (AppController.Active.ServerCommunicator.CurrentUser.verified)
            {
                ActionHelper.StringDelegate responseDelegate = response =>
                {
                    PopulateReports(JSONHelper.GetDetailsReports(response));
                };

                AppController.Active.ServerCommunicator.GetDetailsReports(responseDelegate);
            }
        });
    }

    public void PopulateReports(List<DetailsReport> detailsReports)
    {
        this.detailsReports = new Dictionary<string, DetailsReport>();

        foreach (DetailsReport detailsReport in detailsReports)
        {
            this.detailsReports.Add(detailsReport.ObjectId, detailsReport);
        }

        OnReportsCollectionChangedEvent.Invoke();
    }

    private void SetJobDetails(DetailsReport report, List<JobDetail> jobDetails)
    {
        foreach (JobDetail jobDetail in jobDetails)
        {
            report.AddJobDetail(jobDetail);
        }
    }

    public void AddDetailsReports(DetailsReport detailsReport)
    {
        if (ReferenceEquals(this.detailsReports, null))
        {
            this.detailsReports = new Dictionary<string, DetailsReport>();
        }

        if (this.detailsReports.ContainsKey(detailsReport.ObjectId))
        {
            this.detailsReports[detailsReport.ObjectId] = detailsReport;
        }
        else
        {
            this.detailsReports.Add(detailsReport.ObjectId, detailsReport);
        }

        OnReportsCollectionChangedEvent.Invoke();
    }

    public void RemoveDetailsReports(string objectId)
    {
        if (ReferenceEquals(this.detailsReports, null) || !this.detailsReports.ContainsKey(objectId))
        {
            return;
        }

        this.detailsReports.Remove(objectId);

        OnReportsCollectionChangedEvent.Invoke();
    }

    public void RemoveJobDetails(JobDetail jobDetail)
    {
        if (this.detailsReports.ContainsKey(jobDetail.DetailsReportId))
        {
            this.detailsReports[jobDetail.DetailsReportId].Details.Remove(jobDetail.ObjectId);
        }

        AppController.Active.ServerCommunicator.DeleteJobDetails(jobDetail.ObjectId);

        OnReportsCollectionChangedEvent.Invoke();
    }
}
