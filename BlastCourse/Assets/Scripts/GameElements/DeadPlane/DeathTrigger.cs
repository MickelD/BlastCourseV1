using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    #region Fields

    [SerializeField] bool _onlyKillPlayer = true;

    #endregion

    #region Variables

    BoxCollider _triggerBox;

    #endregion

    #region UnityFunctions

    private void OnValidate()
    {
        //Debug.Log(_triggerBox == null);
        if (_triggerBox == null)
        {
            _triggerBox = GetComponent<BoxCollider>();
        }
        else if (!_triggerBox.isTrigger) _triggerBox.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Health player = other.GetComponent<Health>();
        if(player != null && (other.GetComponent<PlayerMovement>() != null || !_onlyKillPlayer))
        {
            player.Die();
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Vector4(1, 0, 0, 0.6f);
        if(_triggerBox != null)Gizmos.DrawCube(_triggerBox.center + transform.position, _triggerBox.size);
    }

    #endregion
}
