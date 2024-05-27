using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Materials"), Space(3)]
    [SerializeField] Material _offMatreial;
    [SerializeField] Material _onMatreial;
    [SerializeField] GameObject _mesh;

    #endregion

    #region Variables

    [HideInInspector] public Race _race;
    [HideInInspector] public GameObject g_player;
    bool _active;

    #endregion

    #region Methods

    public void Activate(bool isActive)
    {
        _active = isActive;

        if (_active) _mesh.GetComponent<Renderer>().material = _onMatreial;
        else _mesh.GetComponent<Renderer>().material = _offMatreial;
    }

    #endregion

    #region Collisions & Triggers

    private void OnTriggerStay(Collider other)
    {
        if (_active && g_player == other.gameObject)
        {
            _race.EndRace();
            Activate(false);
        }
    }

    #endregion
}
