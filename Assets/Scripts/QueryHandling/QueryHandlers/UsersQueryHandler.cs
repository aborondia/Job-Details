using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UsersQueryHandler : QueryHandler
{
    [SerializeField] private VisualTreeAsset userRowBase;
    ScrollView usersScrollView;
    VisualElement usersScrollViewContentContainer;

    protected override void InitializeElements()
    {

    }

    protected override void SetupButtons()
    {
    }

    protected override void SetupInputs()
    {
    }

    protected override void SetViewElements()
    {
    }

    private void RefreshUsersList()
    {

    }

    private void AddUserRow()
    {
        TemplateContainer baseElement = this.userRowBase.Instantiate();
        VisualElement nameLabelContainer = baseElement.Q<VisualElement>("name-label-container");
        Label nameLabel = nameLabelContainer.Q<Label>();
        VisualElement deleteUserButtonContainer = baseElement.Q<VisualElement>("delete-user-button-container");
        CustomButton deleteUserButton = baseElement.Q<CustomButton>();
        VisualElement verifyUserButtonContainer = baseElement.Q<VisualElement>("verify-user-button-container");
        CustomButton verifyUserButton = baseElement.Q<CustomButton>();
        VisualElement userTypeContainer = baseElement.Q<VisualElement>("user-type-container");
        CustomEnumField userTypeEnumField = baseElement.Q<CustomEnumField>();
    }
}
