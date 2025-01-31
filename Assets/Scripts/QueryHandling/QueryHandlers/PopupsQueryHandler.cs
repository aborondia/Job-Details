using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupsQueryHandler : QueryHandler
{
    private enum UserSelectType
    {
        Email,
        Username,
    }

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
    private Action cancelAction;
    private Action confirmAction;

    #endregion

    #region Notification Popup
    private VisualElement notificationPopup;
    private VisualElement notificationMessageLabelContainer;
    private CustomLabel notificationMessageLabel;
    private VisualElement notificationConfirmButtonContainer;
    private CustomButton notificationConfirmButton;
    #endregion

    #region Send Email Popup
    private const string recipient_placeholder = "Choose recipient";
    private VisualElement sendEmailPopup;
    private VisualElement emailrecipientsScrollviewContainer;
    private CustomInput emailRecipientPlaceholderInput;
    private ScrollView emailRecipientsScrollview;
    private VisualElement sendEmailButtonsContainer;
    private VisualElement sendEmailCancelButtonContainer;
    private CustomButton sendEmailCancelButton;
    private VisualElement sendEmailConfirmButtonContainer;
    private CustomButton sendEmailConfirmButton;
    private VisualElement sendEmailContentInputContainer;
    private CustomInput sendEmailContentInput;
    private string currentUserEmailSelection;
    private Action<string, string> sendEmailAction;
    #endregion

    #region Name Select Popup
    private const string name_select_placeholder = "Select a user";
    private VisualElement nameSelectPopup;
    private VisualElement nameSelectScrollviewContainer;
    private CustomInput nameSelectPlaceholderInput;
    private ScrollView nameSelectScrollview;
    private VisualElement nameSelectButtonsContainer;
    private VisualElement nameSelectCancelButtonContainer;
    private CustomButton nameSelectCancelButton;
    private VisualElement nameSelectConfirmButtonContainer;
    private CustomButton nameSelectConfirmButton;

    #endregion

    private string currentUserNameSelection;
    private Action blurAction;
    private Action closePopupsAction;

    public override void Initialize()
    {
        base.Initialize();

        this.parentElement.focusable = true;
        this.parentElement.RegisterCallback<BlurEvent>(evt => ActionHelper.OnBlur(evt, this.parentElement, this.blurAction));
        SetupClosePopupAction();
        ClosePopup(true);
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

        this.notificationPopup = this.parentElement.Q<VisualElement>("notification-popup");
        this.notificationMessageLabelContainer = this.notificationPopup.Q<VisualElement>("display-message-container");
        this.notificationMessageLabel = this.notificationMessageLabelContainer.Q<CustomLabel>();
        this.notificationConfirmButtonContainer = this.notificationPopup.Q<VisualElement>("confirm-button-container");
        this.notificationConfirmButton = this.notificationConfirmButtonContainer.Q<CustomButton>();

        this.sendEmailPopup = this.parentElement.Q<VisualElement>("send-email-popup");
        this.sendEmailContentInputContainer = this.sendEmailPopup.Q<VisualElement>("content-input-container");
        this.sendEmailContentInput = this.sendEmailContentInputContainer.Q<CustomInput>();
        this.sendEmailButtonsContainer = this.sendEmailPopup.Q<VisualElement>("buttons-container");
        this.sendEmailCancelButtonContainer = this.sendEmailButtonsContainer.Q<VisualElement>("cancel-button-container");
        this.sendEmailCancelButton = this.sendEmailCancelButtonContainer.Q<CustomButton>();
        this.sendEmailConfirmButtonContainer = this.sendEmailButtonsContainer.Q<VisualElement>("confirm-button-container");
        this.sendEmailConfirmButton = this.sendEmailConfirmButtonContainer.Q<CustomButton>();

        this.emailrecipientsScrollviewContainer = this.sendEmailPopup.Q<VisualElement>("recipient-scrollview-container");
        this.emailRecipientPlaceholderInput = this.emailrecipientsScrollviewContainer.Q<CustomInput>();
        this.emailRecipientsScrollview = this.emailrecipientsScrollviewContainer.Q<ScrollView>();

        this.nameSelectPopup = this.parentElement.Q<VisualElement>("name-select-popup");
        this.nameSelectButtonsContainer = this.nameSelectPopup.Q<VisualElement>("buttons-container");
        this.nameSelectCancelButtonContainer = this.nameSelectButtonsContainer.Q<VisualElement>("cancel-button-container");
        this.nameSelectCancelButton = this.nameSelectCancelButtonContainer.Q<CustomButton>();
        this.nameSelectConfirmButtonContainer = this.nameSelectButtonsContainer.Q<VisualElement>("confirm-button-container");
        this.nameSelectConfirmButton = this.nameSelectConfirmButtonContainer.Q<CustomButton>();

        this.nameSelectScrollviewContainer = this.nameSelectPopup.Q<VisualElement>("name-scrollview-container");
        this.nameSelectPlaceholderInput = this.nameSelectScrollviewContainer.Q<CustomInput>();
        this.nameSelectScrollview = this.nameSelectScrollviewContainer.Q<ScrollView>();
    }

    protected override void SetupButtons()
    {
        this.confirmationCancelButton.RegisterCallback<ClickEvent>(evt => ClosePopup(true));
        this.confirmationConfirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonPressed());

        this.notificationConfirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonPressed());

        this.sendEmailCancelButton.RegisterCallback<ClickEvent>(evt => ClosePopup(true));
        this.sendEmailConfirmButton.RegisterCallback<ClickEvent>(evt => OnSendEmailButtonPressed());

        this.nameSelectCancelButton.RegisterCallback<ClickEvent>(evt => ClosePopup(true));
        this.nameSelectConfirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonPressed());
    }

    protected override void SetupInputs()
    {
        this.emailRecipientPlaceholderInput.RegisterCallback<ClickEvent>(evt => OpenRecipientScrollview());
        this.emailRecipientsScrollview.focusable = true;
        this.emailRecipientsScrollview.RegisterCallback<BlurEvent>(evt => OnRecipientScrollviewBlur(evt));

        this.nameSelectPlaceholderInput.RegisterCallback<ClickEvent>(evt => OpenNameSelectScrollview());
        this.nameSelectScrollview.focusable = true;
        this.nameSelectScrollview.RegisterCallback<BlurEvent>(evt => OnNameSelectScrollviewBlur(evt));
    }

    protected override void SetViewElements()
    {

    }

    private void SetupClosePopupAction()
    {
        this.closePopupsAction = () => { };

        this.closePopupsAction += () => VisualElementHelper.SetElementDisplay(this.confirmationPopup, DisplayStyle.None);
        this.closePopupsAction += () => VisualElementHelper.SetElementDisplay(this.notificationPopup, DisplayStyle.None);
        this.closePopupsAction += () => VisualElementHelper.SetElementDisplay(this.sendEmailPopup, DisplayStyle.None);
        this.closePopupsAction += () => VisualElementHelper.SetElementDisplay(this.nameSelectPopup, DisplayStyle.None);
    }

    private void OnOpeningPopup(bool canNavigateAway)
    {
        if (canNavigateAway)
        {
            this.blurAction = () => ClosePopup(true);
        }

        QueryController.Active.BlockInteractions(this.instanceId);

        ActionHelper.ExecuteActionNextFrame(() => this.parentElement.Focus());
    }

    public void OpenConfirmationPopup(Action confirmAction, Action cancelAction = null, string message = "", bool canNavigateAway = true)
    {
        ShowParent();
        OnOpeningPopup(canNavigateAway);

        VisualElementHelper.SetElementDisplay(this.confirmationPopup, DisplayStyle.Flex);
        this.confirmationMessageLabel.text = String.IsNullOrEmpty(message) ? this.defaultConfirmationMessage : message;
        this.confirmAction = confirmAction;
        this.cancelAction = cancelAction;
    }

    public void OpenNotificationPopup(Action confirmAction, string message, bool canNavigateAway = true)
    {
        ShowParent();
        OnOpeningPopup(canNavigateAway);

        VisualElementHelper.SetElementDisplay(this.notificationPopup, DisplayStyle.Flex);
        this.notificationMessageLabel.text = message;
        this.confirmAction = confirmAction;
    }

    public void OpenSendEmailPopup(Action<string, string> sendEmailAction, bool canNavigateAway = false)
    {
        ShowParent();
        OnOpeningPopup(canNavigateAway);

        VisualElementHelper.SetElementDisplay(this.sendEmailPopup, DisplayStyle.Flex);
        this.sendEmailContentInput.value = String.Empty;
        this.sendEmailAction = sendEmailAction;
        this.emailRecipientPlaceholderInput.value = recipient_placeholder;
        this.emailRecipientsScrollview.contentContainer.Clear();

        PopulateUserSelect(UserSelectType.Email);
        UpdateSendEmailButtonState();
    }

    public void OpenNameSelectPopup(Action<string> confirmAction, Action cancelAction = null, bool canNavigateAway = true)
    {
        ShowParent();
        OnOpeningPopup(canNavigateAway);

        VisualElementHelper.SetElementDisplay(this.nameSelectPopup, DisplayStyle.Flex);
        this.nameSelectPlaceholderInput.value = name_select_placeholder;
        this.nameSelectScrollview.contentContainer.Clear();

        this.confirmAction = () =>
        {
            string selectedName = this.currentUserNameSelection;

            confirmAction.Invoke(selectedName);
        };

        this.cancelAction = cancelAction;

        PopulateUserSelect(UserSelectType.Username);
        UpdateSelectNameButtonState();
    }

    private void PopulateUserSelect(UserSelectType userSelectType)
    {
        foreach (User user in AppController.Active.UserDataHandler.Users.Values)
        {
            CustomLabel userLabel;
            string username;
            string userEmail;

            if (user.RoleDTM.name == UserDataHandler._DeveloperRoleServerName)
            {
                continue;
            }

            username = user.DTM.displayName;
            userEmail = user.DTM.email;

            userLabel = new CustomLabel();
            userLabel.RemoveFromClassList("use-default-font-color");
            userLabel.AddToClassList("username-selection-entry");

            userLabel.text = username;

            userLabel.RegisterCallback<ClickEvent>(evt =>
            {
                this.currentUserNameSelection = username;

                switch (userSelectType)
                {
                    case UserSelectType.Email:
                        this.emailRecipientPlaceholderInput.value = this.currentUserNameSelection;
                        this.currentUserEmailSelection = userEmail;
                        CloseRecipientScrollview();
                        UpdateSendEmailButtonState();
                        this.parentElement.Focus();
                        break;
                    case UserSelectType.Username:
                        this.nameSelectPlaceholderInput.value = this.currentUserNameSelection;
                        CloseNameSelectScrollview();
                        UpdateSelectNameButtonState();
                        this.parentElement.Focus();
                        break;
                }
            });

            switch (userSelectType)
            {
                case UserSelectType.Email:
                    this.emailRecipientsScrollview.contentContainer.Add(userLabel);
                    break;
                case UserSelectType.Username:
                    this.nameSelectScrollview.contentContainer.Add(userLabel);
                    break;
            }
        }
    }

    public void ClosePopup(bool cancelled)
    {
        HideParent();
        this.closePopupsAction.Invoke();

        if (cancelled)
        {
            this.cancelAction?.Invoke();
        }

        this.confirmAction = null;
        this.cancelAction = null;
        this.blurAction = null;
        this.confirmationMessageLabel.text = String.Empty;
        this.notificationMessageLabel.text = String.Empty;

        this.currentUserNameSelection = String.Empty;
        this.currentUserEmailSelection = String.Empty;
        this.sendEmailAction = null;

        QueryController.Active.UnblockInteractions(this.instanceId);
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

    private void OnRecipientScrollviewBlur(BlurEvent evt)
    {
        ActionHelper.OnBlur(evt, this.emailRecipientsScrollview, () => CloseRecipientScrollview());
    }

    private void OpenNameSelectScrollview()
    {
        VisualElementHelper.SetElementDisplay(this.nameSelectPlaceholderInput, DisplayStyle.None);
        VisualElementHelper.SetElementDisplay(this.nameSelectScrollview, DisplayStyle.Flex);
        this.nameSelectScrollview.Focus();
    }

    private void CloseNameSelectScrollview()
    {
        VisualElementHelper.SetElementDisplay(this.nameSelectPlaceholderInput, DisplayStyle.Flex);
        VisualElementHelper.SetElementDisplay(this.nameSelectScrollview, DisplayStyle.None);
    }

    private void OnNameSelectScrollviewBlur(BlurEvent evt)
    {
        ActionHelper.OnBlur(evt, this.nameSelectScrollview, () => CloseNameSelectScrollview());
    }

    private void OnConfirmButtonPressed()
    {
        this.confirmAction?.Invoke();

        ClosePopup(false);
    }

    private void OnSendEmailButtonPressed()
    {
        this.sendEmailAction?.Invoke(this.currentUserEmailSelection, this.sendEmailContentInput.value);

        ClosePopup(false);
    }

    private void UpdateSendEmailButtonState()
    {
        if (String.IsNullOrEmpty(this.currentUserEmailSelection) || String.Equals(this.currentUserEmailSelection, recipient_placeholder))
        {
            this.sendEmailConfirmButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
        else
        {
            this.sendEmailConfirmButton.ReinitializeButton(CustomButton.ButtonStyleType.Primary);
        }
    }

    private void UpdateSelectNameButtonState()
    {
        if (String.IsNullOrEmpty(this.currentUserNameSelection) || String.Equals(this.currentUserNameSelection, name_select_placeholder))
        {
            this.nameSelectConfirmButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        }
        else
        {
            this.nameSelectConfirmButton.ReinitializeButton(CustomButton.ButtonStyleType.Primary);
        }
    }
}
