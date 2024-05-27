using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Properties"), Space(3)]
    [SerializeField] protected GameObject _mesh;
    [SerializeField] protected Material _onMaterial;
    [SerializeField] protected Material _offMaterial;
    public string _index;

    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] AudioCue _pickUpSfx;

    #endregion

    #region Variables

    public bool _collected = false;

    #endregion

    #region UnityFunctions

    protected virtual void Start()
    {
        if(_collected) _mesh.GetComponent<Renderer>().material = _offMaterial;
        else _mesh.GetComponent<Renderer>().material = _onMaterial;
    }

    #endregion

    #region Methods

    //Getter && Setters
    public bool GetCollected() { return _collected; }
    public void SetCollected(bool value) 
    {
        _collected = value;
        if (_collected) _mesh.GetComponent<Renderer>().material = _offMaterial;
        else _mesh.GetComponent<Renderer>().material = _onMaterial;
    }

    #endregion

    #region Collision&&Triggers

    private void OnTriggerStay(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (!_collected && player != null)
        {
            AudioManager.TryPlayCueAtPoint(_pickUpSfx, transform.position);
            _collected = true;
            _mesh.GetComponent<Renderer>().material = _offMaterial;
            SaveLoader.Instance?.CollectiblesFound.Add(_index);
        }
    }

    #endregion
}
