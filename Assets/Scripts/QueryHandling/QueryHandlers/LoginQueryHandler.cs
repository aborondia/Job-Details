using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using MainView = Enumerations.MainView;
using Subview = Enumerations.Subview;

public class LoginQueryHandler : QueryHandler
{
    private const string registration_success_message = "You have successfully registered. An administrator must verify your account before you can login.";
    private const string registration_failed_message = "Something went wrong. Please try again.";
    private VisualElement loginBody;
    private VisualElement confirmationBody;
    private VisualElement unregisteredLoginBody;
    private VisualElement unregisteredLoginBackButtonContainer;
    private CustomButton unregisteredLoginBackButton;
    private VisualElement confirmationLabelContainer;
    private CustomLabel confirmationLabel;
    private VisualElement confirmationBackButtonContainer;
    private CustomButton confirmationBackButton;
    private VisualElement userNameInputContainer;
    private CustomInput userNameInput;
    private CustomLabel userNameInputErrorLabel;
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
    private const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private static Regex emailRegex;
    private UnityEvent hideErroLabels = new UnityEvent();

    #region Initialization

    public override void Initialize()
    {
        base.Initialize();

        emailRegex = new Regex(emailPattern);

        AppController.Active.UserDataHandler.OnCurrentUserPopulatedEvent.AddListener(() =>
        {
            QueryController.Active.ChangeView(MainView.DetailsReports, Subview.Default);
        });

        AppController.Active.ServerCommunicator.OnSignInSuccessEvent.AddListener(() => ClearInputs());
        AppController.Active.ServerCommunicator.OnRegisterSuccessEvent.AddListener(() => ClearInputs());
    }

    protected override void InitializeElements()
    {
        this.confirmationBody = this.parentElement.Q<VisualElement>("confirmation-body");
        this.confirmationLabelContainer = this.confirmationBody.Q<VisualElement>("display-label-container");
        this.confirmationLabel = this.confirmationLabelContainer.Q<CustomLabel>();
        this.confirmationBackButtonContainer = this.confirmationBody.Q<VisualElement>("back-button-container");
        this.confirmationBackButton = this.confirmationBackButtonContainer.Q<CustomButton>();

        this.unregisteredLoginBody = this.parentElement.Q<VisualElement>("unregistered-login-body");
        this.unregisteredLoginBackButtonContainer = this.unregisteredLoginBody.Q<VisualElement>("back-button-container");
        this.unregisteredLoginBackButton = this.unregisteredLoginBackButtonContainer.Q<CustomButton>();

        this.loginBody = this.parentElement.Q<VisualElement>("login-body");
        this.userNameInputContainer = this.loginBody.Q<VisualElement>("display-name-input-container");
        this.userNameInput = this.userNameInputContainer.Q<CustomInput>();
        this.userNameInputErrorLabel = this.userNameInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

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
        this.AddSubviewElement(Subview.Login_Register, this.emailInputContainer);
        this.AddSubviewElement(Subview.Login_Register, this.passwordConfirmInputContainer);
        this.AddSubviewElement(Subview.Login_Register, this.loginBody);

        this.AddSubviewElement(Subview.Login_EnterCredentials, this.registerButtonContainer);
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.forgotPasswordButtonContainer);
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.loginBody);

        this.AddSubviewElement(Subview.Login_RegistrationComplete, this.confirmationBody);

        this.AddSubviewElement(Subview.Login_UnregisteredUserLogin, this.unregisteredLoginBody);
    }

    protected override void SetupInputs()
    {
        this.userNameInput.value = "dude@dude.com";
        this.passwordInput.value = "123";
        UpdateSubmitButtonState();

        this.emailInput.RegisterCallback<KeyDownEvent>(evt => OnEmailInputReturnButtonPressed(evt));
        this.emailInput.RegisterValueChangedCallback<string>(evt => OnEmailInputValueChanged(evt));
        this.emailInput.RegisterCallback<BlurEvent>(evt => OnEmailInputBlur(evt));
        this.emailInput.RegisterCallback<FocusEvent>(evt => OnEmailInputFocus(evt));
        this.hideErroLabels.AddListener(() => HideEmailErrorLabel());

        this.userNameInput.RegisterCallback<KeyDownEvent>(evt => OnUserNameInputReturnButtonPressed(evt));
        this.userNameInput.RegisterValueChangedCallback<string>(evt => OnUserNameInputValueChanged(evt));
        this.userNameInput.RegisterCallback<BlurEvent>(evt => OnUserNameInputBlur(evt));
        this.userNameInput.RegisterCallback<FocusEvent>(evt => OnUserNameInputFocus(evt));
        this.hideErroLabels.AddListener(() => HideUserNameErrorLabel());

        this.passwordInput.RegisterValueChangedCallback<string>(evt => OnPasswordInputValueChanged(evt));
        this.passwordInput.RegisterCallback<BlurEvent>(evt => OnPasswordInputBlur(evt));
        this.passwordInput.RegisterCallback<FocusEvent>(evt => OnPasswordInputFocus(evt));
        this.passwordInput.RegisterCallback<KeyDownEvent>(evt => OnPasswordInputReturnButtonPressed(evt));
        this.hideErroLabels.AddListener(() => HidePasswordConfirmErrorLabel());

        this.passwordConfirmInput.RegisterValueChangedCallback<string>(evt => OnPasswordConfirmInputValueChanged(evt));
        this.passwordConfirmInput.RegisterCallback<BlurEvent>(evt => OnPasswordConfirmInputBlur(evt));
        this.passwordConfirmInput.RegisterCallback<FocusEvent>(evt => OnPasswordConfirmInputFocus(evt));
        this.passwordConfirmInput.RegisterCallback<KeyDownEvent>(evt => OnPasswordConfirmInputReturnButtonPressed(evt));
        this.hideErroLabels.AddListener(() => HidePasswordConfirmErrorLabel());
    }

    protected override void SetupButtons()
    {
        this.registerButton.RegisterCallback<ClickEvent>(evt => OnRegisterButtonPressed());

        this.forgotPasswordButton.RegisterCallback<ClickEvent>(evt => OnForgotPasswordButtonPressed());
        this.forgotPasswordButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);

        this.submitButton.RegisterCallback<ClickEvent>(evt => OnSubmitButtonPressed());

        this.cancelButton.RegisterCallback<ClickEvent>(evt => OnCancelButtonPressed());
        this.cancelButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);

        this.confirmationBackButton.RegisterCallback<ClickEvent>(evt => QueryController.Active.ChangeView(MainView.Login, Subview.Login_EnterCredentials));

        this.unregisteredLoginBackButton.RegisterCallback<ClickEvent>(evt => QueryController.Active.ChangeView(MainView.Login, Subview.Login_EnterCredentials));
    }

    #endregion

    #region View Change

    protected override void OnAnyViewChanged()
    {
        base.OnAnyViewChanged();

        ClearInputs();
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
            HideEmailErrorLabel();
        }
        else
        {
            ShowEmailErrorLabel("Not a valid email address");
        }
    }

    private void OnEmailInputFocus(FocusEvent evt)
    {
        HideEmailErrorLabel();
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

    private void OnUserNameInputBlur(BlurEvent evt)
    {
        if (String.IsNullOrEmpty(this.userNameInput.value))
        {
            ShowUserNameErrorLabel("Display name cannot be empty");
        }
        else
        {
            HideUserNameErrorLabel();
        }
    }

    private void OnUserNameInputFocus(FocusEvent evt)
    {
        HideUserNameErrorLabel();
    }

    private void OnUserNameInputValueChanged(ChangeEvent<string> evt)
    {
        UpdateSubmitButtonState();
    }

    private void OnUserNameInputReturnButtonPressed(KeyDownEvent evt)
    {
        if (evt.keyCode != KeyCode.Return)
        {
            return;
        }

        this.emailInput.Focus();
    }

    private void OnPasswordInputBlur(BlurEvent evt)
    {
        if (String.IsNullOrEmpty(this.passwordInput.value))
        {
            ShowPasswordErrorLabel("Password cannot be empty");
        }
        else if (!String.IsNullOrEmpty(this.passwordConfirmInput.value)
        && !String.Equals(this.passwordInput.value, this.passwordConfirmInput.value))
        {
            ShowPasswordConfirmErrorLabel("Passwords do not match");
        }
        else
        {
            HidePasswordErrorLabel();
        }
    }

    private void OnPasswordInputFocus(FocusEvent evt)
    {
        HidePasswordErrorLabel();
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
            if (CredentialsValid())
            {
                SubmitLogin();
            }
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
            ShowPasswordConfirmErrorLabel("Password confirm cannot be empty");
        }
        else if (!String.Equals(this.passwordConfirmInput.value, this.passwordInput.value))
        {
            ShowPasswordConfirmErrorLabel("Passwords do not match");
        }
        else
        {
            HidePasswordConfirmErrorLabel();
        }
    }

    private void OnPasswordConfirmInputFocus(FocusEvent evt)
    {
        HidePasswordConfirmErrorLabel();
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
        else if (!CredentialsValid())
        {
            this.loginBody.Focus();

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

    private void SubmitLogin()
    {
        AppController.Active.ServerCommunicator.LogIn(new UserSignInDTM(this.userNameInput.value, this.passwordInput.value));
    }

    private void SubmitRegistration()
    {
        AppController.Active.ServerCommunicator.CheckRegistrationCredentials(
            this.userNameInput.value,
            this.emailInput.value,
            response =>
            {
                RegistrationValidationDTM validationDTM = JSONHelper.GetRegistrationValidationDTM(response);
                bool canRegister = true;

                if (validationDTM.emailExists)
                {
                    canRegister = false;
                    ShowEmailErrorLabel("That email is already registered");
                }

                if (validationDTM.usernameExists)
                {
                    canRegister = false;
                    ShowUserNameErrorLabel("That user name is already taken");
                }

                if (canRegister)
                {
                    UserSignupDTM userSignupDTM = new UserSignupDTM(
                        this.userNameInput.value,
                        this.emailInput.value,
                        this.passwordInput.value,
                        AppController.Active.UserDataHandler.Roles
                        .FirstOrDefault(entry => entry.Value.name == UserDataHandler._UserRoleServerName).Key);


                    AppController.Active.ServerCommunicator.CreateUser(userSignupDTM, success =>
                    {
                        if (success)
                        {
                            this.confirmationLabel.text = registration_success_message;
                        }
                        else
                        {
                            this.confirmationLabel.text = registration_failed_message;
                        }

                        QueryController.Active.ChangeView(MainView.Login, Subview.Login_RegistrationComplete);
                    });
                }
            });
    }

    private void ValidateRegistrationDetails()
    {

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

    private void ClearInputs()
    {
        this.userNameInput.value = String.Empty;
        this.emailInput.value = String.Empty;
        this.passwordInput.value = String.Empty;
        this.passwordConfirmInput.value = String.Empty;

        UpdateSubmitButtonState();
    }

    #endregion

    #region Error Handling

    private void ShowEmailErrorLabel(string errorText)
    {
        Debug.Log(errorText);
        VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Visible);
        Debug.Log(this.emailInputErrorLabel.resolvedStyle.visibility);
        this.emailInputErrorLabel.text = errorText;
    }

    private void HideEmailErrorLabel()
    {
        Debug.Log("hide");
        VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Hidden);
        Debug.Log(this.emailInputErrorLabel.resolvedStyle.visibility);
    }

    private void ShowUserNameErrorLabel(string errorText)
    {
        VisualElementHelper.SetElementVisibility(this.userNameInputErrorLabel, Visibility.Visible);
        this.userNameInputErrorLabel.text = errorText;
    }

    private void HideUserNameErrorLabel()
    {
        VisualElementHelper.SetElementVisibility(this.userNameInputErrorLabel, Visibility.Hidden);
    }

    private void ShowPasswordErrorLabel(string errorText)
    {
        VisualElementHelper.SetElementVisibility(this.passwordInputErrorLabel, Visibility.Visible);
        this.passwordInputErrorLabel.text = errorText;
    }

    private void HidePasswordErrorLabel()
    {
        VisualElementHelper.SetElementVisibility(this.passwordInputErrorLabel, Visibility.Hidden);
    }

    private void ShowPasswordConfirmErrorLabel(string errorText)
    {
        VisualElementHelper.SetElementVisibility(this.passwordConfirmInputErrorLabel, Visibility.Visible);
        this.passwordConfirmInputErrorLabel.text = errorText;
    }

    private void HidePasswordConfirmErrorLabel()
    {
        VisualElementHelper.SetElementVisibility(this.passwordConfirmInputErrorLabel, Visibility.Hidden);
    }

    #endregion

    #region Validation

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return emailRegex.IsMatch(email);
    }

    private bool CredentialsValid()
    {
        switch (QueryController.Active.CurrentSubview)
        {
            case Subview.Login_EnterCredentials:
                return !String.IsNullOrEmpty(this.userNameInput.value)
                && !String.IsNullOrEmpty(this.passwordInput.value);
            case Subview.Login_Register:
                return IsValidEmail(this.emailInput.value)
                && !String.IsNullOrEmpty(this.userNameInput.value)
                && !String.IsNullOrEmpty(this.passwordInput.value)
                && String.Equals(this.passwordInput.value, this.passwordConfirmInput.value);
            default:
                return false;
        }
    }

    #endregion
}
