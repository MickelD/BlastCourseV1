using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : ActivableButton
{
    private List<GameObject> _loadedObjects = new();

    private void OnDisable()
    {
        //_loadedObjects = 0;

        //if (_loadedObjects <= 0)
        //{
        //    _loadedObjects = 0;
        //    Press(false);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player)) Load(player.gameObject);
        else if (other.TryGetComponent(out PhysicsObject phyObj))
        {
            phyObj.OnObjectDestroyed += Unload;
            Load(phyObj.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player)) Unload(player.gameObject);
        else if (other.TryGetComponent(out PhysicsObject phyObj))
        {
            phyObj.OnObjectDestroyed -= Unload;
            Unload(phyObj.gameObject);
        }
    }

    private void Load(GameObject obj)
    {
        if (_loadedObjects.Contains(obj)) return;

        if (_loadedObjects.Count <= 0) Press(true);
        _loadedObjects.Add(obj);
    }

    private void Unload(GameObject obj)
    {
        if (!_loadedObjects.Contains(obj)) return;

        _loadedObjects.Remove(obj);
        if (_loadedObjects.Count <= 0) Press(false);
    }
}
