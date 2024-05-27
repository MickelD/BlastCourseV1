using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShelfFill : MonoBehaviour
{
    #region Fields

    [SerializeField] bool _executeFillOnAwake;
    [SerializeField] Vector2 _scaleMultiplier;
    [SerializeField] float _yRot;

    [Space(5), Header("Positions"), Space(3)]
    [SerializeField] Transform[] _shelfPositions;

    [Space(5), Header("Shelf Prefabs"), Space(3)]
    [SerializeField] GameObject[] _shelfPrefabs;

    #endregion

    #region UnityFunctions

    int[] numbers;
    
    public void Awake()
    {
        if (_executeFillOnAwake)
        {
            numbers = new int[_shelfPrefabs.Length];

            //Randomize Prefabs
            numbers[0] = Random.Range(0, _shelfPrefabs.Length);
            numbers[1] = Random.Range(0, _shelfPrefabs.Length);
            numbers[2] = Random.Range(0, _shelfPrefabs.Length);
            numbers[3] = Random.Range(0, _shelfPrefabs.Length);
            while (numbers[1] == numbers[0]) numbers[1] = Random.Range(0, _shelfPrefabs.Length);
            while (numbers[2] == numbers[0] || numbers[2] == numbers[1]) numbers[2] = Random.Range(0, _shelfPrefabs.Length);
            while (numbers[3] == numbers[0] || numbers[3] == numbers[1] || numbers[3] == numbers[2]) numbers[3] = Random.Range(0, _shelfPrefabs.Length);

            //Place Prefabs
            for (int i = 0; i < _shelfPositions.Length; i++)
            {
                Instantiate(_shelfPrefabs[numbers[i]], _shelfPositions[i].position, transform.rotation, _shelfPositions[i]);
            }

            DestroyImmediate(this);
        }
    }

    [ContextMenu("AutoFill")]
    public void AutoFill()
    {
        foreach (Transform shelfPos in _shelfPositions)
        {
            foreach (Transform child in shelfPos)
            {
                DestroyImmediate(child.gameObject);
            }

            GameObject inst = Instantiate(_shelfPrefabs[Random.Range(0, _shelfPrefabs.Length)], shelfPos);

            inst.transform.localScale = Vector3.one * Random.Range(_scaleMultiplier.x, _scaleMultiplier.y);
            inst.transform.eulerAngles = Vector3.up * Random.Range(-_yRot, _yRot);
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        foreach (Transform shelfPos in _shelfPositions)
        {
            foreach (Transform child in shelfPos)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    #endregion
}
