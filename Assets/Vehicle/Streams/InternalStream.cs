using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalStream : NearStream
{
    Vector3[] _Inlet;
    Vector3[] _Outlet;
    Vector3 _FlowDir;
    Parcel _Fluid;

    public InternalStream(Vector3[] inlet, Vector3[] outlet)
    {
        _Inlet = new Vector3[2] { inlet[0], inlet[1] };
        _Outlet = new Vector3[2] { outlet[0], outlet[1] };
        _FlowDir = Vector3.Normalize(Vector3.Lerp(_Outlet[0], _Outlet[^1], 0.5f) - Vector3.Lerp(_Inlet[0], _Inlet[^1], 0.5f));
    }

    public override Vector3[] WallPoints(float t)
    {
        return new Vector3[2] { Vector3.Lerp(_Inlet[0], _Outlet[0], t), Vector3.Lerp(_Inlet[^1], _Outlet[^1], t) };
    }

    public override Vector3[] WallVectors()
    {
        return new Vector3[2] { Outlet[0] - Inlet[0], Outlet[^1] - Inlet[^1] };
    }

    public override Vector3[] WallNormals()
    {
        return new Vector3[2] { Vector3.Cross(_Outlet[0] - _Inlet[0], Vector3.back).normalized, Vector3.Cross(_Outlet[^1] - _Inlet[^1], Vector3.forward).normalized };
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
