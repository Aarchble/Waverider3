using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combustor : Component
{
    // Streams
    Stream Up;
    NearStream Current;
    Stream Down;

    // Processes
    Deflect Isolator;
    Combust Injection;

    public Combustor(NearStream stream, Stream inStream, Fuel fuel)
    {
        Isolator = inStream is NearStream nearIn ? new(nearIn, stream) : new((FreeStream)inStream, stream);
        Injection = new(fuel);

        Up = inStream;
        Current = stream;
    }

    public override void Operate()
    {
        // InletRamp -> Engine => DEFLECT (SHOCK)
        Parcel preEngine = Isolator.GetParcel(Up.Fluid); // engine fluid pre-combustion
        // ! This pressure doesn't act anywhere significant
        AddDrawnMesh(DeflectMeshes, Isolator.GetDeflectMesh(Current)); // This will always be a shock, hence only adds one Mesh to list. 


        // Engine -> Nozzle => COMBUST
        float minCombustionLength = 0.0f; // not dimensionless
        // ! Check both upper and lower lengths of engine
        if (Vector3.Dot(Current.Outlet[0] - Isolator.featureVertices[0], Current.FlowDir) < minCombustionLength || Vector3.Dot(Current.Outlet[^1] - Isolator.featureVertices[1], Current.FlowDir) < minCombustionLength)
        {
            // ! Engine shock exits the combustion chamber through the nozzle, combustion unlikely
            Debug.Log("Insufficient Combustion!");
            Current.Fluid = preEngine;
        }
        else
        {
            Current.Fluid = Injection.GetParcel(preEngine); // update to engine fluid post-combustion
        }

        // ! Pressure Forces
        PressureForceAndMoment(Current.WallPoints(0.5f)[0], Current.WallNormals()[0], Current.Fluid.P);
        PressureForceAndMoment(Current.WallPoints(0.5f)[1], Current.WallNormals()[1], Current.Fluid.P);
        // ! Stream Thrust
        float massFlow = Current.Fluid.Rho * Current.Fluid.V * (Current.Inlet[1] - Current.Inlet[0]).magnitude * Width;
        StreamForceAndMoment(Vector3.Lerp(Current.Inlet[0], Current.Inlet[^1], 0.5f), Current.FlowDir, (Current.Fluid.V - preEngine.V) * massFlow);
    }
}
