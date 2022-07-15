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

    public override void BuildPerimeter()
    {
        List<GameObject[]> perimeter = new List<GameObject[]>();

        GameObject[] fuselagePoints = new GameObject[LowerRampPoints.Length + UpperRampPoints.Length + 1]; // +2 for engine -2 for upper points overlap +1 for centroid
        GameObject[] nacellePoints = new GameObject[LowerNacelleRampPoints.Length + 2]; // +1 for engine/nozzle point +1 for centroid

        // Fuselage
        for (int pt = 0; pt < LowerRampPoints.Length; pt++)
        {
            fuselagePoints[pt] = LowerRampPoints[pt];
        }

        fuselagePoints[LowerRampPoints.Length] = LowerEnginePoints[2];

        for (int pt = LowerRampPoints.Length + 1; pt < fuselagePoints.Length - 1; pt++) // follow on from engine point[2]
        {
            fuselagePoints[pt] = UpperRampPoints[UpperRampPoints.Length - 1 - (pt - (LowerRampPoints.Length + 1))];
        }

        fuselagePoints[^1] = fuselagePoints[0];


        // Nacelle
        for (int pt = 0; pt < LowerNacelleRampPoints.Length - 1; pt++)
        {
            nacellePoints[pt] = LowerNacelleRampPoints[pt];
        }

        nacellePoints[LowerNacelleRampPoints.Length - 1] = LowerEnginePoints[3];

        nacellePoints[^1] = nacellePoints[0];


        perimeter.Add(fuselagePoints);
        perimeter.Add(nacellePoints);

        OrderedPerimeter = perimeter;

        IsCentred = false; // Point lists end with return to start
    }

    public override void CentrePoints()
    {
        if (!IsCentred) // Don't let this run if centroid has been applied
        {
            // Fuselage
            float fuselageArea = PolyArea(OrderedPerimeter[0]);
            Vector3 fuselageCentroid = PolyCentroid(OrderedPerimeter[0]);
            OrderedPerimeter[0][^1].transform.localPosition = fuselageCentroid;

            // Nacelle
            float nacelleArea = PolyArea(OrderedPerimeter[1]);
            Vector3 nacelleCentroid = PolyCentroid(OrderedPerimeter[1]);
            OrderedPerimeter[1][^1].transform.localPosition = nacelleCentroid;

            // Combined
            Centroid = (fuselageCentroid * fuselageArea + nacelleCentroid * nacelleArea) / (fuselageArea + nacelleArea);


            // -- Shift Points --
            foreach (GameObject[] pts in OrderedPerimeter)
            {
                foreach (GameObject pt in pts)
                {
                    pt.transform.localPosition -= Centroid;
                }
            }

            IsCentred = true; // Point lists end with centroid
        }
    }

    public override void BuildMeshes() // This runs once per vehicle edit
    {
        fuselage = new Mesh();
        nacelle = new Mesh();

        Vector3[] fuselageVertices = new Vector3[OrderedPerimeter[0].Length];
        Vector3[] nacelleVertices = new Vector3[OrderedPerimeter[1].Length];

        for (int f = 0; f < fuselageVertices.Length; f++)
        {
            fuselageVertices[f] = OrderedPerimeter[0][f].transform.localPosition;
        }

        for (int n = 0; n < fuselageVertices.Length; n++)
        {
            nacelleVertices[n] = OrderedPerimeter[1][n].transform.localPosition;
        }


        fuselage.vertices = fuselageVertices;
        fuselage.vertices[^1] = Vector3.zero;
        fuselage.triangles = TrianglesAboutCentroid(fuselage.vertices);

        nacelle.vertices = nacelleVertices;
        nacelle.vertices[^1] = Vector3.zero;
        nacelle.triangles = TrianglesAboutCentroid(nacelle.vertices);

    }

    public override Mesh[] GetMeshes() // This runs every frame
    {
        return new Mesh[] { fuselage, nacelle };
    }
}
