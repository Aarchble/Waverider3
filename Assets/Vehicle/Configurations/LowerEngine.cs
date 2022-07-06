using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerEngine : VehicleStatic
{
    // Upper Points
    GameObject[] UpperRampPoints;

    // Lower Points
    GameObject[] LowerRampPoints;
    GameObject[] LowerEnginePoints;
    GameObject[] LowerNozzlePoints;
    GameObject[] LowerNacelleRampPoints;

    public override void BuildFlowLines()
    {
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width), new Ramp(LowerNacelleRampPoints, false, Width) };
        Processor[] lowerEngineFlowLine = new Processor[] { lowerFlowLine[0], new Combustor(LowerEnginePoints, ful, Width), new Nozzle(LowerNozzlePoints, upperFlowLine[0].Current[^1], lowerFlowLine[1].Current[^1], Width) };
        
        FlowLines = new Processor[][] { upperFlowLine, lowerFlowLine, lowerEngineFlowLine };
    }
}
