using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEngine : VehicleStatic
{
    // Upper Points
    GameObject[] UpperRampPoints;

    // Lower Points
    GameObject[] LowerRampPoints;

    public override void BuildFlowLines()
    {
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width) };
        
        FlowLines = new Processor[][] { upperFlowLine, lowerFlowLine };
    }
}
