using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperEngine : VehicleStatic
{
    // Upper Points
    public GameObject[] UpperRampPoints;
    public GameObject[] UpperEnginePoints;
    public GameObject[] UpperNozzlePoints;
    public GameObject[] UpperNacelleRampPoints;

    // Lower Points
    public GameObject[] LowerRampPoints;

    // Meshes
    Mesh fuselage;
    Mesh nacelle;


    public override void BuildFlowLines()
    {
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width) };
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width), new Ramp(UpperNacelleRampPoints, true, Width) };
        Processor[] upperEngineFlowLine = new Processor[] { upperFlowLine[0], new Combustor(UpperEnginePoints, fuel, Width), new Nozzle(UpperNozzlePoints, lowerFlowLine[0].Current[^1], upperFlowLine[1].Current[^1], Width) };

        FlowLines = new Processor[][] { lowerFlowLine, upperFlowLine, upperEngineFlowLine };
    }

    public override Mesh[] BuildMeshes()
    {
        Vector3[] fuselageVertices = new Vector3[LowerRampPoints.Length + UpperRampPoints.Length]; // +2 for engine -2 for upper points overlap
        Vector3[] nacelleVertices = new Vector3[UpperNacelleRampPoints.Length + 1]; // +1 for engine/nozzle point
    }

    public override Mesh[] GetMeshes()
    {
        return new Mesh[] { fuselage, nacelle };
    }
}
