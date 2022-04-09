using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing
{
    public float Length; // Chord
    public float Width; // Semi Span
    public float EdgeHalfAngle;

    Mesh WingMesh;

    ExternalStream UpperLead;
    ExternalStream UpperTrail;
    ExternalStream LowerLead;
    ExternalStream LowerTrail;

    public Wing(float chord, float semiSpan, float angle)
    {
        Length = chord;
        Width = semiSpan;
        EdgeHalfAngle = angle * Mathf.Deg2Rad;

        Vector3 leadEdge = new Vector3(chord / 2f, 0f);
        Vector3 maxThick = new Vector3(0f, chord / 2f * Mathf.Tan(EdgeHalfAngle));

        Vector3[] wingVert = new Vector3[] { leadEdge, maxThick, -leadEdge, -maxThick };

        WingMesh.vertices = wingVert;
        WingMesh.triangles = new int[] { 0, 3, 1, 3, 2, 1 };

        UpperLead = new(wingVert[0], wingVert[1], true);
        UpperTrail = new(wingVert[1], wingVert[2], true);
        LowerLead = new(wingVert[0], wingVert[3], false);
        LowerTrail = new(wingVert[3], wingVert[2], false);
    }
}
