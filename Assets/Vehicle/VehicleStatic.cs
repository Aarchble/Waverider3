using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleStatic : MonoBehaviour
{
    // Stores all of the static variables that are operated on elsewhere
    // - Processor streamlines
    // - Dimensions
    // - Meshes

    public float Length;
    public float Width;
    public float Height;
    public Fuel fuel;
    public Vector3 Centroid;

    public Processor[][] FlowLines;

    public abstract void BuildFlowLines();
    public abstract void CentrePoints();
    public abstract void BuildMeshes();
    public abstract Mesh[] GetMeshes();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float TriangleArea(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return 0.5f * Mathf.Abs(p0.x * (p1.y - p2.y) + p1.x * (p2.y - p1.y) + p2.x * (p0.y - p1.y));
    }

    public Vector3 TriangleCentroid(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return new Vector3((p0.x + p1.x + p2.x) / 3f, (p0.y + p1.y + p2.y) / 3f);
    }
}
