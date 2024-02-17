using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MainView = EnumLibrary.MainView;
using Subview = EnumLibrary.Subview;

public class LoginQueryHandler : QueryHandler
{
    private VisualElement loginBody;
    private VisualElement emailInputContainer;
    private CustomInput emailInput;
    private VisualElement passwordInputContainer;
    private CustomInput passwordInput;
    private VisualElement submitButtonContainer;
    private CustomButton submitButton;
    private VisualElement optionsContainer;
    private VisualElement registerButtonContainer;
    private CustomButton registerButton;
    private VisualElement forgotPasswordButtonContainer;
    private CustomButton forgotPasswordButton;

    #region Initialization

    protected override void InitializeElements()
    {
        this.loginBody = this.parentElement.Q<VisualElement>("login-body");
        this.emailInputContainer = this.loginBody.Q<VisualElement>("email-input-container");
        this.emailInput = this.emailInputContainer.Q<CustomInput>();
        this.passwordInputContainer = this.loginBody.Q<VisualElement>("password-input-container");
        this.passwordInput = this.passwordInputContainer.Q<CustomInput>();

        this.submitButtonContainer = this.loginBody.Q<VisualElement>("submit-button-container");
        this.submitButton = this.submitButtonContainer.Q<CustomButton>();

        this.optionsContainer = this.loginBody.Q<VisualElement>("options-container");
        this.registerButtonContainer = this.optionsContainer.Q<VisualElement>("register-button-container");
        this.registerButton = this.registerButtonContainer.Q<CustomButton>();
        this.forgotPasswordButtonContainer = this.optionsContainer.Q<VisualElement>("forgot-password-button-container");
        this.forgotPasswordButton = this.forgotPasswordButtonContainer.Q<CustomButton>();
    }

    protected override void SetViewElements()
    {

    }

    protected override void SetupInputs()
    {

    }

    protected override void SetupButtons()
    {
        this.submitButton.RegisterCallback<ClickEvent>(evt => QueryController.Active.ChangeView(MainView.JobDetails, Subview.Default));
    }

    #endregion
}
