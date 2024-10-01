using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    #region Variables

    public List<Collectibles> _collectibles;
    public Dictionary<string,Collectibles> _collectibleList;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        _collectibleList = new Dictionary<string, Collectibles>();

        if (_collectibles?.Count > 0)
            for (int i = 0; i < _collectibles?.Count; i++)
                _collectibleList?.Add(_collectibles?[i]?._index, _collectibles?[i]);
    }

    public void Start()
    {
        if (SaveLoader.Instance?.CollectiblesFound.Count > 0)
            foreach (string index in SaveLoader.Instance.CollectiblesFound)
                if (_collectibleList.ContainsKey(index))
                    _collectibleList[index].SetCollected(true);
    }

    #endregion
}
