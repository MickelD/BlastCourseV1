using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BouncePad : MonoBehaviour
{
    #region Fields
    [Space(5), Header("Handle"), Space(3)]
    [SerializeField] private Transform c_directionController;
    [SerializeField] private float _handleDistance;
    [SerializeField] private float _force;

    [Space(5), Header("Handle"), Space(3)]
    [SerializeField] private Vector2 _size;
    [SerializeField] private MeshTiler _tiler;

    [Space(5), Header("VFX"), Space(3)]
    [SerializeField] private GameObject _boingVFX;

    [Space(5), Header("Sounds"), Space(3)]
    [SerializeField] AudioCue bounceSound;

    #endregion

    #region Variables

    private Vector3 _bounceDir = Vector3.up;

    #endregion

    #region Method

    public Vector3 Reload()
    {
        _bounceDir = c_directionController.position - transform.position;
        _bounceDir.Normalize();
        _bounceDir *= _handleDistance;

        if(Vector3.Angle(_bounceDir, transform.up) > 90)
        {
            _bounceDir *= -1;
        }

        c_directionController.position = transform.position + _bounceDir;

        return _bounceDir;
    }

    private void OnValidate()
    {
        if (_tiler != null)
        {
            _tiler.ManagedByScript = true;
            _tiler.Area = _size;
        }
    }

    #endregion

    #region Collisions && Triggers

    private void OnTriggerEnter(Collider other)
    {
        
        IBounceable canInteractWithBouncePad = other.GetComponent<IBounceable>();

        if(canInteractWithBouncePad != null)
        {
            AudioManager.TryPlayCueAtPoint(bounceSound, transform.position);
            Destroy(Instantiate(_boingVFX, transform.position + _bounceDir * 0.5f, Quaternion.identity), 5);
            canInteractWithBouncePad.BouncePadInteraction(_bounceDir, _force);
        }

        //RocketBase rocket = other.GetComponent<RocketBase>();
        //if(rocket != null)
        //{
        //    if (Vector3.Angle(rocket.GetVelocity(), transform.up) >= 90)
        //    {
        //        AudioManager.PlayCueAtPoint(bounceSound, transform.position);
        //        rocket.BouncePadInteraction(_direction);
        //    }
        //}
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position,c_directionController.position);
    }

    #endregion
}
