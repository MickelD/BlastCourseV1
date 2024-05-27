using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AndGate : ActivableBase
{
    #region Fields

    [Space(5), Header("And Gate Properties"), Space(3)]

    [Tooltip("Numer of activations required")] public int ContributorCount;
    [Tooltip("Send deactivation event when contributors do not meet the required number")] public bool Reset;

    #endregion

    #region Vars

    private bool _wasActivated;
    private int _contributions;
    public int Contributions
    {
        get { return _contributions; }

        set 
        { 
            _contributions = Mathf.Clamp( value, 0, ContributorCount ); 
            if(_contributions >= ContributorCount)
            {
                SendAllActivations(true);
                _wasActivated = true;
            }
            else if (_wasActivated && Reset)
            {
                SendAllActivations(false);
                _wasActivated = false;
            }
        }
    }

    #endregion

    #region Methods

    [ActivableAction]
    public void Contribute(bool set)
    {
        //check if this object is marked as our contributors
        Contributions += ExtendedDataUtility.Select(set, 1, -1);
    }

    #endregion
}


