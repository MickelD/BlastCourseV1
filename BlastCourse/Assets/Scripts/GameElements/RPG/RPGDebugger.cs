using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGDebugger : MonoBehaviour
{
    #region Fields
    [Space(5), Header("Game Objects"), Space(3)]
    [SerializeField] private GameObject g_positionSphere;
    [SerializeField] private GameObject g_lineCilinder;
    [SerializeField] private GameObject g_rayCilinder;
    [Space(5), Header("Game Objects"), Space(3)]
    [SerializeField] private float duration = 5;

    #endregion

    #region UnityFunctions

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    #endregion

    #region Variables

    public static RPGDebugger Instance;

    #endregion

    #region Methods

    public void DrawLine(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        GameObject GO = Instantiate(g_lineCilinder, start + direction/2, Quaternion.identity, transform);
        Vector3 scale = GO.transform.localScale;
        scale = new Vector3(0.05f, direction.magnitude/2, 0.05f);
        GO.transform.localScale = scale;
        GO.transform.up = direction.normalized;

        GO.SetActive(true);
        Destroy(GO, duration);
    }
    public void DrawRay(Vector3 start, Vector3 direction)
    {
        GameObject GO = Instantiate(g_rayCilinder, start + direction / 2, Quaternion.identity, transform);
        Vector3 scale = GO.transform.localScale;
        scale = new Vector3(0.08f, direction.magnitude/2, 0.08f);
        GO.transform.localScale = scale;
        GO.transform.up = direction.normalized;

        GO.SetActive(true);
        Destroy(GO, duration);
    }
    public void DrawPoint(Vector3 position)
    {
        GameObject GO = Instantiate(g_positionSphere, position, Quaternion.identity,transform);

        GO.SetActive(true);
        Destroy(GO, duration);
    }

    #endregion
}
