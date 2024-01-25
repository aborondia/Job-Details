using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomLabel : Label
{
    public new class UxmlFactory : UxmlFactory<CustomLabel, UxmlTraits> { }
    public UxmlTraits uxmlTraits;
    public new class UxmlTraits : Label.UxmlTraits
    {
        UxmlBoolAttributeDescription m_ShouldNotUpdate =
        new UxmlBoolAttributeDescription { name = "should-not-update", defaultValue = true };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            CustomLabel ate = ve as CustomLabel;

            if (Application.isPlaying)
            {
                ate.generateVisualContent += mgc =>
                {
                    (mgc.visualElement as CustomLabel).InitializeValues();
                };
            }
        }
    }

    public float baseFontSize { get; set; } // editor
    private float baseWidth;
    private float baseHeight;
    public bool Initialized { get; set; }

    private void InitializeValues()
    {
        if (this.Initialized)
        {
            return;
        }

        float width = this.resolvedStyle.width;
        float height = this.resolvedStyle.height;
        float fontSize = this.resolvedStyle.fontSize;

        if (width > 0 && height > 0 && fontSize > 0)
        {
            this.baseWidth = width;
            this.baseHeight = height;
            this.baseFontSize = fontSize;

            this.Initialized = true;
        }
    }
}
