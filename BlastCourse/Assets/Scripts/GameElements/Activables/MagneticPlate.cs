using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider))]
public class MagneticPlate : ActivableBase
{
    #region Fields

    [Space(5), Header("Magnetic Plate"), Space(3)]
    [SerializeField] public Vector3 Size;
    [SerializeField] public float PullStrenght;
    [SerializeField] public BoxCollider _Trigger;
    public BoxCollider _hitBox;
    public Vector3 _hitBoxPadding;
    [SerializeField] public MeshTiler _tiler;
    public Transform _magLeft;
    public Transform _magRight;
    [SerializeField] public Material OnBaseMat;
    [SerializeField] public Material OffBaseMat;
    [SerializeField] public MeshRenderer RingsRenderer;
    [SerializeField] public Vector2 RingsOffset;
    [SerializeField] public Material OnRingMat;
    [SerializeField] public Material OffRingMat;
    [SerializeField] public AudioCue OnSfx;
    [SerializeField] public AudioCue OffSfx;

    #endregion

    #region Vars

    //Maybe consider turning this into a buffered array, but since it does not occur every frame, garbage collection should not be a concern
    private List<IMagnetable> StuckObjects = new();
    private MeshRenderer[] _meshes;

    #endregion

    #region UnityFunctions

    protected override void Start()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>().Where(obj => obj.name == "tempMesh").ToArray();

        base.Start();
    }

    private void OnValidate()
    {
        if (_tiler != null)
        {
            _tiler.ManagedByScript = true;
            _tiler.Area = Size;
        }

        _magLeft.transform.localPosition = new Vector3(Size.x*.5f, 0f, 0.125f);
        _magRight.transform.localPosition = new Vector3(-Size.x*.5f, 0f, 0.125f);

        _Trigger.size = new Vector3(Size.x, Size.y, Size.z);
        _Trigger.center = new Vector3(0f, 0f, Size.z * 0.5f);

        _hitBox.size = new Vector3(Size.x - _hitBoxPadding.x, Size.y - _hitBoxPadding.y, _hitBoxPadding.z);
        _hitBox.center = new Vector3(0f, 0f, _hitBoxPadding.z * 0.5f);

        RingsRenderer.transform.localScale = new Vector3(Size.x - RingsOffset.x, Size.y - RingsOffset.y, 1f);
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
        _Trigger.enabled = set;
        RingsRenderer.material = set ? OnRingMat : OffRingMat;
        foreach (MeshRenderer mesh in _meshes)
        {
            mesh.material = set ? OnBaseMat : OffBaseMat;
        }

        //G_meshRenderer.material = set ? OnMat : OffMat;
        AudioManager.TryPlayCueAtPoint(set ? OnSfx : OffSfx, transform.position);

        if (!set)
        {
            StuckObjects.ForEach(m => m?.SetMagnetization(false, null));

            StuckObjects.Clear();
        }
    }

    #endregion
}


