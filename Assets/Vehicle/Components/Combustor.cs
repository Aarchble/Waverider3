using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combustor : Processor
{
    // Processes
    Deflect Isolator;
    Combust Injection;

    public Combustor(List<GameObject> points, Fuel fuel, float width)
    {
        InternalStream stream = new(new Vector3[] { points[0].transform.localPosition, points[1].transform.localPosition }, new Vector3[] { points[2].transform.localPosition, points[3].transform.localPosition });

        operated = false;
        DeflectMeshes = VehiclePhysics.Instance.Dmesh;

        Width = width;
        Current = new NearStream[] { stream };

        Injection = new(fuel);
    }

    public override void Operate(Stream inStream)
    {
        Isolator = new(inStream, Current[0]);
        // InletRamp -> Engine => DEFLECT (SHOCK)
        Parcel preEngine = Isolator.GetParcel(inStream.Fluid); // engine fluid pre-combustion
        // ! This pressure doesn't act anywhere significant
        AddDrawnMesh(DeflectMeshes, Isolator.GetDeflectMesh(Current[0])); // This will always be a shock, hence only adds one Mesh to list. 

        // Engine -> Nozzle => COMBUST
        float minCombustionLength = 0.0f; // not dimensionless
        // ! Check both upper and lower lengths of engine
        if (Vector3.Dot(Current[0].Outlet[0] - Isolator.featureVertices[0], Current[0].FlowDir) < minCombustionLength || Vector3.Dot(Current[0].Outlet[^1] - Isolator.featureVertices[1], Current[0].FlowDir) < minCombustionLength)
        {
            // ! Engine shock exits the combustion chamber through the nozzle, combustion unlikely
            //Debug.Log("Insufficient Combustion!");
            Current[0].Fluid = preEngine;
        }
        else
        {
            Current[0].Fluid = Injection.GetParcel(preEngine); // update to engine fluid post-combustion
        }

        // ! Pressure Forces
        PressureForceAndMoment(Current[0].WallPoints(0.5f)[0], Current[0].WallNormals()[0], Current[0].Fluid.P);
        PressureForceAndMoment(Current[0].WallPoints(0.5f)[1], Current[0].WallNormals()[1], Current[0].Fluid.P);
        // ! Stream Thrust
        float massFlow = Current[0].Fluid.Rho * Current[0].Fluid.V * (Current[0].Inlet[1] - Current[0].Inlet[0]).magnitude * Width;
        StreamForceAndMoment(Vector3.Lerp(Current[0].Inlet[0], Current[0].Inlet[^1], 0.5f), Current[0].FlowDir, (Current[0].Fluid.V - preEngine.V) * massFlow);

        operated = true;
    }

    public override Stream GetOutput(Processor down)
    {
        return Current[^1];
    }
}
