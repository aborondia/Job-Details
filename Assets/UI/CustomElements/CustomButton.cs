using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CustomButton : Button
{
    public enum ButtonStyleType
    {
        None,
        Regular,
        Primary,
        Secondary,
        Moderate,
        Warning,
        Disabled,
        NoStyle,
        CustomColor,
    }
    public enum SubStyleType
    {
        None,
        ListItem,
    }
    public enum StartState
    {
        Selectable,
        Unselectable,
    }
    public new class UxmlFactory : UxmlFactory<CustomButton, UxmlTraits> { }
    public UxmlTraits uxmlTraits;
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> m_Type =
        new UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> { name = "button-type", defaultValue = CustomButton.ButtonStyleType.Regular };
        UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> m_UnselectableOverride =
        new UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> { name = "unselectable-override", defaultValue = CustomButton.ButtonStyleType.None };
        UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> m_SelectableOverride =
        new UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> { name = "selectable-override", defaultValue = CustomButton.ButtonStyleType.None };
        UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> m_ActiveOverride =
        new UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> { name = "active-override", defaultValue = CustomButton.ButtonStyleType.None };
        UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> m_HoverOverride =
        new UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> { name = "hover-override", defaultValue = CustomButton.ButtonStyleType.None };
        UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> m_SelectedOverride =
        new UxmlEnumAttributeDescription<CustomButton.ButtonStyleType> { name = "selected-override", defaultValue = CustomButton.ButtonStyleType.None };
        UxmlEnumAttributeDescription<CustomButton.StartState> m_StartState =
        new UxmlEnumAttributeDescription<CustomButton.StartState> { name = "default-start-state", defaultValue = CustomButton.StartState.Selectable };
        UxmlBoolAttributeDescription m_UseCustomSelectableBorderColor =
        new UxmlBoolAttributeDescription { name = "use-custom-selectable-border-color", defaultValue = false };
        UxmlColorAttributeDescription m_CustomSelectableBorderColor =
        new UxmlColorAttributeDescription { name = "custom-selectable-border-color", defaultValue = Color.white };
        UxmlBoolAttributeDescription m_UseCustomHoverColor =
        new UxmlBoolAttributeDescription { name = "use-custom-hover-color", defaultValue = false };
        UxmlColorAttributeDescription m_CustomHoverColor =
        new UxmlColorAttributeDescription { name = "custom-hover-color", defaultValue = Color.white };
        UxmlBoolAttributeDescription m_HoverTextHighlight =
        new UxmlBoolAttributeDescription { name = "hover-text-highlight", defaultValue = false };
        UxmlBoolAttributeDescription m_HoverBackgroundHighlight =
        new UxmlBoolAttributeDescription { name = "hover-background-highlight", defaultValue = true };
        UxmlBoolAttributeDescription m_ActiveBackgroundHighlight =
        new UxmlBoolAttributeDescription { name = "active-background-highlight", defaultValue = true };
        UxmlBoolAttributeDescription m_HoverBorderHighlight =
        new UxmlBoolAttributeDescription { name = "hover-border-highlight", defaultValue = true };
        UxmlBoolAttributeDescription m_NoInteractableStyles =
        new UxmlBoolAttributeDescription { name = "no-interactable-styles", defaultValue = false };
        UxmlBoolAttributeDescription m_ActiveButton =
        new UxmlBoolAttributeDescription { name = "active-button", defaultValue = false };
        UxmlBoolAttributeDescription m_RegenerateInEditor =
        new UxmlBoolAttributeDescription { name = "regenerate-in-editor", defaultValue = false };
        UxmlBoolAttributeDescription m_showCursor =
        new UxmlBoolAttributeDescription { name = "show-cursor", defaultValue = true };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {

            base.Init(ve, bag, cc);
            CustomButton ate = ve as CustomButton;
            ate.baseInitializationComplete = false;

            ate.RemoveFromClassList("unity-button");
            ate.RemoveFromClassList("unity-text-element");
            ate.buttonType = m_Type.GetValueFromBag(bag, cc);
            ate.defaultStartState = m_StartState.GetValueFromBag(bag, cc);
            ate.activeOverride = m_ActiveOverride.GetValueFromBag(bag, cc);
            ate.hoverOverride = m_HoverOverride.GetValueFromBag(bag, cc);
            ate.selectedOverride = m_SelectedOverride.GetValueFromBag(bag, cc);
            ate.selectableOverride = m_SelectableOverride.GetValueFromBag(bag, cc);
            ate.unselectableOverride = m_UnselectableOverride.GetValueFromBag(bag, cc);
            ate.useCustomSelectableBorderColor = m_UseCustomSelectableBorderColor.GetValueFromBag(bag, cc);
            ate.customSelectableBorderColor = m_CustomSelectableBorderColor.GetValueFromBag(bag, cc);
            ate.useCustomHoverColor = m_UseCustomHoverColor.GetValueFromBag(bag, cc);
            ate.customHoverColor = m_CustomHoverColor.GetValueFromBag(bag, cc);
            ate.hoverTextHighlight = m_HoverTextHighlight.GetValueFromBag(bag, cc);
            ate.hoverBorderHighlight = m_HoverBorderHighlight.GetValueFromBag(bag, cc);
            ate.hoverBackgroundHighlight = m_HoverBackgroundHighlight.GetValueFromBag(bag, cc);
            ate.activeBackgroundHighlight = m_ActiveBackgroundHighlight.GetValueFromBag(bag, cc);
            ate.noInteractableStyles = m_NoInteractableStyles.GetValueFromBag(bag, cc);
            ate.activeButton = m_ActiveButton.GetValueFromBag(bag, cc);
            ate.showCursor = m_showCursor.GetValueFromBag(bag, cc);

            if (ate.showCursor)
            {
                ate.AddToClassList("show-cursor");
            }

            if (Application.isPlaying || m_RegenerateInEditor.GetValueFromBag(bag, cc))
            {
                ate.InitializeBaseValues();

                ate.generateVisualContent += mgc =>
                {
                    if (!(mgc.visualElement as CustomButton).baseInitializationComplete)
                    {
                        (mgc.visualElement as CustomButton).InitializeBaseValues();
                        (mgc.visualElement as CustomButton).CheckSizeChange();
                    }

                    if (ate.InitializeMesgGenerationContextComplete == false)
                    {
                        ate.InitializeMesgGenerationContextComplete = true;
                        ate.InitialiDisplayStyle = ate.resolvedStyle.display;
                    }
                };
            }
        }
    }

    private static int _idAssigner = 0;
    private int id = -1;
    public bool InitializeMesgGenerationContextComplete = false;
    public DisplayStyle InitialiDisplayStyle;
    private bool productionReady;
    public bool ProductionReady => productionReady;
    private bool showCursor;
    private Label defaultButtonLabel;
    public Label DefaultButtonLabel
    {
        get
        {
            return this.defaultButtonLabel;
        }
    }
    private string defaultButtonText;
    public string DefaultButtonText
    {
        get
        {
            return this.defaultButtonText;
        }
    }
    private bool selectable = false;
    public bool Selectable
    {
        get
        {
            return this.selectable;
        }
    }
    private bool useCustomSelectableBorderColor;
    public bool UseCustomSelectableBorderColor
    {
        get
        {
            return this.useCustomSelectableBorderColor;
        }
        set
        {
            this.useCustomSelectableBorderColor = value;
        }
    }
    private Color customSelectableBorderColor;
    public Color CustomSelectableBorderColor
    {
        get
        {
            return this.customSelectableBorderColor;
        }
        set
        {
            this.customSelectableBorderColor = value;
        }
    }
    private bool useCustomHoverColor;
    public bool UseCustomHoverColor
    {
        get
        {
            return this.useCustomHoverColor;
        }
        set
        {
            this.useCustomHoverColor = value;
        }
    }
    private Color customHoverColor;
    public Color CustomHoverColor
    {
        get
        {
            return this.customHoverColor;
        }
        set
        {
            this.customHoverColor = value;
        }
    }
    private bool hoverTextHighlight;
    public bool HoverTextHighlight
    {
        get
        {
            return this.hoverTextHighlight;
        }
        set
        {
            this.hoverTextHighlight = value;
        }
    }
    private bool hoverBorderHighlight;
    public bool HoverBorderHighlight
    {
        get
        {
            return this.hoverBorderHighlight;
        }
        set
        {
            this.hoverBorderHighlight = value;
        }
    }
    private bool hoverBackgroundHighlight;
    public bool HoverBackgroundHighlight
    {
        get
        {
            return this.hoverBackgroundHighlight;
        }
        set
        {
            this.hoverBackgroundHighlight = value;
        }
    }
    private bool activeBackgroundHighlight;
    public bool ActiveBackgroundHighlight
    {
        get
        {
            return this.activeBackgroundHighlight;
        }
        set
        {
            this.activeBackgroundHighlight = value;
        }
    }
    private bool noInteractableStyles;
    public bool NoInteractableStyles
    {
        get
        {
            return this.noInteractableStyles;
        }
        set
        {
            this.noInteractableStyles = value;
        }
    }
    private bool activeButton;
    public bool ActiveButton
    {
        get
        {
            return this.activeButton;
        }
        set
        {
            this.activeButton = value;
            onActiveStateChange.Invoke();
        }
    }
    private CustomButton.ButtonStyleType buttonType;
    public CustomButton.ButtonStyleType ButtonType
    {
        get
        {
            return this.buttonType;
        }
    }
    private CustomButton.ButtonStyleType unselectableOverride;
    public CustomButton.ButtonStyleType UnselectableOverride
    {
        get
        {
            return this.unselectableOverride;
        }
        set
        {
            this.unselectableOverride = value;
        }
    }
    private CustomButton.ButtonStyleType selectableOverride;
    public CustomButton.ButtonStyleType SelectableOverride
    {
        get
        {
            return this.selectableOverride;
        }
        set
        {
            this.selectableOverride = value;
        }
    }
    private CustomButton.ButtonStyleType activeOverride;
    public CustomButton.ButtonStyleType ActiveOverride
    {
        get
        {
            return this.activeOverride;
        }
        set
        {
            this.activeOverride = value;
        }
    }
    private CustomButton.ButtonStyleType hoverOverride;
    public CustomButton.ButtonStyleType HoverOverride
    {
        get
        {
            return this.hoverOverride;
        }
        set
        {
            this.hoverOverride = value;
        }
    }
    private CustomButton.ButtonStyleType selectedOverride;
    public CustomButton.ButtonStyleType SelectedOverride
    {
        get
        {
            return this.selectedOverride;
        }
        set
        {
            this.selectedOverride = value;
        }
    }
    private CustomButton.StartState defaultStartState;
    public CustomButton.StartState DefaultStartState
    {
        get
        {
            return this.defaultStartState;
        }
        set
        {
            this.defaultStartState = value;
        }
    }
    private VisualElement pulseContainer;
    protected float baseWidth;
    protected float baseHeight;
    private bool baseInitializationComplete = false;
    private bool BaseInitializationComplete => this.baseInitializationComplete;
    private bool firstInitializationComplete = false;
    public bool FirstInitializationComplete
    {
        get
        {
            return this.firstInitializationComplete;
        }
    }
    private UnityEvent onActiveStateChange = new UnityEvent();
    public UnityEvent OnPointerDownEvent = new UnityEvent();
    public UnityEvent OnPointerUpEvent = new UnityEvent();
    public UnityEvent OnHoverInEvent = new UnityEvent();
    public UnityEvent OnHoverOutEvent = new UnityEvent();
    public UnityEvent OnSetActiveEvent = new UnityEvent();
    public UnityEvent OnSetInactiveEvent = new UnityEvent();

    #region Initialization

    private void InitializeBaseValues(bool forceReinitialize = false)
    {
        if (this.id < 0)
        {
            this.id = _idAssigner++;
        }

        if (this.baseInitializationComplete && !forceReinitialize)
        {
            return;
        }

        float width = this.resolvedStyle.width;
        float height = this.resolvedStyle.height;

        if (width > 0 && height > 0)
        {
            this.defaultButtonLabel = this.Q<Label>();

            if (ReferenceEquals(this.defaultButtonLabel, null))
            {
                this.defaultButtonText = "NO DEFAULT SET";
            }
            else
            {
                this.defaultButtonText = this.defaultButtonLabel.text;
            }

            this.baseWidth = width;
            this.baseHeight = height;
            this.focusable = false;

            if (!this.baseInitializationComplete && !this.noInteractableStyles)
            {
                if (!ReferenceEquals(CoroutineHelper.Active, null))
                {
                    CoroutineHelper.Active.StartCoroutine(WaitForInitializationToComplete());
                }

                UpdateBorderClasses();
                UpdateHoverClasses();

                this.RegisterCallback<PointerDownEvent>(evt => OnPointerDown());
                this.RegisterCallback<PointerUpEvent>(evt => OnPointerUp());
                this.RegisterCallback<PointerOverEvent>(evt => OnHover());
                this.RegisterCallback<PointerLeaveEvent>(evt => OnHoverOut());
            }

            if (this.defaultStartState == StartState.Selectable)
            {
                this.selectable = true;
            }
            else
            {
                this.selectable = false;
            }

            this.baseInitializationComplete = true;
        }

        InitializeButton();
    }

    public void InitializeButton()
    {
        if (this.noInteractableStyles)
        {
            return;
        }

        ClearButtonClasses();
        this.style.backgroundColor = StyleKeyword.Null;

        if (this.buttonType == ButtonStyleType.Disabled)
        {
            this.SetEnabled(false);
        }

        UpdateBorderClasses();
        UpdateHoverClasses();

        if (this.buttonType != ButtonStyleType.None)
        {
            if (this.activeButton)
            {
                this.AddToClassList(GetActiveClass());
                this.AddToClassList("active");
            }
            else
            {
                this.RemoveFromClassList(GetActiveClass());
                this.RemoveFromClassList("active");

                if (this.DefaultStartState == StartState.Selectable)
                {
                    this.AddToClassList(GetSelectableClass());
                }
                else
                {
                    this.AddToClassList(GetUnselectableClass());
                }
            }
        }

        if (!this.firstInitializationComplete)
        {
            this.firstInitializationComplete = true;
        }
    }

    private IEnumerator WaitForInitializationToComplete()
    {
        yield return new WaitUntil(() => this.baseInitializationComplete && this.firstInitializationComplete);

        OnFirstInitializationComplete();
    }

    private void OnFirstInitializationComplete()
    {
        UpdateCustomSelectableColors();
    }

    #endregion

    #region Button Actions

    private void OnPointerDown()
    {
        ButtonStyleType buttonStyleType = this.selectedOverride == ButtonStyleType.None ? this.buttonType : this.selectedOverride;

        this.OnPointerDownEvent.Invoke();

        if (!this.ClassListContains(GetSelectedClass()))
        {
            ClearButtonClasses();
            this.AddToClassList(GetSelectedClass());
            this.AddToClassList(GetBorderClass(buttonStyleType));
            MarkDirtyRepaint();
        }
    }

    private void OnPointerUp()
    {
        this.OnPointerUpEvent.Invoke();

        if (this.ClassListContains(GetSelectedClass()))
        {
            InitializeButton();
            this.RemoveFromClassList(GetSelectedClass());
            MarkDirtyRepaint();
        }
    }

    private void OnHover()
    {
        ButtonStyleType buttonStyleType = this.hoverOverride == ButtonStyleType.None ? this.buttonType : this.hoverOverride;

        this.OnHoverInEvent.Invoke();
    }

    private void OnHoverOut()
    {
        this.OnHoverOutEvent.Invoke();
    }

    #endregion

    #region Get Button Properties

    private string GetButtonType(ButtonStyleType buttonStyleType)
    {
        return buttonStyleType.ToString().ToLower();
    }

    private string GetFullSelector(ButtonStyleType buttonType, string selector)
    {
        if (buttonType == ButtonStyleType.None || buttonType == ButtonStyleType.NoStyle)
        {
            return "";
        }
        else
        {
            return $"{GetButtonType(buttonType)}-{selector}";
        }
    }

    private string GetUnselectableClass()
    {
        ButtonStyleType buttonType;

        if (this.unselectableOverride == ButtonStyleType.None)
        {
            buttonType = this.buttonType;
        }
        else
        {
            buttonType = this.unselectableOverride;
        }

        return GetFullSelector(buttonType, "unselectable");
    }

    private string GetSelectableClass()
    {
        ButtonStyleType buttonType;

        if (this.selectableOverride == ButtonStyleType.None)
        {
            buttonType = this.buttonType;
        }
        else
        {
            buttonType = this.selectableOverride;
        }

        return GetFullSelector(buttonType, "selectable");
    }

    private string GetSelectedClass()
    {
        ButtonStyleType buttonType;

        if (this.selectedOverride == ButtonStyleType.None)
        {
            buttonType = this.buttonType;
        }
        else
        {
            buttonType = this.selectedOverride;
        }

        return GetFullSelector(buttonType, "selected");
    }

    private string GetActiveClass()
    {
        ButtonStyleType buttonType;

        if (this.activeOverride == ButtonStyleType.None)
        {
            buttonType = this.buttonType;
        }
        else
        {
            buttonType = this.activeOverride;
        }

        return GetFullSelector(buttonType, "active");
    }

    private string GetHoverClass()
    {
        ButtonStyleType buttonType;

        if (this.hoverOverride == ButtonStyleType.None)
        {
            buttonType = this.buttonType;
        }
        else
        {
            buttonType = this.hoverOverride;
        }

        return GetFullSelector(buttonType, "hover");
    }

    private string GetBorderClass(ButtonStyleType buttonType)
    {
        return GetFullSelector(buttonType, "border");
    }

    private string GetClassFromButtonType(ButtonStyleType buttonStyleType)
    {
        switch (buttonStyleType)
        {
            case ButtonStyleType.Disabled:
                return "disabled";
            case ButtonStyleType.None:
                return "";
            case ButtonStyleType.Moderate:
                return "moderate";
            case ButtonStyleType.Primary:
                return "primary";
            case ButtonStyleType.Regular:
                return "regular";
            case ButtonStyleType.Secondary:
                return "secondary";
            case ButtonStyleType.Warning:
                return "warning";
            default:
                return "";
        }
    }

    #endregion

    #region Set Button Properties

    private void ClearButtonClasses()
    {
        string unselectableClass = GetUnselectableClass();
        string selectableClass = GetSelectableClass();
        string selectedClass = GetSelectedClass();
        string hoverClass = GetHoverClass();
        string activeClass = GetActiveClass();

        this.RemoveFromClassList(unselectableClass);
        this.RemoveFromClassList(selectableClass);
        this.RemoveFromClassList(selectedClass);
        this.RemoveFromClassList(hoverClass);
        this.RemoveFromClassList(activeClass);
    }

    private void UpdateCustomSelectableColors()
    {
        if (!this.useCustomSelectableBorderColor)
        {
            return;
        }

        if (this.useCustomSelectableBorderColor && this.selectable && !this.activeButton && !this.ClassListContains(GetHoverClass()))
        {
            this.style.borderRightColor = this.customSelectableBorderColor;
            this.style.borderLeftColor = this.customSelectableBorderColor;
            this.style.borderTopColor = this.customSelectableBorderColor;
            this.style.borderBottomColor = this.customSelectableBorderColor;
        }
        else
        {
            this.style.borderRightColor = StyleKeyword.Null;
            this.style.borderLeftColor = StyleKeyword.Null;
            this.style.borderTopColor = StyleKeyword.Null;
            this.style.borderBottomColor = StyleKeyword.Null;
        }
    }

    private void UpdateCustomHoverColors()
    {
        if (!this.useCustomHoverColor)
        {
            return;
        }

        if (this.ClassListContains(GetHoverClass()))
        {
            this.style.backgroundColor = this.customHoverColor;
        }
        else
        {
            this.style.backgroundColor = StyleKeyword.Null;
        }
    }

    private void UpdateBorderClasses()
    {
        Action removeOldSelectors = () => { };

        foreach (string selector in this.GetClasses())
        {
            if (selector.Contains("-border"))
            {
                removeOldSelectors += () =>
                {
                    this.RemoveFromClassList(selector);
                };
            }
        }

        removeOldSelectors.Invoke();

        if (this.activeButton && this.activeOverride != ButtonStyleType.None)
        {
            this.AddToClassList(GetBorderClass(this.activeOverride));
        }
        else if (this.selectable && this.selectableOverride != ButtonStyleType.None)
        {
            this.AddToClassList(GetBorderClass(this.selectableOverride));
        }
        else if (this.unselectableOverride != ButtonStyleType.None)
        {
            this.AddToClassList(GetBorderClass(this.unselectableOverride));
        }
        else
        {
            this.AddToClassList(GetBorderClass(this.buttonType));
        }
    }

    private void UpdateHoverClasses()
    {
        if (this.hoverTextHighlight)
        {
            this.AddToClassList("hover-text");
        }
        else
        {
            this.RemoveFromClassList("hover-text");
        }

        if (this.hoverBorderHighlight)
        {
            this.AddToClassList("hover-border");
        }
        else
        {
            this.RemoveFromClassList("hover-border");
        }

        if (this.hoverBackgroundHighlight)
        {
            this.AddToClassList("hover-background");
        }
        else
        {
            this.RemoveFromClassList("hover-background");
        }
    }

    public void ReinitializeButton()
    {
        StartState startState = this.buttonType == ButtonStyleType.Disabled ? StartState.Unselectable : this.defaultStartState;
        bool active = this.buttonType == ButtonStyleType.Disabled ? false : this.activeButton;
        bool selectable = startState == StartState.Selectable ? true : false;

        ClearButtonClasses();

        this.defaultStartState = startState;
        SetActiveState(active, selectable, false);
        InitializeButton();
        MarkDirtyRepaint();
    }

    public void ReinitializeButton(ButtonStyleType newType, StartState newStartState, bool activeButton)
    {
        bool selectable = newType == ButtonStyleType.Disabled ? false : true;

        ClearButtonClasses();

        this.defaultStartState = newStartState;
        this.buttonType = newType;

        SetActiveState(activeButton, selectable, false);
        InitializeButton();
        MarkDirtyRepaint();
    }

    public void ReinitializeButton(ButtonStyleType newType)
    {
        if (newType == this.buttonType)
        {
            return;
        }

        StartState startState = newType == ButtonStyleType.Disabled ? StartState.Unselectable : StartState.Selectable;
        bool active = false;
        bool selectable = startState == StartState.Unselectable ? false : true;

        ClearButtonClasses();

        this.defaultStartState = startState;
        this.buttonType = newType;

        SetActiveState(active, selectable, false);
        InitializeButton();
        MarkDirtyRepaint();
    }

    public void SetButtonText(string newText = "")
    {
        if (ReferenceEquals(this.defaultButtonLabel, null))
        {
            return;
        }

        if (string.IsNullOrEmpty(newText))
        {
            this.defaultButtonLabel.text = this.defaultButtonText;
        }
        else
        {
            this.defaultButtonLabel.text = newText;
        }
    }

    public void SetSelectableState(bool canSelect, bool reinitialize = true)
    {
        this.SetEnabled(canSelect);
        this.selectable = canSelect;

        UpdateCustomSelectableColors();

        if (reinitialize)
        {
            InitializeButton();
            MarkDirtyRepaint();
        }
    }

    public void SetActiveState(bool isActive, bool stillSelectable = false, bool reinitialize = true)
    {
        this.RemoveFromClassList(GetHoverClass());

        if (isActive)
        {
            this.activeButton = true;
            onActiveStateChange.Invoke();

            if (this.activeBackgroundHighlight)
            {
                this.AddToClassList("active-background");
            }

            this.SetEnabled(stillSelectable);
            SetSelectableState(stillSelectable, reinitialize);

            this.OnSetActiveEvent.Invoke();
        }
        else
        {
            this.activeButton = false;
            onActiveStateChange.Invoke();

            if (this.activeBackgroundHighlight)
            {
                this.RemoveFromClassList("active-background");
            }

            this.SetEnabled(true);
            SetSelectableState(true, reinitialize);
            this.OnSetInactiveEvent.Invoke();
        }
    }

    #endregion

    public void AddActiveStateChangeAction(Action action)
    {
        this.onActiveStateChange.AddListener(() => action.Invoke());
    }

    private delegate void PulseDelegate();
    private PulseDelegate pulseDelegate;

    public IEnumerator PulseButton()
    {

        if (ReferenceEquals(this.pulseContainer, null))
        {
            this.pulseContainer = new VisualElement();
            this.pulseContainer.name = "pulse-container";
            this.pulseContainer.AddToClassList("pulse-container");
            this.pulseContainer.pickingMode = PickingMode.Ignore;
            this.Add(pulseContainer);
        }

        yield return new WaitForEndOfFrame();

        this.pulseContainer.AddToClassList("pulse");

        yield return new WaitForSeconds(this.pulseContainer.resolvedStyle.transitionDuration.First().value);

        this.pulseContainer.RemoveFromClassList("pulse");
    }

    protected virtual void CheckSizeChange()
    {
        float currentWidth;
        float currentHeight;

        if (!this.baseInitializationComplete)
        {
            return;
        }

        currentWidth = this.resolvedStyle.width;
        currentHeight = this.resolvedStyle.height;

        if (currentWidth <= 0 || currentHeight <= 0)
        {
            return;
        }

        if (currentWidth != this.baseWidth || currentHeight != this.baseHeight)
        {
            this.currentSizePercent = (currentHeight + currentWidth) / (this.baseHeight + this.baseWidth);
        }
    }

    protected float currentSizePercent = 1;
    public float CurrentSizeModifier
    {
        get
        {
            return this.currentSizePercent;
        }
    }
}
