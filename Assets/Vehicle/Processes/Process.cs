using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Process
{
    public Vector3[] featureVertices;
    public abstract Parcel GetParcel(Parcel i);
}
