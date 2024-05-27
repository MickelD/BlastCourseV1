using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDeathPlane : MonoBehaviour
{
    #region Variables

    public BoxCollider Collider;

    public GameObject gTop;
    public GameObject gBottom;
    public Vector3 TopCorner;
    public Vector3 BottomCorner;

    #endregion

    #region Collisions&&Triggers

    private void OnTriggerEnter(Collider other)
    {
        Health player = other.GetComponent<Health>();

        if (player != null)
        {
            player.Die();
        }
    }

    #endregion

    #region Debug

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float x = (TopCorner.x - BottomCorner.x);
        float y = (TopCorner.y - BottomCorner.y);
        float z = (TopCorner.z - BottomCorner.z);

        Gizmos.DrawWireCube(BottomCorner + new Vector3(x / 2, y / 2, z / 2), new Vector3(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z)));
    }

    #endregion
}
