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
    [SerializeField] private DetailsReportsQueryHandler detailsReportsQueryHandler;
    public DetailsReportsQueryHandler DetailsReportsQueryHandler => detailsReportsQueryHandler;
    [SerializeField] private GameObject queryHandlersParent;
    [SerializeField] private QueryHandler[] queryhandlers;
    private VisualElement interactionBlocker;
    private CustomLabel interactionBlockerLabel;
    private VisualElement debugInfo;
    private CustomLabel debugInfoLabel;
    private FullViewContainer previousView;
    public FullViewContainer PreviousView => previousView;
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
        this.previousView = new FullViewContainer();

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

    public void ChangeView(MainView mainView, Subview subview, bool resetPreviousView = false)
    {
        if (this.currentMainView == mainView && this.currentSubview == subview)
        {
            return;
        }

        if (resetPreviousView)
        {
            this.previousView.ResetView();
        }
        else
        {
            this.previousView.SetView(this.currentMainView, this.currentSubview);
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
            LogHelper.Active.LogError("Previous view not valid!");
        }

        ChangeView(this.previousView.MainView.Value, this.previousView.Subview.Value);
    }

    #endregion

    #region Actions

    public int BlockInteractions(string feedbackMessage = "Processing...")
    {
        int blockingId = this.interactionBlockerIds.Count <= 0 ? 0 : this.interactionBlockerIds.Max() + 1;

        StartCoroutine(UnblockIntereractionsAfterTimeout(blockingId));

        UpdateInteractionBlocker();

        return blockingId;
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

    private IEnumerator UnblockIntereractionsAfterTimeout(int blockingId, float timeout = 5f)
    {
        float elapsedTime = 0;

        yield return new WaitUntil(() =>
        {
            elapsedTime += Time.deltaTime;

            return elapsedTime >= timeout;
        });
    }

    #endregion
}
