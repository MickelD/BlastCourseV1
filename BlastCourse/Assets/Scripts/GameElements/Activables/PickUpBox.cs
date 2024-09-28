using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PickUpBox : PushButton
{
    [SerializeField] public ParticleSystem _vfx;

    public override void SendAllActivations(bool isActive)
    {
        _vfx.Emit(10);
        Locked = true;

        base.SendAllActivations(isActive);
        Locked = true;
        gameObject.GetComponent<Collider>().enabled = false;
        _animator.SetTrigger("Press");

        //if (isActive)
        //{
        //    //SetActiveAnim(isActive);

        //    if (!ResetOnUnpress)
        //    {
        //        Invoke(nameof(ResetButton), 0.5f);
        //    }
        //}
    }
}


