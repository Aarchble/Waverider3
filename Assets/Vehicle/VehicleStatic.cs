using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleStatic : MonoBehaviour
{
    // Stores all of the static variables that are operated on elsewhere
    // - Processor streamlines
    // - Dimensions
    // - Meshes

    public float Width;

    public Processor[][] FlowLines;

    public abstract void BuildFlowLines();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
