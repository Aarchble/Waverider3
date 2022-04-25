using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nozzle : Component
{
    // Streams
    Stream Up;
    NearStream Current;
    Stream Down;

    // Processes
    

    public Nozzle(InternalStream stream, )
    {

    }

    public override void Operate()
    {
        // Nozzle -> Exhaust => NOZZLE
        AreaChange Nozzle = new(afm.NozzleRatio);
        afm.Nozzle.Fluid = Nozzle.GetParcel(afm.Engine.Fluid);
        // ! Pressure Forces
        PressureForceAndMoment(afm.Nozzle.WallPoints(0.2809f)[0], afm.Nozzle.WallNormals()[0], 0.3167f * afm.Nozzle.Fluid.P);
        PressureForceAndMoment(afm.Nozzle.WallPoints(0.2809f)[1], afm.Nozzle.WallNormals()[1], 0.3167f * afm.Nozzle.Fluid.P);
        // ! Stream Thrust
        StreamForceAndMoment(Vector3.Lerp(afm.Nozzle.Inlet[0], afm.Nozzle.Inlet[^1], 0.5f), afm.Nozzle.FlowDir, (afm.Nozzle.Fluid.V - afm.Engine.Fluid.V) * massFlow);

        // Exhaust -> UpperRamp => EXHAUST
        Exhaust UpperExhaust = new(afm.UpperRamp.Fluid, afm.NozzleExpansionAngle, afm.NozzleExitRadius, upper: true);
        Parcel upperPlume = UpperExhaust.GetParcel(afm.Nozzle.Fluid);
        AddDrawnMesh(ExhaustMeshes, UpperExhaust.GetExhaustMesh(afm.Nozzle, effectThickness));


        // Exhaust -> NacelleRamp => EXHAUST
        Exhaust NacelleExhaust = new(afm.NacelleRamp.Fluid, afm.NozzleExpansionAngle, afm.NozzleExitRadius);
        Parcel NacellePlume = NacelleExhaust.GetParcel(afm.Nozzle.Fluid);
        AddDrawnMesh(ExhaustMeshes, NacelleExhaust.GetExhaustMesh(afm.Nozzle, effectThickness));
    }
}
