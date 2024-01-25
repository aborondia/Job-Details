using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<CustomVisualElement, UxmlTraits> { }
    public UxmlTraits uxmlTraits;
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlBoolAttributeDescription m_Regenerate_In_Editor =
        new UxmlBoolAttributeDescription { name = "regenerate-in-editor", defaultValue = false };
        UxmlBoolAttributeDescription m_Is_Image = new UxmlBoolAttributeDescription { name = "is-image", defaultValue = false };
        UxmlFloatAttributeDescription m_Largest_Dimension =
        new UxmlFloatAttributeDescription { name = "largest-dimension", defaultValue = 1 };
        UxmlBoolAttributeDescription m_Enforce_Largest_Dimension =
        new UxmlBoolAttributeDescription { name = "enforce-largest-dimension", defaultValue = false };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            CustomVisualElement ate = ve as CustomVisualElement;
            ate.initialized = false;
            ate.isImage = m_Is_Image.GetValueFromBag(bag, cc);
            ate.largestDimension = m_Largest_Dimension.GetValueFromBag(bag, cc);
            ate.enforceLargestDimension = m_Enforce_Largest_Dimension.GetValueFromBag(bag, cc);

            if (m_Regenerate_In_Editor.GetValueFromBag(bag, cc))
            {
                (ate as CustomVisualElement).InitializeValues();
            }

            if (Application.isPlaying)
            {
                ate.generateVisualContent += mgc =>
                {
                    (mgc.visualElement as CustomVisualElement).InitializeValues();
                    (mgc.visualElement as CustomVisualElement).CheckSizeChange();
                };
            }
        }
    }

    public bool InitializeMesgGenerationContextComplete = false;
    private bool isImage;
    private float largestDimension;
    private bool enforceLargestDimension;
    private string mostRecentIconClass;

    protected virtual void InitializeValues()
    {
        string currentIconClass = this.GetClasses().FirstOrDefault(cic => !string.IsNullOrEmpty(cic) && cic.Length >= 3 && cic.Substring(0, 3) == "gi-" && !cic.Equals(this.mostRecentIconClass));
        bool reinitialize = !string.IsNullOrEmpty(currentIconClass) && !currentIconClass.Equals(this.mostRecentIconClass);

        if (this.initialized && !reinitialize)
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
            if (this.enforceLargestDimension)
            {
                if (ReferenceEquals(this.resolvedStyle.backgroundImage.sprite?.rect, null))
                {
                    return;
                }

                float currentWidth = this.resolvedStyle.backgroundImage.sprite.rect.width;
                float currentHeight = this.resolvedStyle.backgroundImage.sprite.rect.height;
                float largestSpriteDimension;
                float percentDifference;

                if (currentWidth > currentHeight)
                {
                    largestSpriteDimension = currentWidth;
                }
                else
                {
                    largestSpriteDimension = currentHeight;
                }

                percentDifference = this.largestDimension / largestSpriteDimension;

                this.style.width = currentWidth * percentDifference;
                this.style.height = currentHeight * percentDifference;
            }
            else
            {
                this.baseWidth = this.resolvedStyle.backgroundImage.sprite.rect.width;
                this.baseHeight = this.resolvedStyle.backgroundImage.sprite.rect.height;
                this.style.width = this.baseWidth;
                this.style.height = this.baseHeight;
            }

            this.initialized = true;
        }
        else if (width > 0 && height > 0)
        {
            this.baseWidth = width;
            this.baseHeight = height;

            this.initialized = true;
        }

        this.mostRecentIconClass = currentIconClass;
    }

    protected virtual void CheckSizeChange()
    {
        float currentWidth;
        float currentHeight;

        if (!this.initialized)
        {
            return;
        }

        currentWidth = this.resolvedStyle.width;
        currentHeight = this.resolvedStyle.height;

        if (currentWidth <= 0 || currentHeight <= 0)
        {
            return;
        }

        if (currentWidth != this.baseWidth || currentHeight != this.baseHeight)
        {
            this.currentSizePercent = (currentHeight + currentWidth) / (this.baseHeight + this.baseWidth);
        }
    }

    protected float currentSizePercent = 1;
    public float CurrentSizeModifier
    {
        get
        {
            return this.currentSizePercent;
        }
    }
    protected float baseWidth;
    protected float baseHeight;
    public bool initialized { get; set; }
}
