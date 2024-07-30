using UnityEngine;
using TMPro;

public class LoadingScreenText : MonoBehaviour
{
    #region Fields

    public LoadingScreenTextSO Texts;
    public TextMeshProUGUI TextDisplay;

    #endregion

    #region UnityFunctions

    private void OnEnable()
    {
        TextDisplay.text = Texts.Tips[Random.Range(0, Texts.Tips.Length)];
    }

    #endregion
}


