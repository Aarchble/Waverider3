using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleStatic : MonoBehaviour
{
    // Stores all of the static variables that are operated on elsewhere
    // - Processor streamlines
    // - Dimensions
    // - Meshes

    public bool IsCentred = false;

    public float Length;
    public float Width;
    public float Height;
    public Fuel fuel;
    public Vector3 Centroid;
    public Processor[][] FlowLines;
    public List<GameObject[]> OrderedPerimeter;

    public abstract void BuildFlowLines();
    public abstract void BuildPerimeter();
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

    public void BuildVehicle()
    {
        BuildPerimeter();
        CentrePoints();
        BuildFlowLines();
        BuildMeshes();
    }

    public float TriangleArea(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return 0.5f * Mathf.Abs(p0.x * (p1.y - p2.y) + p1.x * (p2.y - p1.y) + p2.x * (p0.y - p1.y));
    }

    public Vector3 TriangleCentroid(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return new Vector3((p0.x + p1.x + p2.x) / 3f, (p0.y + p1.y + p2.y) / 3f);
    }

    public float PolyArea(GameObject[] pts)
    {
        float area = 0f;
        for (int i = 0; i < pts.Length - 1; i++)
        {
            Vector3 current = pts[i].transform.localPosition;
            Vector3 next = pts[i + 1].transform.localPosition;
            area += current.x * next.y - next.x * current.y;
        }
        area *= 0.5f;
        return area;
    }

    public Vector3 PolyCentroid(GameObject[] pts)
    {
        float area = 0f;
        float Cx = 0f;
        float Cy = 0f;
        for (int i = 0; i < pts.Length - 1; i++)
        {
            Vector3 current = pts[i].transform.localPosition;
            Vector3 next = pts[i + 1].transform.localPosition;
            area += current.x * next.y - next.x * current.y;
            Cx += (current.x + next.x) * (current.x * next.y - next.x * current.y);
            Cy += (current.y + next.y) * (current.x * next.y - next.x * current.y);
        }
        area *= 0.5f;
        Cx /= 6f * area;
        Cy /= 6f * area;
        return new Vector3(Cx, Cy);
    }

    public int[] TrianglesAboutCentroid(Vector3[] vertices)
    {
        int[] triangles = new int[vertices.Length * 3];
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            int j = 3 * i; // triangle iterator
            triangles[j] = vertices.Length - 1; // centre point
            triangles[j + 1] = i;

            if (i + 1 == vertices.Length - 1)
            {
                triangles[j + 2] = 0; // return to origin point
            }
            else
            {
                triangles[j + 2] = i + 1;
            }
        }
        return triangles;
    }
}
