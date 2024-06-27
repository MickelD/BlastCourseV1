using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CustomMethods;
using System.Reflection;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

public class MeshTiler : MonoBehaviour
{
    #region Fields

    public bool ManagedByScript;
    public bool is2D;
    public bool castShadows;
    [DrawIf(nameof(ManagedByScript), false, DrawIfAttribute.DisablingType.ReadOnly)]
    [DrawIf(nameof(is2D), false)] public float Lenght;
    [DrawIf(nameof(ManagedByScript), false, DrawIfAttribute.DisablingType.ReadOnly)]
    [DrawIf(nameof(is2D), true)] public Vector2 Area;
    [DrawIf(nameof(is2D), false)] public LenghtDirection lenghtDirection;
    [DrawIf(nameof(is2D), true)] public AreaDirection areaDirection;
    public bool OverrideCollider;
    [DrawIf(nameof(OverrideCollider), true)] public BoxCollider Collision;

    [DrawIf(nameof(is2D), true)] public bool OverrideTileControls;

    public MeshTile MeshStart; //1D
    public MeshTile MeshSegment; //1D
    public MeshTile MeshEnd; //1D
    public MeshTile MeshCorner; //2D
    public MeshTile MeshBody; //2D
    public MeshTile MeshRim; //2D

    public AdvancedTileControls TileControls;

    #endregion

    #region Vars

    private const string _DELNAME = "tempMesh";

    public enum LenghtDirection { X, Y, Z }
    public enum AreaDirection { XY, XZ, YZ }

    [System.Serializable] 
    public class AdvancedTileControls
    {
        public Vector2[] CornerScale = new Vector2[4];
        public float[] CornerAngle = new float[4];
        public float[] RimAngle = new float[4];

        [Tooltip("Fallback segment used when there is no space for multiple tiles")] public GameObject SinglePrefab;

        public bool DifferentiateRims;
        public GameObject SideRim;
        public GameObject FrontRim;
    }

    #endregion

    #region UnityFunctions



    #endregion

    #region Methods

    [ContextMenu("Generate")]

    public void Generate()
    {
        List<GameObject> previousChildren = new();

        foreach (Transform child in transform)
        {
            if (child.name == _DELNAME) previousChildren.Add(child.gameObject);
        }

        if (is2D)
        {
            if (Collision != null && OverrideCollider) { Collision.size = new Vector3(Area.x, Collision.size.y, Area.y); }

            if (Area.x <= MeshBody.Size || Area.y <= MeshBody.Size) TileIn1D(Mathf.Max(Area.x, Area.y));
            else
            {
                int corners = 0;
                for (int i = 0; i < Mathf.RoundToInt(Area.x / MeshBody.Size); i++) //HORIZONTAL
                {
                    for (int j = 0; j < Mathf.RoundToInt(Area.y / MeshBody.Size); j++) //VERTICAL
                    {
                        if (i == 0 || i == Mathf.RoundToInt(Area.x / MeshBody.Size) - 1) //edge colums
                        {
                            if (j == 0 || j == Mathf.RoundToInt(Area.y / MeshBody.Size) - 1) // corners
                            {
                                //just trust the process
                                GameObject cornerGO = CreateTile(MeshCorner, CalculatePositionInGrid(i, j, Area, MeshCorner.Size), ReturnUp(areaDirection) * TileControls.CornerAngle[corners]);
                                cornerGO.transform.localScale = Vector3.Scale(cornerGO.transform.localScale, ReturnRelativeAreaVector(areaDirection, TileControls.CornerScale[corners].x, TileControls.CornerScale[corners].y) + ReturnUp(areaDirection));
                                corners++;
                            }
                            else //rim column
                            {
                                if (TileControls.DifferentiateRims) CreateTile(new MeshTile(TileControls.SideRim, MeshRim.Size), CalculatePositionInGrid(i, j, Area, MeshRim.Size), ReturnUp(areaDirection) * TileControls.RimAngle[ExtendedDataUtility.Select(i == 0, 0, 3)]);
                                else CreateTile(MeshRim, CalculatePositionInGrid(i, j, Area, MeshRim.Size), ReturnUp(areaDirection) * TileControls.RimAngle[ExtendedDataUtility.Select(i == 0, 0, 3)]);
                            }
                        }
                        else
                        {
                            if (j == 0 || j == Mathf.RoundToInt(Area.y / MeshBody.Size) - 1) // rim row
                            {
                                if(TileControls.DifferentiateRims) CreateTile(new MeshTile(TileControls.FrontRim, MeshRim.Size), CalculatePositionInGrid(i, j, Area, MeshRim.Size), ReturnUp(areaDirection) * TileControls.RimAngle[ExtendedDataUtility.Select(j == 0, 2, 1)]);
                                else CreateTile(MeshRim, CalculatePositionInGrid(i, j, Area, MeshRim.Size), ReturnUp(areaDirection) * TileControls.RimAngle[ExtendedDataUtility.Select(j == 0, 2, 1)]);
                            }
                            else //body
                            {
                                CreateTile(MeshBody, CalculatePositionInGrid(i, j, Area, MeshBody.Size));
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Collision != null && OverrideCollider) Collision.size = new Vector3(Collision.size.x, Lenght, Collision.size.z);

            TileIn1D(Lenght);
        }

        previousChildren.ForEach(child => DestroyImmediate(child));
    }

    public void TileIn1D(float size)
    {
        if (size <= MeshSegment.Size) //NOT LARGE ENOUGH FOR MORE THAN 2 SEGMENTS
        {
            if (TileControls.SinglePrefab != null) CreateTile(new MeshTile(TileControls.SinglePrefab, MeshSegment.Size), Vector3.zero);
        }
        else
        {
            for (int i = 0; i < Mathf.RoundToInt(size / MeshSegment.Size); i++)
            {
                if (i == 0) //start
                {
                    CreateTile(MeshStart, ReturnDirection(lenghtDirection) * (MeshStart.Size / 2 + i * MeshStart.Size));
                }
                else if (i == Mathf.RoundToInt(size / MeshSegment.Size) - 1) //end
                {
                    CreateTile(MeshEnd, ReturnDirection(lenghtDirection) * (MeshEnd.Size / 2 + i * MeshEnd.Size));
                }
                else //body
                {
                    CreateTile(MeshSegment, ReturnDirection(lenghtDirection) * (MeshSegment.Size / 2 + i * MeshSegment.Size));
                }
            }
        }
    }

    private Vector3 CalculatePositionInGrid(int posX, int posY, Vector2 gridSize, float tileSize)
    {
        return tileSize * ReturnRelativeAreaVector(areaDirection, posX, posY) + tileSize / 2 * ReturnDirection(areaDirection) - ReturnRelativeAreaVector(areaDirection, gridSize.x / 2, gridSize.y / 2);
    }

    private GameObject CreateTile(MeshTile tile, Vector3 localPos)
    {
        return CreateTile(tile, localPos, Vector3.zero);
    }

    private GameObject CreateTile(MeshTile tile, Vector3 localPos, Vector3 localRot)
    {
        GameObject obj = Instantiate(tile.Prefab, transform);
        obj.name = _DELNAME;
        obj.transform.SetLocalPositionAndRotation(localPos, Quaternion.Euler(localRot));
        obj.transform.Rotate(tile.Rotation, Space.Self);

        obj.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(r => r.shadowCastingMode = castShadows? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off); 

        if (is2D)
        {
            obj.transform.localPosition += areaDirection switch
            {
                AreaDirection.XY => new Vector3(tile.Offset.y, tile.Offset.z, tile.Offset.x),
                AreaDirection.XZ => new Vector3(tile.Offset.x, tile.Offset.y, tile.Offset.z),
                AreaDirection.YZ => new Vector3(tile.Offset.z, tile.Offset.x, tile.Offset.y),
                _ => new Vector3(0, 0, 0),
            };
        }
        else
        {
            obj.transform.localPosition += lenghtDirection switch
            {
                LenghtDirection.X => new Vector3(tile.Offset.z, tile.Offset.x, tile.Offset.y),
                LenghtDirection.Y => new Vector3(tile.Offset.x, tile.Offset.y, tile.Offset.z),
                LenghtDirection.Z => new Vector3(tile.Offset.y, tile.Offset.z, tile.Offset.x),
                _ => new Vector3(0, 0, 0),
            };
        }

        return obj;
    }

    private Vector3 ReturnRelativeAreaVector(AreaDirection dir, float x, float y)
    {
        switch (dir)
        {
            case AreaDirection.XZ:
                return new Vector3(x, 0f, y);
            case AreaDirection.XY:
                return new Vector3(x, y, 0f);
            case AreaDirection.YZ:
            default:
                return new Vector3(0f, y, x);
        }
    }

    private Vector3 ReturnUp(AreaDirection dir)
    {
        switch (dir)
        {
            case AreaDirection.XZ:
                return Vector3.up;
            case AreaDirection.XY:
                return Vector3.forward;
            case AreaDirection.YZ:
            default:
                return Vector3.right;
        }
    }

    private Vector3 ReturnDirection(LenghtDirection dir)
    {
        switch (dir)
        {
            case LenghtDirection.X:
                return Vector3.right;
            case LenghtDirection.Y:
                return Vector3.up;
            case LenghtDirection.Z:
            default:
                return Vector3.forward;
        }
    }

    private Vector3 ReturnDirection(AreaDirection dir)
    {
        switch (dir)
        {
            case AreaDirection.XZ:
                return new Vector3(1, 0, 1);
            case AreaDirection.XY:
                return new Vector3(1, 1, 0);
            case AreaDirection.YZ:
            default:
                return new Vector3(0, 1, 1);
        }
    }

    #endregion
}

[System.Serializable]
public class MeshTile
{
    public GameObject Prefab;
    public float Size;
    public Vector3 Offset;
    public Vector3 Rotation;

    public MeshTile(GameObject prefab, float size)
    {
        Prefab = prefab;
        Size = size;

        Offset = Vector3.zero;
        Rotation = Vector3.zero;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(MeshTiler), true), CanEditMultipleObjects]
public class MeshTilerEditor : Editor
{
    MeshTiler meshTiler;

    public override void OnInspectorGUI()
    {
        meshTiler = (MeshTiler)target;
        if (target == null) return;

        serializedObject.Update();

        foreach (FieldInfo field in target.GetType().GetFields())
        {
            SerializedProperty serProp = serializedObject.FindProperty(field.Name);

            if (field.FieldType == typeof(MeshTile))
            {
                if (meshTiler.is2D) //2D
                {
                    if (field.Name == nameof(MeshTiler.MeshEnd) || field.Name == nameof(MeshTiler.MeshStart) || field.Name == nameof(MeshTiler.MeshSegment)) continue;
                }
                else //1D
                {
                    if (field.Name == nameof(MeshTiler.MeshRim) || field.Name == nameof(MeshTiler.MeshCorner) || field.Name == nameof(MeshTiler.MeshBody)) continue; //do not draw area for 1D
                }
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(serProp);
                EditorGUILayout.Space(10);
            }
            else
            {
                if (field.Name == nameof(MeshTiler.OverrideCollider))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MeshTiler.Collision)), GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }
                else if (field.Name == nameof(MeshTiler.Collision)) continue;
                else if (field.Name == nameof(MeshTiler.TileControls))
                {
                    if (!meshTiler.is2D || !meshTiler.OverrideTileControls) continue;
                    else
                    {
                        EditorGUILayout.Space(10);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                        continue;
                    }
                }

                EditorGUILayout.PropertyField(serProp);
            }
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Regenerate"))
        {
            meshTiler.Generate();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(MeshTile))]
public class MeshTilePropertyDrawer : PropertyDrawer
{
    private bool foldout;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MeshTile tile = fieldInfo.GetValue(property.serializedObject.targetObject) as MeshTile;

        EditorGUI.BeginProperty(position, label, property);

        foldout = EditorGUILayout.Foldout(foldout, label);

        GUI.backgroundColor = new(1, 1, 1, 0.05f);
        GUIStyle style = new() { normal = { background = Texture2D.whiteTexture } };

        GUILayout.BeginVertical(style);

        if (foldout)
        {
            foreach (FieldInfo field in tile.GetType().GetFields())
            {
                if (field.Name == nameof(tile.Prefab))
                {
                    EditorGUILayout.ObjectField(property.FindPropertyRelative(field.Name));

                    if (tile.Prefab == null) break;
                }
                else
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative(field.Name));
                }
            }
        }

        GUILayout.EndVertical();

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0f;
    }
}

#endif