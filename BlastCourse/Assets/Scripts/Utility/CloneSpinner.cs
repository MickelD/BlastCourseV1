using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

public class CloneSpinner : MonoBehaviour
{
    [SerializeField] Vector2 _rotRange;
    [SerializeField] float _resetValue;
    [SerializeField, Range(0f,1f)] float _emptyChance;

    private void IterateCloneParts(Transform cloneGO, System.Action<MeshRenderer> del)
    {
        foreach (MeshRenderer child in cloneGO.GetComponentsInChildren<MeshRenderer>().Where(obj => obj.name.Contains("char") || obj.name.Contains("wires"))) 
            del.Invoke(child);
    }

    private void ForAll(System.Action<Transform> del)
    {
        foreach (Transform cloneGO in FindObjectsOfType<Transform>().Where(obj => obj.name.Contains("mshclon")))
        {
            del.Invoke(cloneGO);
        }
    }

    private void ForEachChild(System.Action<Transform> del)
    {
        foreach (Transform child in transform)
        {
            if (!child.name.Contains("mshclon")) continue;
            del.Invoke(child);
        }
    }


    [ContextMenu("Rotate Children")]
    public void RotateChildren()
    {
        ForEachChild((child) =>
        {
            float angleAB = Random.Range(_rotRange.x, _rotRange.y);
            IterateCloneParts(child, (obj) => obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, angleAB, obj.transform.localEulerAngles.z));
        });
    }

    [ContextMenu("Rotate All")]
    public void RotateAll()
    {
        ForAll((cloneGO) =>
        {
            float angleAB = Random.Range(_rotRange.x, _rotRange.y);
            IterateCloneParts(cloneGO, (obj) => obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, angleAB, obj.transform.localEulerAngles.z));
        });
    }

    [ContextMenu("Reset Children")]
    public void ResetChildren()
    {
        ForEachChild((child) =>
        {
            IterateCloneParts(child, (obj) => obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, _resetValue, obj.transform.localEulerAngles.z));
        });
    }

    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        ForAll((cloneGO) =>
        {
            IterateCloneParts(cloneGO, (obj) => obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, _resetValue, obj.transform.localEulerAngles.z));
        });
    }


    [ContextMenu("Try Empty All")]
    public void TryEmptyAll()
    {
        ForAll((cloneGO) =>
        {
            bool show = Random.value > _emptyChance;
            IterateCloneParts(cloneGO, (obj) => obj.enabled = show);
        });
    }

    [ContextMenu("Try Empty Children")]
    public void TryEmptyChildren()
    {
        ForEachChild((child) =>
        {
            bool show = Random.value > _emptyChance;
            IterateCloneParts(child, (obj) => obj.enabled = show);
        });
    }
}

#endif


