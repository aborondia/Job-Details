using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JobDetails.Enumerations;

public class JobDetail
{

    private string clientName = "Jane Doe";
    public string ClientName => clientName;
    private string clientAddress = "123 Fake Street";
    public string ClientAddress => clientAddress;
    private DateTime startTime = DateTime.Now;
    public DateTime StartTime => startTime;
    private DateTime finishTime = DateTime.Now.AddHours(6);
    public DateTime FinishTime => finishTime;
    private JobTypeEnum jobType = JobTypeEnum.BiWeekly;
    public JobTypeEnum JobType => jobType;
    private List<Cleaner> cleaners = new List<Cleaner>();
    public List<Cleaner> Cleaners => cleaners;
    private PaymentTypeEnum paymentType = PaymentTypeEnum.Cash;
    public PaymentTypeEnum PaymentType => paymentType;
    private string description = "I cleaned their house.";
    public string Description => description;

    public JobDetail()
    {
        this.cleaners.Add(new Cleaner());
        this.cleaners.Add(new Cleaner());
        this.cleaners.Add(new Cleaner());
        this.cleaners.Add(new Cleaner());
    }

    public List<string> GetCleanersContent(int index)
    {
        List<string> cleanerContent;
        string cleanerFirstLine;

        if (index >= this.cleaners.Count)
        {
            Debug.Log("Cleaner list does not contain index " + index);

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
