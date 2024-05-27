using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CustomMethods;

public class SmartLight : ActivableBase
{
    #region Fields

    [ColorUsage(false, true)] public Color _offColor;
    [ColorUsage(false, true)] public Color _onColor;

    [Space(3)] public Renderer _renderer;
    public int _emissiveMaterialIndex;
    public float _colorTransitionTime;

    #endregion

    #region Methods

    [ActivableAction]
    public void SetOn(bool set)
    {
        //_renderer.materials[_emissiveMaterialIndex].DOKill();

        _renderer.materials[_emissiveMaterialIndex].DOColor(ExtendedDataUtility.Select(set, _onColor, _offColor), "_EmissionColor", _colorTransitionTime);

        if(Type != ActivableType.Action) SendAllActivations(set);
    }

    #endregion
}


