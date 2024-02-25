using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class CustomEnumField : EnumField
{
    public new class UxmlFactory : UxmlFactory<CustomEnumField, UxmlTraits> { }
    public UxmlTraits uxmlTraits;
    public new class UxmlTraits : EnumField.UxmlTraits
    {

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            CustomEnumField ate = ve as CustomEnumField;

            ate.AddToClassList("custom-enum-field");
        }
    }
}

