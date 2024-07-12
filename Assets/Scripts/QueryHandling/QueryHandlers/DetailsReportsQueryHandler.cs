using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using MainView = Enumerations.MainView;
using Subview = Enumerations.Subview;

public class DetailsReportsQueryHandler : QueryHandler
{
    [SerializeField] private VisualTreeAsset detailsReportEntryBase;
    [SerializeField] private VisualTreeAsset jobDetailsEntryBase;
    private VisualElement header;
    private VisualElement addReportButtonContainer;
    private CustomButton addReportButton;
    private VisualElement scrollviewContainer;
    private ScrollView scrollview;
    private DetailsReport currentlySelectedDetailsReport;
    public DetailsReport CurrentlySelectedDetailsReport => currentlySelectedDetailsReport;
    private UnityEvent beforeDetailsReportExpandedEvent = new UnityEvent();

    #region Initialization

    public override void Initialize()
    {
        base.Initialize();

        AppController.Active.DetailsReportsHandler.OnReportsCollectionChangedEvent.AddListener(() =>
        {
            QueryController.Active.ChangeView(MainView.DetailsReports, Subview.Default);
        });

        AppController.Active.DetailsReportsHandler.OnReportsCollectionChangedEvent.AddListener(() =>
        {
            if (QueryController.Active.CurrentMainView == this.mainView)
            {
                RefreshDetailsReports();
            }
        });
    }

    protected override void InitializeElements()
    {
        this.header = this.parentElement.Q<VisualElement>("reports-header");
        this.addReportButtonContainer = this.header.Q<VisualElement>("add-button-container");
        this.addReportButton = this.addReportButtonContainer.Q<CustomButton>();
        this.scrollviewContainer = this.parentElement.Q<VisualElement>("details-scrollview-container");
        this.scrollview = this.scrollviewContainer.Q<ScrollView>();
    }

    protected override void SetViewElements()
    {
    }

    protected override void SetupButtons()
    {
        this.addReportButton.RegisterCallback<ClickEvent>(evt =>
        {
            ActionHelper.ReturnStringDelegate responseDelegate = (string response) =>
            {
                DetailsReport newReport = JSONHelper.GetDetailsReportFromCreate(AppController.Active.ServerCommunicator.CurrentUser.objectId, response);

                AppController.Active.DetailsReportsHandler.AddDetailsReports(newReport);
            };

            AppController.Active.ServerCommunicator.CreateDetailsReport(responseDelegate);
        });
    }

    protected override void SetupInputs()
    {
    }

    #endregion

    #region View Update

    protected override void OnMainViewChanged()
    {
        base.OnMainViewChanged();

        if (QueryController.Active.CurrentMainView != this.mainView)
        {
            return;
        }
        
        RefreshDetailsReports();
    }

    protected override void OnSubviewChanged()
    {
        base.OnSubviewChanged();

        RefreshDetailsReports();
    }

    #endregion

    #region Create/Update

    private void RefreshDetailsReports()
    {
        this.scrollview.contentContainer.Clear();
        beforeDetailsReportExpandedEvent.RemoveAllListeners();

        if (ReferenceEquals(AppController.Active.DetailsReportsHandler.DetailsReports, null))
        {
            return;
        }

        foreach (var entry in AppController.Active.DetailsReportsHandler.DetailsReports)
        {
            DetailsReport detailsReport = entry.Value;
            VisualElement reportElement = CreateDetailsReportElement(detailsReport);
            VisualElement detailsContainer = reportElement.Q<VisualElement>("job-details-container");

            detailsContainer.Clear();

            foreach (JobDetail jobDetail in detailsReport.Details.Values)
            {
                detailsContainer.Add(CreateJobDetailsElement(jobDetail));
            }

            this.scrollview.contentContainer.Add(reportElement);
        }
    }

    private VisualElement CreateDetailsReportElement(DetailsReport detailsReport)
    {
        VisualElement mainElement = this.detailsReportEntryBase.Instantiate();
        VisualElement leftColumn = mainElement.Q<VisualElement>("left-column");
        CustomButton expandCollapseButton = leftColumn.Q<VisualElement>("expand-collapse-button-container").Q<CustomButton>();
        VisualElement expandCollapseButtonIcon = expandCollapseButton.Q<VisualElement>("icon");
        VisualElement rightColumn = mainElement.Q<VisualElement>("right-column");
        VisualElement reportDetailsContainer = rightColumn.Q<VisualElement>("report-details-container");
        VisualElement timeRangeLabelsContainer = reportDetailsContainer.Q<VisualElement>("time-range-labels-container");
        CustomLabel timeLabel = timeRangeLabelsContainer.Q<VisualElement>("time-label-container").Q<CustomLabel>();
        VisualElement optionButtonsContainer = reportDetailsContainer.Q<VisualElement>("option-buttons-container");
        CustomButton addDetailsButton = optionButtonsContainer.Q<VisualElement>("add-details-button-container").Q<CustomButton>();
        CustomButton deleteReportButton = optionButtonsContainer.Q<VisualElement>("delete-report-button-container").Q<CustomButton>();
        VisualElement jobDetailsContainer = rightColumn.Q<VisualElement>("job-details-container");

        if (detailsReport.Details.Count <= 0)
        {
            expandCollapseButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
        else
        {
            expandCollapseButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
        }

        expandCollapseButton.RegisterCallback<ClickEvent>(evt => OnReportExpandCollapseButtonPressed(jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon));

        if (detailsReport.Details.Count >= 2)
        {
            List<DateTime> dateTimes = detailsReport.Details.Values.Select(dr => dr.StartTime).ToList();
            DateTime earliestTime = dateTimes.Min();
            DateTime latestTime = dateTimes.Max();
            timeLabel.text = $"{earliestTime} - {latestTime}";
        }
        else
        {
            timeLabel.text = "--";
        }

        this.currentlySelectedDetailsReport = detailsReport;
        addDetailsButton.RegisterCallback<ClickEvent>(evt => QueryController.Active.JobDetailsQueryHandler.OpenNewJobDetails());

        deleteReportButton.RegisterCallback<ClickEvent>(evt =>
        {
            AppController.Active.ServerCommunicator.DeleteDetailsReport(detailsReport.ObjectId);
            AppController.Active.DetailsReportsHandler.RemoveDetailsReports(detailsReport.ObjectId);

        });

        beforeDetailsReportExpandedEvent.AddListener(() => VisualElementHelper.SetElementDisplay(jobDetailsContainer, DisplayStyle.None));
        jobDetailsContainer.Clear();
        // Populate details

        return mainElement;
    }

    private VisualElement CreateJobDetailsElement(JobDetail jobDetail)
    {
        VisualElement mainElement = this.jobDetailsEntryBase.Instantiate();
        Label clientNameLabel = mainElement.Q<VisualElement>("client-name-label-container").Q<Label>();
        Label addressLabel = mainElement.Q<VisualElement>("address-label-container").Q<Label>();
        Label dateTimeLabel = mainElement.Q<VisualElement>("date-time-label-container").Q<Label>();
        Label jobTypeLabel = mainElement.Q<VisualElement>("job-type-label-container").Q<Label>();

        clientNameLabel.text = jobDetail.ClientName;
        addressLabel.text = jobDetail.ClientAddress;
        dateTimeLabel.text = $"Date: {jobDetail.StartTime.ToString("yy/MM/dd")}";
        jobTypeLabel.text = $"Job Type: {jobDetail.JobType.ToString()}";

        return mainElement;
    }

    #endregion

    #region Actions

    private void OnReportExpandCollapseButtonPressed(VisualElement jobDetailsContainer, CustomButton expandCollapseButton, VisualElement expandCollapseButtonIcon)
    {
        bool expand = jobDetailsContainer.resolvedStyle.display == DisplayStyle.None;

        this.beforeDetailsReportExpandedEvent.Invoke();
        expandCollapseButtonIcon.ClearClassList();

        if (expand)
        {
            expandCollapseButtonIcon.AddToClassList("gi-chevron-up");
            VisualElementHelper.SetElementDisplay(jobDetailsContainer, DisplayStyle.Flex);
        }
        else
        {
            expandCollapseButtonIcon.AddToClassList("gi-chevron-down");
        }
    }

    #endregion
}
