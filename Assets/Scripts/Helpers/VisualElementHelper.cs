using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public static class VisualElementHelper
{
    public static void SetElementDisplay(VisualElement element, DisplayStyle displayStyle)
    {
        if (ReferenceEquals(element, null))
        {
            LogHelper.Active.DebugLogError("Element is null!");

            return;
        }

        if (element.resolvedStyle.display == displayStyle)
        {
            return;
        }

        element.style.display = displayStyle;
    }
}
