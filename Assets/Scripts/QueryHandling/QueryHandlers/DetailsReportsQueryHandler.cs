using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailsReportsQueryHandler : QueryHandler
{
    [SerializeField] private VisualTreeAsset detailsReportEntryBase;
    [SerializeField] private VisualTreeAsset jobDetailsEntryBase;
    private VisualElement scrollviewContainer;
    private ScrollView scrollview;
    private Dictionary<string, bool> reportElementExpandedStates = new Dictionary<string, bool>();

    #region Initialization

    public override void Initialize()
    {
        base.Initialize();

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
        this.scrollviewContainer = this.parentElement.Q<VisualElement>("details-scrollview-container");
        this.scrollview = this.scrollviewContainer.Q<ScrollView>();
    }

    protected override void SetViewElements()
    {
    }

    protected override void SetupButtons()
    {

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

        if (ReferenceEquals(AppController.Active.DetailsReportsHandler.DetailsReports, null))
        {
            return;
        }

        foreach (var entry in AppController.Active.DetailsReportsHandler.DetailsReports)
        {
            DetailsReport detailsReport = entry.Value;
            VisualElement reportElement = CreateDetailsReportElement(detailsReport);
            VisualElement detailsContainer = reportElement.Q<VisualElement>("job-details-container");

            this.scrollview.contentContainer.Add(reportElement);
        }
    }

    private VisualElement CreateDetailsReportElement(DetailsReport detailsReport)
    {
        VisualElement mainElement = this.detailsReportEntryBase.Instantiate();
        CustomButton expandCollapseButton = mainElement.Q<VisualElement>("expand-collapse-button-container").Q<CustomButton>();
        VisualElement expandCollapseButtonIcon = expandCollapseButton.Q<VisualElement>("icon");
        VisualElement reportDetailsContainer = mainElement.Q<VisualElement>("report-details-container");
        VisualElement timeRangeLabelsContainer = reportDetailsContainer.Q<VisualElement>("time-range-labels-container");
        CustomLabel timeLabel = timeRangeLabelsContainer.Q<VisualElement>("time-label-container").Q<CustomLabel>();
        VisualElement optionButtonsContainer = reportDetailsContainer.Q<VisualElement>("option-buttons-container");
        CustomButton addDetailsButton = optionButtonsContainer.Q<VisualElement>("add-details-button-container").Q<CustomButton>();
        CustomButton deleteReportButton = optionButtonsContainer.Q<VisualElement>("delete-report-button-container").Q<CustomButton>();
        CustomButton emailReportButton = optionButtonsContainer.Q<VisualElement>("email-button-container").Q<CustomButton>();
        VisualElement jobDetailsContainer = mainElement.Q<VisualElement>("job-details-container");

        if (detailsReport.Details.Count <= 0)
        {
            expandCollapseButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
        else
        {
            expandCollapseButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
        }

        expandCollapseButton.RegisterCallback<ClickEvent>(evt =>
        {
            ToggleJobDetailsExpandState(detailsReport.ObjectId, jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon);
        });

        if (detailsReport.Details.Count >= 1)
        {
            List<DateTime> dateTimes = detailsReport.Details.Values.Select(dr => dr.JobDate).ToList();
            DateTime earliestTime = dateTimes.Min();
            DateTime latestTime = dateTimes.Max();
            string earliestTimeText = earliestTime == DateTime.MinValue ? "--" : earliestTime.ToString("yy/MM/dd");
            string latestTimeText = latestTime == DateTime.MinValue ? "--" : latestTime.ToString("yy/MM/dd");

            timeLabel.text = $"{earliestTimeText} - {latestTimeText}";
        }
        else
        {
            timeLabel.text = "N/A - N/A";
        }

        addDetailsButton.RegisterCallback<ClickEvent>(evt => QueryController.Active.JobDetailsQueryHandler.OpenNewJobDetails(detailsReport));

        deleteReportButton.RegisterCallback<ClickEvent>(evt =>
        {
            QueryController.Active.PopupsQueryHandler.OpenConfirmationPopup(() =>
            {
                AppController.Active.ServerCommunicator.DeleteDetailsReport(detailsReport.ObjectId, successful =>
                {
                    if (successful)
                    {
                        AppController.Active.DetailsReportsHandler.RemoveDetailsReports(detailsReport.ObjectId);

                        RefreshJobDetails(detailsReport, jobDetailsContainer);
                    }
                });
            });
        });

        if (detailsReport.Details.Count > 0)
        {
            emailReportButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
            emailReportButton.RegisterCallback<ClickEvent>(evt =>
            {
                QueryController.Active.PopupsQueryHandler.OpenSendEmailPopup((string recipient, string bodyContent) =>
                {
                    AppController.Active.MailSender.StartSendingEmail(detailsReport, recipient, bodyContent);
                });
            });
        }
        else
        {
            emailReportButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }

        if (!this.reportElementExpandedStates.ContainsKey(detailsReport.ObjectId))
        {
            this.reportElementExpandedStates.Add(detailsReport.ObjectId, false);
        }

        if (this.reportElementExpandedStates[detailsReport.ObjectId])
        {
            ExpandJobDetails(detailsReport.ObjectId, jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon);
        }
        else
        {
            CollapseJobDetails(detailsReport.ObjectId, jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon);
        }

        RefreshJobDetails(detailsReport, jobDetailsContainer);

        return mainElement;
    }

    private void RefreshJobDetails(DetailsReport detailsReport, VisualElement jobDetailsContainer)
    {
        jobDetailsContainer.Clear();

        foreach (JobDetail jobDetail in detailsReport.Details.Values)
        {
            jobDetailsContainer.Add(CreateJobDetailsElement(detailsReport, jobDetail));
        }
    }

    private VisualElement CreateJobDetailsElement(DetailsReport detailsReport, JobDetail jobDetail)
    {
        VisualElement mainElement = this.jobDetailsEntryBase.Instantiate();
        Label clientNameLabel = mainElement.Q<VisualElement>("client-name-label-container").Q<Label>();
        Label addressLabel = mainElement.Q<VisualElement>("address-label-container").Q<Label>();
        Label dateTimeLabel = mainElement.Q<VisualElement>("date-time-label-container").Q<Label>();
        Label jobTypeLabel = mainElement.Q<VisualElement>("job-type-label-container").Q<Label>();
        CustomButton editButton = mainElement.Q<VisualElement>("edit-button-container").Q<CustomButton>();
        CustomButton deleteButton = mainElement.Q<VisualElement>("delete-button-container").Q<CustomButton>();
        string clientNameText = String.IsNullOrWhiteSpace(jobDetail.ClientName) ? "Unknown Client" : jobDetail.ClientName;
        string dateText = jobDetail.StartTime == DateTime.MinValue ? "Unknown Date" : $"Date: {jobDetail.StartTime.ToString("yy/MM/dd")}";
        string jobText = jobDetail.JobType == 0 ? "Unknown" : jobDetail.JobType.ToString();
        string clientAddressText = String.IsNullOrWhiteSpace(jobDetail.ClientAddress) ? "Unknown Address" : jobDetail.ClientAddress;
        ActionHelper.BoolDelegate responseDelegate = isTrue =>
        {
            if (isTrue)
            {
                AppController.Active.DetailsReportsHandler.RemoveJobDetails(jobDetail);
            }
        };

        editButton.RegisterCallback<ClickEvent>(evt =>
        {
            QueryController.Active.JobDetailsQueryHandler.OpenExistingJobDetails(detailsReport, jobDetail);
        });

        deleteButton.RegisterCallback<ClickEvent>(evt =>
        {
            QueryController.Active.PopupsQueryHandler
            .OpenConfirmationPopup(() => AppController.Active.ServerCommunicator.DeleteJobDetails(jobDetail.ObjectId, responseDelegate));
        });

        clientNameLabel.text = clientNameText;
        addressLabel.text = clientAddressText;
        dateTimeLabel.text = dateText;
        jobTypeLabel.text = jobText;

        return mainElement;
    }

    #endregion

    #region Actions

    private void ToggleJobDetailsExpandState(string detailsReportId, VisualElement jobDetailsContainer, CustomButton expandCollapseButton, VisualElement expandCollapseButtonIcon)
    {
        bool expand = jobDetailsContainer.resolvedStyle.display == DisplayStyle.None;

        if (expand)
        {
            ExpandJobDetails(detailsReportId, jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon);
        }
        else
        {
            CollapseJobDetails(detailsReportId, jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon);
        }
    }

    private void ExpandJobDetails(string detailsReportId, VisualElement jobDetailsContainer, CustomButton expandCollapseButton, VisualElement expandCollapseButtonIcon)
    {
        this.reportElementExpandedStates[detailsReportId] = true;
        ChangeJobDetailsExpandedState(jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon, true);
    }

    private void CollapseJobDetails(string detailsReportId, VisualElement jobDetailsContainer, CustomButton expandCollapseButton, VisualElement expandCollapseButtonIcon)
    {
        this.reportElementExpandedStates[detailsReportId] = false;
        ChangeJobDetailsExpandedState(jobDetailsContainer, expandCollapseButton, expandCollapseButtonIcon, false);
    }

    private void ChangeJobDetailsExpandedState(VisualElement jobDetailsContainer, CustomButton expandCollapseButton, VisualElement expandCollapseButtonIcon, bool expand)
    {
        expandCollapseButtonIcon.ClearClassList();

        if (expand)
        {
            expandCollapseButtonIcon.AddToClassList("gi-chevron-up");
            VisualElementHelper.SetElementDisplay(jobDetailsContainer, DisplayStyle.Flex);
        }
        else
        {
            expandCollapseButtonIcon.AddToClassList("gi-chevron-down");
            VisualElementHelper.SetElementDisplay(jobDetailsContainer, DisplayStyle.None);
        }
    }

    #endregion
}
