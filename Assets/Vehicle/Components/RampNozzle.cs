using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampNozzle : Component
{
    // Single Expansion Ramp Nozzle (SERN)

    // Streams
    Stream Up;
    NearStream Current;
    Stream Down;

    // Processes
    Exhaust UpstreamExhaust;
    Exhaust DownstreamExhaust; // After the ExternalStream
    AreaChange Surface;

    public RampNozzle(ExternalStream stream)
    {

    }
}
