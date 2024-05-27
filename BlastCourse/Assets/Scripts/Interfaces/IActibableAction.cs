using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActibableAction
{
    public GameObject gameObject { get; }

    public List<IActivableTrigger> triggers { get; set; } // Objects that trigger this

    public bool _isActivated { get; set; } // Is the object activated
    public void Activate(bool isActive); // Method triggered when it is activated
}
