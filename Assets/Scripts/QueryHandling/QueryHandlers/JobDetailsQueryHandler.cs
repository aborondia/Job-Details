using System;
using System.Collections.Generic;
using UnityEngine;
using MainView = Enumerations.MainView;
using Subview = Enumerations.Subview;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using Newtonsoft.Json;
using SimpleJSON;
using System.Linq;

public class JobDetailsQueryHandler : QueryHandler
{
    [SerializeField] private VisualTreeAsset cleanerRowBase;
    private VisualElement mainContainer;
    private VisualElement mainContentContainer;
    private VisualElement inputsContainer;
    private VisualElement clientNameInputContainer;
    private CustomInput clientNameInput;
    private VisualElement clientAddressInputContainer;
    private CustomInput clientAddressInput;
    private VisualElement dateInputContainer;
    private CustomInput dateInput;
    private VisualElement startTimeInputsContainer;
    private VisualElement startTimeInputContainer;
    private CustomInput startTimeHourInput;
    private CustomInput startTimeMinuteInput;
    private CustomEnumField startTimeOfDayField;
    private VisualElement finishTimeInputsContainer;
    private VisualElement finishTimeInputContainer;
    private CustomInput finishTimeHourInput;
    private CustomInput finishTimeMinuteInput;
    private CustomEnumField finishTimeOfDayField;
    private VisualElement jobTypeInputContainer;
    private CustomEnumField jobTypeInput;
    private VisualElement paymentTypeInputContainer;
    private CustomEnumField paymentTypeInput;
    private VisualElement cleanersContainer;
    private VisualElement cleanersContent;
    private VisualElement cleanersContainerHeader;
    private VisualElement addCleanerButtonContainer;
    private CustomButton addCleanerButton;
    private VisualElement detailsContentContainer;
    private CustomInput detailsInput;
    private VisualElement reportsFooter;
    private VisualElement cancelButtonContainer;
    private CustomButton cancelButton;
    private VisualElement saveButtonContainer;
    private CustomButton saveButton;
    private JobDetail currentJobDetail;
    public JobDetail CurrentJobDetail => currentJobDetail;
    private DateTime? currentDatePickerDate;
    private UnityEvent onCurrentJobDetailChanged = new UnityEvent();

    #region  Initlialization

    protected override void InitializeElements()
    {
        this.mainContainer = this.parentElement.Q<VisualElement>("main");
        this.mainContentContainer = this.mainContainer.Q<VisualElement>("main-content");
        this.inputsContainer = this.mainContentContainer.Q<VisualElement>("inputs-container");

        this.clientNameInputContainer = this.inputsContainer.Q<VisualElement>("client-name-input-container");
        this.clientNameInput = this.clientNameInputContainer.Q<CustomInput>();

        this.clientAddressInputContainer = this.inputsContainer.Q<VisualElement>("client-address-input-container");
        this.clientAddressInput = this.clientAddressInputContainer.Q<CustomInput>();

        this.dateInputContainer = this.inputsContainer.Q<VisualElement>("date-input-container");
        this.dateInput = this.dateInputContainer.Q<CustomInput>();

        this.startTimeInputsContainer = this.inputsContainer.Q<VisualElement>("start-time-inputs-container");
        this.startTimeInputContainer = this.startTimeInputsContainer.Q<VisualElement>("start-time-input-container");
        this.startTimeHourInput = this.startTimeInputContainer.Q<CustomInput>("hour-input");
        this.startTimeMinuteInput = this.startTimeInputContainer.Q<CustomInput>("minute-input");
        this.startTimeOfDayField = this.startTimeInputContainer.Q<CustomEnumField>();

        this.finishTimeInputsContainer = this.inputsContainer.Q<VisualElement>("finish-time-inputs-container");
        this.finishTimeInputContainer = this.finishTimeInputsContainer.Q<VisualElement>("finish-time-input-container");
        this.finishTimeHourInput = this.finishTimeInputContainer.Q<CustomInput>("hour-input");
        this.finishTimeMinuteInput = this.finishTimeInputContainer.Q<CustomInput>("minute-input");
        this.finishTimeOfDayField = this.finishTimeInputContainer.Q<CustomEnumField>();

        this.jobTypeInputContainer = this.inputsContainer.Q<VisualElement>("job-type-input-container");
        this.jobTypeInput = this.jobTypeInputContainer.Q<CustomEnumField>();

        this.paymentTypeInputContainer = this.inputsContainer.Q<VisualElement>("payment-type-input-container");
        this.paymentTypeInput = this.paymentTypeInputContainer.Q<CustomEnumField>();

        this.cleanersContainer = this.inputsContainer.Q<VisualElement>("cleaners-container");
        this.cleanersContent = this.cleanersContainer.Q<VisualElement>("cleaners-content");
        this.cleanersContainerHeader = this.cleanersContainer.Q<VisualElement>("header-container");

        this.addCleanerButtonContainer = this.cleanersContainerHeader.Q<VisualElement>("add-cleaner-button-container");
        this.addCleanerButton = this.addCleanerButtonContainer.Q<CustomButton>();

        this.detailsContentContainer = this.mainContentContainer.Q<VisualElement>("details-content-container");
        this.detailsContentContainer.RegisterCallback<ClickEvent>(evt => this.detailsInput.Focus());
        this.detailsInput = this.detailsContentContainer.Q<CustomInput>();

        this.reportsFooter = this.mainContainer.Q<VisualElement>("reports-footer");

        this.cancelButtonContainer = this.reportsFooter.Q<VisualElement>("cancel-button-container");
        this.cancelButton = this.cancelButtonContainer.Q<CustomButton>();

        this.saveButtonContainer = this.reportsFooter.Q<VisualElement>("save-button-container");
        this.saveButton = this.saveButtonContainer.Q<CustomButton>();
    }

    protected override void SetViewElements()
    {
    }

    protected override void SetupButtons()
    {
        this.addCleanerButton.RegisterCallback<ClickEvent>(evt => CreateCleanerRow());

        this.cancelButton.RegisterCallback<ClickEvent>(evt => QueryController.Active.ReturnToPreviousView());

        this.saveButton.RegisterCallback<ClickEvent>(evt =>
        {
            SetJobDetailProperties();

            ActionHelper.StringDelegate responseDelegate = (string response) =>
            {
                JSONNode resultNode = JSON.Parse(response);

                this.currentJobDetail.OnServerCreation(resultNode["objectId"], resultNode["createdAt"]);

                QueryController.Active.DetailsReportsQueryHandler.CurrentlySelectedDetailsReport.AddJobDetail(this.currentJobDetail);

                QueryController.Active.ChangeView(Enumerations.MainView.DetailsReports, Enumerations.Subview.Default);
            };

            AppController.Active.ServerCommunicator.CreateJobDetails(this.currentJobDetail, responseDelegate);
        });
    }

    protected override void SetupInputs()
    {
        UnityAction<DateTime> updateLabelAction = (DateTime dateTime) =>
        {
            this.currentDatePickerDate = dateTime;
            this.dateInput.value = this.currentDatePickerDate.Value.ToShortDateString();
        };

        DatePickerController.Active.AddDateSelectedAction(updateLabelAction);
        this.dateInput.isReadOnly = true;

        if (this.currentDatePickerDate.HasValue)
        {
            this.dateInput.RegisterCallback<ClickEvent>(evt => DatePickerController.Active.OpenDatePicker(this.currentDatePickerDate));
        }
        else
        {
            this.dateInput.RegisterCallback<ClickEvent>(evt => DatePickerController.Active.OpenDatePicker());
        }

        SetupTimeInput(this.startTimeHourInput, RegexHelper.HourRegex);
        SetupTimeInput(this.startTimeMinuteInput, RegexHelper.MinuteRegex);
        SetupTimeInput(this.finishTimeHourInput, RegexHelper.HourRegex);
        SetupTimeInput(this.finishTimeMinuteInput, RegexHelper.MinuteRegex);
    }

    private void SetupTimeInput(CustomInput input, Regex regex)
    {
        input.RegisterValueChangedCallback<string>(evt =>
        {
            if (String.IsNullOrEmpty(evt.newValue))
            {
                return;
            }

            if (!regex.IsMatch(evt.newValue.Replace(".", "")))
            {
                if (regex.IsMatch(evt.previousValue))
                {
                    input.SetValueWithoutNotify(evt.previousValue);
                }
                else
                {
                    input.SetValueWithoutNotify(String.Empty);
                }
            }
        });
    }
    #endregion

    #region Open Job Details

    public void OpenNewJobDetails()
    {
        UserNameReferenceDTM currentUserReference = AppController.Active.CleanerDataHandler.GetCurrentUserReference();

        if (ReferenceEquals(currentUserReference, null))
        {
            return;
        }

        this.currentJobDetail = new JobDetail();

        RefreshJobDetail();

        QueryController.Active.ChangeView(MainView.JobDetails, Subview.Default);
    }

    public void OpenExistingJobDetails(JobDetail jobDetail)
    {
        this.currentJobDetail = new JobDetail();
        // TODO Set up populating inputs with data
        QueryController.Active.ChangeView(MainView.JobDetails, Subview.Default);
    }

    #endregion

    #region View Change

    protected override void OnSubviewChanged()
    {
        base.OnSubviewChanged();

        RefreshJobDetail();
    }

    #endregion

    #region Getters/Setters

    #endregion

    #region Refresh/Update

    private void RefreshJobDetail()
    {
        RefreshCleanerRows();
    }

    private void RefreshCleanerRows()
    {
        this.cleanersContent.Clear();

        foreach (CleanerJobEntry cleaner in this.currentJobDetail.Cleaners)
        {
            CreateCleanerRow(cleaner.CleanerObjectId);
        }
    }

    #endregion

    #region Actions

    private List<UserNameReferenceDTM> GetValidCleanerNamesForAdding()
    {
        List<string> cleaners = this.currentJobDetail.Cleaners.Select(cleaner => cleaner.CleanerObjectId).ToList();

        return AppController.Active.CleanerDataHandler.UserNameReferences.Values
        .Where(reference => !cleaners.Contains(reference.userObjectId))
        .ToList();
    }

    private void OpenCleanerNameSelect(CleanerJobEntry cleanerJobEntry, Label nameLabel, ScrollView cleanerNameScrollView)
    {
        PopulateCleanerRowNameSelect(cleanerJobEntry, nameLabel, cleanerNameScrollView);

        if (cleanerNameScrollView.childCount <= 0)
        {
            return;
        }

        VisualElementHelper.SetElementDisplay(cleanerNameScrollView, DisplayStyle.Flex);
        cleanerNameScrollView.Focus();
    }

    #endregion

    #region CRUD

    private bool SetJobDetailProperties()
    {
        DateTime jobDate = new DateTime();
        DateTime startTime = new DateTime();
        DateTime finishTime = new DateTime();
        double startHoursValue;
        double startMinutesValue;
        double finishHoursValue;
        double finishMinutesValue;
        int startHoursModifier;
        int finishHoursModifier;

        if (DataValidationChecker.IsDateTimeStringValid(this.dateInput.value))
        {
            if (DateTime.TryParse(this.dateInput.value, out jobDate)
            && double.TryParse(this.startTimeHourInput.value, out startHoursValue)
            && double.TryParse(this.startTimeMinuteInput.value, out startMinutesValue)
            && double.TryParse(this.finishTimeHourInput.value, out finishHoursValue)
            && double.TryParse(this.finishTimeMinuteInput.value, out finishMinutesValue))
            {
                startTime = DateTime.Parse(this.dateInput.value);
                finishTime = DateTime.Parse(this.dateInput.value);

                startHoursModifier = (Enumerations.TimeOfDayEnum)this.startTimeOfDayField.value == Enumerations.TimeOfDayEnum.AM ? 0 : 12;
                finishHoursModifier = (Enumerations.TimeOfDayEnum)this.finishTimeOfDayField.value == Enumerations.TimeOfDayEnum.AM ? 0 : 12;

                startTime = startTime.AddHours(startHoursValue + startHoursModifier);
                startTime = startTime.AddMinutes(startMinutesValue);

                finishTime = finishTime.AddHours(finishHoursValue + finishHoursModifier);
                finishTime = finishTime.AddMinutes(finishMinutesValue);
            }
        }

        this.currentJobDetail.SetJobDetailProperties(
            QueryController.Active.DetailsReportsQueryHandler.CurrentlySelectedDetailsReport.ObjectId,
            this.clientNameInput.value,
            this.clientAddressInput.value,
            jobDate,
            startTime,
            finishTime,
            (Enumerations.JobTypeEnum)this.jobTypeInput.value,
            this.currentJobDetail.Cleaners,
            (Enumerations.PaymentTypeEnum)this.paymentTypeInput.value,
            this.detailsInput.value);

        return true;
    }

    #region Cleaner Row

    private void CreateCleanerRow(string cleanerId = "")
    {
        VisualElement newCleanerElement = this.cleanerRowBase.Instantiate();
        VisualElement nameLabelContainer = newCleanerElement.Q<VisualElement>("name-label-container");
        CustomLabel nameLabel = nameLabelContainer.Q<CustomLabel>();
        ScrollView cleanerNameScrollView = newCleanerElement.Q<ScrollView>();
        VisualElement selectCleanerNameButtonContainer = newCleanerElement.Q<VisualElement>("select-cleaner-button-container");
        CustomButton selectCleanerNameButton = selectCleanerNameButtonContainer.Q<CustomButton>();
        VisualElement deleteCleanerRowButtonContainer = newCleanerElement.Q<VisualElement>("delete-button-container");
        CustomButton deleteCleanerRowButton = deleteCleanerRowButtonContainer.Q<CustomButton>();
        CleanerJobEntry cleanerJobEntry = new CleanerJobEntry();
        VisualElement hoursInputContainer = newCleanerElement.Q<VisualElement>("hours-input-container");
        CustomInput hoursInput = hoursInputContainer.Q<CustomInput>();
        Action deleteCleanerRowAction = () =>
        {
            this.currentJobDetail.RemoveCleaner(cleanerJobEntry);
            newCleanerElement.parent.Remove(newCleanerElement);
        };
        string cleanerName;

        if (!String.IsNullOrEmpty(cleanerId) && AppController.Active.CleanerDataHandler.UserNameReferences.ContainsKey(cleanerId))
        {
            cleanerName = AppController.Active.CleanerDataHandler.UserNameReferences[cleanerId].userName;
            cleanerJobEntry.SetName(cleanerName);
            cleanerJobEntry.SetCleanerObjectId(cleanerId);
            nameLabel.text = cleanerName;
        }

        newCleanerElement.AddToClassList("cleaner-row");

        cleanerNameScrollView.contentContainer.Clear();
        cleanerNameScrollView.focusable = true;

        cleanerNameScrollView.RegisterCallback<BlurEvent>(evt =>
        {
            VisualElementHelper.SetElementDisplay(cleanerNameScrollView, DisplayStyle.None);

            if (String.IsNullOrEmpty(cleanerJobEntry.Name))
            {
                deleteCleanerRowAction.Invoke();
            }
        });

        if (AppController.Active.ServerCommunicator.CurrentUser.objectId == cleanerJobEntry.CleanerObjectId)
        {
            selectCleanerNameButtonContainer.style.visibility = Visibility.Hidden;
            selectCleanerNameButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
            deleteCleanerRowButtonContainer.style.visibility = Visibility.Hidden;
            deleteCleanerRowButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
        else
        {
            deleteCleanerRowButton.RegisterCallback<ClickEvent>(evt => deleteCleanerRowAction.Invoke());

            selectCleanerNameButton.RegisterCallback<ClickEvent>(evt => OpenCleanerNameSelect(cleanerJobEntry, nameLabel, cleanerNameScrollView));
        }

        SetupTimeInput(hoursInput, RegexHelper.FloatRegex);
        SetupCleanerHoursInput(cleanerJobEntry, hoursInput);

        this.cleanersContent.Add(newCleanerElement);

        if (String.IsNullOrEmpty(cleanerId))
        {
            OpenCleanerNameSelect(cleanerJobEntry, nameLabel, cleanerNameScrollView);
        }
    }

    private void PopulateCleanerRowNameSelect(CleanerJobEntry cleanerJobEntry, Label nameLabel, ScrollView cleanerNameScrollView)
    {
        cleanerNameScrollView.contentContainer.Clear();

        foreach (UserNameReferenceDTM entry in GetValidCleanerNamesForAdding())
        {
            CustomLabel cleanerNameLabel = new CustomLabel();

            cleanerNameLabel.AddToClassList("regular-font");
            cleanerNameLabel.style.color = Color.black;

            cleanerNameLabel.text = entry.userName;

            cleanerNameLabel.RegisterCallback<ClickEvent>(evt =>
            {
                cleanerJobEntry.SetName(cleanerNameLabel.text);
                nameLabel.text = cleanerNameLabel.text;
                VisualElementHelper.SetElementDisplay(cleanerNameScrollView, DisplayStyle.None);
                this.currentJobDetail.AddCleaner(cleanerJobEntry);
            });

            cleanerNameScrollView.contentContainer.Add(cleanerNameLabel);
        }
    }

    private void SetupCleanerHoursInput(CleanerJobEntry cleanerJobEntry, CustomInput hoursInput)
    {
        hoursInput.RegisterCallback<BlurEvent>(evt =>
            {
                float value;

                if (float.TryParse(hoursInput.value, out value))
                {
                    hoursInput.SetValueWithoutNotify(value.ToString());
                }
                else
                {
                    hoursInput.value = "0";
                }
            });


        hoursInput.RegisterValueChangedCallback<string>(evt =>
        {
            float hours;

            if (float.TryParse(evt.newValue, out hours))
            {
                cleanerJobEntry.SetHoursWorked(hours);
            }
        });
    }

    #endregion
    #endregion
}
