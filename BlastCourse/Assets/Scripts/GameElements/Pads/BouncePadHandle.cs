using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BouncePadHandle : MonoBehaviour
{
    [SerializeField] BouncePad g_pad;

    private void Update()
    {
        transform.up = g_pad.Reload();
    }
}
