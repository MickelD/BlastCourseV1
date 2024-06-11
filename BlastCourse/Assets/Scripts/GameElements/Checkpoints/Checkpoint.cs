using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : BoxVisualizer
{
    [SerializeField, Tooltip("Debug Only")] private bool _spawnHere;
    [SerializeField] private AudioCue _sfx;

    private bool _passed;

    #if UNITY_EDITOR
    private void Start()
    {
        if (_spawnHere && SaveLoader.Instance != null)
        {
            SaveLoader.Instance.startWithAllUnlocks = true;
            SaveLoader.Instance.SetSpawn(transform.position);
        }
    }
#endif

    protected override Color GetColor()
    {
        return new Color(0, 1, 0, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_passed) return;
        _passed = true;

        if (AudioManager.Instance != null && _sfx.SfxClip != null) AudioManager.TryPlayCueAtPoint(_sfx, transform.position);

        if (SaveLoader.Instance != null)
        {
            SaveLoader.Instance.SetSpawn(transform.position);
        }
    }
}
