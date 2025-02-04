using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using MainView = Enumerations.MainView;
using Subview = Enumerations.Subview;

public class NavigationButtonsQueryHandler : QueryHandler
{
    [SerializeField] private MainView[] viewToDisplayIn;
    private VisualElement submitButtonContainer;
    private CustomButton submitButton;
    private CustomLabel submitButtonLabel;
    private VisualElement cancelButtonContainer;
    private CustomButton cancelButton;
    private CustomLabel cancelButtonLabel;
    private VisualElement originalParent;
    private int originalIndex;
    private Dictionary<MainView, bool> viewDisplaySettings = new Dictionary<MainView, bool>();

    #region Initialization

    public override void Initialize()
    {
        base.Initialize();

        this.originalParent = this.parentElement.parent;
        this.originalIndex = this.originalParent.IndexOf(this.parentElement);

        foreach (MainView mainView in Enum.GetValues(typeof(MainView)))
        {
            this.viewDisplaySettings.Add(mainView, this.viewToDisplayIn.Contains(mainView));
        }
    }

    protected override void InitializeElements()
    {
        this.submitButtonContainer = this.parentElement.Q<VisualElement>("submit-button-container");
        this.submitButton = this.submitButtonContainer.Q<CustomButton>();
        this.submitButtonLabel = this.submitButton.Q<CustomLabel>();
        this.cancelButtonContainer = this.parentElement.Q<VisualElement>("cancel-button-container");
        this.cancelButton = this.cancelButtonContainer.Q<CustomButton>();
        this.cancelButtonLabel = this.cancelButton.Q<CustomLabel>();
    }

    protected override void SetupButtons()
    {
        this.submitButton.RegisterCallback<ClickEvent>(evt => OnSubmitButtonPressed());
        this.cancelButton.RegisterCallback<ClickEvent>(evt => OnCancelButtonPressed());
    }

    protected override void SetupInputs()
    {
    }

    protected override void SetViewElements()
    {
        #region Login
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.submitButtonContainer);

        this.AddSubviewElement(Subview.Login_ForgotUsername, this.cancelButtonContainer);
        this.AddSubviewElement(Subview.Login_ForgotUsername, this.submitButtonContainer);

        this.AddSubviewElement(Subview.Login_ForgotPassword, this.cancelButtonContainer);
        this.AddSubviewElement(Subview.Login_ForgotPassword, this.submitButtonContainer);

        this.AddSubviewElement(Subview.Login_ResetPassword, this.cancelButtonContainer);
        this.AddSubviewElement(Subview.Login_ResetPassword, this.submitButtonContainer);

        this.AddSubviewElement(Subview.Login_Register, this.cancelButtonContainer);
        this.AddSubviewElement(Subview.Login_Register, this.submitButtonContainer);

        this.AddSubviewElement(Subview.Login_RegistrationValidation, this.cancelButtonContainer);
        this.AddSubviewElement(Subview.Login_RegistrationValidation, this.submitButtonContainer);
        #endregion

        #region Job Details
        this.AddMainViewElement(MainView.JobDetails, this.cancelButtonContainer);
        this.AddMainViewElement(MainView.JobDetails, this.submitButtonContainer);
        #endregion

        #region Users
        this.AddMainViewElement(MainView.Users, this.cancelButtonContainer);
        #endregion
    }

    #endregion

    #region View Change

    protected override void OnMainViewChanged()
    {
        base.OnMainViewChanged();

        if (this.viewDisplaySettings[QueryController.Active.CurrentMainView])
        {
            ShowParent();
            UpdateParent();
            UpdateSubmitButtonVisuals();
            UpdateCancelButtonVisuals();
        }
        else
        {
            HideParent();
        }
    }

    #endregion

    #region Actions

    private void OnSubmitButtonPressed()
    {
        switch (QueryController.Active.CurrentMainView)
        {
            case MainView.Login:
                QueryController.Active.LoginQueryHandler.PerformSubmitAction();
                break;
            case MainView.JobDetails:
                QueryController.Active.JobDetailsQueryHandler.SaveDetails();
                break;
        }
    }

    private void OnCancelButtonPressed()
    {
        switch (QueryController.Active.CurrentMainView)
        {
            case MainView.JobDetails:
                QueryController.Active.JobDetailsQueryHandler.CloseJobDetails();
            break;
            default:
                QueryController.Active.ReturnToPreviousView();
                break;
        }
    }

    #endregion

    #region Updates

    private void UpdateParent()
    {
        switch (QueryController.Active.CurrentMainView)
        {
            case MainView.Login:
                if (this.parentElement.parent != QueryController.Active.LoginQueryHandler.NavigationButtonsContainer)
                {
                    QueryController.Active.LoginQueryHandler.NavigationButtonsContainer.Add(this.parentElement);
                }
                break;
            default:
                if (this.parentElement.parent != this.originalParent)
                {
                    this.originalParent.Insert(this.originalIndex, this.parentElement);
                }
                break;
        }
    }

    public void UpdateSubmitButtonState(CustomButton.ButtonStyleType buttonStyleType)
    {
        this.submitButton.ReinitializeButton(buttonStyleType);
    }

    private void UpdateSubmitButtonVisuals()
    {
        switch (QueryController.Active.CurrentMainView)
        {
            case MainView.JobDetails:
                this.submitButton.ReinitializeButton(CustomButton.ButtonStyleType.Secondary);
                this.submitButtonLabel.text = "Save";
                break;
            default:
                // this.submitButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
                this.submitButtonLabel.text = "Submit";
                break;
        }
    }

    private void UpdateCancelButtonVisuals()
    {
        switch (QueryController.Active.CurrentMainView)
        {
            case MainView.JobDetails:
                this.cancelButtonLabel.text = "Cancel";
                break;
            default:
                this.cancelButtonLabel.text = "Back";
                break;
        }
    }

    #endregion
}
