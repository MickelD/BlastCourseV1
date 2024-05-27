using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActibableTest : ActivableBase
{
    public bool _isActivated { get; set; }

    [ActivableAction]
    public void Action(bool isActive)
    {
        Debug.Log("Ding");
    }
}
