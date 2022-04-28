using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nozzle : Component
{
    // Streams
    Stream Up;
    NearStream Current;
    Stream[] Down;

    // Processes
    AreaChange Surface;

    public Nozzle(NearStream stream, Stream inStream, Stream outUpper, Stream outLower)
    {
        Up = inStream;
        Current = stream;
        Down = new Stream[] { outUpper, outLower };

        Surface = new((stream.Outlet[1] - stream.Outlet[0]).magnitude / (stream.Inlet[1] - stream.Inlet[0]).magnitude);
    }

    public override void Operate()
    {
        // Nozzle -> Exhaust => NOZZLE
        Current.Fluid = Surface.GetParcel(Up.Fluid);
        // ! Pressure Forces
        PressureForceAndMoment(Current.WallPoints(0.2809f)[0], Current.WallNormals()[0], 0.3167f * Current.Fluid.P);
        PressureForceAndMoment(Current.WallPoints(0.2809f)[1], Current.WallNormals()[1], 0.3167f * Current.Fluid.P);
        // ! Stream Thrust
        float massFlow = Up.Fluid.Rho * Up.Fluid.V * (Current.Inlet[1] - Current.Inlet[0]).magnitude * Width;
        StreamForceAndMoment(Vector3.Lerp(Current.Inlet[0], Current.Inlet[^1], 0.5f), Current.FlowDir, (Current.Fluid.V - Up.Fluid.V) * massFlow);

        // Exhaust -> UpperRamp => EXHAUST
        //Exhaust UpperExhaust = new(afm.UpperRamp.Fluid, afm.NozzleExpansionAngle, afm.NozzleExitRadius, upper: true);
        //Parcel upperPlume = UpperExhaust.GetParcel(afm.Nozzle.Fluid);
        //AddDrawnMesh(ExhaustMeshes, UpperExhaust.GetExhaustMesh(afm.Nozzle, effectThickness));


        //// Exhaust -> NacelleRamp => EXHAUST
        //Exhaust NacelleExhaust = new(afm.NacelleRamp.Fluid, afm.NozzleExpansionAngle, afm.NozzleExitRadius);
        //Parcel NacellePlume = NacelleExhaust.GetParcel(afm.Nozzle.Fluid);
        //AddDrawnMesh(ExhaustMeshes, NacelleExhaust.GetExhaustMesh(afm.Nozzle, effectThickness));
    }
}
