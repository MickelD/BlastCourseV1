using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using CustomMethods;

[System.Serializable]
public class Explosion
{
    #region Fields

    [Space(5), Header("Prefab"), Space(3)]
    [SerializeField] GameObject g_explosionPrefab;
    [SerializeField] GameObject[] g_boomTextPrefab;

    [Space(5), Header("Stats"), Space(3)]
    [field: SerializeField] public float BlastRadius;
    [field: SerializeField] public float BlastForce;
    [field: SerializeField] public float Damage;
    [field: SerializeField] public Health.Source Source;
    [HideInInspector] public Vector3 SourcePos;
    [HideInInspector] public Transform Parent;
    [Space(5), Header("Utility"), Space(3)]
    [field: SerializeField] public ExplosionRules ExplosionRules;

    [Space(5), Header("Audio"), Space(3)]
    [field: SerializeField] public AudioCue ExplosionSound;

    #endregion

    Collider[] hitColliders;
    private List<IExplodable> _explodablesList;
    private List<Rigidbody> _rbList;
    private List<DestructibleObject> _destructibleList;

    #region Methods

    public void Explode(Vector3 origin, Vector3 normal)
    {
        SourcePos = Vector3.zero;
        Parent = null;
        GameObject _instantiatedExplosion = Object.Instantiate(g_explosionPrefab, origin + normal * 0.5f, Quaternion.identity, AudioManager.Instance.transform);
        Object.Destroy(_instantiatedExplosion, ExplosionRules.ExplosionLifetime);
        GameObject _instantiatedTextExplosion = Object.Instantiate(g_boomTextPrefab[Random.Range(0, g_boomTextPrefab.Length)], origin + normal, Quaternion.identity, AudioManager.Instance.transform);
        Object.Destroy(_instantiatedTextExplosion, ExplosionRules.ExplosionLifetime);
        AudioManager.TryPlayCueAtPoint(ExplosionSound, _instantiatedTextExplosion.transform.position);

        _instantiatedExplosion.transform.localScale = Vector3.one * BlastRadius;

        hitColliders = new Collider[25];

        RepeatedCollisionChecks(origin, normal);
    }

    public void Explode(Vector3 origin, Vector3 normal, Vector3 sourcePos, Transform parent = null)
    {
        SourcePos = sourcePos;
        Parent = parent;
        GameObject _instantiatedExplosion = Object.Instantiate(g_explosionPrefab, origin + normal*0.5f, Quaternion.identity, AudioManager.Instance.transform);
        Object.Destroy(_instantiatedExplosion, ExplosionRules.ExplosionLifetime);
        GameObject _instantiatedTextExplosion = Object.Instantiate(g_boomTextPrefab[Random.Range(0,g_boomTextPrefab.Length)], origin + normal, Quaternion.identity, AudioManager.Instance.transform);
        Object.Destroy(_instantiatedTextExplosion, ExplosionRules.ExplosionLifetime);
        AudioManager.TryPlayCueAtPoint(ExplosionSound, _instantiatedTextExplosion.transform.position);

        _instantiatedExplosion.transform.localScale = Vector3.one * BlastRadius;

        hitColliders = new Collider[50];

        RepeatedCollisionChecks(origin, normal);
    }

    private async void RepeatedCollisionChecks(Vector3 origin, Vector3 normal)
    {
        float t = 0f;
        _rbList = new List<Rigidbody>();
        _explodablesList = new List<IExplodable>();
        _destructibleList = new List<DestructibleObject>();

        while (t < ExplosionRules.SphereCastDuration) //REPEATED CHECKS
        {
            t += Time.deltaTime;

            int numColliders = Physics.OverlapSphereNonAlloc(origin, BlastRadius, hitColliders, ExplosionRules.ExplosionLayerMask);

            for (int i = 0; i < numColliders; i++)
            {
                IExplodable explodable = hitColliders[i].GetComponent<IExplodable>();
                DestructibleObject destructible = hitColliders[i].GetComponent<DestructibleObject>();

                if(destructible != null)               
                    if(!_destructibleList.Contains(destructible)) _destructibleList.Add(destructible);
                if (explodable != null)
                    if (!_explodablesList.Contains(explodable)) _explodablesList.Add(explodable);
                //else if (hitRB != null)
                //    if (!_rbList.Contains(hitRB)) _rbList.Add(hitRB);
            }

            if (_rbList.Count > 0 || _explodablesList.Count > 0 || _destructibleList.Count > 0)
            {
                //ApplyForce(_rbList, origin, normal);
                ApplyExplosionBehaviour(_explodablesList, origin, normal);
                ApplyDestruction(_destructibleList);
                return;
            }

            await Task.Yield();
        }
    }

    private void ApplyExplosionBehaviour(List<IExplodable> explodables, Vector3 origin, Vector3 normal)
    {
        foreach (IExplodable explodable in explodables)
        {
            explodable.ExplosionBehaviour(origin, this, normal);
        }
    }

    private void ApplyDestruction(List<DestructibleObject> destructibles)
    {
        foreach (DestructibleObject destructible in destructibles)
        {
            destructible.AddDamage();
        }
    }


    private void ApplyForce(List<Rigidbody> hitRBList, Vector3 origin, Vector3 normal)
    {
        foreach(Rigidbody hitRB in hitRBList)
        {
            if(hitRB != null)
            {
                //Vector3 xzDir = ExtendedMathUtility.HorizontalDirection(origin, hitRB.worldCenterOfMass);

                //hitRB.AddForce(
                //    BlastForce * 
                //    ApplyDirectionModifier(xzDir + Vector3.up, ExplosionRules.ObjectDirectionDistribution)
                //    , ForceMode.Impulse);     
            }
        }
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Multiplies Y component of passed direction by passed modifier.
    /// Returns resulting vector normalized
    /// </summary>
    public Vector3 ApplyDirectionModifier(Vector3 dirToModify, Vector2 modiferToUse)
    {
        dirToModify.x *= modiferToUse.x;
        dirToModify.z *= modiferToUse.x;
        dirToModify.y *= modiferToUse.y;
        return dirToModify;
    }

    #endregion

    /*
      Rigidbody hitRB = hitColliders[i].gameObject.GetComponent<Rigidbody>();

                if (hitRB != null)
                {
                    bool shouldAdd = true;
                    foreach(Rigidbody rb in _rbList)
                    {
                        if (rb == hitRB) shouldAdd = false;
                    }
                    if (shouldAdd) _rbList.Add(hitRB);
                }

                Health hitHP = hitColliders[i].gameObject.GetComponent<Health>();

                if (hitHP != null)
                {
                    bool shouldAdd = true;
                    foreach (Health hp in _hpList)
                    {
                        if (hp == hitHP) shouldAdd = false;
                    }
                    if (shouldAdd) _hpList.Add(hitHP);
                }

                ExplosionDetect hitTrigger = hitColliders[i].gameObject.GetComponent<ExplosionDetect>();

                if(hitTrigger != null)
                {
                    bool shouldAdd = true;
                    foreach (ExplosionDetect trigger in _triggerList)
                    {
                        if (trigger == hitTrigger) shouldAdd = false;
                    }
                    if (shouldAdd) _triggerList.Add(hitTrigger);
                }

            }
            if(_hpList.Count > 0
               || _rbList.Count > 0
               || _triggerList.Count > 0)
            {
                ApplyForce(_rbList, origin, normal);
                ApplyDamage(_hpList);
                ApplyTriggers(_triggerList);
                return;

                t += Time.deltaTime;

            CollisionCheck(origin, _explodablesList); //SINGLE CHECK FOR IEXPLODABLES
            CollisionCheck(origin, _rbList); //RIGIDBODY CHECK FOR RIGIDBOIES

            ApplyForce(_rbList, origin, normal);
            ApplyExplosionBehaviour(_explodablesList, origin, normal);

            await Task.Yield();


        private List<Health> _hpList;
    private void ApplyDamage(List<Health> hitHPList)
    {
        foreach (Health hitHP in hitHPList)
        {
            if(hitHP != null) hitHP.SufferDamage(Damage, Source);
        }
    }

    private List<ExplosionDetect> _triggerList;
    private void ApplyTriggers(List<ExplosionDetect> hitTriggerList)
    {
        foreach (ExplosionDetect hitTrigger in hitTriggerList)
        {
            if (hitTrigger != null) hitTrigger.CallActivate(true);
        }
    }



                CollisionCheck(origin, _explodablesList); //SINGLE CHECK FOR IEXPLODABLES
            CollisionCheck(origin, _rbList); //RIGIDBODY CHECK FOR RIGIDBOIES

            ApplyForce(_rbList, origin, normal); 
            ApplyExplosionBehaviour(_explodablesList, origin, normal);
     


                Health hitHP = hitColliders[i].gameObject.GetComponent<Health>();

            if (hitHP != null)
            {
                bool shouldAdd = true;
                foreach (Health hp in _hpList)
                {
                    if (hp == hitHP) shouldAdd = false;
                }
                if (shouldAdd) _hpList.Add(hitHP);
            }

            ExplosionDetect hitTrigger = hitColliders[i].gameObject.GetComponent<ExplosionDetect>();

            if (hitTrigger != null)
            {
                bool shouldAdd = true;
                foreach (ExplosionDetect trigger in _triggerList)
                {
                    if (trigger == hitTrigger) shouldAdd = false;
                }
                if (shouldAdd) _triggerList.Add(hitTrigger);
            }
     */
}
