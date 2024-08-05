using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;

public class RocketBase : ScaledTimeMonoBehaviour, IBounceable
{
    #region Fields
    [Space(5), Header("Components"), Space(3)]
    [SerializeField] protected ParticleSystem c_rocketParticles;
    [SerializeField] protected GameObject g_defuseParticles;
    [SerializeField] protected Collider c_playerTrigger;
    [SerializeField] protected AudioCue _explodeSfx;
    [SerializeField] protected AudioCue _defuseSfx;

    #endregion

    #region Variables

    [HideInInspector] public RpgBase rpg;

    protected bool _alreadyExplodedOrDiffused = false;

    #endregion

    #region UnityFunctions

    protected virtual void OnEnable()
    {
        c_playerTrigger.enabled = false;
    }

    protected override void Start()
    {
        base.Start();

        EventManager.RocketCount++;
        rpg.rockets.Add(this);
        if (rpg.rockets.Count >= rpg._stats.ActiveRocketCap)
        {
            rpg.rockets[0].Explode(rpg.rockets[0].transform.position, Vector3.up);
        }
    }

    protected virtual void OnDestroy()
    {
        EventManager.RocketCount--;
        rpg.rockets.Remove(this);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //SHOULD ROCKET EXPLOTE ON CONTACT WITH THIS SURFACE
        if (ShouldExplodeOnThisSurface(collision)) Explode(collision.contacts[0].point, collision.GetContact(0).normal);
        else Defuse();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            Explode(transform.position, Vector3.up);

            _alreadyExplodedOrDiffused = true;
        }
    }

    #endregion

    #region Methods

    protected bool ShouldExplodeOnThisSurface(Collision col)
    {
        //check material of collided Surface

        if(col.collider.gameObject.CompareTag("Defuse"))
        {
            return false;
        }
        else if (col.collider.TryGetComponent(out MeshRenderer meshRenderer))
        {
            MeshCollider meshCollider = col.collider as MeshCollider;

            int submesh = 0;

            //Mesh Collider, check material of hit face
            if (meshCollider != null && 
                meshCollider.sharedMesh != null && 
                meshCollider.Raycast(new Ray(transform.position, col.GetContact(0).point - transform.position), out RaycastHit hitInfo, 1f)) 
            {
                submesh = ExtendedDataUtility.GetSubmeshFromTriangle(hitInfo.triangleIndex, meshCollider.sharedMesh);
            }

            return !rpg._stats.Explosion.ExplosionRules.DeffuserMaterials.Contains(meshRenderer.sharedMaterials[submesh]);
        }
        else
        {
            return true;
        }  
    }

    public virtual void Defuse()
    {
        if (!_alreadyExplodedOrDiffused)
        {
            Destroy(Instantiate(g_defuseParticles, transform.position, Quaternion.identity, AudioManager.Instance.transform), 4f);

            AudioManager.TryPlayCueAtPoint(_defuseSfx, transform.position);

            if (c_rocketParticles != null)
            {
                c_rocketParticles.Stop();
                Destroy(c_rocketParticles.gameObject, 2f);
                c_rocketParticles.gameObject.transform.parent = null;
            }

            _alreadyExplodedOrDiffused = true;

            Destroy(gameObject);
        }
    }

    public virtual void Explode(Vector3 center, Vector3 direction)
    {
        if (!_alreadyExplodedOrDiffused)
        {
            rpg._stats.Explosion.Explode(center, direction, rpg._rpgHolder.g_camera.transform.position);

            AudioManager.TryPlayCueAtPoint(_explodeSfx, transform.position);

            if(c_rocketParticles != null)
            {
                c_rocketParticles.Stop();
                Destroy(c_rocketParticles.gameObject, 2f);
                c_rocketParticles.gameObject.transform.parent = null;
            }

            _alreadyExplodedOrDiffused = true;

            Destroy(gameObject);
        }
    }
    public virtual void BouncePadInteraction(Vector3 dir, float force)
    {
        //USE THIS METHO TO CHANGE ROCKET LAYER MASK SO THAT IT CAN NOW COLLIDE WITH PLAYER, BUT OVERRIDE IT FOR A FEW

        if (rpg.ExplodeOnPlayerUponReflection) c_playerTrigger.enabled = true;

        SetVelocity(dir.normalized * Body.velocity.magnitude);
    }

    protected override void OnFreezeTime(bool freeze)
    {
        if (freeze)
        {
            if (rpg.ExplodeOnPlayerUponReflection) c_playerTrigger.enabled = true;
        }
    }

    #endregion

    #region Getters && Setters

    public virtual void SetVelocity(Vector3 vel)
    {
        Body.velocity = vel;
        transform.rotation = Quaternion.LookRotation(vel.normalized);
    }

    public Vector3 GetVelocity()
    {
        return Body.velocity;
    }

    #endregion
}
