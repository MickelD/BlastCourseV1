using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FinalFan : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] float _startPow;
    [SerializeField] Material _offMat;
    [SerializeField] Material _onMat;
    [SerializeField] UnityEvent _OnFirstCharge;
    [SerializeField] UnityEvent _OnSecondCharge;
    [SerializeField] UnityEvent _OnThirdCharge;
    [SerializeField] UnityEvent _OnFourCharge;
    private int _charges;
    private float _power;

    private void OnTriggerStay(Collider other)
    {
        other.attachedRigidbody.AddForce(Vector3.up * _power * Time.deltaTime, ForceMode.Acceleration);
    }

    private void Start()
    {
        _power = _startPow;
        for (int i = 1; i < _meshRenderer.materials.Length; i++)
            _meshRenderer.materials[i].CopyPropertiesFromMaterial(_offMat);
    }

    public void SetPowerTo(float p) => _power = p;

    private void OnEnable()
    {
        _animator.SetFloat("Charge", _charges / 4f);
    }

    public void FeedFan()
    {
        _charges++;
        _animator.SetFloat("Charge", _charges / 4f);
        _meshRenderer.materials[_charges].CopyPropertiesFromMaterial(_onMat);

        switch (_charges)
        {
            case 1:
                _OnFirstCharge.Invoke();
                break;
            case 2:
                _OnSecondCharge.Invoke();
                break;
            case 3:
                _OnThirdCharge.Invoke();
                break;
            case 4:
                _OnFourCharge.Invoke();
                break;
            default:
                break;
        }
    }
}


