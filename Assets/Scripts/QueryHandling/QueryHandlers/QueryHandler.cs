using System.Collections;
using System.Collections.Generic;
using EnumLibrary;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class QueryHandler : MonoBehaviour
{
    [SerializeField] protected VisualTreeAsset parentBaseElement;
    [SerializeField] protected MainView mainView;
    protected VisualElement parentElement;
    public VisualElement ParentElement => parentElement;
    protected List<VisualElement> mainViewElements;
    protected Dictionary<Subview, List<VisualElement>> subviewElements;
    protected bool initialized;
    public bool Initialized => initialized;
    protected abstract void InitializeElements();
    protected abstract void SetViewElements();
    protected abstract void SetupInputs();
    protected abstract void SetupButtons();

    #region Initialization

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    public virtual void Initialize()
    {
        if (this.initialized)
        {
            return;
        }

        SetParentElement();
        InitializeElements();
        SetViewElements();
        SetupInputs();
        SetupButtons();

        QueryController.Active.OnMainViewChangedEvent.AddListener(OnMainViewChanged);
        QueryController.Active.OnSubviewChangedEvent.AddListener(OnSubviewChanged);
        QueryController.Active.OnAnyViewChangedEvent.AddListener(OnAnyViewChanged);

        ResetView();

        this.initialized = true;
    }

    protected void SetParentElement()
    {
        this.parentElement = QueryController.Active.RootDocument.rootVisualElement.Q<TemplateContainer>(this.parentBaseElement.name);

        this.mainViewElements = new List<VisualElement>();
        this.subviewElements = new Dictionary<Subview, List<VisualElement>>();
    }

    protected void AddMainViewElement(VisualElement element)
    {
        this.mainViewElements.Add(element);
    }

    protected void AddSubviewElement(Subview subview, VisualElement element)
    {
        if (!this.subviewElements.ContainsKey(subview))
        {
            this.subviewElements.Add(subview, new List<VisualElement>());
        }

        this.subviewElements[subview].Add(element);
    }

    #endregion

    #region View Change

    protected virtual void OnMainViewChanged()
    {
        if (this.mainView != MainView.None)
        {
            if (QueryController.Active.CurrentMainView == this.mainView)
            {
                ShowParent();
            }
            else
            {
                HideParent();
            }
        }
    }

    protected virtual void OnSubviewChanged()
    {
        Subview? previousSubview;
        Subview currentSubview;

        if (this.mainView != MainView.None && QueryController.Active.CurrentMainView != this.mainView)
        {
            this.HideParent();

            return;
        }

        previousSubview = QueryController.Active.PreviousView.Subview;
        currentSubview = QueryController.Active.CurrentSubview;

        if (previousSubview.HasValue && this.subviewElements.ContainsKey(previousSubview.Value))
        {
            HideSubviewElements(previousSubview.Value);
        }

        ShowSubviewElements(currentSubview);
    }

    protected virtual void OnAnyViewChanged()
    {

    }

    public void ResetView()
    {
        Subview currentSubview;

        if (this.mainView != MainView.None && QueryController.Active.CurrentMainView != this.mainView)
        {
            this.HideParent();

            return;
        }

        currentSubview = QueryController.Active.CurrentSubview;

        foreach (Subview subview in this.subviewElements.Keys)
        {
            if (subview != currentSubview)
            {
                HideSubviewElements(subview);
            }
        }

        if (this.subviewElements.ContainsKey(currentSubview))
        {
            ShowSubviewElements(currentSubview);
        }
    }

    #endregion

    #region Update

    public void ShowParent()
    {
        VisualElementHelper.SetElementDisplay(this.parentElement, DisplayStyle.Flex);
    }

    public void HideParent()
    {
        VisualElementHelper.SetElementDisplay(this.parentElement, DisplayStyle.None);
    }

    protected void ShowSubviewElements(Subview subview)
    {
        if (!this.subviewElements.ContainsKey(subview))
        {
            return;
        }

        foreach (VisualElement element in this.subviewElements[subview])
        {
            ShowElement(element);
        }
    }

    protected void HideSubviewElements(Subview subview)
    {
        if (!this.subviewElements.ContainsKey(subview))
        {
            return;
        }

        foreach (VisualElement element in this.subviewElements[subview])
        {
            HideElement(element);
        }
    }

    protected void ShowElement(VisualElement element)
    {
        VisualElementHelper.SetElementDisplay(element, DisplayStyle.Flex);
    }

    protected void HideElement(VisualElement element)
    {
        VisualElementHelper.SetElementDisplay(element, DisplayStyle.None);
    }

    #endregion

    #region Getters

    public bool ViewingMainView()
    {
        return QueryController.Active.CurrentMainView == this.mainView;
    }

    #endregion
}
