using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class InvokeHelper
{
    /*
     * Use this class to Invoke methods by passing in delegates and anonymous functions,
     * instead of method name, which saves on having to create auxiliary methods.
     * For this to work you must use this.Invoke();
     */
    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        if(mb.isActiveAndEnabled) mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}
