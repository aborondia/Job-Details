using System;
using System.Collections.Generic;
using System.Linq;
using UI.Dates;
using UnityEngine;
using UnityEngine.Events;

public class DatePickerController : MonoBehaviour
{
    private const string date_picker_inline_name = "DatePicker - Inline";
    public static DatePickerController Active;
    [SerializeField] private DatePicker datePicker;
    [SerializeField] private Canvas datePickerCanvas;
    [SerializeField] private Transform datePickerDayTable;
    private RectTransform datePickerInlineTransform;
    public DatePicker DatePicker => datePicker;
    public DateTime CurrentDate => datePicker.SelectedDate.Date;
    private bool datePickerElementStylesSet = false;

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

        if (!this.datePickerElementStylesSet)
        {
            ActionHelper.ExecuteActionNextFrame(() =>
            {
                this.datePickerInlineTransform = this.datePickerCanvas.transform
                .GetComponentsInChildren<RectTransform>()
                .FirstOrDefault(rt => rt.transform.name == date_picker_inline_name);

                if (!ReferenceEquals(this.datePickerInlineTransform, null))
                {
                    this.datePickerInlineTransform.anchorMin = Vector2.zero;
                    this.datePickerInlineTransform.anchorMax = Vector2.one;

                    this.datePickerInlineTransform.offsetMin = Vector2.zero;
                    this.datePickerInlineTransform.offsetMax = Vector2.zero;
                }
            });
        }
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
