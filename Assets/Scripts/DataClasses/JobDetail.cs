using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerations;

public class JobDetail
{
    private string objectId;
    public string ObjectId => objectId;
    private string detailsReportId;
    public string DetailsReportId => detailsReportId;
    private string clientName;
    public string ClientName => clientName;
    private string clientAddress;
    public string ClientAddress => clientAddress;
    private DateTime startTime;
    public DateTime StartTime => startTime;
    private DateTime finishTime;
    public DateTime FinishTime => finishTime;
    private DateTime createdAt;
    public DateTime CreatedAt => createdAt;
    private JobTypeEnum jobType;
    public JobTypeEnum JobType => jobType;
    private List<Cleaner> cleaners;
    public List<Cleaner> Cleaners => cleaners;
    private PaymentTypeEnum paymentType;
    public PaymentTypeEnum PaymentType => paymentType;
    private string description;
    public string Description => description;

    public void SetJobDetailProperties(
                string detailsReportId,
        string clientName,
        string clientAddress,
        DateTime startTime,
        DateTime finishTime,
        JobTypeEnum jobType,
        List<Cleaner> cleaners,
        PaymentTypeEnum paymentType,
        string description
    )
    {
        this.detailsReportId = detailsReportId;
        this.clientName = clientName;
        this.clientAddress = clientAddress;
        this.startTime = startTime;
        this.finishTime = finishTime;
        this.jobType = jobType;
        this.cleaners = cleaners;
        this.paymentType = paymentType;
        this.description = description;
    }

    public void OnServerCreation(string objectId, DateTime createdAt)
    {
        this.objectId = objectId;
        this.createdAt = createdAt;
    }

    public void AddCleaner(Cleaner cleaner)
    {
        if (ReferenceEquals(this.cleaners, null))
        {
            this.cleaners = new List<Cleaner>();
        }

        this.cleaners.Add(cleaner);
    }

    public List<string> GetCleanersContent(int index)
    {
        List<string> cleanerContent;
        string cleanerFirstLine;

        if (index >= this.cleaners.Count)
        {
            LogHelper.Active.Log("Cleaner list does not contain index " + index);

            return null;
        }

        if (index > 0)
        {
            cleanerFirstLine = String.Empty;
        }
        else
        {
            cleanerFirstLine = "Cleaners:";
        }

        cleanerContent = new List<string>{
        cleanerFirstLine,
        $"{index + 1}){this.cleaners[index].Name}",
        $"Hrs={this.cleaners[index].Hours}",
        };

        return cleanerContent;
    }


}
