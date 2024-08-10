using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaTrigger : ActivableBase
{
    [Space(5), Header("Area Trigger Properties"), Space(3)]

    [Tooltip("Send deactivation event on player exit")] public bool ResetOnExit;
    public bool DoOnce;

    //private List<Collider> triggerers = new List<Collider>();
    private bool entered;
    private bool left;
    private bool preventEntry;
    private bool preventExit;
    //private void Update()
    //{
    //    //if(triggerers != null) foreach(Collider c in triggerers) if (c == null && ResetOnExit)
    //    //        {
    //    //            SendAllActivations(false);
    //    //            triggerers.Remove(c);
    //    //            return;
    //    //        }
    //}

    [ActivableAction]
    public void PreventEntry(bool set)
    {
        preventEntry = set;
    }

    [ActivableAction]
    public void PreventExit(bool set)
    {
        preventExit = set;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((DoOnce && entered) || preventEntry) return;
        SendAllActivations(true);
        //triggerers.Add(other);
        entered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if ((DoOnce && left) || preventExit) return;
        if (ResetOnExit) SendAllActivations(false);
        //triggerers.Remove(other);
        left = true;
    }
}
