using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaTrigger : ActivableBase
{
    [Space(5), Header("Area Trigger Properties"), Space(3)]

    [Tooltip("Send deactivation event on player exit")] public bool ResetOnExit;
    public bool DoOnce;

    private List<Collider> triggerers = new List<Collider>();
    private bool passed;

    private void Update()
    {
        if(triggerers != null) foreach(Collider c in triggerers) if (c == null && ResetOnExit)
                {
                    SendAllActivations(false);
                    triggerers.Remove(c);
                    return;
                }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DoOnce && passed) return;
        SendAllActivations(true);
        triggerers.Add(other);
        passed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (DoOnce && passed) return;
        if (ResetOnExit) SendAllActivations(false);
        triggerers.Remove(other);
        passed = true;
    }
}
