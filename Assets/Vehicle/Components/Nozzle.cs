using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nozzle : Component
{
    // Streams
    //Stream Up;
    //NearStream Current;
    //NearStream[] Down;

    // Processes
    AreaChange Surface;
    Exhaust UpperExhaust;
    Exhaust LowerExhaust;

    public Nozzle(InternalStream stream, Stream inStream, Stream outUpper, Stream outLower, float width)
    {
        ExhaustMeshes = new();

        Width = width;
        Up = new Stream[] { inStream };
        Current = new NearStream[] { stream };
        Down = new Stream[] { outUpper, outLower };

        Surface = new((stream.Outlet[1] - stream.Outlet[0]).magnitude / (stream.Inlet[1] - stream.Inlet[0]).magnitude);
        UpperExhaust = new(Current[0], Down[0]);
        LowerExhaust = new(Current[0], Down[1]);
    }

    public override void Operate()
    {
        // Nozzle -> Exhaust => NOZZLE
        Current[0].Fluid = Surface.GetParcel(Up[0].Fluid);
        // ! Pressure Forces
        PressureForceAndMoment(Current[0].WallPoints(0.2809f)[0], Current[0].WallNormals()[0], 0.3167f * Current[0].Fluid.P);
        PressureForceAndMoment(Current[0].WallPoints(0.2809f)[1], Current[0].WallNormals()[1], 0.3167f * Current[0].Fluid.P);
        // ! Stream Thrust
        float massFlow = Up[0].Fluid.Rho * Up[0].Fluid.V * (Current[0].Inlet[1] - Current[0].Inlet[0]).magnitude * Width;
        StreamForceAndMoment(Vector3.Lerp(Current[0].Inlet[0], Current[0].Inlet[^1], 0.5f), Current[0].FlowDir, (Current[0].Fluid.V - Up[0].Fluid.V) * massFlow);


        // Exhaust -> UpperRamp => EXHAUST
        Parcel upperPlume = UpperExhaust.GetParcel(Current[0].Fluid);
        AddDrawnMesh(ExhaustMeshes, UpperExhaust.GetExhaustMesh(Current[0]));


        // Exhaust -> NacelleRamp => EXHAUST
        Parcel lowerPlume = LowerExhaust.GetParcel(Current[0].Fluid);
        AddDrawnMesh(ExhaustMeshes, LowerExhaust.GetExhaustMesh(Current[0]));
    }
}
