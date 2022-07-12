using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerEngine : VehicleStatic
{
    // Upper Points
    public GameObject[] UpperRampPoints;

    // Lower Points
    public GameObject[] LowerRampPoints;
    public GameObject[] LowerEnginePoints; // rule: {inlet 0, inlet 1, outlet 0, outlet 1}
    public GameObject[] LowerNozzlePoints; // rule: {inlet 0, inlet 1, outlet 0, outlet 1}
    public GameObject[] LowerNacelleRampPoints;

    // Meshes
    Mesh fuselage;
    Mesh nacelle;


    public override void BuildFlowLines() // This runs once per vehicle edit
    {
        CentrePoints();
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width), new Ramp(LowerNacelleRampPoints, false, Width) };
        Processor[] lowerEngineFlowLine = new Processor[] { lowerFlowLine[0], new Combustor(LowerEnginePoints, fuel, Width), new Nozzle(LowerNozzlePoints, upperFlowLine[0].Current[^1], lowerFlowLine[1].Current[^1], Width) };
        
        FlowLines = new Processor[][] { upperFlowLine, lowerFlowLine, lowerEngineFlowLine };
    }

    public override void CentrePoints()
    {
        // Fuselage
        Vector3 fuselageCentroid = TriangleCentroid(UpperRampPoints[0].transform.localPosition, UpperRampPoints[1].transform.localPosition, LowerEnginePoints[3].transform.localPosition);
        float fuselageArea = TriangleArea(UpperRampPoints[0].transform.localPosition, UpperRampPoints[1].transform.localPosition, LowerEnginePoints[3].transform.localPosition);

        // Nacelle
        Vector3 nacelleCentroid = TriangleCentroid(LowerNacelleRampPoints[0].transform.localPosition, LowerNacelleRampPoints[1].transform.localPosition, LowerEnginePoints[4].transform.localPosition);
        float nacelleArea = TriangleArea(LowerNacelleRampPoints[0].transform.localPosition, LowerNacelleRampPoints[1].transform.localPosition, LowerEnginePoints[4].transform.localPosition);

        // Combined
        Centroid = (fuselageCentroid * fuselageArea + nacelleCentroid * nacelleArea) / (fuselageArea + nacelleArea);
    }

    public override Mesh[] BuildMeshes() // This runs once per vehicle edit
    {
        fuselage = new Mesh();
        nacelle = new Mesh();

        // -- Vertices --
        Vector3[] fuselageVertices = new Vector3[LowerRampPoints.Length + UpperRampPoints.Length + 1]; // +2 for engine -2 for upper points overlap +1 for centroid
        Vector3[] nacelleVertices = new Vector3[LowerNacelleRampPoints.Length + 2]; // +1 for engine/nozzle point +1 for centroid

        // Fuselage
        for (int pt = 0; pt < LowerRampPoints.Length - 1; pt++)
        {
            fuselageVertices[pt] = LowerRampPoints[pt].transform.localPosition;
        }

        fuselageVertices[LowerRampPoints.Length - 1] = LowerEnginePoints[0].transform.localPosition;
        fuselageVertices[LowerRampPoints.Length] = LowerEnginePoints[2].transform.localPosition;

        for (int pt = LowerRampPoints.Length + 1; pt < fuselageVertices.Length; pt++) // follow on from engine point[2]
        {
            fuselageVertices[pt] = UpperRampPoints[UpperRampPoints.Length - 1 - (pt - (LowerRampPoints.Length + 1))].transform.localPosition;
        }

        // Nacelle
        for (int pt = 0; pt < LowerNacelleRampPoints.Length - 1; pt++)
        {
            nacelleVertices[pt] = LowerNacelleRampPoints[pt].transform.localPosition;
        }

        nacelleVertices[LowerNacelleRampPoints.Length - 1] = LowerEnginePoints[4].transform.localPosition;
    }

    public override Mesh[] GetMeshes() // This runs every frame
    {
        return new Mesh[] { fuselage, nacelle };
    }
}
