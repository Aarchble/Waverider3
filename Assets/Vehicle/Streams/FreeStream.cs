using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeStream : Stream
{
    public FreeStream(Vector3 flowDir)
    {
        FlowDir = Vector3.Normalize(flowDir);
    }
}
