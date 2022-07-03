using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nozzle : Processor
{
    // Processes
    AreaChange Surface;
    Exhaust UpperExhaust;
    Exhaust LowerExhaust;

    public Nozzle(GameObject[] points, Stream outUpper, Stream outLower, float width, List<Mesh> exhaustMeshes = null)
    {
        InternalStream stream = new(new Vector3[] { points[0].transform.localPosition, points[1].transform.localPosition }, new Vector3[] { points[2].transform.localPosition, points[3].transform.localPosition });

        operated = false;
        ExhaustMeshes = exhaustMeshes == null ? new() : exhaustMeshes;

        Width = width;
        Current = new NearStream[] { stream };
        //Down = new Stream[] { outUpper, outLower };

        Surface = new((Current[0].Outlet[1] - Current[0].Outlet[0]).magnitude / (Current[0].Inlet[1] - Current[0].Inlet[0]).magnitude);
        UpperExhaust = new(Current[0], outUpper);
        LowerExhaust = new(Current[0], outLower);
    }

    public override void Operate(Stream inStream)
    {
        // Nozzle -> Exhaust => NOZZLE
        Current[0].Fluid = Surface.GetParcel(inStream.Fluid);
        // ! Pressure Forces
        PressureForceAndMoment(Current[0].WallPoints(0.2809f)[0], Current[0].WallNormals()[0], 0.3167f * Current[0].Fluid.P);
        PressureForceAndMoment(Current[0].WallPoints(0.2809f)[1], Current[0].WallNormals()[1], 0.3167f * Current[0].Fluid.P);
        // ! Stream Thrust
        float massFlow = inStream.Fluid.Rho * inStream.Fluid.V * (Current[0].Inlet[1] - Current[0].Inlet[0]).magnitude * Width;
        StreamForceAndMoment(Vector3.Lerp(Current[0].Inlet[0], Current[0].Inlet[^1], 0.5f), Current[0].FlowDir, (Current[0].Fluid.V - inStream.Fluid.V) * massFlow);


        // Exhaust -> UpperRamp => EXHAUST
        Parcel upperPlume = UpperExhaust.GetParcel(Current[0].Fluid);
        AddDrawnMesh(ExhaustMeshes, UpperExhaust.GetExhaustMesh(Current[0]));


        // Exhaust -> NacelleRamp => EXHAUST
        Parcel lowerPlume = LowerExhaust.GetParcel(Current[0].Fluid);
        AddDrawnMesh(ExhaustMeshes, LowerExhaust.GetExhaustMesh(Current[0]));

        operated = true;
    }

    public override Stream GetOutput(Processor down)
    {
        return Current[^1];
    }
}
