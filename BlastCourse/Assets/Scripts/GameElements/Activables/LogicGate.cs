using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LogicGate : MonoBehaviour, IActivableTrigger, IActibableAction
{
    public enum GateType
    {
        AND,  // ALL:YES  /SOME:NO   /NONE:NO
        OR,   // ALL:NO   /SOME:YES  /NONE:NO
        NOT,  // ALL:NO   /SOME:NO   /NONE:YES
        XAND, // ALL:NO   /SOME:YES  /NONE:YES
        XOR,  // ALL:YES  /SOME:NO   /NONE:YES
        XNOT, // ALL:YES  /SOME:YES  /NONE:NO
    }

    public GateType _gateType;
    public bool _isActivated { get; set; } 
    public List<IActibableAction> _actions { get; set; }
    public List<IActivableTrigger> triggers { get; set; }
    public void CallActivate(bool isActive) 
    {
        foreach (IActibableAction activate in _actions)
        {
            activate.Activate(isActive);
        }
    }
    public void Activate(bool isActive)
    {

    }
}
