using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

public class CloneSpinner : MonoBehaviour
{
    [SerializeField] Vector2 _rotRange;
    [SerializeField] float _resetValue;
    [SerializeField, Range(0f,1f)] float _emptyChance;

    [ContextMenu("Rotate Children")]
    public void RotateChildren()
    {
        foreach (Transform child  in transform)
        {
            Transform person = child.Find("clon_character");
            Transform tubes = child.Find("clon_tube_character");

            float angleAB = Random.Range(_rotRange.x, _rotRange.y);

            if (person != null) person.localEulerAngles = new Vector3(person.localEulerAngles.x, angleAB, person.localEulerAngles.z);
            if (tubes != null) tubes.localEulerAngles = new Vector3(tubes.localEulerAngles.x, angleAB, tubes.localEulerAngles.z);

            Transform bubbles = child.Find("clon_bubbles");
            if (bubbles != null) bubbles.localEulerAngles = new Vector3(bubbles.localEulerAngles.x, Random.Range(_rotRange.x, _rotRange.y), bubbles.localEulerAngles.z);
        }
    }

    [ContextMenu("Rotate All")]
    public void RotateAll()
    {
        foreach (Transform person in FindObjectsOfType<Transform>().Where(obj => obj.gameObject.name == "clon_character"))
        {
            float angleAB = Random.Range(_rotRange.x, _rotRange.y);

            person.localEulerAngles = new Vector3(person.localEulerAngles.x, angleAB, person.localEulerAngles.z);

            Transform tubes = person.parent.Find("clon_tube_character");
            if (tubes != null) tubes.localEulerAngles = new Vector3(tubes.localEulerAngles.x, angleAB, tubes.localEulerAngles.z);

            Transform bubbles = person.parent.Find("clon_bubbles");
            if (bubbles != null) bubbles.localEulerAngles = new Vector3(bubbles.localEulerAngles.x, Random.Range(_rotRange.x, _rotRange.y), bubbles.localEulerAngles.z);
        }
    }

    [ContextMenu("Reset Children")]
    public void ResetChildren()
    {
        foreach (Transform child in transform)
        {
            Transform person = child.Find("clon_character");
            Transform tubes = child.Find("clon_tube_character");

            if (person != null) person.localEulerAngles = new Vector3(person.localEulerAngles.x, _resetValue, person.localEulerAngles.z);
            if (tubes != null) tubes.localEulerAngles = new Vector3(tubes.localEulerAngles.x, _resetValue, tubes.localEulerAngles.z);

            Transform bubbles = child.Find("clon_bubbles");
            if (bubbles != null) bubbles.localEulerAngles = new Vector3(bubbles.localEulerAngles.x, _resetValue, bubbles.localEulerAngles.z);
        }
    }

    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        foreach (Transform person in FindObjectsOfType<Transform>().Where(obj => obj.gameObject.name == "clon_character"))
        {
            person.localEulerAngles = new Vector3(person.localEulerAngles.x, _resetValue, person.localEulerAngles.z);

            Transform tubes = person.parent.Find("clon_tube_character");
            if (tubes != null) tubes.localEulerAngles = new Vector3(tubes.localEulerAngles.x, _resetValue, tubes.localEulerAngles.z);

            Transform bubbles = person.parent.Find("clon_bubbles");
            if (bubbles != null) bubbles.localEulerAngles = new Vector3(bubbles.localEulerAngles.x, _resetValue, bubbles.localEulerAngles.z);
        }
    }


    [ContextMenu("Try Empty All")]
    public void TryEmptyAll()
    {
        foreach (MeshRenderer person in FindObjectsOfType<MeshRenderer>().Where(obj => obj.gameObject.name == "clon_character"))
        {
            bool show = Random.value > _emptyChance;

            person.enabled = show;

            Transform tubes = person.transform.parent.Find("clon_tube_character");
            if (tubes != null && (tubes.TryGetComponent(out MeshRenderer tubeRend))) tubeRend.enabled = show;

        }
    }

    [ContextMenu("Try Empty Children")]
    public void TryEmptyChildren()
    {
        foreach (Transform child in transform)
        {
            bool show = Random.value > _emptyChance;

            Transform person = child.Find("clon_character");
            Transform tubes = child.Find("clon_tube_character");

            if (person != null && person.TryGetComponent(out MeshRenderer personRend)) personRend.enabled = show;
            if (tubes != null && tubes.TryGetComponent(out MeshRenderer tubesRend)) tubesRend.enabled = show;

        }
    }
}

#endif

