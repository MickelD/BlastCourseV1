using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MagneticPlate : ActivableBase
{
    #region Fields

    [Space(5), Header("Magnetic Plate"), Space(3)]
    [SerializeField] public Vector3 Size;
    [SerializeField] public float PullStrenght;
    [SerializeField] public BoxCollider C_hitbox;
    [SerializeField] public Transform G_meshTransform;
    [SerializeField] public MeshTiler _tiler;
    [SerializeField] public MeshRenderer G_meshRenderer;
    [SerializeField] public Material OnMat;
    [SerializeField] public Material OffMat;
    [SerializeField] public AudioCue OnSfx;
    [SerializeField] public AudioCue OffSfx;

    #endregion

    #region Vars

    //Maybe consider turning this into a buffered array, but since it does not occur every frame, garbage collection should not be a concern
    private List<IMagnetable> StuckObjects = new(); 

    #endregion

    #region UnityFunctions

    private void OnValidate()
    {
        if (_tiler != null)
        {
            _tiler.ManagedByScript = true;
            _tiler.Area = Size;
        }

        C_hitbox.size = new Vector3(Size.x, Size.y, Size.z);
        C_hitbox.center = new Vector3(0f, 0f, Size.z * 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        IMagnetable magnetable = other.GetComponent<IMagnetable>();

        if (magnetable != null)
        {
            magnetable.SetMagnetization(true, this); 
            
            if (!StuckObjects.Contains(magnetable)) StuckObjects.Add(magnetable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IMagnetable magnetable = other.GetComponent<IMagnetable>();

        if (magnetable != null)
        {
            magnetable.SetMagnetization(false, null);

            if (StuckObjects.Contains(magnetable)) StuckObjects.Remove(magnetable);
        }
    }

    #endregion

    #region Methods

    [ActivableAction]
    public void Magnetize(bool set)
    {
        C_hitbox.enabled = set;

        G_meshRenderer.material = set ? OnMat : OffMat;
        AudioManager.TryPlayCueAtPoint(set ? OnSfx : OffSfx, transform.position);

        if (!set)
        {
            StuckObjects.ForEach(m => m?.SetMagnetization(false, null));

            StuckObjects.Clear();
        }
    }

    #endregion
}


