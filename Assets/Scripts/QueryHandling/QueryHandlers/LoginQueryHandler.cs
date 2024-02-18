using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using MainView = EnumLibrary.MainView;
using Subview = EnumLibrary.Subview;

public class LoginQueryHandler : QueryHandler
{
    private VisualElement loginBody;
    private VisualElement emailInputContainer;
    private CustomInput emailInput;
    private CustomLabel emailInputErrorLabel;
    private VisualElement passwordInputContainer;
    private CustomInput passwordInput;
    private CustomLabel passwordInputErrorLabel;
    private VisualElement passwordConfirmInputContainer;
    private CustomInput passwordConfirmInput;
    private CustomLabel passwordConfirmInputErrorLabel;
    private VisualElement navigationButtonsContainer;
    private VisualElement submitButtonContainer;
    private CustomButton submitButton;
    private VisualElement cancelButtonContainer;
    private CustomButton cancelButton;
    private VisualElement optionsContainer;
    private VisualElement registerButtonContainer;
    private CustomButton registerButton;
    private VisualElement forgotPasswordButtonContainer;
    private CustomButton forgotPasswordButton;
    private UnityEvent hideErroLabels = new UnityEvent();

    #region Initialization

    public override void Initialize()
    {
        base.Initialize();

        AppController.Active.ServerCommunicator.OnSignInSuccessEvent.AddListener(() =>
        {
            QueryController.Active.ChangeView(MainView.JobDetails, Subview.JobDetails_ReportList);
        });

        AppController.Active.ServerCommunicator.OnRegisterSuccessEvent.AddListener(() =>
        {
            SubmitLogin();
        });
    }

    protected override void InitializeElements()
    {
        this.loginBody = this.parentElement.Q<VisualElement>("login-body");
        this.emailInputContainer = this.loginBody.Q<VisualElement>("email-input-container");
        this.emailInput = this.emailInputContainer.Q<CustomInput>();
        this.emailInputErrorLabel = this.emailInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.passwordInputContainer = this.loginBody.Q<VisualElement>("password-input-container");
        this.passwordInput = this.passwordInputContainer.Q<CustomInput>();
        this.passwordInputErrorLabel = this.passwordInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.passwordConfirmInputContainer = this.loginBody.Q<VisualElement>("password-confirm-input-container");
        this.passwordConfirmInput = this.passwordConfirmInputContainer.Q<CustomInput>();
        this.passwordConfirmInputErrorLabel = this.passwordConfirmInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.navigationButtonsContainer = this.loginBody.Q<VisualElement>("navigation-buttons-container");
        this.submitButtonContainer = this.loginBody.Q<VisualElement>("submit-button-container");
        this.submitButton = this.submitButtonContainer.Q<CustomButton>();
        this.cancelButtonContainer = this.loginBody.Q<VisualElement>("cancel-button-container");
        this.cancelButton = this.cancelButtonContainer.Q<CustomButton>();

        this.optionsContainer = this.loginBody.Q<VisualElement>("options-container");
        this.registerButtonContainer = this.optionsContainer.Q<VisualElement>("register-button-container");
        this.registerButton = this.registerButtonContainer.Q<CustomButton>();
        this.forgotPasswordButtonContainer = this.optionsContainer.Q<VisualElement>("forgot-password-button-container");
        this.forgotPasswordButton = this.forgotPasswordButtonContainer.Q<CustomButton>();

        UpdateSubmitButtonState();
    }

    protected override void SetViewElements()
    {
        this.AddSubviewElement(Subview.Login_Register, this.passwordConfirmInputContainer);

        this.AddSubviewElement(Subview.Login_EnterCredentials, this.registerButtonContainer);
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.forgotPasswordButtonContainer);
    }

    protected override void SetupInputs()
    {
        this.emailInput.value = "dude@dude.com";
        this.passwordInput.value = "123";
        UpdateSubmitButtonState();

        this.emailInput.RegisterCallback<KeyDownEvent>(evt => OnEmailInputReturnButtonPressed(evt));
        this.emailInput.RegisterValueChangedCallback<string>(evt => OnEmailInputValueChanged(evt));
        this.emailInput.RegisterCallback<BlurEvent>(evt => OnEmailInputBlur(evt));
        this.emailInput.RegisterCallback<FocusEvent>(evt => OnEmailInputFocus(evt));
        this.hideErroLabels.AddListener(() => VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Hidden));

        this.passwordInput.RegisterValueChangedCallback<string>(evt => OnPasswordInputValueChanged(evt));
        this.passwordInput.RegisterCallback<BlurEvent>(evt => OnPasswordInputBlur(evt));
        this.passwordInput.RegisterCallback<FocusEvent>(evt => OnPasswordInputFocus(evt));
        this.passwordInput.RegisterCallback<KeyDownEvent>(evt => OnPasswordInputReturnButtonPressed(evt));
        this.hideErroLabels.AddListener(() => VisualElementHelper.SetElementVisibility(this.passwordInputErrorLabel, Visibility.Hidden));

        this.passwordConfirmInput.RegisterValueChangedCallback<string>(evt => OnPasswordConfirmInputValueChanged(evt));
        this.passwordConfirmInput.RegisterCallback<BlurEvent>(evt => OnPasswordConfirmInputBlur(evt));
        this.passwordConfirmInput.RegisterCallback<FocusEvent>(evt => OnPasswordConfirmInputFocus(evt));
        this.passwordConfirmInput.RegisterCallback<KeyDownEvent>(evt => OnPasswordConfirmInputReturnButtonPressed(evt));
        this.hideErroLabels.AddListener(() => VisualElementHelper.SetElementVisibility(this.passwordConfirmInputErrorLabel, Visibility.Hidden));
    }

    protected override void SetupButtons()
    {
        this.registerButton.RegisterCallback<ClickEvent>(evt => OnRegisterButtonPressed());

        this.forgotPasswordButton.RegisterCallback<ClickEvent>(evt => OnForgotPasswordButtonPressed());
        this.forgotPasswordButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);

        this.submitButton.RegisterCallback<ClickEvent>(evt => OnSubmitButtonPressed());

        this.cancelButton.RegisterCallback<ClickEvent>(evt => OnCancelButtonPressed());
        this.cancelButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
    }

    #endregion

    #region View Change

    protected override void OnAnyViewChanged()
    {
        base.OnAnyViewChanged();

        UpdateSubmitButtonState();
        UpdateCancelButtonState();

        this.hideErroLabels.Invoke();
    }

    #endregion

    #region Input Handling

    private void OnEmailInputBlur(BlurEvent evt)
    {
        if (IsValidEmail(this.emailInput.value))
        {
            VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Hidden);
        }
        else
        {
            this.emailInputErrorLabel.text = "Not a valid email address";
            VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Visible);
        }
    }

    private void OnEmailInputFocus(FocusEvent evt)
    {
        VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Hidden);
    }

    private void OnEmailInputValueChanged(ChangeEvent<string> evt)
    {
        UpdateSubmitButtonState();
    }

    private void OnEmailInputReturnButtonPressed(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.Return)
        {
            return;
        }

        this.passwordInput.Focus();
    }

    private void OnPasswordInputBlur(BlurEvent evt)
    {
        if (String.IsNullOrEmpty(this.passwordInput.value))
        {
            this.passwordInputErrorLabel.text = "Password cannot be empty";
            VisualElementHelper.SetElementVisibility(this.passwordInputErrorLabel, Visibility.Visible);
        }
        else
        {
            VisualElementHelper.SetElementVisibility(this.passwordInputErrorLabel, Visibility.Hidden);
        }
    }

    private void OnPasswordInputFocus(FocusEvent evt)
    {
        VisualElementHelper.SetElementVisibility(this.passwordInputErrorLabel, Visibility.Hidden);
    }

    private void OnPasswordInputValueChanged(ChangeEvent<string> evt)
    {
        UpdateSubmitButtonState();
    }

    private void OnPasswordInputReturnButtonPressed(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.Return)
        {
            return;
        }

        if (QueryController.Active.CurrentSubview == Subview.Login_EnterCredentials)
        {
            SubmitLogin();
        }
        else if (QueryController.Active.CurrentSubview == Subview.Login_Register)
        {
            this.passwordConfirmInput.Focus();
        }
    }

    private void OnPasswordConfirmInputBlur(BlurEvent evt)
    {
        if (String.IsNullOrEmpty(this.passwordConfirmInput.value))
        {
            this.passwordConfirmInputErrorLabel.text = "Password confirm cannot be empty";
            VisualElementHelper.SetElementVisibility(this.passwordConfirmInputErrorLabel, Visibility.Visible);
        }
        else if (!String.Equals(this.passwordConfirmInput.value, this.passwordInput.value))
        {
            this.passwordConfirmInputErrorLabel.text = "Passwords do not match";
            VisualElementHelper.SetElementVisibility(this.passwordConfirmInputErrorLabel, Visibility.Visible);
        }
        else
        {
            VisualElementHelper.SetElementVisibility(this.passwordConfirmInputErrorLabel, Visibility.Hidden);
        }
    }

    private void OnPasswordConfirmInputFocus(FocusEvent evt)
    {
        VisualElementHelper.SetElementVisibility(this.passwordConfirmInputErrorLabel, Visibility.Hidden);
    }

    private void OnPasswordConfirmInputValueChanged(ChangeEvent<string> evt)
    {
        UpdateSubmitButtonState();
    }

    private void OnPasswordConfirmInputReturnButtonPressed(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.Return)
        {
            return;
        }

        SubmitRegistration();
    }

    #endregion

    #region Button Handling

    private void OnRegisterButtonPressed()
    {
        QueryController.Active.ChangeView(MainView.Login, Subview.Login_Register);
    }

    private void OnForgotPasswordButtonPressed()
    {
        QueryController.Active.ChangeView(MainView.Login, Subview.Login_ForgotPassword);
    }

    private void OnSubmitButtonPressed()
    {
        switch (QueryController.Active.CurrentSubview)
        {
            case Subview.Login_EnterCredentials:
                SubmitLogin();
                break;
            case Subview.Login_Register:
                SubmitRegistration();
                break;
        }
    }

    private void OnCancelButtonPressed()
    {
        QueryController.Active.ReturnToPreviousView();
    }

    #endregion

    #region Actions

    private void SubmitLogin(string email = "", string password = "")
    {
        if (String.IsNullOrEmpty(email))
        {
            email = this.emailInput.value;
        }

        if (String.IsNullOrEmpty(password))
        {
            password = this.passwordInput.value;
        }

        AppController.Active.ServerCommunicator.SignIn(new UserSignInDTM(this.emailInput.value, this.passwordInput.value));
    }

    private void SubmitRegistration()
    {
        AppController.Active.ServerCommunicator.CreateUser(new UserSignupDTM(this.emailInput.value, this.emailInput.value, this.passwordInput.value));
    }

    #endregion

    #region Update

    private void UpdateSubmitButtonState()
    {
        if (CredentialsValid())
        {
            this.submitButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
        }
        else
        {
            this.submitButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
    }

    private void UpdateCancelButtonState()
    {
        if (QueryController.Active.CurrentSubview == Subview.Login_EnterCredentials)
        {
            this.cancelButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
        else
        {
            this.cancelButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
        }
    }

    #endregion

    #region Validation

    public static bool IsValidEmail(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (string.IsNullOrEmpty(email))
            return false;

        Regex regex = new Regex(emailPattern);
        return regex.IsMatch(email);
    }

    private bool CredentialsValid()
    {
        if (QueryController.Active.CurrentSubview == Subview.Login_Register && !String.Equals(this.passwordInput.value, this.passwordConfirmInput.value))
        {
            return false;
        }

        if (IsValidEmail(this.emailInput.value) && !String.IsNullOrEmpty(this.passwordInput.value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
