using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPropagator : MonoBehaviour
{
    public enum PropagateTarget { Parent, Reference, Root}

    public PropagateTarget PropagateTo;
    [DrawIf(nameof(PropagateTo), PropagateTarget.Reference)] public MonoBehaviour mono;

    private void Start()
    {
        if (PropagateTo == PropagateTarget.Parent && transform.parent != null && transform.parent.TryGetComponent(out mono)) return;
        else if (PropagateTo == PropagateTarget.Root && transform.root != null && transform.root.TryGetComponent(out mono)) return;
    }

    private void OnTriggerEnter(Collider other)
    {
        mono.StartCoroutine(nameof(OnTriggerEnter), other);
    }

    private void OnTriggerExit(Collider other)
    {
        mono.StartCoroutine(nameof(OnTriggerExit), other);
    }
}


