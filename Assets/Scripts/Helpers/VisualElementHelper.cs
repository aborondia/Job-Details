using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public static class VisualElementHelper
{
    public static void SetElementDisplay(VisualElement element, DisplayStyle displayStyle)
    {
        if (ReferenceEquals(element, null))
        {
            LogHelper.Active.LogError("Element is null!");

            return;
        }

        if (element.resolvedStyle.display == displayStyle)
        {
            return;
        }

        element.style.display = displayStyle;
    }

    public static void SetElementVisibility(VisualElement element, Visibility visibility)
    {
        if (ReferenceEquals(element, null))
        {
            LogHelper.Active.LogError("Element is null!");

            return;
        }

        if (element.resolvedStyle.visibility == visibility)
        {
            return;
        }

        element.style.visibility = visibility;
    }
}
