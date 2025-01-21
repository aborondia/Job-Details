using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupsQueryHandler : QueryHandler
{
    #region Confirmation Popup
    [SerializeField] private string defaultConfirmationMessage = "This action cannot be undone.";
    private VisualElement confirmationPopup;
    private VisualElement confirmationMessageLabelContainer;
    private CustomLabel confirmationMessageLabel;
    private VisualElement confirmationButtonsContainer;
    private VisualElement confirmationCancelButtonContainer;
    private CustomButton confirmationCancelButton;
    private VisualElement confirmationConfirmButtonContainer;
    private CustomButton confirmationConfirmButton;
    private Action confirmAction;

    #endregion

    #region Send Email Popup
    private const string recipient_placeholder = "Choose recipient";
    private VisualElement sendEmailPopup;
    private VisualElement emailrecipientsScrollviewContainer;
    private CustomInput emailRecipientPlaceholderInput;
    private ScrollView emailRecipientsScrollview;
    private VisualElement emailButtonsContainer;
    private VisualElement sendEmailCancelButtonContainer;
    private CustomButton sendEmailCancelButton;
    private VisualElement sendEmailConfirmButtonContainer;
    private CustomButton sendEmailConfirmButton;
    private VisualElement sendEmailContentInputContainer;
    private CustomInput sendEmailContentInput;
    private string currentEmailRecipient;
    private Action<string> sendEmailAction;

    #endregion

    private Action closePopupsAction;

    public override void Initialize()
    {
        base.Initialize();

        SetupClosePopupAction();
        ClosePopup();
    }


    protected override void InitializeElements()
    {
        this.confirmationPopup = this.parentElement.Q<VisualElement>("confirmation-popup");
        this.confirmationMessageLabelContainer = this.confirmationPopup.Q<VisualElement>("display-message-container");
        this.confirmationMessageLabel = this.confirmationMessageLabelContainer.Q<CustomLabel>();

        this.confirmationButtonsContainer = this.confirmationPopup.Q<VisualElement>("buttons-container");
        this.confirmationCancelButtonContainer = this.confirmationButtonsContainer.Q<VisualElement>("cancel-button-container");
        this.confirmationCancelButton = this.confirmationCancelButtonContainer.Q<CustomButton>();
        this.confirmationConfirmButtonContainer = this.confirmationButtonsContainer.Q<VisualElement>("confirm-button-container");
        this.confirmationConfirmButton = this.confirmationConfirmButtonContainer.Q<CustomButton>();

        this.sendEmailPopup = this.parentElement.Q<VisualElement>("send-email-popup");

        this.sendEmailContentInputContainer = this.sendEmailPopup.Q<VisualElement>("content-input-container");
        this.sendEmailContentInput = this.sendEmailContentInputContainer.Q<CustomInput>();

        this.emailrecipientsScrollviewContainer = this.sendEmailPopup.Q<VisualElement>("recipient-scrollview-container");
        this.emailRecipientPlaceholderInput = this.emailrecipientsScrollviewContainer.Q<CustomInput>();
        this.emailRecipientsScrollview = this.emailrecipientsScrollviewContainer.Q<ScrollView>();

        this.emailButtonsContainer = this.sendEmailPopup.Q<VisualElement>("buttons-container");

        this.sendEmailCancelButtonContainer = this.emailButtonsContainer.Q<VisualElement>("cancel-button-container");
        this.sendEmailCancelButton = this.sendEmailCancelButtonContainer.Q<CustomButton>();

        this.sendEmailConfirmButtonContainer = this.emailButtonsContainer.Q<VisualElement>("confirm-button-container");
        this.sendEmailConfirmButton = this.sendEmailConfirmButtonContainer.Q<CustomButton>();
    }

    protected override void SetupButtons()
    {
        this.confirmationCancelButton.RegisterCallback<ClickEvent>(evt => ClosePopup());
        this.confirmationConfirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonPressed());

        this.sendEmailCancelButton.RegisterCallback<ClickEvent>(evt => ClosePopup());
        this.sendEmailConfirmButton.RegisterCallback<ClickEvent>(evt => OnSendEmailButtonPressed());
    }

    protected override void SetupInputs()
    {
        this.emailRecipientPlaceholderInput.RegisterCallback<ClickEvent>(evt => OpenRecipientScrollview());
        this.emailRecipientsScrollview.focusable = true;
        this.emailRecipientsScrollview.RegisterCallback<BlurEvent>(evt => CloseRecipientScrollview());
    }

    protected override void SetViewElements()
    {

    }

    private void SetupClosePopupAction()
    {
        this.closePopupsAction = () => { };

        this.closePopupsAction += () => VisualElementHelper.SetElementDisplay(this.confirmationPopup, DisplayStyle.None);
        this.closePopupsAction += () => VisualElementHelper.SetElementDisplay(this.sendEmailPopup, DisplayStyle.None);
    }

    public void OpenConfirmationPopup(Action confirmAction, string message = "")
    {
        ShowParent();

        VisualElementHelper.SetElementDisplay(this.confirmationPopup, DisplayStyle.Flex);
        this.confirmationMessageLabel.text = String.IsNullOrEmpty(message) ? this.defaultConfirmationMessage : message;
        this.confirmAction = confirmAction;
    }

    public void OpenSendEmailPopup(Action<string> sendEmailAction)
    {
        ShowParent();

        VisualElementHelper.SetElementDisplay(this.sendEmailPopup, DisplayStyle.Flex);
        this.sendEmailAction = sendEmailAction;
        this.emailRecipientPlaceholderInput.value = recipient_placeholder;
        this.emailRecipientsScrollview.contentContainer.Clear();

        foreach (User user in AppController.Active.UserDataHandler.Users.Values)
        {
            CustomLabel recipientEmailLabel = new CustomLabel();

            recipientEmailLabel.AddToClassList("regular-font");
            recipientEmailLabel.style.color = Color.black;

            recipientEmailLabel.text = user.DTM.email;

            recipientEmailLabel.RegisterCallback<ClickEvent>(evt =>
            {
                this.currentEmailRecipient = user.DTM.email;
                this.emailRecipientPlaceholderInput.value = this.currentEmailRecipient;
                CloseRecipientScrollview();
                UpdateSendEmailButtonState();
            });

            this.emailRecipientsScrollview.contentContainer.Add(recipientEmailLabel);
        }

        UpdateSendEmailButtonState();
    }

    public void ClosePopup()
    {
        HideParent();
        this.closePopupsAction.Invoke();

        this.confirmAction = null;
        this.confirmationMessageLabel.text = String.Empty;

        this.currentEmailRecipient = recipient_placeholder;
        this.sendEmailAction = null;
    }

    private void OpenRecipientScrollview()
    {
        VisualElementHelper.SetElementDisplay(this.emailRecipientPlaceholderInput, DisplayStyle.None);
        VisualElementHelper.SetElementDisplay(this.emailRecipientsScrollview, DisplayStyle.Flex);
        this.emailRecipientsScrollview.Focus();
    }

    private void CloseRecipientScrollview()
    {
        VisualElementHelper.SetElementDisplay(this.emailRecipientPlaceholderInput, DisplayStyle.Flex);
        VisualElementHelper.SetElementDisplay(this.emailRecipientsScrollview, DisplayStyle.None);
    }

    private void OnConfirmButtonPressed()
    {
        this.confirmAction?.Invoke();

        ClosePopup();
    }

    private void OnSendEmailButtonPressed()
    {
        this.sendEmailAction?.Invoke(this.currentEmailRecipient);

        ClosePopup();
    }

    private void UpdateSendEmailButtonState()
    {
        if (String.IsNullOrEmpty(this.currentEmailRecipient) || String.Equals(this.currentEmailRecipient, recipient_placeholder))
        {
            this.sendEmailConfirmButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
        else
        {
            this.sendEmailConfirmButton.ReinitializeButton(CustomButton.ButtonStyleType.Primary);
        }
    }
}
