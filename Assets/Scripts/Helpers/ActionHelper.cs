using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public static class ActionHelper
{
    public delegate bool ReturnBoolDelegate();
    public delegate void StringDelegate(string value);
    public delegate void BoolDelegate(bool value);
    public delegate void RoleTypeDelegate(Enumerations.UserRoleEnum value);
    public delegate void ActionCompleteDelegate();

    public static void ExecuteActionNextFrame(Action action)
    {
        CoroutineHelper.Active.RunCoroutine(StartExecutingActionNextFrame(action));
    }

    private static IEnumerator StartExecutingActionNextFrame(Action action)
    {
        yield return new WaitForEndOfFrame();

        action?.Invoke();
    }

    public static void ExecuteActionAfterDelay(Action action, float delay = .25f)
    {
        CoroutineHelper.Active.RunCoroutine(StartExecutingActionAfterDelay(action, delay));
    }

    private static IEnumerator StartExecutingActionAfterDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }

    public static void ExecuteActionWhenTrue(Action action, ReturnBoolDelegate returnBoolDelegate)
    {
        CoroutineHelper.Active.RunCoroutine(StartExecutingActionWhenTrue(action, returnBoolDelegate));
    }

    private static IEnumerator StartExecutingActionWhenTrue(Action action, ReturnBoolDelegate returnBoolDelegate)
    {
        yield return new WaitUntil(() => returnBoolDelegate.Invoke());

        action?.Invoke();
    }

    public static void OnBlur(BlurEvent evt, VisualElement parentElement, Action blurAction)
    {
        EventCallback<BlurEvent> newBlurEvent;
        VisualElement eventTarget;

        if (evt?.relatedTarget == null ||
        (evt.relatedTarget.GetType().IsAssignableFrom(typeof(VisualElement))
        && !parentElement.Contains(evt.relatedTarget as VisualElement)))
        {
            blurAction?.Invoke();
        }
        else
        {
            eventTarget = evt.relatedTarget as VisualElement;

            if (eventTarget == parentElement)
            {
                return;
            }

            newBlurEvent = evt => OnBlur(evt, parentElement, blurAction);
            newBlurEvent += evt => eventTarget.UnregisterCallback(newBlurEvent);

            eventTarget.RegisterCallback(newBlurEvent);
        }
    }
}
