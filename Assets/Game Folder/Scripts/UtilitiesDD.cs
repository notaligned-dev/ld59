using UnityEngine;
using System;

public static class UtilitiesDD
{
    public static void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }

    public static void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void RequireNotNull(object obj, string nameOfObject)
    {
        if (obj == null)
            throw new ArgumentNullException(nameOfObject);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void RequireNotNull(params (object value, string paramName)[] arguments)
    {
        foreach (var (value, name) in arguments)
            RequireNotNull(value, name);
    }
}
