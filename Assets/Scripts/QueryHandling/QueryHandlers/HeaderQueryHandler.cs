using System;
using UnityEngine.UIElements;
using System.Linq;
using MainView = Enumerations.MainView;
using Subview = Enumerations.Subview;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class HeaderQueryHandler : QueryHandler
{
    private VisualElement headerLabelContainer;
    private Label headerLabel;
    private VisualElement logoElement;
    private VisualElement rightHeaderContainer;
    private VisualElement addDetailsReportButtonContainer;
    private CustomButton addDetailsReportButton;
    private VisualElement logoutButtonContainer;
    private CustomButton logoutButton;
    private VisualElement teamButtonDisplayParent;
    private VisualElement teamButtonContainer;
    private CustomButton teamButton;
    private VisualElement viewUsersNotificationContainer;
    private Label viewUsersNotificationLabel;
    private Texture2D logoImage;

    protected override void Start()
    {
        base.Start();

        RetrieveLogo();
    }

    protected override void InitializeElements()
    {
        this.logoElement = this.parentElement.Q<VisualElement>("logo");
        this.headerLabelContainer = this.parentElement.Q<VisualElement>("header-label-container");
        this.headerLabel = this.headerLabelContainer.Q<Label>();
        this.rightHeaderContainer = this.parentElement.Q<VisualElement>("right-header-container");
        this.addDetailsReportButtonContainer = this.rightHeaderContainer.Q<VisualElement>("add-details-report-button-container");
        this.addDetailsReportButton = this.addDetailsReportButtonContainer.Q<CustomButton>();
        this.teamButtonDisplayParent = this.rightHeaderContainer.Q<VisualElement>("team-button-display-parent");
        this.teamButtonContainer = this.teamButtonDisplayParent.Q<VisualElement>("team-button-container");
        this.teamButton = this.teamButtonContainer.Q<CustomButton>();
        this.logoutButtonContainer = this.rightHeaderContainer.Q<VisualElement>("logout-button-container");
        this.logoutButton = this.logoutButtonContainer.Q<CustomButton>();
        this.viewUsersNotificationContainer = this.teamButton.Q<VisualElement>("notification-container");
        this.viewUsersNotificationLabel = this.viewUsersNotificationContainer.Q<Label>("notification-label");

        VisualElementHelper.SetElementDisplay(this.logoElement, DisplayStyle.None);
        VisualElementHelper.SetElementDisplay(this.logoutButtonContainer, DisplayStyle.None);
    }

    protected override void SetupButtons()
    {
        this.teamButton.ReinitializeButton(CustomButton.ButtonStyleType.Disabled);
        VisualElementHelper.SetElementDisplay(this.addDetailsReportButtonContainer, DisplayStyle.None);
        VisualElementHelper.SetElementDisplay(this.logoutButtonContainer, DisplayStyle.None);
        VisualElementHelper.SetElementDisplay(this.teamButtonContainer, DisplayStyle.None);
        VisualElementHelper.SetElementDisplay(this.viewUsersNotificationContainer, DisplayStyle.None);

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
            UpdateUserNotification();
        });

        QueryController.Active.UsersQueryHandler.OnUserDataChange.AddListener(() => UpdateUserNotification());

        this.addDetailsReportButton.RegisterCallback<ClickEvent>(evt =>
        {
            ActionHelper.StringDelegate responseDelegate = (string response) =>
            {
                DetailsReport newReport = JSONHelper.GetDetailsReportFromCreate(response);

                AppController.Active.DetailsReportsHandler.AddDetailsReports(newReport);
            };

            AppController.Active.ServerCommunicator.CreateDetailsReport(responseDelegate);

            AppController.Active.DetailsReportsHandler.RefreshReports();
        });

        this.teamButton.RegisterCallback<ClickEvent>(evt =>
        {
            QueryController.Active.ChangeView(MainView.Users, Subview.Default);
        });

        this.logoutButton.RegisterCallback<ClickEvent>(evt => OnLogoutButtonPressed());
    }

    protected override void SetupInputs()
    {
    }

    protected override void SetViewElements()
    {
        AddMainViewElement(MainView.DetailsReports, this.addDetailsReportButtonContainer);
        AddMainViewElement(MainView.DetailsReports, this.logoutButtonContainer);
        AddMainViewElement(MainView.DetailsReports, this.teamButtonContainer);

        AddMainViewElement(MainView.Users, this.logoutButtonContainer);

        AddMainViewElement(MainView.JobDetails, this.logoutButtonContainer);
    }

    protected override void OnAnyViewChanged()
    {
        base.OnAnyViewChanged();

        UpdateHeaderLabel();
    }

    private void OnLogoutButtonPressed()
    {
        QueryController.Active.PopupsQueryHandler.OpenConfirmationPopup(() =>
        {
            AppController.Active.ServerCommunicator.LogOut(success =>
            {
                QueryController.Active.ChangeView(MainView.Login, Subview.Login_EnterCredentials);
                QueryController.Active.PopupsQueryHandler.OpenNotificationPopup(null, "You have been logged out.");
            });
        }, null, "Are you sure you want to log out?");
    }

    private void UpdateHeaderLabel()
    {
        switch (QueryController.Active.CurrentMainView)
        {
            case MainView.DetailsReports:
                this.headerLabel.text = "Reports";
                break;
            case MainView.JobDetails:
                this.headerLabel.text = "Details";
                break;
            case MainView.Login:
                this.headerLabel.text = "Login";
                break;
            case MainView.Users:
                this.headerLabel.text = "Users";
                break;
            default:
                this.headerLabel.text = String.Empty;
                break;
        }
    }

    private void UpdateUserNotification()
    {
        if (AppController.Active.UserDataHandler.CurrentUser.RoleDTM.name == UserDataHandler._UserRoleServerName)
        {
            VisualElementHelper.SetElementDisplay(this.teamButtonDisplayParent, DisplayStyle.None);

            return;
        }

        this.teamButton.ReinitializeButton(CustomButton.ButtonStyleType.Regular);

        int unverifiedUserCount = AppController.Active.UserDataHandler.Users.Values
        .Where(user => !user.DTM.verified).Count();

        VisualElementHelper.SetElementDisplay(this.teamButtonDisplayParent, DisplayStyle.Flex);

        if (unverifiedUserCount > 0)
        {
            VisualElementHelper.SetElementDisplay(this.viewUsersNotificationContainer, DisplayStyle.Flex);
            this.viewUsersNotificationLabel.text = unverifiedUserCount.ToString();
        }
        else
        {
            VisualElementHelper.SetElementDisplay(this.viewUsersNotificationContainer, DisplayStyle.None);
        }
    }

    private void RetrieveLogo()
    {
        LogoImageDTM logoImageDTM;

        ActionHelper.StringDelegate completeAction = result =>
        {
            if (!String.IsNullOrEmpty(result))
            {
                logoImageDTM = JSONHelper.GetLogoImageDTM(result);
                this.logoImage = new Texture2D(2, 2);
                this.logoImage.LoadImage(logoImageDTM.imageBytesContent);

                this.logoElement.style.backgroundImage = new StyleBackground(this.logoImage);
                VisualElementHelper.SetElementDisplay(this.logoElement, DisplayStyle.Flex);
            }
        };

        AppController.Active.ServerCommunicator.RetrieveLogo(completeAction);
    }
}
