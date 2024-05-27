using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public List<GameObject> _worldCredits = new List<GameObject>();
    public static CreditManager Instance;

    public void Awake()
    {
        Instance = this;
        _worldCredits = new List<GameObject>();
    }

    public void RespawnCredits()
    {
        foreach(GameObject credit in _worldCredits)
        {
            credit.GetComponent<Credits>().Activate(true);
        }
    }
}
