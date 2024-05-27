using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Properties"), Space(3)]
    [SerializeField] protected GameObject _mesh;
    [SerializeField] protected Material _onMaterial;
    [SerializeField] protected Material _offMaterial;

    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] AudioCue creditSound;

    #endregion

    #region Variables

    protected bool _active = true;
    protected bool _collected;

    #endregion

    #region UnityFunctions

    protected virtual void Start()
    {
        CreditManager.Instance._worldCredits.Add(gameObject);
    }

    #endregion

    #region Methods

    public void Activate(bool isActive)
    {
        _active = isActive;

        if (!_active) _mesh.SetActive(false);
        else _mesh.SetActive(true);
        if (!_collected) _mesh.GetComponent<Renderer>().material = _onMaterial;
        else _mesh.GetComponent<Renderer>().material = _offMaterial;
    }

    #endregion

    #region Collision&&Triggers

    private void OnTriggerStay(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if(_active && player != null)
        {
            if (!_collected)
            {
                AudioManager.TryPlayCueAtPoint(creditSound, transform.position);
                _collected = true;
                //ShopManager.Instance._credits++;
            }
            Activate(false);
        }
    }

    #endregion
}
