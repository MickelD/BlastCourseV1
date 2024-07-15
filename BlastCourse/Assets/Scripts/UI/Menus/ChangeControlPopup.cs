using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChangeControlPopup : MonoBehaviour
{
    #region Vars

    [HideInInspector] public int currentInput;
    public OptionsMenu options;
    public TextMeshProUGUI text;

    #endregion

    #region UnityFunctions

    private void Update()
    {
        if(OptionsLoader.Instance != null && OptionsLoader.Instance.Keys != null)
            foreach(KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if(k != KeyCode.Escape && Input.GetKeyDown(k))
                {
                    OptionsLoader.Instance.Keys[currentInput] = k;
                    options.UpdateSliders();
                    gameObject.SetActive(false);
                }
                else if(Input.GetKeyDown(k)) gameObject.SetActive(false);
            }
        else gameObject.SetActive(false);

        text.text = "Press any key to set to " + ((InputActions)currentInput).ToString() + "\nor Esc to go back. \nCurrent key using: " + OptionsLoader.Instance.Keys[currentInput].ToString();
    }

    #endregion
}


