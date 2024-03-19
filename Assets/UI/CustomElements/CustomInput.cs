using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomInput : TextField
{
    public new class UxmlFactory : UxmlFactory<CustomInput, UxmlTraits> { }
    public UxmlTraits uxmlTraits;
    public new class UxmlTraits : TextField.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            CustomInput ate = ve as CustomInput;

            ate.AddToClassList("custom-input");
        }
    }

    public void AddCustomOnFocusOutCallback(EventCallback<FocusOutEvent> callback)
    {
        this.RegisterCallback<FocusOutEvent>(callback);
    }

    public void AddCustomBlurCallback(EventCallback<BlurEvent> callback)
    {
        this.RegisterCallback<BlurEvent>(callback);
    }

    public void AddCustomFocusInCallback(EventCallback<FocusInEvent> callback)
    {
        this.RegisterCallback<FocusInEvent>(callback);
    }

    public void AddCustomFocusCallback(EventCallback<FocusEvent> callback)
    {
        this.RegisterCallback<FocusEvent>(callback);
    }
}
