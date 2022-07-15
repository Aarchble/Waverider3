using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleEngine : VehicleStatic
{
    // Upper Points
    public GameObject[] UpperRampPoints;
    public GameObject[] UpperEnginePoints;
    public GameObject[] UpperNozzlePoints;
    public GameObject[] UpperNacelleRampPoints;

    // Lower Points
    public GameObject[] LowerRampPoints;
    public GameObject[] LowerEnginePoints;
    public GameObject[] LowerNozzlePoints;
    public GameObject[] LowerNacelleRampPoints;

    // Meshes
    Mesh fuselage;
    Mesh upperNacelle;
    Mesh lowerNacelle;


    public override void BuildFlowLines()
    {
        // External Flows
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width), new Ramp(UpperNacelleRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width), new Ramp(LowerNacelleRampPoints, false, Width) };

        // Internal Flows
        Processor[] upperEngineFlowLine = new Processor[] { upperFlowLine[0], new Combustor(UpperEnginePoints, fuel, Width), new Nozzle(UpperNozzlePoints, lowerFlowLine[0].Current[^1], upperFlowLine[1].Current[^1], Width) };
        Processor[] lowerEngineFlowLine = new Processor[] { lowerFlowLine[0], new Combustor(LowerEnginePoints, fuel, Width), new Nozzle(LowerNozzlePoints, upperFlowLine[0].Current[^1], lowerFlowLine[1].Current[^1], Width) };

        FlowLines = new Processor[][] { upperFlowLine, upperEngineFlowLine, lowerFlowLine, lowerEngineFlowLine };
    }

    public override void CentrePoints()
    {
        // Fuselage
        Vector3 upperFuselageCentroid = TriangleCentroid(UpperRampPoints[0].transform.localPosition, UpperNozzlePoints[2].transform.localPosition, UpperEnginePoints[2].transform.localPosition);
        float upperFuselageArea = TriangleArea(UpperRampPoints[0].transform.localPosition, UpperNozzlePoints[2].transform.localPosition, UpperEnginePoints[2].transform.localPosition);

        Vector3 lowerFuselageCentroid = TriangleCentroid(LowerRampPoints[0].transform.localPosition, LowerNozzlePoints[2].transform.localPosition, LowerEnginePoints[2].transform.localPosition);
        float lowerFuselageArea = TriangleArea(LowerRampPoints[0].transform.localPosition, LowerNozzlePoints[2].transform.localPosition, LowerEnginePoints[2].transform.localPosition);

        float fuselageArea = upperFuselageArea + lowerFuselageArea;
        Vector3 fuselageCentroid = (upperFuselageCentroid * upperFuselageArea + lowerFuselageCentroid * lowerFuselageArea) / fuselageArea;


        // Nacelle
        Vector3 upperNacelleCentroid = TriangleCentroid(UpperNacelleRampPoints[0].transform.localPosition, UpperNacelleRampPoints[^1].transform.localPosition, UpperEnginePoints[3].transform.localPosition);
        float upperNacelleArea = TriangleArea(UpperNacelleRampPoints[0].transform.localPosition, UpperNacelleRampPoints[^1].transform.localPosition, UpperEnginePoints[3].transform.localPosition);

        Vector3 lowerNacelleCentroid = TriangleCentroid(LowerNacelleRampPoints[0].transform.localPosition, LowerNacelleRampPoints[^1].transform.localPosition, LowerEnginePoints[3].transform.localPosition);
        float lowerNacelleArea = TriangleArea(LowerNacelleRampPoints[0].transform.localPosition, LowerNacelleRampPoints[^1].transform.localPosition, LowerEnginePoints[3].transform.localPosition);

        float nacelleArea = upperNacelleArea + lowerNacelleArea;
        Vector3 nacelleCentroid = (upperNacelleCentroid * upperNacelleArea + lowerNacelleCentroid * lowerNacelleArea) / nacelleArea;


        // Combined
        Centroid = (fuselageCentroid * fuselageArea + nacelleCentroid * nacelleArea) / (fuselageArea + nacelleArea);
    }

    public override void BuildMeshes()
    {
        Vector3[] fuselageVertices = new Vector3[LowerRampPoints.Length + UpperRampPoints.Length + 2]; // +2 for upperengine +2 for lowerengine -2 for upper points overlap
        Vector3[] upperNacelleVertices = new Vector3[UpperNacelleRampPoints.Length + 1]; // +1 for engine/nozzle point
        Vector3[] lowerNacelleVertices = new Vector3[LowerNacelleRampPoints.Length + 1]; // +1 for engine/nozzle point
    }

    public override Mesh[] GetMeshes()
    {
        return new Mesh[] { fuselage, upperNacelle, lowerNacelle };
    }
}
