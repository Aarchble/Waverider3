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
    public float Width = 1f;
    public float Height;
    public Fuel fuel = new(290.3f, 1.238f, 0.0291f, 119.95e6f);
    public Vector3 Centroid;
    public Processor[][] FlowLines;
    public List<GameObject[]> OrderedPerimeter;

    public abstract void BuildFlowLines();
    public abstract void BuildPerimeter();
    public abstract void CentrePoints();
    public abstract void BuildMeshes();
    public abstract Mesh[] GetMeshes();

    private void Start()
    {
        BuildVehicle();
    }

    public void BuildVehicle()
    {
        BuildPerimeter();
        CentrePoints();
        CalcDimensions();
        BuildFlowLines();
        BuildMeshes();
    }

    public float PolyArea(GameObject[] pts)
    {
        float area = 0f;
        for (int i = 0; i < pts.Length; i++)
        {
            Vector3 current = pts[i].transform.localPosition;
            Vector3 next = i < pts.Length - 1 ? pts[i + 1].transform.localPosition : pts[0].transform.localPosition; // return to start if last index
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
        for (int i = 0; i < pts.Length; i++)
        {
            Vector3 current = pts[i].transform.localPosition;
            Vector3 next = i < pts.Length - 1 ? pts[i + 1].transform.localPosition : pts[0].transform.localPosition; // return to start if last index
            area += current.x * next.y - next.x * current.y;
            Cx += (current.x + next.x) * (current.x * next.y - next.x * current.y);
            Cy += (current.y + next.y) * (current.x * next.y - next.x * current.y);
        }
        area *= 0.5f;
        Cx /= 6f * area;
        Cy /= 6f * area;
        return new Vector3(Cx, Cy);
    }

    public void CalcDimensions()
    {
        Vector3 vmax = Vector3.negativeInfinity;
        Vector3 vmin = Vector3.positiveInfinity;

        foreach (GameObject[] pts in OrderedPerimeter)
        {
            foreach (GameObject pt in pts)
            {
                vmax.x = pt.transform.localPosition.x > vmax.x ? pt.transform.localPosition.x : vmax.x;
                vmax.y = pt.transform.localPosition.y > vmax.y ? pt.transform.localPosition.y : vmax.y;
                vmin.x = pt.transform.localPosition.x < vmin.x ? pt.transform.localPosition.x : vmin.x;
                vmin.y = pt.transform.localPosition.y < vmin.y ? pt.transform.localPosition.y : vmin.y;
            }
        }

        Vector3 delta = vmax - vmin;
        Length = Mathf.Abs(delta.x);
        Height = Mathf.Abs(delta.y);
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
