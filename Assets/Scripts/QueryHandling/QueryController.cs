using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Events;
using MainView = Enumerations.MainView;
using Subview = Enumerations.Subview;
using System;
using System.Linq;
using System.Text;
using CircularBuffer;

public class QueryController : MonoBehaviour
{
    public static QueryController Active;
    [SerializeField] private MainView defaultMainView;
    [SerializeField] private Subview defaultSubview;
    [SerializeField] private UIDocument rootDocument;
    public UIDocument RootDocument => rootDocument;
    [SerializeField] private LoginQueryHandler loginQueryHandler;
    public LoginQueryHandler LoginQueryHandler => loginQueryHandler;
    [SerializeField] private UsersQueryHandler usersQueryHandler;
    public UsersQueryHandler UsersQueryHandler => usersQueryHandler;
    [SerializeField] private JobDetailsQueryHandler jobDetailsQueryHandler;
    public JobDetailsQueryHandler JobDetailsQueryHandler => jobDetailsQueryHandler;
    [SerializeField] private HeaderQueryHandler headerQueryHandler;
    public HeaderQueryHandler HeaderQueryHandler => headerQueryHandler;
    [SerializeField] private DetailsReportsQueryHandler detailsReportsQueryHandler;
    public DetailsReportsQueryHandler DetailsReportsQueryHandler => detailsReportsQueryHandler;
    [SerializeField] private PopupsQueryHandler popupsQueryHandler;
    public PopupsQueryHandler PopupsQueryHandler => popupsQueryHandler;
    [SerializeField] private GameObject queryHandlersParent;
    [SerializeField] private QueryHandler[] queryhandlers;
    private VisualElement interactionBlocker;
    private CustomLabel interactionBlockerLabel;
    private VisualElement debugInfo;
    private CustomLabel debugInfoLabel;
    private CircularBuffer<FullViewContainer> previousViews;
    public CircularBuffer<FullViewContainer> PreviousViews => previousViews;
    private HashSet<int> interactionBlockerIds;
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

        this.debugInfo = this.rootDocument.rootVisualElement.Q<VisualElement>("debug-info");

#if UNITY_EDITOR
        this.debugInfoLabel = this.debugInfo.Q<CustomLabel>();
        VisualElementHelper.SetElementDisplay(this.debugInfo, DisplayStyle.Flex);
        this.OnAnyViewChangedEvent.AddListener(() => this.debugInfoLabel.text = $"{this.currentMainView} - {this.currentSubview}");
#else
        VisualElementHelper.SetElementDisplay(this.debugInfo, DisplayStyle.None);
#endif

        Initialize();
    }

    private void Initialize()
    {
        this.previousViews = new CircularBuffer<FullViewContainer>(10);

        this.interactionBlocker = this.rootDocument.rootVisualElement.Q<TemplateContainer>("InteractionBlocker");
        this.interactionBlockerLabel = this.interactionBlocker.Q<CustomLabel>();
        this.interactionBlockerIds = new HashSet<int>();
        UpdateInteractionBlocker();

        this.OnMainViewChangedEvent.AddListener(() => this.OnAnyViewChangedEvent.Invoke());
        this.OnSubviewChangedEvent.AddListener(() => this.OnAnyViewChangedEvent.Invoke());

        SetupEvents();
        ChangeView(this.defaultMainView, this.defaultSubview);
        GetAllQueryHandlers();
        InitializeAllQueryHandlers();

        this.OnMainViewChangedEvent.Invoke();

        this.initialized = true;
    }

    private void SetupEvents()
    {
        UnityAction onServerRequestStarted = () =>
        {
            int blockingId = BlockInteractions();
            UnityAction onServerRequestCompleted = () =>
            {
                UnblockInteractions(blockingId);
            };

            onServerRequestCompleted += () => AppController.Active.ServerCommunicator.OnRequestCompletedEvent.RemoveListener(onServerRequestCompleted);
            AppController.Active.ServerCommunicator.OnRequestCompletedEvent.AddListener(onServerRequestCompleted);
        };

        AppController.Active.ServerCommunicator.OnRequestStartedEvent.AddListener(onServerRequestStarted);
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

    public void ChangeView(MainView mainView, Subview subview)
    {
        if (this.currentMainView == mainView && this.currentSubview == subview)
        {
            return;
        }

        this.previousViews.PushFront(new FullViewContainer(this.currentMainView, this.currentSubview));

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
        FullViewContainer previousView;

        if (this.previousViews.Count() <= 0)
        {
            LogHelper.Active.LogError("Previous view not valid!");
        }

        previousView = this.previousViews.Front();
        this.previousViews.PopFront();

        ChangeView(previousView.MainView, previousView.Subview);

        this.previousViews.PopFront();
    }

    #endregion

    #region Actions

    public int BlockInteractions(string feedbackMessage = "Processing...")
    {
        int blockingId = this.interactionBlockerIds.Count <= 0 ? 0 : this.interactionBlockerIds.Max() + 1;


        this.interactionBlockerIds.Add(blockingId);
        UpdateInteractionBlocker();

        return blockingId;
    }

    public void BlockInteractions(int blockingId)
    {
        this.interactionBlockerIds.Add(blockingId);

        UpdateInteractionBlocker();
    }

    public void UnblockInteractions(int blockingId)
    {
        this.interactionBlockerIds.Remove(blockingId);

        UpdateInteractionBlocker();
    }

    private void UpdateInteractionBlocker()
    {
        if (this.interactionBlockerIds.Count > 0)
        {
            VisualElementHelper.SetElementDisplay(this.interactionBlocker, DisplayStyle.Flex);
        }
        else
        {
            VisualElementHelper.SetElementDisplay(this.interactionBlocker, DisplayStyle.None);
        }
    }

    #endregion
}
