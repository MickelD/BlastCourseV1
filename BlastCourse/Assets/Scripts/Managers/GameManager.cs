using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton Framework
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    #region Methods

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    #endregion

    #region Debug

    public Vector3 _mostRecentExplosionPoint;
    public float _mostRecentExplosionRadius;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_mostRecentExplosionPoint, _mostRecentExplosionRadius);
    }
    #endregion
}
