using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampNozzle : Processor
{
    // Single Expansion Ramp Nozzle (SERN)

    // Processes
    Exhaust UpstreamExhaust;
    Exhaust DownstreamExhaust; // After the ExternalStream
    AreaChange Surface;

    public RampNozzle(ExternalStream stream, float width)
    {
        operated = false;
        ExhaustMeshes = new();

        Width = width;
    }

    public override void Operate(Stream inStream)
    {
        operated = true;
        throw new System.NotImplementedException();
    }

    public override Stream GetOutput(Processor down)
    {
        return Current[^1];
    }
}
