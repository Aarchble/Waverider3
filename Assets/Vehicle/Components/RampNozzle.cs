using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampNozzle : Component
{
    // Single Expansion Ramp Nozzle (SERN)

    // Processes
    Exhaust UpstreamExhaust;
    Exhaust DownstreamExhaust; // After the ExternalStream
    AreaChange Surface;

    public RampNozzle(ExternalStream stream, float width)
    {
        ExhaustMeshes = new();

        Width = width;
    }

    public override void Operate(Stream inStream)
    {
        throw new System.NotImplementedException();
    }

    public override Stream GetOutput(Component down)
    {
        return Current[^1];
    }
}
