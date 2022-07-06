using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleEngine : VehicleStatic
{
    // Upper Points
    GameObject[] UpperRampPoints;
    GameObject[] UpperEnginePoints;
    GameObject[] UpperNozzlePoints;
    GameObject[] UpperNacelleRampPoints;

    // Lower Points
    GameObject[] LowerRampPoints;
    GameObject[] LowerEnginePoints;
    GameObject[] LowerNozzlePoints;
    GameObject[] LowerNacelleRampPoints;

    public override void BuildFlowLines()
    {
        // External Flows
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width), new Ramp(UpperNacelleRampPoints, true, Width) };
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width), new Ramp(LowerNacelleRampPoints, false, Width) };

        // Internal Flows
        Processor[] upperEngineFlowLine = new Processor[] { upperFlowLine[0], new Combustor(UpperEnginePoints, ful, Width), new Nozzle(UpperNozzlePoints, lowerFlowLine[0].Current[^1], upperFlowLine[1].Current[^1], Width) };
        Processor[] lowerEngineFlowLine = new Processor[] { lowerFlowLine[0], new Combustor(LowerEnginePoints, ful, Width), new Nozzle(LowerNozzlePoints, upperFlowLine[0].Current[^1], lowerFlowLine[1].Current[^1], Width) };

        FlowLines = new Processor[][] { upperFlowLine, upperEngineFlowLine, lowerFlowLine, lowerEngineFlowLine };
    }
}
