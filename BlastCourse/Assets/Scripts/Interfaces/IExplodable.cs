using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExplodable
{
    public void ExplosionBehaviour(Vector3 origin, Explosion exp, Vector3 normal);
}
