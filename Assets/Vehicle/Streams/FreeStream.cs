using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeStream : Stream
{
    public FreeStream(Vector3 flowDir)
    {
        FlowDir = Vector3.Normalize(flowDir);
    }

    public override Vector3[] WallNormals(Stream matchStream = null)
    {
        Vector3 freeNormal = Vector3.Cross(FlowDir, Vector3.forward).normalized;
        Vector3 matchNormal = Vector3.Dot(freeNormal, matchStream.WallNormals()[0]) > 0 ? freeNormal : -freeNormal;
        return new Vector3[1] { matchNormal };
    }
}
