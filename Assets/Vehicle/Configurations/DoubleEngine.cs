using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleEngine : VehicleStatic
{
    // Upper Points
    public List<GameObject> UpperRampPoints;
    public List<GameObject> UpperEnginePoints;
    public List<GameObject> UpperNozzlePoints;
    public List<GameObject> UpperNacelleRampPoints;

    // Lower Points
    public List<GameObject> LowerRampPoints;
    public List<GameObject> LowerEnginePoints;
    public List<GameObject> LowerNozzlePoints;
    public List<GameObject> LowerNacelleRampPoints;

    // Meshes
    Mesh fuselage;
    Mesh upperNacelle;
    Mesh lowerNacelle;

    // Local Centroids
    Vector3 fuselageCentroid;
    Vector3 upperNacelleCentroid;
    Vector3 lowerNacelleCentroid;


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

    public override void BuildPerimeter()
    {
        List<GameObject[]> perimeter = new List<GameObject[]>();

        GameObject[] fuselagePoints = new GameObject[LowerRampPoints.Count + UpperRampPoints.Count + 2]; // +2 for upperengine +2 for lowerengine -2 for upper points overlap
        GameObject[] upperNacellePoints = new GameObject[UpperNacelleRampPoints.Count + 1]; // +1 for engine/nozzle point
        GameObject[] lowerNacellePoints = new GameObject[LowerNacelleRampPoints.Count + 1]; // +1 for engine/nozzle point

        // Select perimeter points from hardcoded groupings in a clockwise manner
        // Fuselage
        for (int pt = 0; pt < LowerRampPoints.Count; pt++)
        {
            fuselagePoints[pt] = LowerRampPoints[pt];
        }

        fuselagePoints[LowerRampPoints.Count] = LowerEnginePoints[2];
        fuselagePoints[LowerRampPoints.Count + 1] = LowerNozzlePoints[2];
        fuselagePoints[LowerRampPoints.Count + 2] = UpperEnginePoints[2];

        for (int pt = LowerRampPoints.Count + 3; pt < fuselagePoints.Length; pt++) // follow on from engine point[2]
        {
            fuselagePoints[pt] = UpperRampPoints[UpperRampPoints.Count - 1 - (pt - (LowerRampPoints.Count + 3))];
        }


        // Upper Nacelle
        upperNacellePoints[0] = UpperEnginePoints[1];
        upperNacellePoints[1] = UpperEnginePoints[3];

        for (int pt = 2; pt < upperNacellePoints.Length; pt++) // follow on from engine point[3]
        {
            upperNacellePoints[pt] = UpperNacelleRampPoints[UpperRampPoints.Count - 1 - (pt - 2)];
        }


        // Lower Nacelle
        for (int pt = 0; pt < LowerNacelleRampPoints.Count; pt++)
        {
            lowerNacellePoints[pt] = LowerNacelleRampPoints[pt];
        }

        lowerNacellePoints[LowerNacelleRampPoints.Count] = LowerEnginePoints[3];


        perimeter.Add(fuselagePoints);
        perimeter.Add(upperNacellePoints);
        perimeter.Add(lowerNacellePoints);

        OrderedPerimeter = perimeter;

        IsCentred = false; // Point lists end with return to start
    }

    public override void CentrePoints()
    {
        if (!IsCentred)
        {
            // Fuselage
            float fuselageArea = PolyArea(OrderedPerimeter[0]);
            fuselageCentroid = PolyCentroid(OrderedPerimeter[0]);


            // Nacelle
            float upperNacelleArea = PolyArea(OrderedPerimeter[1]);
            upperNacelleCentroid = PolyCentroid(OrderedPerimeter[1]);

            float lowerNacelleArea = PolyArea(OrderedPerimeter[2]);
            lowerNacelleCentroid = PolyCentroid(OrderedPerimeter[2]);

            float nacelleArea = upperNacelleArea + lowerNacelleArea;
            Vector3 nacelleCentroid = (upperNacelleCentroid * upperNacelleArea + lowerNacelleCentroid * lowerNacelleArea) / nacelleArea;


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
            upperNacelleCentroid -= Centroid;
            lowerNacelleCentroid -= Centroid;

            IsCentred = true; // Point lists end with centroid
        }
    }

    public override void BuildMeshes()
    {
        fuselage = new Mesh();
        upperNacelle = new Mesh();
        lowerNacelle = new Mesh();

        Vector3[] fuselageVertices = new Vector3[OrderedPerimeter[0].Length + 1];
        Vector3[] upperNacelleVertices = new Vector3[OrderedPerimeter[1].Length + 1];
        Vector3[] lowerNacelleVertices = new Vector3[OrderedPerimeter[2].Length + 1];

        for (int f = 0; f < fuselageVertices.Length - 1; f++)
        {
            fuselageVertices[f] = OrderedPerimeter[0][f].transform.localPosition;
        }
        fuselageVertices[^1] = fuselageCentroid;

        for (int n = 0; n < upperNacelleVertices.Length - 1; n++)
        {
            upperNacelleVertices[n] = OrderedPerimeter[1][n].transform.localPosition;
        }
        upperNacelleVertices[^1] = upperNacelleCentroid;

        for (int n = 0; n < lowerNacelleVertices.Length - 1; n++)
        {
            lowerNacelleVertices[n] = OrderedPerimeter[2][n].transform.localPosition;
        }
        lowerNacelleVertices[^1] = lowerNacelleCentroid;


        fuselage.vertices = fuselageVertices;
        fuselage.vertices[^1] = Vector3.zero;
        fuselage.triangles = TrianglesAboutCentroid(fuselage.vertices);

        upperNacelle.vertices = upperNacelleVertices;
        upperNacelle.vertices[^1] = Vector3.zero;
        upperNacelle.triangles = TrianglesAboutCentroid(upperNacelle.vertices);

        lowerNacelle.vertices = lowerNacelleVertices;
        lowerNacelle.vertices[^1] = Vector3.zero;
        lowerNacelle.triangles = TrianglesAboutCentroid(lowerNacelle.vertices);
    }

    public override void AddRampPoint(GameObject newPoint)
    {
        List<List<GameObject>> ramps = new List<List<GameObject>> { LowerRampPoints, LowerNacelleRampPoints, UpperRampPoints, UpperNacelleRampPoints };
        // Compare newPoint-ramp[0] and newPoint-ramp[^1] for each ramp
        foreach (List<GameObject> ramp in ramps)
        {

        }

        // Add newPoint to ramp with lowest combined distance -> chosenRamp


        // Compare newPoint-chosenRamp[i]


        // Place newPoint between lowest and second lowest distance

    }

    public override Mesh[] GetMeshes()
    {
        return new Mesh[] { fuselage, upperNacelle, lowerNacelle };
    }
}
