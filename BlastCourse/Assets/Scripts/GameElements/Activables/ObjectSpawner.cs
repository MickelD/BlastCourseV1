using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : ActivableBase
{
    #region Fields

    [Space(5), Header("Object Spawn"), Space(3)]
    [Tooltip("Still spawn even if there is already an object belonging to this spawner. " +
        "The objects already spawned get destroyed")] public bool AllowSpawnReset;
    public GameObject ObjectPrefab;
    public Vector3 SpawnPos;
    [Tooltip("Only updates On Start, do not change at runtime")] public float Delay;
    [Tooltip("If the object should be destroyed on exit")] public bool DeletoOnExit;

    [Space(5), Header("Volume Properties"), Space(3)]
    public Vector3 Size = Vector3.one;
    public Vector3 Center;

    #endregion

    #region Vars

    [HideInInspector] public PhysicsObject SpawnedObject;
    private WaitForSeconds _spawnTime;
    private System.Action _spawnAction;

    #endregion

    #region UnityFunctions

    protected override void Start()
    {
        _spawnTime = new WaitForSeconds(Delay);

        _spawnAction = Delay > 0.1f ? () => StartCoroutine(SpawnDelayed()) : () => Spawn();

        base.Start();
    }

    protected void Update()
    {
        if(DeletoOnExit 
           && ObjectPrefab != null 
           && SpawnedObject != null
           && !CustomMethods.ExtendedDataUtility.IsPointInArea(SpawnedObject.transform.position, Center + transform.position, Size))
        {
            SpawnedObject.ShouldRespawn = false;
            SpawnedObject.DestroyObject();
            _spawnAction();
        }
    }

    #endregion

    #region Methods

    [ActivableAction]

    
    public void TrySpawn(bool spawn)
    {
        if (spawn && ObjectPrefab != null)
        {
            if(SpawnedObject == null)
            {
                _spawnAction();
            }
            else if (AllowSpawnReset)
            {
                SpawnedObject.ShouldRespawn = false;
                SpawnedObject.DestroyObject();
                _spawnAction();
            }
        }
    }

    private void Spawn()
    {
        if(!destroyed) SpawnedObject = Instantiate(ObjectPrefab, transform.position + SpawnPos, Quaternion.identity, transform).GetComponent<PhysicsObject>();
        if (SpawnedObject != null)
        {
            SpawnedObject.gameObject.SetActive(true);
            SpawnedObject.GravityController.EnableGravity = true;
            SpawnedObject.ShouldRespawn = true;
            SpawnedObject.Spawner = this;
            SpawnedObject.transform.parent = transform.parent;
        }
    }

    private IEnumerator SpawnDelayed()
    {
        yield return _spawnTime;

        Spawn();
    }

    private bool destroyed;
    private void OnDestroy()
    {
        destroyed = true;
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position + SpawnPos, "iconBoxSpawn");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + Center, Size);
    }

    #endregion
}
