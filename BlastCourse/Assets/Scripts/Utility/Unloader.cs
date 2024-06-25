using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Unloader : BoxVisualizer
{
    [SerializeField] bool unloadOnStart;
    [SerializeField] UnloadMode unloadMode;

    [SerializeField] GameObject[] targetObjects;
    [SerializeField] Behaviour[] targetComponents;

    public enum UnloadMode
    {
        LoadOnEntry,
        LoadOnExit,
        UnloadOnEntry,
        UnloadOnExit,
        UnloadOnEntryLoadOnExit,
        UnloadOnExitLoadOnEntry,
        LoadOnCrossing
    }

    private void Load(bool load)
    {
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
        if (unloadOnStart)Load(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            if (isLoaded()) //LOADED
            {
                if (unloadMode is UnloadMode.UnloadOnEntry or UnloadMode.UnloadOnEntryLoadOnExit)
                    Load(false);
                else if((unloadMode is UnloadMode.LoadOnCrossing) && Vector3.Dot(other.attachedRigidbody.velocity.normalized, transform.forward) <= 0f)
                {
                    Load(false);
                }
            }
            else //UNLOADED
            {
                if(unloadMode is UnloadMode.UnloadOnExitLoadOnEntry or UnloadMode.LoadOnEntry)
                    Load(true);
                else if ((unloadMode is UnloadMode.LoadOnCrossing) && Vector3.Dot(other.attachedRigidbody.velocity.normalized, transform.forward) >= 0f)
                {
                    Load(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            if (isLoaded()) //LOADED
            {
                if (unloadMode is UnloadMode.UnloadOnExit or UnloadMode.UnloadOnExitLoadOnEntry)
                    Load(false);
                else if ((unloadMode is UnloadMode.LoadOnCrossing) && Vector3.Dot(other.attachedRigidbody.velocity.normalized, transform.forward) <= 0f)
                {
                    Load(false);
                }
            }
            else //UNLOADED
            {
                if (unloadMode is UnloadMode.UnloadOnEntryLoadOnExit or UnloadMode.LoadOnExit)
                    Load(true);
                else if ((unloadMode is UnloadMode.LoadOnCrossing) && Vector3.Dot(other.attachedRigidbody.velocity.normalized, transform.forward) >= 0f)
                {
                    Load(true);
                }
            }
        }
    }

    private bool isLoaded()
    {
        if (targetObjects.Length > 0)
        {
            return targetObjects[0].activeInHierarchy;
        }
        else if (targetComponents.Length > 0)
        {
            return targetComponents[0].enabled;
        }
        else return false;
    }

    protected override Color GetColor()
    {
        return new Color(0, 0, 1, 0.5f);
    }
}


