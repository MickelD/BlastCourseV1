using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObjManager : MonoBehaviour
{
    #region Variables

    public List<KeyInteractable> _keyObj;
    public Dictionary<string, KeyInteractable> _keyObjList;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        _keyObjList = new Dictionary<string, KeyInteractable>();

        if (_keyObj?.Count > 0)
            for (int i = 0; i < _keyObj?.Count; i++)
                _keyObjList?.Add(_keyObj?[i]?._index, _keyObj?[i]);
    }

    public void Start()
    {
        if (SaveLoader.Instance?.KeysReached.Count > 0)
            foreach (string index in SaveLoader.Instance.KeysReached)
                if (_keyObjList.ContainsKey(index))
                {
                    StartCoroutine(_keyObjList[index].StartUpActivate());
                }
    }

    #endregion
}
