using System;
using System.Collections.Generic;
using UI.Dates;
using UnityEngine;
using UnityEngine.Events;

public class DatePickerController : MonoBehaviour
{
    public static DatePickerController Active;
    [SerializeField] private DatePicker datePicker;
    [SerializeField] private Canvas datePickerCanvas;
    [SerializeField] private Transform datePickerDayTable;
    public DatePicker DatePicker => datePicker;
    public DateTime CurrentDate => datePicker.SelectedDate.Date;
   
    private void Awake()
    {
        if (Active != null)
        {
            GameObject.Destroy(Active);
        }

        Active = this;

        InitializeDatePicker();
    }

    private void InitializeDatePicker()
    {
        this.datePicker.Config.Events.OnDatePickerClosed.AddListener((DateTime dateTime) =>
        {
            OnClosingDatePicker();
            this.datePicker.Config.Misc.CloseWhenDateSelected = false;
        });

        ActionHelper.ExecuteActionWhenTrue(() =>
        {
            this.datePicker.SelectedDate = DateTime.Now;
            this.datePickerCanvas.sortingOrder = 10;
        }, new ActionHelper.ReturnBoolDelegate(() => this.datePickerDayTable.childCount > 0));
    }

    public void OpenDatePicker(DateTime? dateTime = null)
    {
        this.datePicker.Show();
        this.datePickerCanvas.enabled = true;

        if (dateTime.HasValue)
        {
            this.datePicker.SelectedDate = dateTime.Value;
        }
        this.datePicker.Config.Misc.CloseWhenDateSelected = true;
    }

    public void CloseDatePicker()
    {
        this.datePicker.Hide();
    }

    public void OnClosingDatePicker()
    {
        this.datePickerCanvas.enabled = false;
    }

    public void AddDateSelectedAction(UnityAction<DateTime> action)
    {
        this.datePicker.Config.Events.OnDaySelected.AddListener(action);
    }

    public void RemoveDateSelectedAction(UnityAction<DateTime> action)
    {
        this.datePicker.Config.Events.OnDaySelected.RemoveListener(action);
    }

    public void AddOnCloseAction(UnityAction<DateTime> action)
    {
        this.datePicker.Config.Events.OnDatePickerClosed.AddListener(action);
    }

    public void RemoveOnCloseAction(UnityAction<DateTime> action)
    {
        this.datePicker.Config.Events.OnDatePickerClosed.RemoveListener(action);
    }
}
