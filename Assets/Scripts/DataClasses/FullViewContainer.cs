using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerations;

public class FullViewContainer
{
    private MainView mainView;
    public MainView MainView => mainView;
    private Subview subview;
    public Subview Subview => subview;
    // public bool PreviousViewValid => !ReferenceEquals(mainView, null) && !ReferenceEquals(subview, null);

    public FullViewContainer(MainView mainView, Subview subview)
    {
        this.mainView = mainView;
        this.subview = subview;
    }
    // public void SetView(MainView mainView, Subview subview)
    // {
    //     this.mainView = mainView;
    //     this.subview = subview;
    // }

    // public void ResetView()
    // {
    //     this.mainView = null;
    //     this.subview = null;
    // }
}
