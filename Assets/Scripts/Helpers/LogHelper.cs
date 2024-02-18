using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogHelper : MonoBehaviour
{
    public static LogHelper Active;
    [SerializeField] private bool editorOnly = true;

    private void Awake()
    {
        if (Active != null)
        {
            Destroy(Active);
        }

        Active = this;
    }

    public void Log(string value)
    {
#if UNITY_EDITOR
        if (editorOnly)
        {
            return;
        }
#endif

        Debug.Log(value);
    }

    public void LogError(string value)
    {
#if UNITY_EDITOR
        if (editorOnly)
        {
            return;
        }
#endif

        Debug.LogError(value);
    }
}
