using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : ActivableButton
{
    private int _loadedObjects;

    private void OnDisable()
    {
        _loadedObjects = 0;

        if (_loadedObjects <= 0)
        {
            _loadedObjects = 0;
            Press(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>()) Load();
        else if (other.TryGetComponent(out PhysicsObject phyObj))
        {
            phyObj.OnObjectDestroyed += Unload;
            Load();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMovement>()) Unload();
        else if (other.TryGetComponent(out PhysicsObject phyObj))
        {
            phyObj.OnObjectDestroyed -= Unload;
            Unload();
        }
    }

    private void Load()
    {
        if (_loadedObjects <= 0) Press(true);
        _loadedObjects++;
    }

    private void Unload()
    {
        _loadedObjects--;

        if (_loadedObjects <= 0)
        {
            _loadedObjects = 0;
            Press(false);
        }
    }
}
