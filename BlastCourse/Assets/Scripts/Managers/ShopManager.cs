using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    #region Variables

    public static ShopManager Instance;
    public int _credits;

    #endregion

    #region UnityFunctions

    private void Awake()
    {
        Instance = this;
    }

    #endregion
}
