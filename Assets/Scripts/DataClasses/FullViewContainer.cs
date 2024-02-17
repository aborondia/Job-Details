using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumLibrary;

public class FullViewContainer
{
    private MainView? mainView = null;
    public MainView? MainView => mainView;
    private Subview? subview = null;
    public Subview? Subview => subview;
    public bool PreviousViewValid => !ReferenceEquals(mainView, null) && !ReferenceEquals(subview, null);

    public void SetView(MainView mainView, Subview subview)
    {
        this.mainView = mainView;
        this.subview = subview;
    }

    public void ResetView()
    {
        this.mainView = null;
        this.subview = null;
    }
}
