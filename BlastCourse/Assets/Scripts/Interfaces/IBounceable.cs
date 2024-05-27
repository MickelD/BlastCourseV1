using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBounceable
{
    public void BouncePadInteraction(Vector3 dir, float force);
}
