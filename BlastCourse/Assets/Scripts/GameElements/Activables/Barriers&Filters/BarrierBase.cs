using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierBase : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Properties"), Space(3)]
    [SerializeField] protected Vector2 _size;

    [Space(5), Header("Visuals"), Space(3)]
    [SerializeField] protected Material _openMat;
    [SerializeField] protected Material _closedMat;

    [Space(5), Header("Components"), Space(3)]
    [SerializeField] protected Collider _playerBarrier;
    [SerializeField] protected MeshRenderer _renderer;
    [SerializeField] protected Collider _rocketBarrier;
    [SerializeField] protected BoxCollider _trigger;

    #endregion

    #region UnityFunctions

    public void Awake()
    {
        Open(false);
    }

    #endregion

    #region Methods

    protected virtual void OnValidate()
    {
        _playerBarrier.transform.localScale = _rocketBarrier.transform.localScale = _renderer.transform.localScale = new Vector3(_size.x, _size.y, 1f);
        _trigger.size = new Vector3(_size.x, _size.y, 0.25f);
        _playerBarrier.transform.localPosition = _rocketBarrier.transform.localPosition = _renderer.transform.localPosition = _trigger.center = Vector3.up * _size.y / 2;
    }

    public virtual void Open(bool isActive)
    {
        _playerBarrier.gameObject.SetActive(isActive);
        _rocketBarrier.gameObject.SetActive(!isActive);
        _renderer.material = isActive ? _openMat : _closedMat;
    }

    #endregion
}
