using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEngine : VehicleStatic
{
    // Upper Points
    public GameObject[] UpperRampPoints;

    // Lower Points
    public GameObject[] LowerRampPoints;

    //Meshes
    Mesh fuselage;


    public override void BuildFlowLines()
    {
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width) };
        
        FlowLines = new Processor[][] { upperFlowLine, lowerFlowLine };
    }

    public override void BuildMeshes()
    {
        Vector3[] fuselageVertices = new Vector3[LowerRampPoints.Length + UpperRampPoints.Length - 2]; // -2 for upper points overlap
    }

    public override Mesh[] GetMeshes()
    {
        return new Mesh[] { fuselage };
    }
}
