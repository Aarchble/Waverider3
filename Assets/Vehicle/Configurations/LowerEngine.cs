using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerEngine : VehicleStatic
{
    // Upper Points
    public List<GameObject> UpperRampPoints;

    // Lower Points
    public List<GameObject> LowerRampPoints;
    public List<GameObject> LowerEnginePoints; // rule: {inlet 0, inlet 1, outlet 0, outlet 1}
    public List<GameObject> LowerNozzlePoints; // rule: {inlet 0, inlet 1, outlet 0, outlet 1}
    public List<GameObject> LowerNacelleRampPoints;

    // Meshes
    Mesh fuselage;
    Mesh nacelle;

    // Local Centroids
    Vector3 fuselageCentroid;
    Vector3 nacelleCentroid;


    public override void BuildFlowLines() // This runs once per vehicle edit
    {
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width), new Ramp(LowerNacelleRampPoints, false, Width) };
        Processor[] lowerEngineFlowLine = new Processor[] { lowerFlowLine[0], new Combustor(LowerEnginePoints, fuel, Width), new Nozzle(LowerNozzlePoints, upperFlowLine[0].Current[^1], lowerFlowLine[1].Current[^1], Width) };
        
        FlowLines = new Processor[][] { upperFlowLine, lowerFlowLine, lowerEngineFlowLine };
    }

    public override void BuildPerimeter()
    {
        List<GameObject[]> perimeter = new();

        GameObject[] fuselagePoints = new GameObject[LowerRampPoints.Count + UpperRampPoints.Count]; // +2 for engine -2 for upper points overlap
        GameObject[] nacellePoints = new GameObject[LowerNacelleRampPoints.Count + 1]; // +1 for engine/nozzle point

        // Select perimeter points from hardcoded groupings in a clockwise manner
        // Fuselage
        for (int pt = 0; pt < LowerRampPoints.Count; pt++)
        {
            fuselagePoints[pt] = LowerRampPoints[pt];
        }

        fuselagePoints[LowerRampPoints.Count] = LowerEnginePoints[2];

        for (int pt = LowerRampPoints.Count + 1; pt < fuselagePoints.Length; pt++) // follow on from engine point[2]
        {
            fuselagePoints[pt] = UpperRampPoints[UpperRampPoints.Count - 1 - (pt - (LowerRampPoints.Count + 1))];
        }


        // Nacelle
        for (int pt = 0; pt < LowerNacelleRampPoints.Count; pt++)
        {
            nacellePoints[pt] = LowerNacelleRampPoints[pt];
        }

        nacellePoints[LowerNacelleRampPoints.Count] = LowerEnginePoints[3];


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

    public override void BuildMeshes() // This runs once per vehicle edit
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
        fuselage.triangles = TrianglesAboutCentroid(fuselage.vertices);

        nacelle.vertices = nacelleVertices;
        nacelle.vertices[^1] = Vector3.zero;
        nacelle.triangles = TrianglesAboutCentroid(nacelle.vertices);
    }

    public override void AddRampPoint(GameObject newPoint)
    {
        List<List<GameObject>> ramps = new List<List<GameObject>> { UpperRampPoints, LowerRampPoints, LowerNacelleRampPoints };
        // Compare newPoint-ramp[0] and newPoint-ramp[^1] for each ramp
        int mindex = 0; // index of the ramp that has the minimum distance to the new point
        float mindist = float.MaxValue; // minimum distance to new point
        for (int i = 0; i < ramps.Count; i++)
        {
            float dist = NewPointDistances(newPoint, ramps[i][0], ramps[i][^1]);
            if (dist < mindist)
            {
                mindex = i;
                mindist = dist;
            }
        }

        // Add newPoint to ramp with lowest combined distance -> chosenRamp

        // Compare newPoint-chosenRamp[i]
        mindist = float.MaxValue;
        int mindexLocal = 0;
        for (int i = 1; i < ramps[mindex].Count; i++)
        {
            float dist = NewPointDistances(newPoint, ramps[mindex][i-1], ramps[mindex][i]);
            if (dist < mindist)
            {
                mindexLocal = i;
                mindist = dist;
            }
        }

        // Place newPoint between lowest and second lowest distance
        ramps[mindex].Insert(mindexLocal, newPoint);
    }

    public override Mesh[] GetMeshes() // This runs every frame
    {
        return new Mesh[] { fuselage, nacelle };
    }
}
