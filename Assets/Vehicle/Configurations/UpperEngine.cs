using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperEngine : VehicleStatic
{
    // Upper Points
    GameObject[] UpperRampPoints;
    GameObject[] UpperEnginePoints;
    GameObject[] UpperNozzlePoints;
    GameObject[] UpperNacelleRampPoints;

    // Lower Points
    GameObject[] LowerRampPoints;

    public override void BuildFlowLines()
    {
        Processor[] lowerFlowLine = new Processor[] { new Ramp(LowerRampPoints, false, Width) };
        Processor[] upperFlowLine = new Processor[] { new Ramp(UpperRampPoints, true, Width), new Ramp(UpperNacelleRampPoints, true, Width) };
        Processor[] upperEngineFlowLine = new Processor[] { upperFlowLine[0], new Combustor(UpperEnginePoints, ful, Width), new Nozzle(UpperNozzlePoints, lowerFlowLine[0].Current[^1], upperFlowLine[1].Current[^1], Width) };

        FlowLines = new Processor[][] { lowerFlowLine, upperFlowLine, upperEngineFlowLine };
    }
}
