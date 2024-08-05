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
    public float _killTime;
    private WaitForSeconds _killTimer;
    private Coroutine _killTimerCoroutine;
    #endregion

    #region Methods

    protected override void Start()
    {
        base.Start();

        _killTimer = new WaitForSeconds(_killTime);
    }

    [ActivableAction]
    public void SetOn(bool set)
    {
        //_renderer.materials[_emissiveMaterialIndex].DOKill();

        _renderer.materials[_emissiveMaterialIndex].DOColor(ExtendedDataUtility.Select(set, _onColor, _offColor), "_EmissionColor", _colorTransitionTime);

        if(Type != ActivableType.Action) SendAllActivations(set);
    }

    [ActivableAction]
    public void SetOnAndOff(bool set)
    {
        if (!set) return;

        if (_killTimerCoroutine != null) StopCoroutine(_killTimerCoroutine);

        _killTimerCoroutine = StartCoroutine(OnAndOffCoroutine());
    }

    private IEnumerator OnAndOffCoroutine()
    {

        _renderer.materials[_emissiveMaterialIndex].DOColor(_onColor, "_EmissionColor", _colorTransitionTime);
        yield return _killTimer;
        _renderer.materials[_emissiveMaterialIndex].DOColor(_offColor, "_EmissionColor", _colorTransitionTime);
    }

    #endregion
}


