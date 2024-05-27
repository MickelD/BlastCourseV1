using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IActivableTrigger 
{
    public GameObject gameObject { get; }

    public bool _isActivated { get; set; } // Is the object activated
    public List<IActibableAction> _actions { get; set; } // The objects being activated
    public void CallActivate(bool isActive); // The function called when it is triggered
}
