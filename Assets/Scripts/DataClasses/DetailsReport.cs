using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailsReport
{
    private string createdBy;
    public string CreatedBy => createdBy;
    private string objectId = "";
    public string ObjectId => objectId;
    private Dictionary<string, JobDetail> details = new Dictionary<string, JobDetail>();
    public Dictionary<string, JobDetail> Details => details;

    public DetailsReport(DetailsReportDTM dtm)
    {
        this.createdBy = dtm.createdBy;
        this.objectId = dtm.objectId;
    }
}
