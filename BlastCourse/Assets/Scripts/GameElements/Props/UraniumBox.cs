using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UraniumBox : PhysicsObject
{
    #region Vars

    [SerializeField] Vector3 _recallPos;

    private bool _consuming;
    public string id;

    #endregion

    #region UnityFunctions

    protected override void OnDestroy()
    {
        if (!_consuming)
        {
            base.OnDestroy();
        }
    }

    #endregion

    #region Methods

    public void Consume(Transform feeder)
    {
        foreach (RocketRemoteExplosion rem in transform.GetComponentsInChildren<RocketRemoteExplosion>())
        {
            Destroy(rem.gameObject);
        }

        if (_Grabbed && _CurrentInteractor != null) 
        {
            _CurrentInteractor.CancelCurrentInteraction();
            Locked = true;
        }

        GravityController.EnableGravity = false;
        C_collider.enabled = false;
        c_rb.isKinematic = true;

        transform.parent = feeder;
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        SetConsuming(true);
    }

    public void SetConsuming(bool consuming)
    {
        _consuming = consuming;
        if (!SaveLoader.Instance.UsedBoxes.Contains(id))
        {
            SaveLoader.Instance.UsedBoxes.Add(id);
            SaveLoader.Instance.Save();
        }
    }

    public string GetIndex() { return id; }
    public void SetIndex(string i) { id = i; }

    public void Recall()
    {
        transform.rotation = Quaternion.identity;
        transform.position = _recallPos;
        c_rb.velocity = Vector3.zero;

        foreach (RocketRemoteExplosion rem in transform.GetComponentsInChildren<RocketRemoteExplosion>())
        {
            rem.Defuse();
        }
    }

    [ContextMenu("ResetRecallPos")]
    public void ResetRecallPos()
    {
        _recallPos = transform.position;
    }

    #endregion

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_recallPos, 0.75f);
    }

#endif
}


