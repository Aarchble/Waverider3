using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NearStream : Stream
{
    public Vector3[] Inlet { get; set; }
    public Vector3[] Outlet { get; set; }

    public abstract Vector3[] WallPoints(float t);
    public abstract Vector3[] WallVectors();
}
