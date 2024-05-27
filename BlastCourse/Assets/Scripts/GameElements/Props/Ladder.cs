using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Ladder : MonoBehaviour
{
    #region Fields

    [Space(3), Header("Values"), Space(5)]
    [SerializeField, Tooltip("The height of the stairs the stairs")] private float _length = 5;
    [SerializeField, Tooltip("The width of the stairs")] private float _width = 1;
    //[Tooltip("Maximun speed the player can have to be able to climb the stairs")] public float MaximunAcceptedSpeed = 10;
    [Tooltip("The speed the player climbs the stairs")] public float ClimbingSpeed = 200;
    [Tooltip("The speed at which the player will be launched off the stairs once they reach the end")] public float ExitSpeed = 60;

    [Space(3), Header("Mesh"), Space(5)]
    [SerializeField] MeshTiler _tiler;

    #endregion

    #region Vars

    private PlayerMovement _player;
    private BoxCollider _collider;

    #endregion

    #region Unity Functions

    private void Update()
    {
        if(_player != null && _player.transform.localPosition.y > _length)
        {
            _player.PushPull(-1000 * ExitSpeed * Time.deltaTime * (transform.forward));
            StartCoroutine(_player.IgnoreLadderForABit());
            _player.SetLadder(false);
        }
    }

    #endregion

    #region Getters && Setters

    public void SetPlayer(PlayerMovement pm)
    {
        _player = pm;

        if(EventManager.GameRpgHolder != null)
        {
            EventManager.GameRpgHolder.EnableShooting(pm == null);
        }
    }

    #endregion


#if UNITY_EDITOR

    private void OnValidate()
    {
        //Collider
        if (_collider != null) 
        {
            _collider.size = new Vector3(_width, _length + 0.2f, 0.5f);
            _collider.center = Vector3.up * ((_length + 0.2f )/2);
        } 
        else if(gameObject.GetComponent<BoxCollider>() == null) _collider = gameObject.AddComponent<BoxCollider>();
        else _collider = gameObject.GetComponent<BoxCollider>();

        //Debug
        if(_tiler != null)
        {
            _tiler.ManagedByScript = true;
            _tiler.Lenght = _length;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        if(_collider != null)Gizmos.DrawWireCube(_collider.center, _collider.size);
    }

#endif
}


