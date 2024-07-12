using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailsReport
{
    private string createdBy;
    public string CreatedBy => createdBy;
    private string objectId = "";
    private bool allJobDetailsAdded = false;
    public bool AllJobDetailsAdded => allJobDetailsAdded;
    public string ObjectId => objectId;
    private Dictionary<string, JobDetail> details = new Dictionary<string, JobDetail>();
    public Dictionary<string, JobDetail> Details => details;

    public DetailsReport(DetailsReportDTM dtm)
    {
        this.createdBy = dtm.createdBy;
        this.objectId = dtm.objectId;
    }

    public void AddJobDetail(JobDetail jobDetail)
    {
        if (this.details.ContainsKey(jobDetail.ObjectId))
        {
            LogHelper.Active.Log($"{jobDetail.ObjectId} is already in report {this.objectId}");

            return;
        }

        this.details.Add(jobDetail.ObjectId, jobDetail);
    }

    public void OnPopulatingAllJobDetails()
    {
        this.allJobDetailsAdded = true;
        Debug.Log("INVOKE WITH: " + this.details.Count);
        DetailsReportsHandler.Active.OnReportsCollectionChangedEvent.Invoke();
    }
}
