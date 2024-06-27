using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadSizeController : MonoBehaviour
{
    #region Fields
    [Space(5), Header("Variables"), Space(3)]
    [SerializeField] Vector3 _padSize = Vector3.one;

    [Space(5), Header("Scaling Properties"), Space(3)]
    [SerializeField] private bool _shouldDrawGizmos;
    [SerializeField] private float _AreaOfEffectScaleDiff;
    [SerializeField] private float _particleShapeScaleDiff;
    private Vector3 _systemCenter;

    [Space(5), Header("Components"), Space(3)]
    [SerializeField] Transform g_base;
    [SerializeField] Transform g_pad;
    [SerializeField] BoxCollider c_collider;
    #endregion

    #region UnityFunctions

    private void OnValidate()
    {
        //UPDATE CENTER
        _systemCenter = Vector3.up * (_padSize.y / 2);

        //MESH RESCALING
        g_base.localScale = new Vector3(_padSize.x, g_base.localScale.y, _padSize.z);
        g_pad.localScale = new Vector3(_padSize.x - _AreaOfEffectScaleDiff, g_pad.localScale.y, _padSize.z - _AreaOfEffectScaleDiff);    

        //TRIGGER RESCALING
        c_collider.size = new Vector3(_padSize.x - _AreaOfEffectScaleDiff, _padSize.y, _padSize.z - _AreaOfEffectScaleDiff);
        c_collider.center = _systemCenter;
    }
    #endregion

    #region Methods


    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        //Draw Cube that shows the trigger Area
        if (_shouldDrawGizmos)
        {
            Gizmos.color = Color.magenta;
            Gizmos.matrix = transform.localToWorldMatrix; //Gizmos CAN be rotated, you just need to change the Transformation matrix they use
            Gizmos.DrawWireCube(_systemCenter, new Vector3(_padSize.x - _AreaOfEffectScaleDiff, _padSize.y, _padSize.z - _AreaOfEffectScaleDiff));
        }

        //Gizmos.color = Color.blue;
        //Vector3 a = transform.position + transform.up * 0.1f + (transform.right * ( _size.x - 0.6f) + transform.forward * (5 * _size.z - 0.6f))/2;
        //Gizmos.DrawLine(a, a + transform.up * _size.y);
        //Vector3 b = transform.position + transform.up * 0.1f - (transform.right * (5 * _size.x - 0.6f) + transform.forward * (5 * _size.z - 0.6f))/2;
        //Gizmos.DrawLine(b, b + transform.up * _size.y);
        //Vector3 c = transform.position + transform.up * 0.1f + (transform.right * (5 * _size.x - 0.6f) - transform.forward * (5 * _size.z - 0.6f))/2;
        //Gizmos.DrawLine(c, c + transform.up * _size.y);
        //Vector3 d = transform.position + transform.up * 0.1f - (transform.right * (5 * _size.x - 0.6f) - transform.forward * (5 * _size.z - 0.6f))/2;
        //Gizmos.DrawLine(d, d + transform.up * _size.y);
        //Gizmos.DrawLine(a + transform.up * _size.y, b + transform.up * _size.y);
        //Gizmos.DrawLine(a + transform.up * _size.y, c + transform.up * _size.y);
        //Gizmos.DrawLine(a + transform.up * _size.y, d + transform.up * _size.y);
        //Gizmos.DrawLine(b + transform.up * _size.y, c + transform.up * _size.y);
        //Gizmos.DrawLine(b + transform.up * _size.y, d + transform.up * _size.y);
        //Gizmos.DrawLine(c + transform.up * _size.y, d + transform.up * _size.y);
    }
    #endregion
}
