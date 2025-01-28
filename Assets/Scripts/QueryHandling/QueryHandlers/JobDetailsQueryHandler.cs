using System;
using System.Collections.Generic;
using UnityEngine;
using MainView = Enumerations.MainView;
using Subview = Enumerations.Subview;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using UnityEngine.Events;
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
    private DropdownField startTimeHourInput;
    private DropdownField startTimeMinuteInputDouble;
    private DropdownField startTimeMinuteInputSingle;
    private CustomEnumField startTimeOfDayField;
    private VisualElement finishTimeInputsContainer;
    private VisualElement finishTimeInputContainer;
    private DropdownField finishTimeHourInput;
    private DropdownField finishTimeMinuteDoubleInput;
    private DropdownField finishTimeMinuteSingleInput;
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
    private DetailsReport currentDetailsReport;
    private JobDetail currentJobDetail;
    public JobDetail CurrentJobDetail => currentJobDetail;
    private bool editingExistingDetails;
    private DateTime? currentDatePickerDate;

    #region  Initlialization

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void InitializeElements()
    {
        this.mainContainer = this.parentElement.Q<VisualElement>("main");
        this.mainContentContainer = this.mainContainer.Q<VisualElement>("main-content");
        this.inputsContainer = this.mainContentContainer.Q<VisualElement>("inputs-container");

        this.clientNameInputContainer = this.inputsContainer.Q<VisualElement>("client-name-input-container");
        this.clientNameInput = this.clientNameInputContainer.Q<CustomInput>();
        this.clientNameInput.RegisterCallback<BlurEvent>(evt => OnJobDetailsChanged());

        this.clientAddressInputContainer = this.inputsContainer.Q<VisualElement>("client-address-input-container");
        this.clientAddressInput = this.clientAddressInputContainer.Q<CustomInput>();
        this.clientAddressInput.RegisterCallback<BlurEvent>(evt => OnJobDetailsChanged());

        this.dateInputContainer = this.inputsContainer.Q<VisualElement>("date-input-container");
        this.dateInput = this.dateInputContainer.Q<CustomInput>();
        this.dateInput.RegisterCallback<BlurEvent>(evt => OnJobDetailsChanged());

        this.startTimeInputsContainer = this.inputsContainer.Q<VisualElement>("start-time-inputs-container");
        this.startTimeInputContainer = this.startTimeInputsContainer.Q<VisualElement>("start-time-input-container");
        this.startTimeHourInput = this.startTimeInputContainer.Q<VisualElement>("hour-dropdown").Q<DropdownField>();
        this.startTimeHourInput.RegisterCallback<BlurEvent>(evt => OnJobDetailsChanged());

        this.startTimeMinuteInputDouble = this.startTimeInputContainer.Q<VisualElement>("minute-dropdown-double").Q<DropdownField>();
        this.startTimeMinuteInputSingle = this.startTimeInputContainer.Q<VisualElement>("minute-dropdown-single").Q<DropdownField>();
        this.startTimeOfDayField = this.startTimeInputContainer.Q<CustomEnumField>();

        this.finishTimeInputsContainer = this.inputsContainer.Q<VisualElement>("finish-time-inputs-container");
        this.finishTimeInputContainer = this.finishTimeInputsContainer.Q<VisualElement>("finish-time-input-container");
        this.finishTimeHourInput = this.finishTimeInputContainer.Q<VisualElement>("hour-dropdown").Q<DropdownField>();
        this.finishTimeHourInput.RegisterCallback<BlurEvent>(evt => OnJobDetailsChanged());

        this.finishTimeMinuteDoubleInput = this.finishTimeInputContainer.Q<VisualElement>("minute-dropdown-double").Q<DropdownField>();
        this.finishTimeMinuteSingleInput = this.finishTimeInputContainer.Q<VisualElement>("minute-dropdown-single").Q<DropdownField>();
        this.finishTimeOfDayField = this.finishTimeInputContainer.Q<CustomEnumField>();
        this.finishTimeOfDayField.RegisterCallback<BlurEvent>(evt => OnJobDetailsChanged());

        this.jobTypeInputContainer = this.inputsContainer.Q<VisualElement>("job-type-input-container");
        this.jobTypeInput = this.jobTypeInputContainer.Q<CustomEnumField>();
        this.jobTypeInput.RegisterValueChangedCallback(evt => OnJobDetailsChanged());

        this.paymentTypeInputContainer = this.inputsContainer.Q<VisualElement>("payment-type-input-container");
        this.paymentTypeInput = this.paymentTypeInputContainer.Q<CustomEnumField>();
        this.paymentTypeInput.RegisterValueChangedCallback(evt => OnJobDetailsChanged());

        this.cleanersContainer = this.inputsContainer.Q<VisualElement>("cleaners-container");
        this.cleanersContent = this.cleanersContainer.Q<VisualElement>("cleaners-content");
        this.cleanersContainerHeader = this.cleanersContainer.Q<VisualElement>("header-container");

        this.addCleanerButtonContainer = this.cleanersContainerHeader.Q<VisualElement>("add-cleaner-button-container");
        this.addCleanerButton = this.addCleanerButtonContainer.Q<CustomButton>();

        this.detailsContentContainer = this.mainContentContainer.Q<VisualElement>("details-content-container");
        this.detailsContentContainer.RegisterCallback<ClickEvent>(evt => this.detailsInput.Focus());
        this.detailsInput = this.detailsContentContainer.Q<CustomInput>();
        this.detailsInput.RegisterCallback<BlurEvent>(evt => OnJobDetailsChanged());
    }

    protected override void SetViewElements()
    {
    }

    protected override void SetupButtons()
    {
        this.addCleanerButton.RegisterCallback<ClickEvent>(evt =>
        {
            QueryController.Active.PopupsQueryHandler.OpenNameSelectPopup(
                selectedName =>
                {
                    this.currentJobDetail.AddCleaner(new CleanerJobEntry(selectedName));
                    RefreshJobDetail();
                });
        });
    }

    protected override void SetupInputs()
    {
        List<string> hourInputChoices = new List<string>();
        List<string> minuteDoubleInputChoices = new List<string>();
        List<string> minuteSingleInputChoices = new List<string>();

        for (int i = 0; i <= 12; i++)
        {
            if (i <= 5)
            {
                minuteDoubleInputChoices.Add(i.ToString());
            }

            if (i <= 9)
            {
                minuteSingleInputChoices.Add(i.ToString());
            }

            if (i > 0)
            {
                hourInputChoices.Add(i.ToString());
            }
        }

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

        SetupTimeDropDown(this.startTimeHourInput, hourInputChoices);
        SetupTimeDropDown(this.startTimeMinuteInputDouble, minuteDoubleInputChoices);
        SetupTimeDropDown(this.startTimeMinuteInputSingle, minuteSingleInputChoices);
        SetupTimeDropDown(this.finishTimeHourInput, hourInputChoices);
        SetupTimeDropDown(this.finishTimeMinuteDoubleInput, minuteDoubleInputChoices);
        SetupTimeDropDown(this.finishTimeMinuteSingleInput, minuteSingleInputChoices);
    }

    private void SetupTimeInput(CustomInput input, Regex regex)
    {
        input.RegisterValueChangedCallback<string>(evt =>
        {
            if (String.IsNullOrEmpty(evt.newValue))
            {
                return;
            }

            if (!regex.IsMatch(evt.newValue.Replace(".", "")) && !regex.IsMatch(evt.newValue))
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

    private void SetupTimeDropDown(DropdownField input, List<string> values)
    {
        input.choices = values;
    }

    #endregion

    #region Open Job Details

    public void OpenNewJobDetails(DetailsReport detailsReport)
    {
        this.currentDetailsReport = detailsReport;
        this.editingExistingDetails = false;
        this.currentJobDetail = new JobDetail();
        this.currentJobDetail.AddCleaner(new CleanerJobEntry(AppController.Active.UserDataHandler.CurrentUser.DTM.displayName));
        RefreshJobDetail();
        QueryController.Active.ChangeView(MainView.JobDetails, Subview.Default);
    }

    public void OpenExistingJobDetails(DetailsReport detailsReport, JobDetail jobDetail)
    {
        this.currentDetailsReport = detailsReport;
        this.editingExistingDetails = true;
        this.currentJobDetail = jobDetail;
        RefreshJobDetail();
        QueryController.Active.ChangeView(MainView.JobDetails, Subview.Default);
    }

    #endregion

    #region View Change

    protected override void OnSubviewChanged()
    {
        base.OnSubviewChanged();

        if (QueryController.Active.CurrentMainView != this.mainView)
        {
            return;
        }

        RefreshJobDetail();
    }

    #endregion

    #region Getters/Setters

    #endregion

    #region Refresh/Update

    private void OnJobDetailsChanged()
    {
        SetJobDetailProperties();
        RefreshJobDetail();
    }

    private void RefreshJobDetail()
    {
        RefreshClientName();
        RefreshClientAddress();
        RefreshDate();
        RefreshJobType();
        RefreshPaymentType();
        RefreshStartTime();
        RefreshFinishTime();
        RefreshCleanerRows();
        RefreshAddCleanerButton();
        RefreshDetailsDescription();
    }

    private void RefreshClientName()
    {
        if (ReferenceEquals(this.currentJobDetail.ClientName, null))
        {
            this.clientNameInput.SetValueWithoutNotify(String.Empty);
        }
        else
        {
            this.clientNameInput.SetValueWithoutNotify(this.currentJobDetail.ClientName);
        }

    }

    private void RefreshClientAddress()
    {
        if (ReferenceEquals(this.currentJobDetail.ClientAddress, null))
        {
            this.clientAddressInput.SetValueWithoutNotify(String.Empty);
        }
        else
        {
            this.clientAddressInput.SetValueWithoutNotify(this.currentJobDetail.ClientAddress);
        }

    }

    private void RefreshDate()
    {
        if (ReferenceEquals(this.currentJobDetail.JobDate, null) || this.currentJobDetail.JobDate == DateTime.MinValue)
        {
            this.dateInput.SetValueWithoutNotify(DateTime.Now.ToShortDateString());
        }
        else
        {
            this.dateInput.SetValueWithoutNotify(this.currentJobDetail.JobDate.ToShortDateString());
        }
    }

    private void RefreshJobType()
    {
        if (ReferenceEquals(this.currentJobDetail.JobType, null))
        {
            this.jobTypeInput.SetValueWithoutNotify((Enumerations.JobTypeEnum)0);

            return;
        }

        this.jobTypeInput.SetValueWithoutNotify(this.currentJobDetail.JobType);
    }

    private void RefreshPaymentType()
    {
        if (ReferenceEquals(this.currentJobDetail.PaymentType, null))
        {
            this.paymentTypeInput.SetValueWithoutNotify((Enumerations.PaymentTypeEnum)0);

            return;
        }

        this.paymentTypeInput.SetValueWithoutNotify(this.currentJobDetail.PaymentType);
    }

    private void RefreshStartTime()
    {
        bool isPMStartTime;
        int startHours;
        int startMinutes;
        string startHoursString;

        isPMStartTime = this.currentJobDetail.StartTime.Hour > 12;
        startHours = this.currentJobDetail.StartTime.Hour;
        startMinutes = this.currentJobDetail.StartTime.Minute;
        startHoursString = startHours > 12 ? (startHours - 12).ToString() : startHours.ToString();

        this.startTimeOfDayField.SetValueWithoutNotify(isPMStartTime ? Enumerations.TimeOfDayEnum.PM : Enumerations.TimeOfDayEnum.AM);
        this.startTimeHourInput.SetValueWithoutNotify(startHoursString);
        this.startTimeMinuteInputDouble.SetValueWithoutNotify((startMinutes / 10).ToString());
        this.startTimeMinuteInputSingle.SetValueWithoutNotify((startMinutes % 10).ToString());
    }

    private void RefreshFinishTime()
    {
        bool isPMFinishTime;
        int finishHours;
        int finishMinutes;
        string finishHoursString;

        isPMFinishTime = this.currentJobDetail.FinishTime.Hour > 12;
        finishHours = this.currentJobDetail.FinishTime.Hour;
        finishMinutes = this.currentJobDetail.FinishTime.Minute;
        finishHoursString = finishHours > 12 ? (finishHours - 12).ToString() : finishHours.ToString();

        this.finishTimeOfDayField.SetValueWithoutNotify(isPMFinishTime ? Enumerations.TimeOfDayEnum.PM : Enumerations.TimeOfDayEnum.AM);
        this.finishTimeHourInput.SetValueWithoutNotify(finishHoursString);
        this.finishTimeMinuteDoubleInput.SetValueWithoutNotify((finishMinutes / 10).ToString());
        this.finishTimeMinuteSingleInput.SetValueWithoutNotify((finishMinutes % 10).ToString());
    }

    private void RefreshCleanerRows()
    {
        this.cleanersContent.Clear();

        if (ReferenceEquals(this.currentJobDetail.Cleaners, null))
        {
            return;
        }

        foreach (CleanerJobEntry cleaner in this.currentJobDetail.Cleaners)
        {
            CreateCleanerRow(cleaner);
        }
    }

    private void RefreshAddCleanerButton()
    {
        IEnumerable<string> availableCleaners;

        if (this.currentJobDetail.Cleaners.Count >= 6)
        {
            this.addCleanerButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);

            return;
        }

        availableCleaners = AppController.Active.UserDataHandler.Users.Keys
        .Where(userName => !this.currentJobDetail.Cleaners
        .Select(cleaner => cleaner.Name).Contains(userName));

        if (availableCleaners.Count() > 0)
        {
            this.addCleanerButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
        }
        else
        {
            this.addCleanerButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
    }

    private void RefreshDetailsDescription()
    {
        if (String.IsNullOrEmpty(this.currentJobDetail?.Description))
        {
            this.detailsInput.SetValueWithoutNotify(String.Empty);

            return;
        }

        this.detailsInput.SetValueWithoutNotify(this.currentJobDetail.Description);
    }

    #endregion

    #region Actions

    public void SaveDetails()
    {
        SetJobDetailProperties();

        ActionHelper.BoolDelegate responseDelegate = success =>
        {
            if (success)
            {
                QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null, "Details saved sucessfully.");
                AppController.Active.DetailsReportsHandler.RefreshReports();
                QueryController.Active.ChangeView(MainView.DetailsReports, Subview.Default);
            }
            else
            {
                QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null, "Something went wrong. Please try again later.");
            }
        };

        if (this.editingExistingDetails)
        {
            AppController.Active.ServerCommunicator.UpdateJobDetails(this.currentJobDetail, responseDelegate);
        }
        else
        {
            AppController.Active.ServerCommunicator.CreateJobDetails(this.currentJobDetail, responseDelegate);
        }
    }

    #endregion

    #region CRUD

    private bool SetJobDetailProperties()
    {
        DateTime jobDate = new DateTime();
        DateTime startTime = new DateTime();
        DateTime finishTime = new DateTime();
        double startHoursValue;
        double startMinutesDoubleValue;
        double startMinutesSingleValue;
        double finishHoursValue;
        double finishMinutesDoubleValue;
        double finishMinutesSingleValue;
        int startHoursModifier;
        int finishHoursModifier;

        if (DataValidationChecker.IsDateTimeStringValid(this.dateInput.value))
        {
            if (DateTime.TryParse(this.dateInput.value, out jobDate)
            && double.TryParse(this.startTimeHourInput.value, out startHoursValue)
            && double.TryParse(this.startTimeMinuteInputSingle.value, out startMinutesSingleValue)
            && double.TryParse(this.startTimeMinuteInputDouble.value, out startMinutesDoubleValue)
            && double.TryParse(this.finishTimeHourInput.value, out finishHoursValue)
            && double.TryParse(this.finishTimeMinuteSingleInput.value, out finishMinutesSingleValue)
            && double.TryParse(this.finishTimeMinuteDoubleInput.value, out finishMinutesDoubleValue))
            {
                startTime = DateTime.Parse(this.dateInput.value);
                finishTime = DateTime.Parse(this.dateInput.value);

                startHoursModifier = (Enumerations.TimeOfDayEnum)this.startTimeOfDayField.value == Enumerations.TimeOfDayEnum.AM ? 0 : 12;
                finishHoursModifier = (Enumerations.TimeOfDayEnum)this.finishTimeOfDayField.value == Enumerations.TimeOfDayEnum.AM ? 0 : 12;

                startTime = startTime.AddHours(startHoursValue + startHoursModifier);
                startTime = startTime.AddMinutes(startMinutesSingleValue + (startMinutesDoubleValue * 10));

                finishTime = finishTime.AddHours(finishHoursValue + finishHoursModifier);
                finishTime = finishTime.AddMinutes(finishMinutesSingleValue + (finishMinutesDoubleValue * 10));
            }
        }

        this.currentJobDetail.SetJobDetailProperties(
            this.currentDetailsReport.ObjectId,
            this.clientNameInput.value,
            this.clientAddressInput.value,
            jobDate,
            startTime,
            finishTime,
            (Enumerations.JobTypeEnum)this.jobTypeInput.value,
            this.currentJobDetail.Cleaners,
            (Enumerations.PaymentTypeEnum)this.paymentTypeInput.value,
            this.detailsInput.value,
            this.currentJobDetail.ObjectId);

        return true;
    }

    #region Cleaner Row

    private void CreateCleanerRow(CleanerJobEntry cleanerJobEntry)
    {
        VisualElement newCleanerElement = this.cleanerRowBase.Instantiate();
        VisualElement nameLabelContainer = newCleanerElement.Q<VisualElement>("name-label-container");
        CustomLabel nameLabel = nameLabelContainer.Q<CustomLabel>();
        VisualElement selectCleanerNameButtonContainer = newCleanerElement.Q<VisualElement>("select-cleaner-button-container");
        CustomButton selectCleanerNameButton = selectCleanerNameButtonContainer.Q<CustomButton>();
        VisualElement deleteCleanerRowButtonContainer = newCleanerElement.Q<VisualElement>("delete-button-container");
        CustomButton deleteCleanerRowButton = deleteCleanerRowButtonContainer.Q<CustomButton>();
        VisualElement hoursInputContainer = newCleanerElement.Q<VisualElement>("hours-input-container");
        CustomInput hoursInput = hoursInputContainer.Q<CustomInput>();
        Action editCleanerNameAction = () =>
        {
            QueryController.Active.PopupsQueryHandler.OpenNameSelectPopup(
                selectedName =>
                {
                    cleanerJobEntry.SetName(selectedName);
                    RefreshJobDetail();
                }
            );
        };
        Action deleteCleanerRowAction = () =>
        {
            QueryController.Active.PopupsQueryHandler.OpenConfirmationPopup(() =>
            {
                this.currentJobDetail.RemoveCleaner(cleanerJobEntry);
                RefreshJobDetail();
            });
        };

        if (ReferenceEquals(cleanerJobEntry, null))
        {
            return;
        }

        if (!String.IsNullOrEmpty(cleanerJobEntry.Name))
        {
            cleanerJobEntry.SetName(cleanerJobEntry.Name);
            nameLabel.text = cleanerJobEntry.Name;
        }

        newCleanerElement.AddToClassList("cleaner-row");

        deleteCleanerRowButton.RegisterCallback<ClickEvent>(evt => deleteCleanerRowAction.Invoke());
        selectCleanerNameButton.RegisterCallback<ClickEvent>(evt => editCleanerNameAction.Invoke());

        SetupTimeInput(hoursInput, RegexHelper.FloatRegex);
        SetupCleanerHoursInput(cleanerJobEntry, hoursInput);

        hoursInput.value = cleanerJobEntry.HoursWorked.ToString();

        this.cleanersContent.Add(newCleanerElement);
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
