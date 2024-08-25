using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Unloader : MonoBehaviour
{
    [SerializeField] bool unloadOnStart;
    [SerializeField] UnloadMode unloadMode;

    [SerializeField] GameObject[] targetObjects;
    [SerializeField] GameObject[] targetRenderers;
    private List<Component> targetComponents;

    int _playerEntries;

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

        foreach (Component rend in targetComponents)
        {
            if (rend is Behaviour) (rend as Behaviour).enabled = load;
            else if (rend is Renderer) (rend as Renderer).enabled = load;
        }
    }

    private IEnumerator Start()
    {
        targetComponents = new();

        foreach (GameObject rend in targetRenderers)
        {
            foreach (Component childRend in rend.GetComponentsInChildren<Component>().Where((x) => (x is Behaviour or Renderer)))
            {
                targetComponents.Add(childRend);
            }
        }

        yield return null;

        if (unloadOnStart && _playerEntries <= 0) Load(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            _playerEntries++;
            if(_playerEntries != 1) return;

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
            _playerEntries--;
            if (_playerEntries != 0) return;

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
        else if (targetRenderers.Length > 0)
        {
            Behaviour rend = targetRenderers[0].GetComponentInChildren<Behaviour>();
            if (rend != null) { return rend.enabled; }
            else return false;
        }
        else return false;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        foreach (BoxCollider col in gameObject.GetComponents<BoxCollider>())
        {
            Gizmos.DrawCube(col.center, col.size);
        }
    }
#endif
}


