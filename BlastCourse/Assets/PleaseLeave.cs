using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PleaseLeave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (SteamIntegrator.Instance != null) SteamIntegrator.Instance.UnlockAchievement("achRegret");
    }
}
