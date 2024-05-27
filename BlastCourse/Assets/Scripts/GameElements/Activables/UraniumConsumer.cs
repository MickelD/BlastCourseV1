using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Consume me papa, for I have sinned
public class UraniumConsumer : ActivableBase
{
    #region Fields

    [Space(5), Header("Variables"), Space(3)]
    [SerializeField, Tooltip("The amount of Uranium Boxes that must be inputed for it to activate")] public int _chargesRequired;
    [SerializeField, Tooltip("The amount of force with which the unnecesary PhysicsObjects are pushed out")] public float _extractForce;

    [Space(5), Header("Audio"), Space(3)]
    public AudioCue _consumeSfx;
    public AudioCue _rejectSfx;

    #endregion

    #region Variables

    private int _currentCharges = 0; //Manslaughter, Armed Robery, and Aggravated Assault
    private bool _full = false;

    #endregion

    #region Methods

    public override void SendAllActivations(bool isActive)
    {
        _full = isActive;
        base.SendAllActivations(isActive);
    }

    #endregion

    #region Collisions && Triggers

    private void OnTriggerEnter(Collider other)
    {
        UraniumBox u = other.GetComponent<UraniumBox>();
        if(u != null && !_full )
        {

            _currentCharges++;
            u.SetConsuming(true);
            Destroy(u.gameObject);

            if (_consumeSfx.SfxClip != null && AudioManager.Instance != null) AudioManager.TryPlayCueAtPoint(_consumeSfx, transform.position);
            if (_currentCharges >= _chargesRequired)
            {
                SendAllActivations(true);
            }
        }
        else
        {
            if (_rejectSfx.SfxClip != null && AudioManager.Instance != null) AudioManager.TryPlayCueAtPoint(_rejectSfx, transform.position);
            PhysicsObject box = other.GetComponent<PhysicsObject>();
            if (box != null) box.Push(transform.up * 100 * _extractForce);
        }
    }

    #endregion
}


