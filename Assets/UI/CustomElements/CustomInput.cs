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
        }
    }
}
