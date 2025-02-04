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
    private const string registration_code_enter_message = "Please enter your verification code.";
    private const string registration_failed_message = "Something went wrong. Please try again.";
    private const string forgot_credentials_message = "Enter the email associated with your account below.";
    private const string password_reset_message = "Enter your new password, along with your password reset code below.";
    private VisualElement displayLabelContainer;
    private CustomLabel displayLabel;
    private VisualElement codeInputContainer;
    private VisualElement inputsContainer;
    private CustomInput codeInput;
    private CustomLabel codeInputErrorLabel;
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
    public VisualElement NavigationButtonsContainer => navigationButtonsContainer;
    private VisualElement optionsContainer;
    private VisualElement validateRegistrationOptionButtonContainer;
    private CustomButton validateRegistrationOptionButton;
    private VisualElement registerOptionButtonContainer;
    private CustomButton registerOptionButton;
    private VisualElement forgotUsernameOptionButtonContainer;
    private CustomButton forgotUsernameOptionButton;
    private VisualElement forgotPasswordOptionButtonContainer;
    private CustomButton forgotPasswordOptionButton;
    private VisualElement resetPasswordOptionButtonContainer;
    private CustomButton resetPasswordOptionButton;
    private const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private const string passwordPattern = @"^.{4,20}$";
    private static Regex emailRegex;
    private static Regex passwordRegex;
    private UnityEvent hideErrorLabels = new UnityEvent();

    #region Initialization

    public override void Initialize()
    {
        base.Initialize();

        emailRegex = new Regex(emailPattern);
        passwordRegex = new Regex(passwordPattern);

        AppController.Active.UserDataHandler.OnCurrentUserPopulatedEvent.AddListener(() =>
        {
            QueryController.Active.ChangeView(MainView.DetailsReports, Subview.Default);
        });

        AppController.Active.ServerCommunicator.OnLoginSuccessEvent.AddListener(() => ClearInputs());
        AppController.Active.ServerCommunicator.OnRegisterSuccessEvent.AddListener(() => ClearInputs());
    }

    protected override void InitializeElements()
    {
        this.displayLabelContainer = this.parentElement.Q<VisualElement>("display-label-container");
        this.displayLabel = this.displayLabelContainer.Q<CustomLabel>();
        this.inputsContainer = this.parentElement.Q<VisualElement>("inputs-container");
        this.codeInputContainer = this.inputsContainer.Q<VisualElement>("code-input-container");
        this.codeInput = this.codeInputContainer.Q<CustomInput>();
        this.codeInputErrorLabel = this.codeInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.userNameInputContainer = this.inputsContainer.Q<VisualElement>("display-name-input-container");
        this.userNameInput = this.userNameInputContainer.Q<CustomInput>();
        this.userNameInputErrorLabel = this.userNameInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.emailInputContainer = this.inputsContainer.Q<VisualElement>("email-input-container");
        this.emailInput = this.emailInputContainer.Q<CustomInput>();
        this.emailInputErrorLabel = this.emailInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.passwordInputContainer = this.inputsContainer.Q<VisualElement>("password-input-container");
        this.passwordInput = this.passwordInputContainer.Q<CustomInput>();
        this.passwordInputErrorLabel = this.passwordInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.passwordConfirmInputContainer = this.inputsContainer.Q<VisualElement>("password-confirm-input-container");
        this.passwordConfirmInput = this.passwordConfirmInputContainer.Q<CustomInput>();
        this.passwordConfirmInputErrorLabel = this.passwordConfirmInputContainer.Q<VisualElement>("error-label-container").Q<CustomLabel>();

        this.navigationButtonsContainer = this.parentElement.Q<VisualElement>("navigation-buttons-container");

        this.optionsContainer = this.parentElement.Q<VisualElement>("options-container");

        this.validateRegistrationOptionButtonContainer = this.optionsContainer.Q<VisualElement>("validate-registration-button-container");
        this.validateRegistrationOptionButton = this.validateRegistrationOptionButtonContainer.Q<CustomButton>();

        this.registerOptionButtonContainer = this.optionsContainer.Q<VisualElement>("register-button-container");
        this.registerOptionButton = this.registerOptionButtonContainer.Q<CustomButton>();

        this.forgotUsernameOptionButtonContainer = this.optionsContainer.Q<VisualElement>("forgot-username-button-container");
        this.forgotUsernameOptionButton = this.forgotUsernameOptionButtonContainer.Q<CustomButton>();

        this.forgotPasswordOptionButtonContainer = this.optionsContainer.Q<VisualElement>("forgot-password-button-container");
        this.forgotPasswordOptionButton = this.forgotPasswordOptionButtonContainer.Q<CustomButton>();

        this.resetPasswordOptionButtonContainer = this.optionsContainer.Q<VisualElement>("reset-password-button-container");
        this.resetPasswordOptionButton = this.resetPasswordOptionButtonContainer.Q<CustomButton>();
    }

    protected override void SetViewElements()
    {
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.userNameInputContainer);
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.passwordInputContainer);
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.registerOptionButtonContainer);
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.forgotPasswordOptionButtonContainer);
        this.AddSubviewElement(Subview.Login_EnterCredentials, this.forgotUsernameOptionButtonContainer);

        this.AddSubviewElement(Subview.Login_ForgotUsername, this.emailInputContainer);
        this.AddSubviewElement(Subview.Login_ForgotUsername, this.displayLabelContainer);

        this.AddSubviewElement(Subview.Login_ForgotPassword, this.emailInputContainer);
        this.AddSubviewElement(Subview.Login_ForgotPassword, this.resetPasswordOptionButtonContainer);
        this.AddSubviewElement(Subview.Login_ForgotPassword, this.displayLabelContainer);

        this.AddSubviewElement(Subview.Login_ResetPassword, this.codeInputContainer);
        this.AddSubviewElement(Subview.Login_ResetPassword, this.passwordInputContainer);
        this.AddSubviewElement(Subview.Login_ResetPassword, this.passwordConfirmInputContainer);
        this.AddSubviewElement(Subview.Login_ResetPassword, this.displayLabelContainer);

        this.AddSubviewElement(Subview.Login_Register, this.userNameInputContainer);
        this.AddSubviewElement(Subview.Login_Register, this.emailInputContainer);
        this.AddSubviewElement(Subview.Login_Register, this.passwordInputContainer);
        this.AddSubviewElement(Subview.Login_Register, this.passwordConfirmInputContainer);
        this.AddSubviewElement(Subview.Login_Register, this.validateRegistrationOptionButtonContainer);

        this.AddSubviewElement(Subview.Login_RegistrationValidation, this.codeInputContainer);
        this.AddSubviewElement(Subview.Login_RegistrationValidation, this.displayLabelContainer);
    }

    protected override void SetupInputs()
    {
        this.emailInput.RegisterCallback<KeyUpEvent>(evt => OnEmailInputReturnButtonPressed(evt));
        this.emailInput.RegisterValueChangedCallback<string>(evt => OnEmailInputValueChanged(evt));
        this.emailInput.RegisterCallback<BlurEvent>(evt => OnEmailInputBlur(evt));
        this.emailInput.RegisterCallback<FocusEvent>(evt => OnEmailInputFocus(evt));
        this.hideErrorLabels.AddListener(() => HideEmailErrorLabel());

        this.userNameInput.RegisterCallback<KeyUpEvent>(evt => OnUserNameInputReturnButtonPressed(evt));
        this.userNameInput.RegisterValueChangedCallback<string>(evt => OnUserNameInputValueChanged(evt));
        this.userNameInput.RegisterCallback<BlurEvent>(evt => OnUserNameInputBlur(evt));
        this.userNameInput.RegisterCallback<FocusEvent>(evt => OnUserNameInputFocus(evt));
        this.hideErrorLabels.AddListener(() => HideUserNameErrorLabel());

        this.passwordInput.RegisterValueChangedCallback<string>(evt => OnPasswordInputValueChanged(evt));
        this.passwordInput.RegisterCallback<BlurEvent>(evt => OnPasswordInputBlur(evt));
        this.passwordInput.RegisterCallback<FocusEvent>(evt => OnPasswordInputFocus(evt));
        this.passwordInput.RegisterCallback<KeyUpEvent>(evt => OnPasswordInputReturnButtonPressed(evt));
        this.hideErrorLabels.AddListener(() => HidePasswordErrorLabel());

        this.passwordConfirmInput.RegisterValueChangedCallback<string>(evt => OnPasswordConfirmInputValueChanged(evt));
        this.passwordConfirmInput.RegisterCallback<BlurEvent>(evt => OnPasswordConfirmInputBlur(evt));
        this.passwordConfirmInput.RegisterCallback<FocusEvent>(evt => OnPasswordConfirmInputFocus(evt));
        this.passwordConfirmInput.RegisterCallback<KeyUpEvent>(evt => OnPasswordConfirmInputReturnButtonPressed(evt));
        this.hideErrorLabels.AddListener(() => HidePasswordConfirmErrorLabel());

        this.codeInput.RegisterValueChangedCallback<string>(evt => OnCodeInputValueChanged(evt));
        this.codeInput.RegisterCallback<BlurEvent>(evt => OnCodeInputBlur(evt));
        this.codeInput.RegisterCallback<FocusEvent>(evt => OnCodeInputFocus(evt));
        this.codeInput.RegisterCallback<KeyUpEvent>(evt => OnCodeInputReturnButtonPressed(evt));
        this.hideErrorLabels.AddListener(() => HideCodeErrorLabel());
    }

    protected override void SetupButtons()
    {
        this.validateRegistrationOptionButton.RegisterCallback<ClickEvent>(evt => OnValidateRegistrationOptionButtonPressed());
        this.registerOptionButton.RegisterCallback<ClickEvent>(evt => OnRegisterOptionButtonPressed());
        this.forgotUsernameOptionButton.RegisterCallback<ClickEvent>(evt => OnForgotUsernameOptionButtonPressed());
        this.forgotPasswordOptionButton.RegisterCallback<ClickEvent>(evt => OnForgotPasswordOptionButtonPressed());
        this.resetPasswordOptionButton.RegisterCallback<ClickEvent>(evt => OnResetPasswordOptionButtonPressed());
    }

    #endregion

    #region View Change

    protected override void OnAnyViewChanged()
    {
        base.OnAnyViewChanged();

        ClearInputs();
        UpdateSubmitButtonState();

        this.hideErrorLabels.Invoke();
    }

    protected override void OnSubviewChanged()
    {
        base.OnSubviewChanged();

        UpdateConfirmationLabel();
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

    private void OnEmailInputReturnButtonPressed(KeyUpEvent evt)
    {
        OnReturnButtonPressed(evt, () =>
        {
            if (QueryController.Active.CurrentSubview == Subview.Login_Register)
            {
                this.passwordInput.Focus();
            }
            else if (CredentialsAreValid())
            {
                PerformSubmitAction();
            }
            else
            {
                this.parentElement.Focus();
            }
        });
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

    private void OnUserNameInputReturnButtonPressed(KeyUpEvent evt)
    {
        OnReturnButtonPressed(evt, () =>
        {
            if (QueryController.Active.CurrentSubview == Subview.Login_EnterCredentials)
            {
                this.passwordInput.Focus();
            }
            else
            {
                this.emailInput.Focus();
            }
        });
    }

    private void OnPasswordInputBlur(BlurEvent evt)
    {
        if (!IsValidPassword(this.passwordInput.value))
        {
            ShowPasswordErrorLabel("Password must be between 4 to 20 characters");
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

    private void OnPasswordInputReturnButtonPressed(KeyUpEvent evt)
    {
        OnReturnButtonPressed(evt, () =>
        {
            if (QueryController.Active.CurrentSubview == Subview.Login_EnterCredentials
            && CredentialsAreValid())
            {
                PerformSubmitAction();
            }
            else
            {
                this.passwordConfirmInput.Focus();
            }
        });
    }

    private void OnPasswordConfirmInputBlur(BlurEvent evt)
    {
        if (!String.IsNullOrEmpty(this.passwordInput.value)
        && !String.Equals(this.passwordConfirmInput.value, this.passwordInput.value))
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

    private void OnPasswordConfirmInputReturnButtonPressed(KeyUpEvent evt)
    {
        OnReturnButtonPressed(evt, () =>
        {
            if (QueryController.Active.CurrentSubview == Subview.Login_ResetPassword)
            {
                this.codeInput.Focus();
            }
            else if (CredentialsAreValid())
            {
                PerformSubmitAction();
            }
            else
            {
                this.parentElement.Focus();
            }
        });
    }

    private void OnCodeInputBlur(BlurEvent evt)
    {
        if (CredentialsAreValid())
        {
            return;
        }

        if (String.IsNullOrEmpty(this.codeInput.value))
        {
            ShowCodeErrorLabel("The input cannot be empty");
        }
        else
        {
            switch (QueryController.Active.CurrentSubview)
            {
                case Subview.Login_ForgotPassword:
                    ShowCodeErrorLabel("Please enter a valid email");
                    break;
                case Subview.Login_ForgotUsername:
                    ShowCodeErrorLabel("Please enter a valid email");
                    break;
                case Subview.Login_ResetPassword:
                case Subview.Login_RegistrationValidation:
                    break;
            }
        }
    }

    private void OnCodeInputFocus(FocusEvent evt)
    {
        HideCodeErrorLabel();
    }

    private void OnCodeInputValueChanged(ChangeEvent<string> evt)
    {
        UpdateSubmitButtonState();
    }

    private void OnCodeInputReturnButtonPressed(KeyUpEvent evt)
    {
        OnReturnButtonPressed(evt, () =>
        {
            if (CredentialsAreValid())
            {
                PerformSubmitAction();
            }
            else
            {
                this.parentElement.Focus();
            }
        });
    }

    private void OnReturnButtonPressed(KeyUpEvent evt, Action action)
    {
        if (evt.keyCode != KeyCode.Return)
        {
            return;
        }

        action?.Invoke();
    }

    #endregion

    #region Button Handling

    private void OnValidateRegistrationOptionButtonPressed()
    {
        QueryController.Active.ChangeView(MainView.Login, Subview.Login_RegistrationValidation);
    }

    private void OnRegisterOptionButtonPressed()
    {
        QueryController.Active.ChangeView(MainView.Login, Subview.Login_Register);
    }

    private void OnForgotUsernameOptionButtonPressed()
    {
        QueryController.Active.ChangeView(MainView.Login, Subview.Login_ForgotUsername);
    }

    private void OnForgotPasswordOptionButtonPressed()
    {
        QueryController.Active.ChangeView(MainView.Login, Subview.Login_ForgotPassword);
    }

    private void OnResetPasswordOptionButtonPressed()
    {
        QueryController.Active.ChangeView(MainView.Login, Subview.Login_ResetPassword);
    }

    public void PerformSubmitAction()
    {
        switch (QueryController.Active.CurrentSubview)
        {
            case Subview.Login_EnterCredentials:
                SubmitLogin();
                break;
            case Subview.Login_Register:
                SubmitRegistration();
                break;
            case Subview.Login_ForgotUsername:
                SubmitForgotUsernameRequest();
                break;
            case Subview.Login_ForgotPassword:
                SubmitForgotPasswordRequest();
                break;
            case Subview.Login_RegistrationValidation:
                SubmitUserRegistrationConfirmation();
                break;
            case Subview.Login_ResetPassword:
                SubmitResetRequestPassword();
                break;
        }
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
                            this.displayLabel.text = registration_code_enter_message;
                            QueryController.Active.ChangeView(MainView.Login, Subview.Login_RegistrationValidation);
                            QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null, "Please check your email for your verification code.");
                        }
                        else
                        {
                            QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null, registration_failed_message);
                        }
                    });
                }
                else
                {
                    QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null,
                    "Could not register. Please check the inputs and try again.");
                }
            });
    }

    private void SubmitUserRegistrationConfirmation()
    {
        AppController.Active.ServerCommunicator.VerifyRegistration(this.codeInput.value, () =>
        {
            QueryController.Active.ChangeView(MainView.Login, Subview.Login_EnterCredentials);
            QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null, "You have successfuly registered. Login to your account once an administrator has verified it.");
        });
    }

    private void SubmitForgotUsernameRequest()
    {
        AppController.Active.ServerCommunicator.SendForgottenUsername(this.emailInput.value, success =>
        {
            QueryController.Active.ChangeView(MainView.Login, Subview.Login_EnterCredentials);
            QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null,
            "If an account is found your username will be emailed to you.");
        });
    }

    private void SubmitForgotPasswordRequest()
    {
        AppController.Active.ServerCommunicator.SendForgotPasswordRequest(this.emailInput.value, success =>
        {
            QueryController.Active.ChangeView(MainView.Login, Subview.Login_ResetPassword);
            QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null,
            "If an account is found your reset code will be emailed to you.");
        });
    }

    private void SubmitResetRequestPassword()
    {
        AppController.Active.ServerCommunicator.SendPasswordResetRequest(this.codeInput.value, this.passwordInput.value, success =>
        {
            if (success)
            {
                QueryController.Active.ChangeView(MainView.Login, Subview.Login_EnterCredentials);
                QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null,
                "Your password has been successfully reset.");
            }
            else
            {
                QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null,
                "Could not reset password. Please ensure your code is valid.");
            }
        });
    }

    #endregion

    #region Update

    private void UpdateSubmitButtonState()
    {
        if (CredentialsAreValid())
        {
            QueryController.Active.NavigationButtonsQueryHandler.UpdateSubmitButtonState(CustomButton.ButtonStyleType.Regular);
        }
        else
        {
            QueryController.Active.NavigationButtonsQueryHandler.UpdateSubmitButtonState(CustomButton.ButtonStyleType.Disabled);
        }
    }

    private void UpdateConfirmationLabel()
    {
        string displayLabelText;

        switch (QueryController.Active.CurrentSubview)
        {
            case Subview.Login_ForgotPassword:
                displayLabelText = forgot_credentials_message;
                break;
            case Subview.Login_ForgotUsername:
                displayLabelText = forgot_credentials_message;
                break;
            case Subview.Login_ResetPassword:
                displayLabelText = password_reset_message;
                break;
            case Subview.Login_RegistrationValidation:
                displayLabelText = registration_code_enter_message;
                break;
            default:
                displayLabelText = String.Empty;
                return;
        }

        this.displayLabel.text = displayLabelText;
    }

    private void ClearInputs()
    {
        this.userNameInput.value = String.Empty;
        this.emailInput.value = String.Empty;
        this.passwordInput.value = String.Empty;
        this.passwordConfirmInput.value = String.Empty;
        this.codeInput.value = String.Empty;

        UpdateSubmitButtonState();
    }

    #endregion

    #region Error Handling

    private void ShowEmailErrorLabel(string errorText)
    {
        VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Visible);
        this.emailInputErrorLabel.text = errorText;
    }

    private void HideEmailErrorLabel()
    {
        VisualElementHelper.SetElementVisibility(this.emailInputErrorLabel, Visibility.Hidden);
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

    private void ShowCodeErrorLabel(string errorText)
    {
        VisualElementHelper.SetElementVisibility(this.codeInputErrorLabel, Visibility.Visible);
        this.codeInputErrorLabel.text = errorText;
    }

    private void HideCodeErrorLabel()
    {
        VisualElementHelper.SetElementVisibility(this.codeInputErrorLabel, Visibility.Hidden);
    }

    #endregion

    #region Validation

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return emailRegex.IsMatch(email);
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        return passwordRegex.IsMatch(password);
    }

    private bool CredentialsAreValid()
    {
        switch (QueryController.Active.CurrentSubview)
        {
            case Subview.Login_EnterCredentials:
                return !String.IsNullOrEmpty(this.userNameInput.value)
                && IsValidPassword(this.passwordInput.value);
            case Subview.Login_Register:
                return IsValidEmail(this.emailInput.value)
                && !String.IsNullOrEmpty(this.userNameInput.value)
                && IsValidPassword(this.passwordInput.value)
                && String.Equals(this.passwordInput.value, this.passwordConfirmInput.value);
            case Subview.Login_ForgotPassword:
                return IsValidEmail(this.emailInput.value);
            case Subview.Login_ForgotUsername:
                return IsValidEmail(this.emailInput.value);
            case Subview.Login_ResetPassword:
                return !String.IsNullOrEmpty(this.codeInput.value)
                && IsValidPassword(this.passwordInput.value)
                && String.Equals(this.passwordInput.value, this.passwordConfirmInput.value);
            case Subview.Login_RegistrationValidation:
                return !String.IsNullOrEmpty(this.codeInput.value);
            default:
                return false;
        }
    }

    #endregion
}
