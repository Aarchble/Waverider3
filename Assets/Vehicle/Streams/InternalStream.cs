using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalStream : NearStream
{
    public InternalStream(Vector3[] inlet, Vector3[] outlet)
    {
        Inlet = new Vector3[2] { inlet[0], inlet[1] };
        Outlet = new Vector3[2] { outlet[0], outlet[1] };
        FlowDir = Vector3.Normalize(Vector3.Lerp(Outlet[0], Outlet[1], 0.5f) - Vector3.Lerp(Inlet[0], Inlet[1], 0.5f));
    }

    public override Vector3[] WallPoints(float t)
    {
        return new Vector3[2] { Vector3.Lerp(Inlet[0], Outlet[0], t), Vector3.Lerp(Inlet[1], Outlet[1], t) };
    }

    public override Vector3[] WallVectors()
    {
        return new Vector3[2] { Outlet[0] - Inlet[0], Outlet[1] - Inlet[1] };
    }

    public override Vector3[] WallNormals(Stream matchStream = null)
    {
        return new Vector3[2] { Vector3.Cross(Outlet[0] - Inlet[0], Vector3.back).normalized, Vector3.Cross(Outlet[1] - Inlet[1], Vector3.forward).normalized };
    }
}
