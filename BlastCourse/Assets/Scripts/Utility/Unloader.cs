using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Unloader : MonoBehaviour
{
    [SerializeField] bool unloadOnStart;
    [SerializeField] UnloadMode unloadMode;

    [SerializeField] GameObject[] targetObjects;
    [SerializeField] Behaviour[] targetComponents;

    private bool loaded;

    public enum UnloadMode
    {
        UnloadOnEntry,
        UnloadOnExit,
        UnloadOnEntryLoadOnExit,
        UnloadOnExitLoadOnEntry
    }

    private void Load(bool load)
    {
        loaded = load;

        foreach (GameObject obj in targetObjects)
        {
            obj.SetActive(load);
        }

        foreach (Behaviour comp in targetComponents)
        {
            comp.enabled = load;
        }
    }

    private void Start()
    {
        Load(!unloadOnStart);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerMovement>() != null)
        {
            if (loaded) //LOADED
            {
                if (unloadMode is UnloadMode.UnloadOnEntry or UnloadMode.UnloadOnEntryLoadOnExit)
                    Load(false);
            }
            else //UNLOADED
            {
                if(unloadMode is UnloadMode.UnloadOnExitLoadOnEntry)
                    Load(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            if (loaded) //LOADED
            {
                if (unloadMode is UnloadMode.UnloadOnExit or UnloadMode.UnloadOnExitLoadOnEntry)
                    Load(false);
            }
            else //UNLOADED
            {
                if (unloadMode is UnloadMode.UnloadOnEntryLoadOnExit)
                    Load(true);
            }
        }
    }
    
}


