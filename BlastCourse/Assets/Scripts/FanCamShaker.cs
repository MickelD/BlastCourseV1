using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FanCamShaker : MonoBehaviour
{
    [SerializeField] CameraMovementEffects _camEffects;
    private float _intensity;
    private float _Intensity
    {
        get { return _intensity; }
        set 
        { 
            _intensity = value;
            if (_in) _camEffects._noise.m_AmplitudeGain = _Intensity * (OptionsLoader.Instance ? OptionsLoader.Instance.CameraShake : 1f);
        }
    }
    private bool _in;

    private void OnTriggerEnter(Collider other)
    {
        _in = true;
        _camEffects._noise.m_AmplitudeGain = _Intensity * (OptionsLoader.Instance ? OptionsLoader.Instance.CameraShake : 1f);
    }

    private void OnTriggerExit(Collider other)
    {
        _in = false;
        _camEffects._noise.m_AmplitudeGain = 0f;
    }

    public void SetShakeIntensity(float intensity)
    {
        _Intensity = intensity;
    }
}


