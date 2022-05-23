using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VehiclePoint : MonoBehaviour
{
    public bool gridSnap = false;
    public float scale = 10f;
    public List<NearStream> ConnectedStreams;

    void Update()
    {
        if (transform.hasChanged == true)
        {
            transform.hasChanged = false;
            transform.position = (Vector3)Vector3Int.RoundToInt(transform.position * scale) / scale;
        }
    }
}
