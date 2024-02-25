using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class CleanerNameList : ListView
{
    public new class UxmlFactory : UxmlFactory<CleanerNameList, UxmlTraits> { }
    public UxmlTraits uxmlTraits;
    public new class UxmlTraits : ListView.UxmlTraits
    {

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            CleanerNameList ate = ve as CleanerNameList;

            ate.AddToClassList("cleaner-name-list");

            ate.generateVisualContent += mgc =>
            {
                if (ate.Initialized)
                {
                    return;
                }

                ate.InitializeList();
            };
        }
    }

    private bool initialized = false;
    public bool Initialized => initialized;

    private void InitializeList()
    {
        // Create a list of data. In this case, numbers from 1 to 1000.
        List<string> items = new List<string> { "Cleaner 1", "Cleaner 2", "Cleaner 3" };

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        // Func<VisualElement> makeItem = () => new Label();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        // Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        // Provide the list view with an explict height for every row
        // so it can calculate how many items to actually display

        this.fixedItemHeight = 16;
        this.makeItem = () => new Label();
        this.bindItem = (e, i) => (e as Label).text = items[i];
        var listView = new ListView(items, itemHeight, makeItem, bindItem);

        this.selectionType = SelectionType.Single;

        this.itemsChosen += objects => Debug.Log(objects);
        this.selectionChanged += objects => Debug.Log(objects);

        this.style.flexGrow = 1.0f;

        // rootVisualElement.Add(listView);
    }
}