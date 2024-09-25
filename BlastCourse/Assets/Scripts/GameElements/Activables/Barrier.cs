using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Barrier : ActivableBase
{
    #region Fields
    [Header("Barrier Properties")]
    public float Lenght;

    public enum BarrierType { Objects, Player}

    public BarrierType BlockingType;

    [Header("References")]
    public MeshRenderer gRenderer;
    public Collider gHitbox;
    public GameObject gLeftEdge;
    public GameObject gRightEdge;
    public string BlockPlayerLayer;
    public string BlockObjectsLayer;
    public Material playerMaterial;
    public Material objectsMaterial;
    public AudioCue OnSfx;
    public AudioCue OffSfx;

    private bool _firstFrame = false;

    #endregion

    #region Methods

    protected override void Start()
    {
        gHitbox.gameObject.layer = LayerMask.NameToLayer(ExtendedDataUtility.Select(BlockingType == BarrierType.Player, BlockPlayerLayer, BlockObjectsLayer));
        base.Start();
        _firstFrame = true;
    }

    private void OnValidate()
    {
        if (gLeftEdge != null) gLeftEdge.transform.localPosition =   Vector3.left * Lenght * 0.5f;
        if (gRightEdge != null) gRightEdge.transform.localPosition =  Vector3.right * Lenght * 0.5f;
        gRenderer.material = BlockingType == BarrierType.Player ? playerMaterial : objectsMaterial;
        gRenderer.transform.localScale = gHitbox.transform.localScale = new Vector3(3f, Lenght, 0.25f);
    }

    [ActivableAction]
    public void DisableBarrier(bool active)
    {
        SetBlocking(!active);
    }

    [ActivableAction]
    public void EnableBarrier(bool active)
    {
        SetBlocking(active);
    }

    private void SetBlocking(bool set)
    {
        gHitbox.enabled = set;
        if(_firstFrame)AudioManager.TryPlayCueAtPoint(set ? OnSfx : OffSfx, transform.position);

        gRenderer.material.DOFloat(ExtendedDataUtility.Select(set, -0.1f, 1f), "_Dissipation", 0.5f);
    }

    #endregion

#if UNITY_EDITOR

    //[CustomEditor(typeof(Barrier))]
    //public class BarrierEditor : ActivableEditor
    //{
    //    Barrier barrier;
    //    bool f_barrierList;
    //    float size;
    //    float offset = 1.5f;

    //    private void OnEnable()
    //    {
    //        barrier = (Barrier)target;
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        GUILayout.Space(10);
    //        GUILayout.BeginHorizontal();
    //        f_barrierList = EditorGUILayout.Foldout(f_barrierList,"");
    //        EditorGUILayout.LabelField("Game Objects");
    //        GUILayout.EndHorizontal();
    //        if (f_barrierList)
    //        {
    //            EditorGUILayout.LabelField("___________________________________________________________________________________________________________");
    //            GUILayout.Space(10);

    //            EditorGUILayout.LabelField("Barriers");
    //            GUILayout.Space(5);
    //            barrier._playerBarrier = EditorGUILayout.ObjectField("Player", barrier._playerBarrier, typeof(GameObject), true) as GameObject;
    //            GUILayout.Space(2);
    //            barrier._rocketBarrier = EditorGUILayout.ObjectField("Rockets", barrier._rocketBarrier, typeof(GameObject), true) as GameObject;
    //            GUILayout.Space(2);
    //            barrier._boxBarrier = EditorGUILayout.ObjectField("Physics Boxes", barrier._boxBarrier, typeof(GameObject), true) as GameObject;
    //            GUILayout.Space(12);

    //            EditorGUILayout.LabelField("Edges");
    //            GUILayout.Space(5);
    //            barrier._edgeL = EditorGUILayout.ObjectField("Left Edge", barrier._edgeL, typeof(GameObject), true) as GameObject;
    //            GUILayout.Space(2);
    //            barrier._edgeR = EditorGUILayout.ObjectField("Right Edge", barrier._edgeR, typeof(GameObject), true) as GameObject;
    //            GUILayout.Space(10);

    //            EditorGUILayout.LabelField("___________________________________________________________________________________________________________");
    //        }

    //        GUILayout.Space(20);

    //        size = EditorGUILayout.FloatField("Size", barrier._size);
    //        if(size != barrier._size)
    //        {
    //            barrier._size = size;
    //            Vector3 scale = new Vector3(size, 3, 0.15f);

    //            if(barrier._playerBarrier != null)
    //            {
    //                barrier._playerBarrier.transform.localScale = scale;
    //                barrier._playerBarrier.transform.localPosition = Vector3.up * offset;
    //            }

    //            if (barrier._rocketBarrier != null)
    //            {
    //                barrier._rocketBarrier.transform.localScale = scale;
    //                barrier._rocketBarrier.transform.localPosition = Vector3.up * offset;
    //            }

    //            if (barrier._boxBarrier != null)
    //            {
    //                barrier._boxBarrier.transform.localScale = scale;
    //                barrier._boxBarrier.transform.localPosition = Vector3.up * offset;
    //            }


    //            if (barrier._edgeL != null) barrier._edgeL.transform.localPosition = Vector3.up * offset + Vector3.left * size/2;
    //            if (barrier._edgeR != null) barrier._edgeR.transform.localPosition = Vector3.up * offset + Vector3.right * size / 2;
    //        }
    //        GUILayout.Space(10);

    //        barrier._blockPlayer = EditorGUILayout.Toggle("Should Block Player", barrier._blockPlayer);
    //        GUILayout.Space(2);
    //        if (barrier._playerBarrier != null) barrier._playerBarrier.SetActive(barrier._blockPlayer);

    //        barrier._blockRocket = EditorGUILayout.Toggle("Should Block Rockets", barrier._blockRocket);
    //        GUILayout.Space(2);
    //        if (barrier._rocketBarrier != null) barrier._rocketBarrier.SetActive(barrier._blockRocket);

    //        barrier._blockBox = EditorGUILayout.Toggle("Should Block Boxes", barrier._blockBox);
    //        GUILayout.Space(2);
    //        if(barrier._boxBarrier != null) barrier._boxBarrier.SetActive(barrier._blockBox);

    //        GUILayout.Space(30);
    //        EditorUtility.SetDirty(barrier);

    //        base.OnInspectorGUI();
    //    }
    //}

    #endif
}


