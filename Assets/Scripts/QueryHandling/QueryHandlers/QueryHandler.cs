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
    protected Dictionary<Subview, VisualElement> subviewElements;
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

        this.initialized = true;
    }

    protected void SetParentElement()
    {
        this.parentElement = QueryController.Active.RootDocument.rootVisualElement.Q<TemplateContainer>(this.parentBaseElement.name);

        this.mainViewElements = new List<VisualElement>();
        this.subviewElements = new Dictionary<Subview, VisualElement>();
    }

    protected void AddMainViewElement(VisualElement element)
    {
        this.mainViewElements.Add(element);
    }

    protected void AddSubviewElement(Subview subview, VisualElement element)
    {
        this.subviewElements.Add(subview, element);
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
        if (this.mainView != MainView.None && QueryController.Active.CurrentMainView != this.mainView)
        {
            return;
        }

        foreach (var entry in this.subviewElements)
        {
            if (QueryController.Active.CurrentSubview == entry.Key)
            {
                VisualElementHelper.SetElementDisplay(entry.Value, DisplayStyle.Flex);
            }
            else
            {
                VisualElementHelper.SetElementDisplay(entry.Value, DisplayStyle.None);
            }
        }
    }

    protected virtual void OnAnyViewChanged()
    {

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
