using System.Collections;
using System;
using UnityEngine;

public static class ActionHelper
{
    public delegate bool ReturnBoolDelegate();
    public delegate void ReturnStringDelegate(string value);

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
}
