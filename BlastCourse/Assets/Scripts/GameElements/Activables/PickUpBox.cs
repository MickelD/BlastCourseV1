using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PickUpBox : PushButton
{
    public override void SendAllActivations(bool isActive)
    {
        base.SendAllActivations(isActive);

        if (isActive)
        {
            Locked = true;
            SetActiveAnim(isActive);

            if (!ResetOnUnpress)
            {
                Invoke(nameof(ResetButton), 0.5f);
            }
        }

        gameObject.SetActive(false);
    }
}


