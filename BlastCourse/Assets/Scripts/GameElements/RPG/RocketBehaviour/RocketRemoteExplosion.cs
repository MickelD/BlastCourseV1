using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketRemoteExplosion : RocketBase
{
    [Space(5), Header("Specific Components"), Space(3)]
    [SerializeField] private Collider c_col;

    private bool _armed = true;
    private Vector3 _localPos;

    protected override void Start()
    {
        base.Start();

        EventManager.OnFireOrDetonateRemote?.Invoke(true);
    }

    private void Update()
    {
        if (transform.parent != null && !transform.parent.GetComponent<AudioManager>()) transform.localPosition = _localPos;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (ShouldExplodeOnThisSurface(collision))
        {
            _armed = true;
            Stick(collision);
        }
        else
        {
            Defuse();
        }
    }

    private void Stick(Collision col)
    {
        //c_col.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("StickedRockets");
        Destroy(c_rocketParticles.gameObject);
        c_rocketParticles.gameObject.transform.parent = null;

        Body.isKinematic = true;
        if(Mathf.Abs(Vector3.Angle(-col.contacts[0].normal, transform.forward)) > 75)transform.forward = -col.contacts[0].normal;
        transform.position = col.contacts[0].point + col.contacts[0].normal * 0.1f + transform.forward * (Mathf.Abs(Vector3.Angle(-col.contacts[0].normal, transform.forward)/420) + 0.1f);
        Debug.Log(Mathf.Abs(Vector3.Angle(-col.contacts[0].normal, transform.forward) / 180));

        transform.parent = col.transform;
        _localPos = transform.localPosition;
    }

    public override void Explode(Vector3 center, Vector3 direction)
    {
        if (!_alreadyExplodedOrDiffused)
        {
            if (_armed) 
            {
                base.Explode(center, direction); 
            }
            else Defuse();
        }
    }

    private void OnDisable()
    {
        EventManager.OnFireOrDetonateRemote?.Invoke(false);
    }

}
