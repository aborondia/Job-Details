using UnityEngine.UIElements;
using UnityEngine;

public static class PositionHelper
{
    public enum ExtentTarget
    {
        Center,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
    }

    public static Vector2 GetTargetPosition(VisualElement targetElement, ExtentTarget targetSide)
    {
        return GetFinalPosition(targetElement, targetSide);
    }

    private static Vector2 GetFinalPosition(VisualElement element, ExtentTarget extentTarget)
    {
        float worldPositionX = element.worldBound.xMin;
        float worldPositionY = element.worldBound.yMin;
        float width = element.layout.width;
        float height = element.layout.height;

        switch (extentTarget)
        {
            case ExtentTarget.Bottom:
                return new Vector2(worldPositionX + (width / 2), worldPositionY + height);
            case ExtentTarget.BottomLeft:
                return new Vector2(worldPositionX, worldPositionY + height);
            case ExtentTarget.BottomRight:
                return new Vector2(worldPositionX + width, worldPositionY + height);
            case ExtentTarget.Center:
                return new Vector2(worldPositionX + (width / 2), worldPositionY + (height / 2));
            case ExtentTarget.Left:
                return new Vector2(worldPositionX, worldPositionY + (height / 2));
            case ExtentTarget.Right:
                return new Vector2(worldPositionX + width, worldPositionY + (height / 2));
            case ExtentTarget.Top:
                return new Vector2(worldPositionX + (width / 2), worldPositionY);
            case ExtentTarget.TopLeft:
                return new Vector2(worldPositionX, worldPositionY);
            case ExtentTarget.TopRight:
                return new Vector2(worldPositionX + width, worldPositionY);
            default:
                return Vector2.zero;
        }
    }

    public static ExtentTarget GetHourLabelExtentTarget(int value)
    {
        switch (value)
        {
            case 1:
                return ExtentTarget.BottomLeft;
            case 2:
                return ExtentTarget.BottomLeft;
            case 3:
                return ExtentTarget.Left;
            case 4:
                return ExtentTarget.TopLeft;
            case 5:
                return ExtentTarget.TopLeft;
            case 6:
                return ExtentTarget.Top;
            case 7:
                return ExtentTarget.TopRight;
            case 8:
                return ExtentTarget.TopRight;
            case 9:
                return ExtentTarget.Right;
            case 10:
                return ExtentTarget.BottomRight;
            case 11:
                return ExtentTarget.BottomRight;
            case 12:
                return ExtentTarget.Bottom;
            default:
                return ExtentTarget.Center;
        }
    }

    public static ExtentTarget GetMinuteLabelExtentTarget(int value)
    {
        if (value >= 55 || value <= 5)
        {
            return ExtentTarget.Bottom;
        }
        else if (value >= 51)
        {
            return ExtentTarget.BottomRight;
        }
        else if (value >= 40)
        {
            return ExtentTarget.Right;
        }
        else if (value >= 36)
        {
            return ExtentTarget.TopRight;
        }
        else if (value >= 25)
        {
            return ExtentTarget.Top;
        }
        else if (value >= 21)
        {
            return ExtentTarget.TopLeft;
        }
        else if (value >= 10)
        {
            return ExtentTarget.Left;
        }
        else if (value >= 6)
        {
            return ExtentTarget.BottomLeft;
        }
        else
        {
            return ExtentTarget.Center;
        }
    }
}