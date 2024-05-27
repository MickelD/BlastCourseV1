using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectPad : MonoBehaviour
{
    #region Collisions && Triggers

    private void OnTriggerEnter(Collider other)
    {
        RocketBase rocket = other.GetComponent<RocketBase>();
        if (rocket != null)
        {
            Vector3 ray = rocket.GetVelocity();
            if (Vector3.Angle(ray, transform.up) >= 90)
            {
                ray *= -1;
                //rocket.SetVelocity(ray);
            }
        }
    }

    #endregion
}
