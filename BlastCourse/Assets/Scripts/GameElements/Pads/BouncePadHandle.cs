using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BouncePadHandle : MonoBehaviour
{
    [SerializeField] BouncePad g_pad;
    [SerializeField] PadSizeController g_size;

    private void Update()
    {
        transform.up = g_size.DirectionalParticles(g_pad.Reload());
    }
}
