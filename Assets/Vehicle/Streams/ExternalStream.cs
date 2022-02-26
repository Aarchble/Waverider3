using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalStream : NearStream
{
    Vector3[] _Inlet;
    Vector3[] _Outlet;
    Vector3 _FlowDir;
    Parcel _Fluid;
    bool Upper;

    public ExternalStream(Vector3 inlet, Vector3 outlet, bool upper, Vector3? flowDir = null)
    {
        Upper = upper;
        _Inlet = new Vector3[1] { inlet };
        _Outlet = new Vector3[1] { outlet };
        if (flowDir != null)
        {
            _FlowDir = Vector3.Normalize((Vector3)flowDir); // Cast nullable V3 to V3
        }
        else
        {
            _FlowDir = Vector3.Normalize(_Outlet[0] - _Inlet[0]);
        }
    }

    public override Vector3[] WallPoints(float t)
    {
        return new Vector3[1] { Vector3.Lerp(_Inlet[0], _Outlet[0], t) };
    }

    public override Vector3[] WallVectors()
    {
        return new Vector3[1] { Outlet[0] - Inlet[0] };
    }

    public override Vector3[] WallNormals()
    {
        if (Upper)
        {
            return new Vector3[1] { Vector3.Cross(_Outlet[0] - _Inlet[0], Vector3.forward).normalized };
        }
        else
        {
            return new Vector3[1] { Vector3.Cross(_Outlet[0] - _Inlet[0], Vector3.back).normalized };
        }
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

    public override Vector3[] Inlet
    {
        get { return _Inlet; }
        set { _Inlet = value; }
    }

    public override Vector3[] Outlet
    {
        get { return _Outlet; }
        set { _Outlet = value; }
    }
}
