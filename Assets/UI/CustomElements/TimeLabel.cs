using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TimeLabel : CustomLabel
{
    private PositionHelper.ExtentTarget extentTarget;
    public PositionHelper.ExtentTarget ExtentTarget => extentTarget;
    private TimeSelectQueryHandler.TimeType timeType;
    private int value;
    public int Value => value;
    private string timeDisplayText;
    public string TimeDisplayLabel => timeDisplayText;
    private bool displayOnClock;
    public bool DisplayOnClock => displayOnClock;

    public TimeLabel()
    {
        // RegisterCallback<GeometryChangedEvent>(e => { });
        // RegisterCallback<DetachFromPanelEvent>(e => { });
    }

    public TimeLabel(int value, TimeSelectQueryHandler.TimeType timeType, PositionHelper.ExtentTarget extentTarget)
    {
        this.value = value;
        this.timeType = timeType;
        this.extentTarget = extentTarget;
        this.name = $"{timeType} ({value})";

        if (this.timeType == TimeSelectQueryHandler.TimeType.Hour)
        {
            SetDisplayPropertiesHours();
        }
        else
        {
            SetDisplayPropertiesMinutes();
        }

        SetDisplayPropertiesMinutes();
        RegisterCallback<GeometryChangedEvent>(e => { });
        RegisterCallback<DetachFromPanelEvent>(e => { });
    }

    private void SetDisplayPropertiesHours()
    {
        this.timeDisplayText = this.value.ToString();
        this.displayOnClock = true;
        this.text = this.timeDisplayText;
    }

    private void SetDisplayPropertiesMinutes()
    {
        if (this.value < 10)
        {
            this.timeDisplayText = $"0{this.value}";
        }
        else
        {
            this.timeDisplayText = this.value.ToString();
        }

        this.displayOnClock = this.value % 5 == 0;

        if (this.displayOnClock)
        {
            this.text = this.timeDisplayText;
        }
    }
}
