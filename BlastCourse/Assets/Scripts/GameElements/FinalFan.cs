using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Cinemachine;

public class FinalFan : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] Material _offMat;
    [SerializeField] Material _onMat;
    [SerializeField] UnityEvent _onZeroCharges;
    [SerializeField] UnityEvent _OnFirstCharge;
    [SerializeField] UnityEvent _OnSecondCharge;
    [SerializeField] UnityEvent _OnThirdCharge;
    [SerializeField] UnityEvent _OnFourCharge;
    [SerializeField] CinemachineImpulseSource _camShake;
    [SerializeField] Vector4 _shakeForces;
    private int _charges;
    private float _power;

    private void OnTriggerStay(Collider other)
    {
        other.attachedRigidbody.AddForce(Vector3.up * _power * Time.deltaTime, ForceMode.Acceleration);
    }

    private void Start()
    {
        for (int i = 1; i < _meshRenderer.materials.Length; i++)
            _meshRenderer.materials[i].CopyPropertiesFromMaterial(_offMat);

        _onZeroCharges.Invoke();
    }

    public void SetPowerTo(float p) => _power = p;

    private void OnEnable()
    {
        _animator.SetFloat("Charge", _charges / 4f);
    }

    public void FeedFan(bool _alreadyFed = false)
    {
        _charges++;
        _animator.SetFloat("Charge", _charges / 4f);
        _meshRenderer.materials[_charges].CopyPropertiesFromMaterial(_onMat);

        switch (_charges)
        {
            case 1:
                _OnFirstCharge.Invoke();
                if (!_alreadyFed) _camShake.GenerateImpulse(_shakeForces.x * (OptionsLoader.Instance ? OptionsLoader.Instance.CameraShake : 1f));
                break;
            case 2:
                _OnSecondCharge.Invoke();
                if (!_alreadyFed) _camShake.GenerateImpulse(_shakeForces.y * (OptionsLoader.Instance ? OptionsLoader.Instance.CameraShake : 1f));
                break;
            case 3:
                _OnThirdCharge.Invoke();
                if (!_alreadyFed) _camShake.GenerateImpulse(_shakeForces.z * (OptionsLoader.Instance ? OptionsLoader.Instance.CameraShake : 1f));
                break;
            case 4:
                _OnFourCharge.Invoke();
                if (!_alreadyFed) _camShake.GenerateImpulse(_shakeForces.w * (OptionsLoader.Instance ? OptionsLoader.Instance.CameraShake : 1f));
                break;
            default:
                break;
        }
    }
}


