using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DecalAutoTurn : MonoBehaviour
{
    #region Fields

    public int cloudCount;
    public float sphereRadius;
    public Material sunDecal;
    public Material[] cloudDecal;
    public GameObject decalPrefab;


    #endregion

    #region Variables

    private List<GameObject> _decalChildren;
    private Vector3 _trueScale;

    #endregion

    #region UnityFunctions

    [ContextMenu("Generate")]
    private void Generate()
    {
        _trueScale = transform.localScale;
        transform.localScale = Vector3.one;

        if (_decalChildren == null) _decalChildren = new List<GameObject>();
        while (_decalChildren.Count > cloudCount + 1)
        {
            //Delete the extra Objects
            GameObject go = _decalChildren[cloudCount + 1];
            _decalChildren.RemoveAt(cloudCount + 1);
            GameObject.DestroyImmediate(go);
        }
        for (int i = 0; i < cloudCount + 1; i++)
        {
            if (_decalChildren.Count > i)
            {
                //Change Decal
                GameObject go = _decalChildren[i];
                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                if (i > 0) mr.material = cloudDecal[Random.Range(0, cloudDecal.Length)];
                //Resize Object
                //ResizeCloud(go);
                //Move The Existing Object
                MoveCloud(go);
            }
            else
            {
                //Create New Object
                GameObject go = GameObject.Instantiate(decalPrefab, transform.position, Quaternion.identity, transform);
                //Change Decal
                MeshRenderer mr = go.GetComponent<MeshRenderer>();
                if (i > 0) mr.material = cloudDecal[Random.Range(0, cloudDecal.Length)];
                else mr.material = sunDecal;
                //Resize Object
                //ResizeCloud(go);
                //Move Objects
                MoveCloud(go);
                //Add Object to List
                _decalChildren.Add(go);
            }
        }

        transform.localScale = _trueScale;
        for (int i = 0;i < _decalChildren.Count;i++)
        {
            GameObject go = _decalChildren[i];
            go.transform.parent = null;
            ResizeCloud(go);
            go.transform.parent = transform;
        }
    }

    private void MoveCloud(GameObject go)
    {
        Vector3 k = Random.insideUnitSphere.normalized;
        go.transform.position = transform.position + new Vector3(k.x,Mathf.Abs(k.y),k.z).normalized * sphereRadius;
        go.transform.forward = (go.transform.position - transform.position).normalized;
    }
    private void ResizeCloud(GameObject go)
    {
        go.transform.localScale = new Vector3(Random.Range(22.5f,48f), Random.Range(15f, 32f),14);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,sphereRadius);
    }
    #endregion
}


