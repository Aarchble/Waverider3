using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalStream : NearStream
{
    bool Upper;

    public ExternalStream(Vector3 inlet, Vector3 outlet, bool upper)
    {
        Upper = upper;
        Inlet = new Vector3[1] { inlet };
        Outlet = new Vector3[1] { outlet };
        FlowDir = Vector3.Normalize(Outlet[0] - Inlet[0]);
    }

    public override Vector3[] WallPoints(float t)
    {
        return new Vector3[1] { Vector3.Lerp(Inlet[0], Outlet[0], t) };
    }

    public override Vector3[] WallVectors()
    {
        return new Vector3[1] { Outlet[0] - Inlet[0] };
    }

    public override Vector3[] WallNormals()
    {
        if (Upper)
        {
            return new Vector3[1] { Vector3.Cross(Outlet[0] - Inlet[0], Vector3.forward).normalized };
        }
        else
        {
            return new Vector3[1] { Vector3.Cross(Outlet[0] - Inlet[0], Vector3.back).normalized };
        }
    }
}
