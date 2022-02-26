using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeStream : Stream
{
    Vector3 _FlowDir;
    Parcel _Fluid;

    public FreeStream(Vector3 flowDir)
    {
        _FlowDir = Vector3.Normalize(flowDir);
    }

    public override Vector3 FlowDir
    {
        get { return _FlowDir; }
        set { _FlowDir = value; }
    }

    public override Parcel Fluid
    {
        get { return _Fluid; }
        set { _Fluid = value; }
    }
}
