using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using MainView = EnumLibrary.MainView;
using Subview = EnumLibrary.Subview;

public class QueryController : MonoBehaviour
{
    public static QueryController Active;
    [SerializeField] private MainView defaultMainView;
    [SerializeField] private Subview defaultSubview;
    [SerializeField] private UIDocument rootDocument;
    public UIDocument RootDocument => rootDocument;
    [SerializeField] private LoginQueryHandler loginQueryHandler;
    public LoginQueryHandler LoginQueryHandler => loginQueryHandler;
    [SerializeField] private JobDetailsQueryHandler jobDetailsQueryHandler;
    public JobDetailsQueryHandler JobDetailsQueryHandler => jobDetailsQueryHandler;
    [SerializeField] private GameObject queryHandlersParent;
    [SerializeField] private QueryHandler[] queryhandlers;
    private FullViewContainer previousView;
    public FullViewContainer PreviousView => previousView;
    private bool initialized;
    public bool Initialized => initialized;
    private MainView currentMainView;
    public MainView CurrentMainView => currentMainView;
    private Subview currentSubview;
    public Subview CurrentSubview => currentSubview;
    public UnityEvent OnMainViewChangedEvent = new UnityEvent();
    public UnityEvent OnSubviewChangedEvent = new UnityEvent();
    public UnityEvent OnAnyViewChangedEvent = new UnityEvent();

    #region Initialization

    private void Awake()
    {
        if (Active != null)
        {
            GameObject.Destroy(Active);
        }

        Active = this;

        Initialize();
    }

    private void Initialize()
    {
        this.previousView = new FullViewContainer();

        this.OnMainViewChangedEvent.AddListener(() => this.OnAnyViewChangedEvent.Invoke());
        this.OnSubviewChangedEvent.AddListener(() => this.OnAnyViewChangedEvent.Invoke());

        GetAllQueryHandlers();
        InitializeAllQueryHandlers();

        ChangeView(this.defaultMainView, this.defaultSubview);

        this.initialized = true;
    }

    private void GetAllQueryHandlers()
    {
        this.queryhandlers = this.queryHandlersParent.GetComponentsInChildren<QueryHandler>();
    }

    private void InitializeAllQueryHandlers()
    {
        foreach (QueryHandler queryHandler in this.queryhandlers)
        {
            queryHandler.Initialize();
        }
    }

    #endregion

    #region View Change

    public void ChangeView(MainView mainView, Subview subview, bool resetPreviousView = false)
    {
        if (this.currentMainView == mainView && this.currentSubview == subview)
        {
            return;
        }

        if (resetPreviousView)
        {
            previousView.ResetView();
        }
        else
        {
            previousView.SetView(this.currentMainView, this.currentSubview);
        }

        if (this.currentMainView != mainView)
        {
            ChangeMainView(mainView);
        }

        if (this.currentSubview != subview)
        {
            ChangeSubview(subview);
        }
    }

    private void ChangeMainView(MainView view)
    {
        if (this.currentMainView == view)
        {
            return;
        }

        this.currentMainView = view;

        this.OnMainViewChangedEvent.Invoke();
    }

    private void ChangeSubview(Subview subview)
    {
        if (this.currentSubview == subview)
        {
            return;
        }

        this.currentSubview = subview;

        this.OnSubviewChangedEvent.Invoke();
    }

    public void ReturnToPreviousView()
    {
        if (!this.previousView.PreviousViewValid)
        {
            LogHelper.Active.DebugLogError("Previous view not valid!");
        }

        ChangeView(this.previousView.MainView.Value, this.previousView.Subview.Value, true);
    }

    #endregion
}
