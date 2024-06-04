using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MailTrigger : ActivableBase
{
    #region Fields

    [SerializeField] public BoxCollider _trigger;
    [SerializeField] public Vector3 _areaSize = Vector3.one;
    [SerializeField] public Transform _shaderPlane;
    [SerializeField] public float _maxShaderSize;
    [SerializeField] public AudioCue _interactSfx;

    #endregion

    #region Vars

    private bool _used = false;

    #endregion

    #region UnityFunctions

    private void OnEnable()
    {
        if (_trigger == null)
        {
            _trigger = gameObject.AddComponent<BoxCollider>();
            _trigger.size = _areaSize;
            _trigger.isTrigger = true;
        }
    }
    private void OnValidate()
    {
        if(_trigger != null)_trigger.size = _areaSize;
        if(_shaderPlane != null)
        {
            _shaderPlane.localPosition = -transform.up * ((_areaSize.y) / 2) + transform.up * 0.01f;
            if(_maxShaderSize <= _areaSize.x && _maxShaderSize <= _areaSize.z) _shaderPlane.localScale = new Vector3(_areaSize.x, _maxShaderSize, _areaSize.z) / 10;
            else _shaderPlane.localScale = new Vector3(_areaSize.x, _areaSize.x <= _areaSize.z ? _areaSize.x : _areaSize.z, _areaSize.z) / 10;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_used && other.GetComponent<PlayerMovement>())
        {
            SendAllActivations(true);
        }
    }

    #endregion

    #region Methods

    public override void SendAllActivations(bool isActive)
    {
        AudioManager.TryPlayCueAtPoint(_interactSfx, transform.position);
        _used = isActive;
        base.SendAllActivations(isActive);

        gameObject.SetActive(false);
    }

    #endregion

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Vector4(0, 0, 1, 0.4f);
        if (_trigger != null) Gizmos.DrawCube(_trigger.center + transform.position, _trigger.size);
    }

#endif
}


