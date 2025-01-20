using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PopupsQueryHandler : QueryHandler
{
    [SerializeField] private string defaultMessage = "This action cannot be undone.";
    private VisualElement confirmationPopup;
    private VisualElement confirmationMessageLabelContainer;
    private CustomLabel confirmationMessageLabel;
    private VisualElement confirmationButtonsContainer;
    private VisualElement confirmationCancelButtonContainer;
    private CustomButton confirmationCancelButton;
    private VisualElement confirmationConfirmButtonContainer;
    private CustomButton confirmationConfirmButton;
    private Action confirmAction;
    private Action closePopupsAction;

    public override void Initialize()
    {
        base.Initialize();

        SetupClosePopupAction();

        HideParent();
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
    }

    protected override void SetupButtons()
    {
        this.confirmationCancelButton.RegisterCallback<ClickEvent>(evt => ClosePopup());
        this.confirmationConfirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonPressed());
    }

    protected override void SetupInputs()
    {

    }

    protected override void SetViewElements()
    {

    }

    private void SetupClosePopupAction()
    {
        this.closePopupsAction = () => { };

        this.closePopupsAction += () => VisualElementHelper.SetElementDisplay(this.confirmationPopup, DisplayStyle.None);
    }

    public void OpenConfirmationPopup(Action confirmAction, string message = "")
    {
        ShowParent();

        VisualElementHelper.SetElementDisplay(this.confirmationPopup, DisplayStyle.Flex);
        this.confirmationMessageLabel.text = String.IsNullOrEmpty(message) ? this.defaultMessage : message;
        this.confirmAction = confirmAction;
    }

    public void ClosePopup()
    {
        HideParent();
        this.closePopupsAction.Invoke();
        this.confirmAction = null;
        this.confirmationMessageLabel.text = String.Empty;
    }

    private void OnConfirmButtonPressed()
    {
        this.confirmAction?.Invoke();

        ClosePopup();
    }
}
