using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AtemporalArea : ActivableBase
{
    #region Fields

    [Space(5), Header("Atenporal Area"), Space(3)]
    public GameObject _forceFieldMesh;
    public AudioCue OnSound;
    public AudioCue OffSound;

    #endregion

    #region Vars

    //vector value is velocity object had before it was frozen
    private List<ScaledTimeMonoBehaviour> FrozenBodies = new();

    #endregion

    #region UnityFunctions

    public void OnTriggerEnter(Collider other)
    {
        ScaledTimeMonoBehaviour mono = other.GetComponent<ScaledTimeMonoBehaviour>();

        if (mono != null)
        {
            SetFreezeBody(mono, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        ScaledTimeMonoBehaviour mono = other.GetComponent<ScaledTimeMonoBehaviour>();

        if (mono != null)
        {
            if (mono is RocketBase)
            {
                mono.DOKill();
            }
            else
            {
                SetFreezeBody(mono, false);
                if (FrozenBodies.Contains(mono)) FrozenBodies.Remove(mono);
            }
        }
    }

    #endregion

    #region Methods

    [ActivableAction]
    public void FreezeTime(bool freeze)
    {
        _forceFieldMesh.SetActive(freeze);
        AudioManager.TryPlayCueAtPoint(freeze ? OnSound : OffSound, transform.position);


        if (freeze)
        {
            //Collider[] cols = new Collider[10];
        }
        else
        {
            foreach (ScaledTimeMonoBehaviour mono in FrozenBodies) if (mono != null) SetFreezeBody(mono, false);
            FrozenBodies.Clear();
        }
    }

    private void SetFreezeBody(ScaledTimeMonoBehaviour mono, bool freeze)
    {
        if (freeze)
        {
            if (!FrozenBodies.Contains(mono))
            {
                FrozenBodies.Add(mono);
                mono.FreezeTime(true);
            }

            FrozenBodies.RemoveAll(b => b == null);
        }
        else
        {
            if (FrozenBodies.Contains(mono))
            {
                mono.FreezeTime(false);
                //FrozenBodies.Remove(mono);
            }
        }
    }

    #endregion
}