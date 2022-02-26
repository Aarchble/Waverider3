using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickLine
{
    Mesh Line;

    public ThickLine(Vector3 start, Vector3 end, float thickness)
    {
        Vector3 norm = Vector3.Cross((end - start).normalized, Vector3.forward);
        Line = new();
        Line.vertices = new Vector3[4] { end + norm * thickness, start + norm * thickness, start - norm * thickness, end - norm * thickness };
        Line.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
        Line.uv = new Vector2[4] { new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
    }

    public Mesh GetMesh()
    {
        return Line;
    }
}
