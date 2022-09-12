using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEngine : VehicleStatic
{
    // Upper Points
    public List<GameObject> UpperRampPoints;

    // Lower Points
    public List<GameObject> LowerRampPoints;

    //Meshes
    Mesh fuselage;


    public override void BuildFlowLines()
    {
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width) };
        
        FlowLines = new Processor[][] { upperFlowLine, lowerFlowLine };
    }

    public override void BuildPerimeter()
    {
        List<GameObject[]> perimeter = new List<GameObject[]>();

        GameObject[] fuselagePoints = new GameObject[LowerRampPoints.Count + UpperRampPoints.Count - 2]; // -2 for upper points overlap

        // Select perimeter points from hardcoded groupings in a clockwise manner
        // Fuselage
        for (int pt = 0; pt < LowerRampPoints.Count; pt++)
        {
            fuselagePoints[pt] = LowerRampPoints[pt];
        }

        for (int pt = LowerRampPoints.Count; pt < fuselagePoints.Length; pt++) // follow on from engine point[2]
        {
            fuselagePoints[pt] = UpperRampPoints[UpperRampPoints.Count - 2 - (pt - LowerRampPoints.Count)];
        }


        perimeter.Add(fuselagePoints);

        OrderedPerimeter = perimeter;

        IsCentred = false; // Point lists end with return to start
    }

    public override void CentrePoints()
    {
        if (!IsCentred) // Don't let this run if centroid has been applied
        {
            // Fuselage
            Centroid = PolyCentroid(OrderedPerimeter[0]);


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

    public override void BuildMeshes()
    {
        fuselage = new Mesh();

        Vector3[] fuselageVertices = new Vector3[OrderedPerimeter[0].Length];

        for (int f = 0; f < fuselageVertices.Length; f++)
        {
            fuselageVertices[f] = OrderedPerimeter[0][f].transform.localPosition;
        }


        fuselage.vertices = fuselageVertices;
        fuselage.vertices[^1] = Vector3.zero;
        fuselage.triangles = TrianglesAboutCentroid(fuselage.vertices);
    }

    public override void AddRampPoint(GameObject newPoint)
    {
        List<List<GameObject>> ramps = new List<List<GameObject>> { UpperRampPoints, LowerRampPoints };
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
        return new Mesh[] { fuselage };
    }
}
