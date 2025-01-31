using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeSelectQueryHandler : QueryHandler
{
    public enum TimeType
    {
        Hour,
        Minute,
    }
    private const int total_hours = 12;
    private const int total_minutes = 60;
    private VisualElement dial;
    private VisualElement clockCenterReference;
    private VisualElement drawCanvas;
    private VisualElement hourLabelsContainer;
    private VisualElement minuteLabelsContainer;
    private VisualElement timeDisplayContainer;
    private VisualElement timeDisplayHourLabelContainer;
    private CustomLabel timeDisplayHourLabel;
    private VisualElement timeDisplayMinutesLabelContainer;
    private CustomLabel timeDisplayMinutesLabel;
    private TimeLabel currentLabel;
    private List<TimeLabel> hourLabels;
    private List<TimeLabel> minuteLabels;
    private TimeType currentTimeSelectType = TimeType.Hour;
    private bool isDragging;
    private float currentAngle;
    private bool timeSelectOpen = false;
    private int selectedHour;
    private int selectedMinutes;
    private Action<TimeContentHolder> onTimeSelectedAction;

    #region Initialization

    public override void Initialize()
    {
        base.Initialize();

        this.HideParent();
    }

    protected override void InitializeElements()
    {
        this.parentElement.focusable = true;
        this.parentElement.RegisterCallback<BlurEvent>(evt => ActionHelper.OnBlur(evt, this.parentElement, () => CloseTimeSelect(false)));
        this.drawCanvas = QueryController.Active.RootDocument.rootVisualElement.Q<VisualElement>("root").Q<VisualElement>("draw-canvas");

        this.dial = this.parentElement.Q<VisualElement>("main");
        this.hourLabelsContainer = this.parentElement.Q<VisualElement>("hour-labels-container");
        this.minuteLabelsContainer = this.parentElement.Q<VisualElement>("minute-labels-container");
        this.clockCenterReference = this.dial.Q<VisualElement>("center-reference");
        this.timeDisplayContainer = this.parentElement.Q<VisualElement>("time-display-container");
        this.timeDisplayHourLabelContainer = this.timeDisplayContainer.Q<VisualElement>("hour-label-container");
        this.timeDisplayHourLabel = this.timeDisplayHourLabelContainer.Q<CustomLabel>();
        this.timeDisplayMinutesLabelContainer = this.timeDisplayContainer.Q<VisualElement>("minutes-label-container");
        this.timeDisplayMinutesLabel = this.timeDisplayMinutesLabelContainer.Q<CustomLabel>();

        this.drawCanvas.generateVisualContent += mgc =>
        {
            if (!ReferenceEquals(this.currentLabel, null))
            {
                Painter2D painter = mgc.painter2D;
                Color regularColor = new Color32(0, 255, 0, 255);
                Color transparentColor = new Color32(0, 255, 0, 64);
                painter.strokeColor = regularColor;
                painter.fillColor = regularColor;
                painter.lineJoin = LineJoin.Round;
                painter.lineCap = LineCap.Round;
                painter.lineWidth = 3;

                Vector2 startPosition = PositionHelper.GetTargetPosition(this.clockCenterReference, PositionHelper.ExtentTarget.Center);
                Vector2 centerTargetPosition = PositionHelper.GetTargetPosition(this.currentLabel, PositionHelper.ExtentTarget.Center);
                Vector2 endPosition = PositionHelper.GetTargetPosition(this.currentLabel, this.currentLabel.ExtentTarget);

                painter.BeginPath();
                painter.Arc(startPosition, 5, 0, 360);
                painter.Fill();

                painter.BeginPath();
                painter.MoveTo(startPosition);
                painter.LineTo(endPosition);
                painter.Stroke();

                painter.fillColor = transparentColor;
                painter.BeginPath();
                painter.Arc(centerTargetPosition, 20, 0, 360);
                painter.Fill();
            }
        };

        CreateTimeLabels();

        // dial.RegisterCallback<GeometryChangedEvent>(evt => PositionLabels());
        dial.RegisterCallback<PointerDownEvent>(OnPointerDown);
        dial.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        dial.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected override void SetViewElements()
    {
    }

    protected override void SetupInputs()
    {
    }

    protected override void SetupButtons()
    {
        dial.RegisterCallback<PointerDownEvent>(evt => this.isDragging = true);
        dial.RegisterCallback<PointerUpEvent>(evt =>
        {
            this.isDragging = false;
        });
    }

    private void CreateTimeLabels()
    {
        CreateHourLabels();
        CreateMinuteLabels();
    }

    private void CreateHourLabels()
    {
        this.hourLabels = new List<TimeLabel>();

        CreateHourTimeLabel(12);

        for (int i = 1; i < 12; i++)
        {
            CreateHourTimeLabel(i);
        }
    }

    private void CreateHourTimeLabel(int value)
    {
        TimeLabel timeLabel = new TimeLabel(value, TimeType.Hour, PositionHelper.GetHourLabelExtentTarget(value));
        timeLabel.AddToClassList("hour-label");
        this.hourLabelsContainer.Add(timeLabel);
        this.hourLabels.Add(timeLabel);
    }

    private void CreateMinuteLabels()
    {
        this.minuteLabels = new List<TimeLabel>();

        for (int i = 0; i < 60; i++)
        {
            CreateMinuteTimeLabel(i);
        }
    }

    private void CreateMinuteTimeLabel(int value)
    {
        TimeLabel timeLabel = new TimeLabel(value, TimeType.Minute, PositionHelper.GetMinuteLabelExtentTarget(value));
        timeLabel.AddToClassList("hour-label");
        this.minuteLabelsContainer.Add(timeLabel);
        this.minuteLabels.Add(timeLabel);
    }

    #endregion

    #region Actions

    public void OpenTimeSelect(
        Action<TimeContentHolder> timeSelectAction,
        TimeType timeType = TimeType.Hour,
        int initialHour = 0,
        int initialMinutes = 0)
    {
        if (this.timeSelectOpen)
        {
            return;
        }

        QueryController.Active.BlockInteractions(this.instanceId);

        this.onTimeSelectedAction = timeSelectAction;
        this.currentTimeSelectType = timeType;
        this.selectedHour = initialHour;
        this.selectedMinutes = initialMinutes;
        this.timeSelectOpen = true;

        UpdateTimeDisplayLabels();
        ShowParent();
        RefreshDisplay();
        WhileClockOpen().Forget();
        this.parentElement.Focus();
    }

    public void CloseTimeSelect(bool invokeCompletion)
    {

        if (invokeCompletion)
        {
            this.onTimeSelectedAction?.Invoke(new TimeContentHolder(this.selectedHour, this.selectedMinutes));
        }else{
            this.onTimeSelectedAction?.Invoke(null);
        }

        this.timeSelectOpen = false;
        HideParent();
        QueryController.Active.UnblockInteractions(this.instanceId);
    }


    private void OnPointerDown(PointerDownEvent evt)
    {
        this.isDragging = true;
        Vector2 pointerPos = evt.localPosition;
        this.currentAngle = GetAngleFromPosition(pointerPos);
        UpdatePointerPosition(this.currentAngle);
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (this.isDragging)
        {
            Vector2 pointerPos = evt.localPosition;
            this.currentAngle = GetAngleFromPosition(pointerPos);
            UpdatePointerPosition(this.currentAngle);
        }
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        this.isDragging = false;

        if (this.currentTimeSelectType == TimeType.Hour)
        {
            this.selectedHour = this.currentLabel.Value;
            this.currentTimeSelectType = TimeType.Minute;
        }
        else
        {
            this.selectedMinutes = this.currentLabel.Value;
            CloseTimeSelect(true);
        }

        this.currentLabel = null;

        RefreshDisplay();
    }

    #endregion

    #region Update

    private async UniTaskVoid WhileClockOpen()
    {
        await UniTask.WaitWhile(() =>
        {
            PositionLabels();
            this.drawCanvas.MarkDirtyRepaint();

            return this.timeSelectOpen;
        });
    }

    private void UpdateTimeDisplayLabels()
    {
        this.timeDisplayHourLabel.text = this.selectedHour.ToString();
        this.timeDisplayMinutesLabel.text = this.selectedMinutes > 9 ? this.selectedMinutes.ToString() : $"0{this.selectedMinutes}";
    }

    private void UpdateTimeDisplayHourLabel(int value)
    {
        this.timeDisplayHourLabel.text = value.ToString();
    }

    private void UpdateTimeDisplayMinutesLabel(int value)
    {
        this.timeDisplayMinutesLabel.text = value > 9 ? value.ToString() : $"0{value}";
    }

    private void RefreshDisplay()
    {
        if (this.currentTimeSelectType == TimeType.Hour)
        {
            VisualElementHelper.SetElementDisplay(this.hourLabelsContainer, DisplayStyle.Flex);
            VisualElementHelper.SetElementDisplay(this.minuteLabelsContainer, DisplayStyle.None);
        }
        else
        {
            VisualElementHelper.SetElementDisplay(this.hourLabelsContainer, DisplayStyle.None);
            VisualElementHelper.SetElementDisplay(this.minuteLabelsContainer, DisplayStyle.Flex);
        }
    }

    private void PositionLabels()
    {
        float dialSize = this.dial.resolvedStyle.width;
        float labelSize;
        float radius;
        int timeCount = this.currentTimeSelectType == TimeType.Hour ? total_hours : total_minutes;
        float angleStep = 360f / timeCount;
        List<TimeLabel> timeLabels = this.currentTimeSelectType == TimeType.Hour ? this.hourLabels : this.minuteLabels;


        for (int i = 0; i < timeCount; i++)
        {
            TimeLabel timeLabel = timeLabels[i];
            float angle = angleStep * i;

            labelSize = timeLabels[i].resolvedStyle.width;
            radius = (dialSize / 2) - (labelSize / 2);

            Vector2 pos = GetPositionAtAngle(angle, radius, dialSize);

            timeLabel.style.left = pos.x - (timeLabel.resolvedStyle.width / 2);
            timeLabel.style.top = pos.y - (timeLabel.resolvedStyle.height / 2);
        }
    }

    private void UpdatePointerPosition(float angle)
    {
        float correctedAngle = GetCorrectedAngle(angle);

        SetClosestLabel(correctedAngle);
    }

    private void SetClosestLabel(float angle)
    {
        int totalLabels = this.currentTimeSelectType == TimeType.Hour ? total_hours : total_minutes;
        List<TimeLabel> currentCollection = this.currentTimeSelectType == TimeType.Hour ? this.hourLabels : this.minuteLabels;
        float correctedAngle = GetCorrectedAngle(angle);

        float angleStep = 360f / totalLabels;
        int nearestLabelIndex = Mathf.RoundToInt(correctedAngle / angleStep) % totalLabels;

        this.currentLabel = currentCollection[nearestLabelIndex];

        switch (this.currentTimeSelectType)
        {
            case TimeType.Hour:
                UpdateTimeDisplayHourLabel(this.currentLabel.Value);
                break;
            case TimeType.Minute:
                UpdateTimeDisplayMinutesLabel(this.currentLabel.Value);
                break;
        }
    }

    #endregion

    #region Getters/Setters

    private float GetCorrectedAngle(float angle)
    {
        float correctedAngle = (angle - 90f) % 360f;

        if (correctedAngle < 0)
        {
            correctedAngle += 360f;
        }

        return correctedAngle;
    }

    private float GetAngleFromPosition(Vector2 pointerPos)
    {
        float dialSize = dial.resolvedStyle.width;
        Vector2 center = new Vector2(dialSize / 2, dialSize / 2);
        Vector2 direction = pointerPos - center;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle -= 90f;

        if (angle < 0)
        {
            angle += 360f;
        }

        return angle;
    }

    private Vector2 GetPositionAtAngle(float angle, float radius, float dialSize)
    {
        float radians = Mathf.Deg2Rad * angle;

        float centerX = dialSize / 2;
        float centerY = dialSize / 2;
        float x = centerX + (Mathf.Sin(radians) * radius);
        float y = centerY - (Mathf.Cos(radians) * radius);

        return new Vector2(x, y);
    }

    #endregion
}
