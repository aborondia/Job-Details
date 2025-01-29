using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TimeLabel : CustomLabel
{
    public TimeLabel()
    {
        RegisterCallback<GeometryChangedEvent>(e => { });
        RegisterCallback<DetachFromPanelEvent>(e => { });
    }

    public TimeLabel(int value, string label, TimeSelectQueryHandler.TimeType timeType, PositionHelper.ExtentTarget extentTarget)
    {
        this.value = value;
        this.timeType = timeType;
        this.extentTarget = extentTarget;
        this.text = label;
        this.name = $"{timeType} ({value})";
        RegisterCallback<GeometryChangedEvent>(e => { });
        RegisterCallback<DetachFromPanelEvent>(e => { });
    }

    private PositionHelper.ExtentTarget extentTarget;
    public PositionHelper.ExtentTarget ExtentTarget => extentTarget;
    private TimeSelectQueryHandler.TimeType timeType;
    private int value;
    public int Value => value;
}
