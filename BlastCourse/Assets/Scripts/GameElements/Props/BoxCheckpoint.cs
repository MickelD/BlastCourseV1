using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxCheckpoint : BoxVisualizer
{
    [SerializeField] bool _onlyOnce;
    [SerializeField] bool _alsoSetSpawn;
    [SerializeField, DrawIf(nameof(_alsoSetSpawn), true)] Vector3 _spawn;
    [SerializeField, DrawIf(nameof(_alsoSetSpawn), true)] float _rot;
    [SerializeField] List<string> _ignoreIds = new();

    #region UnityFunctions

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out UraniumBox b) && !_ignoreIds.Contains(b.id))
        {
            if (_onlyOnce) _ignoreIds.Add(b.id);

            if (SaveLoader.Instance.SetBoxPos(b, transform.position) && _alsoSetSpawn) 
                SaveLoader.Instance.SetSpawn(_spawn, _rot);
        }
    }

    #endregion

    #region Methods

    protected override Color GetColor()
    {
        return new Color(0f, 1f, 1f, 0.5f);
    }

    #endregion


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (_alsoSetSpawn) Gizmos.DrawSphere(_spawn, 0.25f);
    }
}


