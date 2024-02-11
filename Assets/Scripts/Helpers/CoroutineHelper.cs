using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    public static CoroutineHelper Active;

    private void Awake()
    {
        if (Active != null)
        {
            GameObject.Destroy(Active);
        }

        Active = this;
    }

    public void RunCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
