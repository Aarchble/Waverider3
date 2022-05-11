using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing
{
    public Vector3 Position;

    public float Length; // Chord
    public float Width; // Semi Span
    float EdgeHalfAngle;
    float Alpha; // Angle of Attack

    public Mesh WingMesh;

    public ExternalStream[] Upper;
    public ExternalStream[] Lower;

    public Wing(Vector3 position, float chord, float semiSpan, float thickAngle, float alpha)
    {
        Position = position;
        Length = chord;
        Width = semiSpan;
        EdgeHalfAngle = thickAngle * Mathf.Deg2Rad;
        Alpha = alpha * Mathf.Deg2Rad;

        WingMesh = new Mesh();

        Vector3 leadEdge = new Vector3((chord / 2f) * Mathf.Cos(Alpha), (chord / 2f) * Mathf.Sin(Alpha));
        Vector3 maxThick = new Vector3(-(chord / 2f * Mathf.Tan(EdgeHalfAngle)) * Mathf.Sin(Alpha), (chord / 2f * Mathf.Tan(EdgeHalfAngle)) * Mathf.Cos(Alpha));

        Vector3[] wingVert = new Vector3[] { Position + leadEdge, Position + maxThick, Position - leadEdge, Position - maxThick };

        WingMesh.vertices = wingVert;
        WingMesh.triangles = new int[] { 0, 3, 1, 3, 2, 1 };

        Upper = new ExternalStream[] { new ExternalStream(wingVert[0], wingVert[1], true), new ExternalStream(wingVert[1], wingVert[2], true) };
        Lower = new ExternalStream[] { new ExternalStream(wingVert[0], wingVert[3], false), new ExternalStream(wingVert[3], wingVert[2], false) };
    }
}
