using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Linq;

public class HeaderQueryHandler : QueryHandler
{
    private VisualElement headerLabelContainer;
    private Label headerLabel;
    private VisualElement rightHeaderContainer;
    private VisualElement detailsReportRightHeaderContainer;
    private VisualElement addDetailsReportButtonContainer;
    private CustomButton addDetailsReportButton;
    private VisualElement teamButtonContainer;
    private CustomButton teamButton;
    private VisualElement viewUsersNotificationContainer;
    private Label viewUsersNotificationLabel;

    protected override void InitializeElements()
    {
        this.headerLabelContainer = this.parentElement.Q<VisualElement>("header-label-container");
        this.headerLabel = this.headerLabelContainer.Q<Label>();
        this.rightHeaderContainer = this.parentElement.Q<VisualElement>("right-header-container");
        this.detailsReportRightHeaderContainer = this.rightHeaderContainer.Q<VisualElement>("details-report-right-header-container");
        this.addDetailsReportButtonContainer = this.detailsReportRightHeaderContainer.Q<VisualElement>("add-details-report-button-container");
        this.addDetailsReportButton = this.addDetailsReportButtonContainer.Q<CustomButton>();
        this.teamButtonContainer = this.detailsReportRightHeaderContainer.Q<VisualElement>("team-button-container");
        this.teamButton = this.teamButtonContainer.Q<CustomButton>();
        this.viewUsersNotificationContainer = this.teamButton.Q<VisualElement>("notification-container");
        this.viewUsersNotificationLabel = this.viewUsersNotificationContainer.Q<Label>("notification-label");
    }

    protected override void SetupButtons()
    {
        this.teamButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        VisualElementHelper.SetElementDisplay(this.viewUsersNotificationContainer, DisplayStyle.None);

        this.teamButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);

        AppController.Active.UserDataHandler.OnCurrentUserPopulatedEvent.AddListener(() =>
        {
            if (AppController.Active.UserDataHandler.CurrentUser.RoleDTM.name != UserDataHandler._UserRoleServerName)
            {
                VisualElementHelper.SetElementDisplay(this.teamButtonContainer, DisplayStyle.Flex);
            }
            else
            {
                VisualElementHelper.SetElementDisplay(this.teamButtonContainer, DisplayStyle.None);
            }
        });

        AppController.Active.UserDataHandler.OnUsersPopulatedEvent.AddListener(() =>
        {
            if (AppController.Active.UserDataHandler.CurrentUser.RoleDTM.name != UserDataHandler._UserRoleServerName)
            {
                this.teamButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);

                int unverifiedUserCount = AppController.Active.UserDataHandler.UnverifiedUsers.Count;

                this.teamButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);
                VisualElementHelper.SetElementDisplay(this.teamButtonContainer, DisplayStyle.Flex);

                if (unverifiedUserCount > 0)
                {
                    VisualElementHelper.SetElementDisplay(this.viewUsersNotificationContainer, DisplayStyle.Flex);
                    this.viewUsersNotificationLabel.text = unverifiedUserCount.ToString();
                }

                this.teamButton.RegisterCallback<ClickEvent>(evt =>
                {
                    QueryController.Active.ChangeView(Enumerations.MainView.Users, Enumerations.Subview.Default);
                });
            }
            else
            {
                VisualElementHelper.SetElementDisplay(this.teamButtonContainer, DisplayStyle.None);
            }
        });

        this.addDetailsReportButton.RegisterCallback<ClickEvent>(evt =>
        {
            ActionHelper.StringDelegate responseDelegate = (string response) =>
            {
                DetailsReport newReport = JSONHelper.GetDetailsReportFromCreate(AppController.Active.ServerCommunicator.CurrentUser.objectId, response);

                AppController.Active.DetailsReportsHandler.AddDetailsReports(newReport);
            };

            AppController.Active.ServerCommunicator.CreateDetailsReport(responseDelegate);

            AppController.Active.DetailsReportsHandler.RefreshReports();
        });
    }

    protected override void SetupInputs()
    {
    }

    protected override void SetViewElements()
    {
        AddMainViewElement(Enumerations.MainView.DetailsReports, this.teamButtonContainer);
        AddMainViewElement(Enumerations.MainView.DetailsReports, this.detailsReportRightHeaderContainer);
    }

    protected override void OnAnyViewChanged()
    {
        base.OnAnyViewChanged();

        UpdateHeaderLabel();
    }

    private void UpdateHeaderLabel()
    {
        switch (QueryController.Active.CurrentMainView)
        {
            case Enumerations.MainView.DetailsReports:
                this.headerLabel.text = "Details Reports";
                break;
            case Enumerations.MainView.JobDetails:
                this.headerLabel.text = "Details";
                break;
            case Enumerations.MainView.Login:
                this.headerLabel.text = "Login";
                break;
            case Enumerations.MainView.Users:
                this.headerLabel.text = "Users";
                break;
            default:
                this.headerLabel.text = String.Empty;
                break;
        }
    }
}
