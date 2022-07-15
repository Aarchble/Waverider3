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

    public override void CentrePoints()
    {
        // Fuselage
        Vector3 fuselageCentroid = TriangleCentroid(LowerRampPoints[0].transform.localPosition, LowerRampPoints[^1].transform.localPosition, UpperEnginePoints[2].transform.localPosition);
        float fuselageArea = TriangleArea(LowerRampPoints[0].transform.localPosition, LowerRampPoints[^1].transform.localPosition, UpperEnginePoints[2].transform.localPosition);

        // Nacelle
        Vector3 nacelleCentroid = TriangleCentroid(UpperNacelleRampPoints[0].transform.localPosition, UpperNacelleRampPoints[^1].transform.localPosition, UpperEnginePoints[3].transform.localPosition);
        float nacelleArea = TriangleArea(UpperNacelleRampPoints[0].transform.localPosition, UpperNacelleRampPoints[^1].transform.localPosition, UpperEnginePoints[3].transform.localPosition);

        // Combined
        Centroid = (fuselageCentroid * fuselageArea + nacelleCentroid * nacelleArea) / (fuselageArea + nacelleArea);
    }

    public override void BuildMeshes()
    {
        fuselage = new Mesh();
        nacelle = new Mesh();

        Vector3[] fuselageVertices = new Vector3[LowerRampPoints.Length + UpperRampPoints.Length + 1]; // +2 for engine -2 for upper points overlap +1 for centroid
        Vector3[] nacelleVertices = new Vector3[UpperNacelleRampPoints.Length + 2]; // +1 for engine/nozzle point +1 for centroid


        // Fuselage
        for (int pt = 0; pt < LowerRampPoints.Length; pt++)
        {
            fuselageVertices[pt] = LowerRampPoints[pt].transform.localPosition;
        }

        fuselageVertices[LowerRampPoints.Length] = UpperEnginePoints[2].transform.localPosition;

        for (int pt = LowerRampPoints.Length + 1; pt < fuselageVertices.Length - 1; pt++) // follow on from engine point[2]
        {
            fuselageVertices[pt] = UpperRampPoints[UpperRampPoints.Length - 1 - (pt - (LowerRampPoints.Length + 1))].transform.localPosition;
        }

        fuselageVertices[^1] = Centroid;


        // Nacelle
        nacelleVertices[0] = UpperEnginePoints[1].transform.localPosition;
        nacelleVertices[1] = UpperEnginePoints[3].transform.localPosition;

        for (int pt = 2; pt < nacelleVertices.Length - 1; pt++) // follow on from engine point[3]
        {
            nacelleVertices[pt] = UpperNacelleRampPoints[UpperRampPoints.Length - 1 - (pt - (LowerRampPoints.Length + 1))].transform.localPosition;
        }

        nacelleVertices[^1] = Centroid;


        fuselage.vertices = fuselageVertices;
        fuselage.triangles = TrianglesAboutCentroid(fuselageVertices);

        nacelle.vertices = nacelleVertices;
        nacelle.triangles = TrianglesAboutCentroid(nacelleVertices);

    }

    public override Mesh[] GetMeshes()
    {
        return new Mesh[] { fuselage, nacelle };
    }
}
