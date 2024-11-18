using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FreeLookCamaraEvit : MonoBehaviour
{
   private void Start()
    {
 
        var camera = GetComponent<Camera>();

        LayerMask layerMask = ~LayerMask.GetMask("Icono");
        camera.cullingMask = ~camera.cullingMask & layerMask;

    }
}