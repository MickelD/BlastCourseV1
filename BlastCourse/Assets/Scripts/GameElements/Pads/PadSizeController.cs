using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadSizeController : MonoBehaviour
{
    #region Fields
    [Space(5), Header("Variables"), Space(3)]
    [SerializeField] Vector3 _padSize = Vector3.one;
    [SerializeField] bool _invertedParticles;

    [Space(5), Header("Scaling Properties"), Space(3)]
    [SerializeField] private bool _shouldDrawGizmos;
    [SerializeField] private float _AreaOfEffectScaleDiff;
    [SerializeField] private float _particleShapeScaleDiff;
    private Vector3 _systemCenter;

    [Space(5), Header("Components"), Space(3)]
    [SerializeField] Transform g_base;
    [SerializeField] Transform g_pad;
    [SerializeField] ParticleSystem c_particleField;
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

        //PARTICLE RESCALING
        //Remmeber that ParticleShape is most likely rotated 90-X to define particle direction, so vector components may be swapped
        ParticleSystem.ShapeModule particleShape = c_particleField.shape;

        
        particleShape.scale = new Vector3(_padSize.x - _particleShapeScaleDiff, _padSize.z - _particleShapeScaleDiff, 0.1f);
        if (_invertedParticles)
        {
            particleShape.position = new Vector3(0f, _padSize.y, 0f);
            particleShape.rotation = Vector3.right * 90f;

            ParticleSystem.MainModule particleMain = c_particleField.main;
            //time equals distance over speed, these are very complex calculations (Now particles are sure to travel all the way to the pad)
            particleMain.startLifetime = (_padSize.y - _particleShapeScaleDiff) / particleMain.startSpeed.constant;
        }
        else
        {
            particleShape.position = Vector3.zero;
            particleShape.rotation = Vector3.right * -90f;
        }
        

        //TRIGGER RESCALING
        c_collider.size = new Vector3(_padSize.x - _AreaOfEffectScaleDiff, _padSize.y, _padSize.z - _AreaOfEffectScaleDiff);
        c_collider.center = _systemCenter;
    }
    #endregion

    #region Methods

    public Vector3 DirectionalParticles(Vector3 direction)
    {
        ParticleSystem.VelocityOverLifetimeModule particleVelocity = c_particleField.velocityOverLifetime;
        particleVelocity.xMultiplier = direction.x * 2;
        particleVelocity.yMultiplier = direction.y * 2;
        particleVelocity.zMultiplier = direction.z * 2;

        return direction.normalized;
    }

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
