using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetailsReportsHandler : MonoBehaviour
{
    private Dictionary<string, DetailsReport> detailsReports;
    public Dictionary<string, DetailsReport> DetailsReports => detailsReports;
    public UnityEvent OnReportsCollectionChangedEvent = new UnityEvent();

    private void Awake()
    {
        AppController.Active.ServerCommunicator.OnSignInSuccessEvent.AddListener(() =>
        {
            ActionHelper.ReturnStringDelegate responseDelegate = (string response) =>
            {
                // Get attached job details
                PopulateReports(JSONHelper.GetDetailsReports(response));
            };

            AppController.Active.ServerCommunicator.GetDetailsReports(responseDelegate);
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
}
