using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stream
{
    public abstract Vector3 FlowDir { get; set; }
    public abstract Parcel Fluid { get; set; }

    public float AngleTo(Stream other)
    {
        return Vector3.SignedAngle(FlowDir, other.FlowDir, new Vector3(0f, 0f, 1f)) * Mathf.Deg2Rad;
    }

    public float AngleToPoint(Vector3 pt0, Vector3 pt1)
    {
        Vector3 temp = pt1 - pt0;
        return Vector3.SignedAngle(FlowDir, temp, new Vector3(0f, 0f, 1f)) * Mathf.Deg2Rad;
    }
}
