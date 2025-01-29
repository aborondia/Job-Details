using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CustomVisualElement : VisualElement
{
    public enum SizeConstraint
    {
        None,
        EnforceSmallest,
        EnforceLargest,
    }

    public CustomVisualElement()
    {
        RegisterCallback<GeometryChangedEvent>(e => InitializeValues());
        RegisterCallback<DetachFromPanelEvent>(e => { });
    }

    [UxmlAttribute][SerializeField] private float largestDimension = 100;
    [UxmlAttribute][SerializeField] private float smallestDimension = 75;
    [UxmlAttribute][SerializeField] private SizeConstraint sizeConstraint = SizeConstraint.EnforceSmallest;
    private bool isImage;
    private string mostRecentIconClass;
    [UxmlAttribute][SerializeField] private bool shouldInitialize;

    protected virtual void InitializeValues()
    {
        string currentIconClass = this.GetClasses().FirstOrDefault(cic => !string.IsNullOrEmpty(cic) && cic.Length >= 3 && cic.Substring(0, 3) == "gi-" && !cic.Equals(this.mostRecentIconClass));
        bool reinitialize = !string.IsNullOrEmpty(currentIconClass) && !currentIconClass.Equals(this.mostRecentIconClass);

        if (!this.shouldInitialize && !reinitialize)
        {
            return;
        }

        if (this.GetClasses().Any(cl => cl.Contains("gi-")))
        {
            this.isImage = true;
        }
        else
        {
            this.isImage = false;
        }

        float width = this.resolvedStyle.width;
        float height = this.resolvedStyle.height;

        if (this.isImage)
        {
            this.pickingMode = PickingMode.Ignore;

            if (ReferenceEquals(this.resolvedStyle.backgroundImage.sprite?.rect, null))
            {
                return;
            }

            float currentWidth = this.resolvedStyle.backgroundImage.sprite.rect.width;
            float currentHeight = this.resolvedStyle.backgroundImage.sprite.rect.height;
            float largestSpriteDimension;
            float percentDifference;
            float finalHeight;
            float finalWidth;

            if (currentWidth > currentHeight)
            {
                largestSpriteDimension = currentWidth;
            }
            else
            {
                largestSpriteDimension = currentHeight;
            }

            switch (this.sizeConstraint)
            {
                case SizeConstraint.EnforceLargest:
                    percentDifference = largestSpriteDimension / this.largestDimension;
                    finalHeight = currentHeight * percentDifference;
                    finalWidth = currentWidth * percentDifference;
                    break;
                case SizeConstraint.EnforceSmallest:
                    percentDifference = this.smallestDimension / largestSpriteDimension;
                    finalHeight = currentHeight * percentDifference;
                    finalWidth = currentWidth * percentDifference;
                    break;
                default:
                    finalHeight = currentHeight;
                    finalWidth = currentWidth;
                    break;
            }

            this.style.width = finalWidth;
            this.style.height = finalHeight;

            this.shouldInitialize = false;
        }
        else if (width > 0 && height > 0)
        {
            this.shouldInitialize = false;
        }

        this.mostRecentIconClass = currentIconClass;
    }

    protected virtual void CheckSizeChange()
    {
        float currentWidth;
        float currentHeight;

        if (!this.shouldInitialize)
        {
            return;
        }

        currentWidth = this.resolvedStyle.width;
        currentHeight = this.resolvedStyle.height;

        if (currentWidth <= 0 || currentHeight <= 0)
        {
            return;
        }
    }

}
