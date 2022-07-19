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

    // Local Centroids
    Vector3 fuselageCentroid;
    Vector3 nacelleCentroid;


    public override void BuildFlowLines()
    {
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width) };
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width), new Ramp(UpperNacelleRampPoints, true, Width) };
        Processor[] upperEngineFlowLine = new Processor[] { upperFlowLine[0], new Combustor(UpperEnginePoints, fuel, Width), new Nozzle(UpperNozzlePoints, lowerFlowLine[0].Current[^1], upperFlowLine[1].Current[^1], Width) };

        FlowLines = new Processor[][] { lowerFlowLine, upperFlowLine, upperEngineFlowLine };
    }

    public override void BuildPerimeter()
    {
        List<GameObject[]> perimeter = new List<GameObject[]>();

        GameObject[] fuselagePoints = new GameObject[LowerRampPoints.Length + UpperRampPoints.Length]; // +2 for engine -2 for upper points overlap
        GameObject[] nacellePoints = new GameObject[UpperNacelleRampPoints.Length + 1]; // +1 for engine/nozzle point

        // Select perimeter points from hardcoded groupings in a clockwise manner
        // Fuselage
        for (int pt = 0; pt < LowerRampPoints.Length; pt++)
        {
            fuselagePoints[pt] = LowerRampPoints[pt];
        }

        fuselagePoints[LowerRampPoints.Length] = UpperEnginePoints[2];

        for (int pt = LowerRampPoints.Length + 1; pt < fuselagePoints.Length - 1; pt++) // follow on from engine point[2]
        {
            fuselagePoints[pt] = UpperRampPoints[UpperRampPoints.Length - 1 - (pt - (LowerRampPoints.Length + 1))];
        }


        // Nacelle
        nacellePoints[0] = UpperEnginePoints[1];
        nacellePoints[1] = UpperEnginePoints[3];

        for (int pt = 2; pt < nacellePoints.Length - 1; pt++) // follow on from engine point[3]
        {
            nacellePoints[pt] = UpperNacelleRampPoints[UpperRampPoints.Length - 1 - (pt - (LowerRampPoints.Length + 1))];
        }


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
            fuselageCentroid = PolyCentroid(OrderedPerimeter[0]);


            // Nacelle
            float nacelleArea = PolyArea(OrderedPerimeter[1]);
            nacelleCentroid = PolyCentroid(OrderedPerimeter[1]);


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
            fuselageCentroid -= Centroid;
            nacelleCentroid -= Centroid;

            IsCentred = true; // Point lists end with centroid
        }
    }

    public override void BuildMeshes()
    {
        fuselage = new Mesh();
        nacelle = new Mesh();

        Vector3[] fuselageVertices = new Vector3[OrderedPerimeter[0].Length + 1];
        Vector3[] nacelleVertices = new Vector3[OrderedPerimeter[1].Length + 1];

        for (int f = 0; f < fuselageVertices.Length - 1; f++)
        {
            fuselageVertices[f] = OrderedPerimeter[0][f].transform.localPosition;
        }
        fuselageVertices[^1] = fuselageCentroid;

        for (int n = 0; n < nacelleVertices.Length - 1; n++)
        {
            nacelleVertices[n] = OrderedPerimeter[1][n].transform.localPosition;
        }
        nacelleVertices[^1] = nacelleCentroid;


        fuselage.vertices = fuselageVertices;
        fuselage.vertices[^1] = Vector3.zero;
        fuselage.triangles = TrianglesAboutCentroid(fuselageVertices);

        nacelle.vertices = nacelleVertices;
        nacelle.vertices[^1] = Vector3.zero;
        nacelle.triangles = TrianglesAboutCentroid(nacelleVertices);

    }

    public override Mesh[] GetMeshes()
    {
        return new Mesh[] { fuselage, nacelle };
    }
}
