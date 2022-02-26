using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflect : Process
{
    public float Theta;
    public Process Chosen;
    float[] Angles;
    float Tol;
    float InvertUpper;
    bool InternalFlow;

    public Deflect(float theta, bool upper = false, bool internalFlow = false)
    {
        // Facade for Shocks and Expansion Fans for general cases (not internal flow where shock > expFan)

        if (upper)
        {
            InvertUpper = -1f;
        }
        else
        {
            InvertUpper = 1f;
        }

        Theta = theta * InvertUpper;
        Tol = 0.5f * Mathf.Deg2Rad;
    }

    public override Parcel GetParcel(Parcel i)
    {
        if (Theta > Tol || InternalFlow)
        {
            // Shock
            Shock s = new(Mathf.Abs(Theta));
            Parcel f = s.GetParcel(i);
            Angles = new float[1] { s.ShockAngle };
            Chosen = s;
            return f;
        }
        else if (Theta < -Tol)
        {
            // Expansion Fan
            ExpansionFan e = new ExpansionFan(Mathf.Abs(Theta));
            Parcel f = e.GetParcel(i);
            Angles = e.FanAngles;
            Chosen = e;
            return f;
        }
        else
        {
            return i;
        }
    }

    public float[] GetAngles()
    {
        if (Chosen != null)
        {
            return Angles;
        }
        else
        {
            return new float[1] { 0f };
        }
    }

    public Mesh GetDeflectMesh(NearStream nearStream, float length, float thickness, bool _debug = false)
    {
        // NO MASK
        if (Theta > Tol)
        {
            // Shock
            Mesh mesh = new();
            Vector3 wave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);

            // Thickness
            Vector3 walldir = (nearStream.Outlet[0] - nearStream.Inlet[0]).normalized;
            Vector3 waveThickness = walldir * thickness / Mathf.Sin(Vector3.Angle(walldir, wave) * Mathf.Deg2Rad);

            mesh.vertices = new Vector3[4] { nearStream.Inlet[0], nearStream.Inlet[0] + waveThickness, nearStream.Inlet[0] + wave * length, nearStream.Inlet[0] + wave * length + waveThickness };
            mesh.uv = new Vector2[4] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
            mesh.triangles = new int[6] { 0, 2, 3, 3, 1, 0 };

            if (_debug)
            {
                Debug.DrawRay(nearStream.Inlet[0], wave * length, Color.red);
            }
            
            return mesh;
        }
        else if (Theta < -Tol)
        {
            // Expansion Fan
            Mesh mesh = new();
            Vector3 upstreamWave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);
            Vector3 downstreamWave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[1]);

            Vector3[] fanVertices = new Vector3[3] { nearStream.Inlet[0], nearStream.Inlet[0] + upstreamWave * length, nearStream.Inlet[0] + downstreamWave * length };
            Vector3 fanCentroid = new Vector3((fanVertices[0].x + fanVertices[1].x + fanVertices[2].x) / 3f, (fanVertices[0].y + fanVertices[1].y + fanVertices[2].y) / 3f);
            mesh.vertices = new Vector3[4] { fanVertices[0], fanVertices[1], fanVertices[2], fanCentroid };
            mesh.uv = new Vector2[4] { new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(0f, 0f)};
            mesh.triangles = new int[9] { 0, 2, 3, 2, 1, 3, 1, 0, 3 };

            if (_debug)
            {
                Debug.DrawRay(nearStream.Inlet[0], upstreamWave * length, Color.green);
                Debug.DrawRay(nearStream.Inlet[0], downstreamWave * length, Color.green);
            }

            return mesh;
        }
        else
        {
            return null;
        }
    }

    public Mesh GetDeflectMesh(NearStream nearStream, float scaleLength, float thickness, Vector3 limStart, Vector3 limVec, bool _debug = false) // should change limVec to limEnd and make according changes, i.e. calculate limVec internally. 
    {
        // LIMIT VECTOR MASK
        if (Theta > Tol)
        {
            // Shock
            Mesh mesh = new();
            Vector3 wave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);

            // linePoint1 = nearStream.Inlet[0], lineVec1 = wave
            // linePoint2 = limStart, lineVec2 = limVec
            Vector3 lineVec3 = limStart - nearStream.Inlet[0];
            Vector3 crossVec1and2 = Vector3.Cross(wave, limVec);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, limVec);
            float length = Mathf.Clamp(Mathf.Abs(Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude), 0f, scaleLength);
            if (Vector3.Dot(limVec, nearStream.Inlet[0] + wave * length - limStart) < 0f)
            {
                length = scaleLength; // Don't cut the wave short
            }

            // Thickness
            Vector3 walldir = (nearStream.Outlet[0] - nearStream.Inlet[0]).normalized;
            Vector3 waveThickness = walldir * thickness / Mathf.Sin(Vector3.Angle(walldir, wave) * Mathf.Deg2Rad);

            mesh.vertices = new Vector3[4] { nearStream.Inlet[0], nearStream.Inlet[0] + waveThickness, nearStream.Inlet[0] + wave * length, nearStream.Inlet[0] + wave * length + waveThickness };
            mesh.uv = new Vector2[4] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
            mesh.triangles = new int[6] { 0, 2, 3, 3, 1, 0 };

            if (_debug)
            {
                Debug.DrawRay(nearStream.Inlet[0], wave * length, Color.red);
            }

            return mesh;
        }
        else if (Theta < -Tol)
        {
            // Expansion Fan
            Mesh mesh = new();
            Vector3 upstreamWave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);
            Vector3 downstreamWave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[1]);

            // -- Cut Wave Length to Limit Vector --
            // linePoint1 = nearStream.Inlet[0], lineVec1 = wave
            // linePoint2 = limStart, lineVec2 = limVec
            Vector3 lineVec3 = limStart - nearStream.Inlet[0];
            Vector3 upstreamcrossVec1and2 = Vector3.Cross(upstreamWave, limVec);
            Vector3 downstreamcrossVec1and2 = Vector3.Cross(downstreamWave, limVec);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, limVec);
            float upstreamLength = Mathf.Clamp(Mathf.Abs(Vector3.Dot(crossVec3and2, upstreamcrossVec1and2) / upstreamcrossVec1and2.sqrMagnitude), 0f, scaleLength);
            float downstreamLength = Mathf.Clamp(Mathf.Abs(Vector3.Dot(crossVec3and2, downstreamcrossVec1and2) / downstreamcrossVec1and2.sqrMagnitude), 0f, scaleLength);
            if (Vector3.Dot(limVec, nearStream.Inlet[^1] + upstreamWave * upstreamLength - limStart) < 0f)
            {
                upstreamLength = scaleLength; // Don't cut the wave short
            }
            if (Vector3.Dot(limVec, nearStream.Inlet[^1] + downstreamWave * downstreamLength - limStart) < 0f)
            {
                downstreamLength = scaleLength; // Don't cut the wave short
            }

            Vector3[] fanVertices = new Vector3[3] { nearStream.Inlet[0], nearStream.Inlet[0] + upstreamWave * upstreamLength, nearStream.Inlet[0] + downstreamWave * downstreamLength };
            Vector3 fanCentroid = new Vector3((fanVertices[0].x + fanVertices[1].x + fanVertices[2].x) / 3f, (fanVertices[0].y + fanVertices[1].y + fanVertices[2].y) / 3f);
            mesh.vertices = new Vector3[4] { fanVertices[0], fanVertices[1], fanVertices[2], fanCentroid };
            mesh.uv = new Vector2[4] { new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(0f, 0f) };
            mesh.triangles = new int[9] { 0, 2, 3, 2, 1, 3, 1, 0, 3 };

            if (_debug)
            {
                Debug.DrawRay(nearStream.Inlet[^1], upstreamWave * upstreamLength, Color.green);
                Debug.DrawRay(nearStream.Inlet[^1], downstreamWave * downstreamLength, Color.green);
            }

            return mesh;
        }
        else
        {
            return null;
        }
    }

    public Mesh GetDeflectMesh(NearStream nearStream, float thickness, bool _debug = false)
    {
        // INTERNAL MASK
        if (Theta > Tol)
        {
            // Upper (Internal) Shock [0]
            Mesh mesh = new();
            Vector3 wave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);

            // -- Cut Wave Length to Limit Vector --
            // linePoint1 = nearStream.Inlet[0], lineVec1 = wave
            // linePoint2 = nearStream.Inlet[^1], lineVec2 = limVec
            Vector3 limVec = nearStream.Outlet[^1] - nearStream.Inlet[^1];
            Vector3 lineVec3 = nearStream.Inlet[^1] - nearStream.Inlet[0];
            Vector3 crossVec1and2 = Vector3.Cross(wave, limVec);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, limVec);
            float length = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            Vector3 intersection = nearStream.Inlet[0] + wave * length;

            // Thickness
            Vector3 walldir = (nearStream.Outlet[0] - nearStream.Inlet[0]).normalized;
            Vector3 waveThickness = walldir * thickness / Mathf.Sin(Vector3.Angle(walldir, wave) * Mathf.Deg2Rad);

            //mesh.vertices = new Vector3[4] { nearStream.Inlet[0], nearStream.Inlet[0] + waveThickness, intersection, intersection + waveThickness };
            mesh.vertices = new Vector3[4] { nearStream.Inlet[0], intersection, intersection + waveThickness, nearStream.Inlet[0] + waveThickness,  };
            mesh.uv = new Vector2[4] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f),  new Vector2(1f, 1f)};
            mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };

            if (_debug)
            {
                Debug.DrawRay(nearStream.Inlet[0], wave * length, Color.red);
            }

            return mesh;
        }
        else if (Theta < -Tol)
        {
            // Lower (Internal) Shock [^1]
            Mesh mesh = new();
            Vector3 wave = nearStream.FlowDir + nearStream.WallNormals()[^1] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);

            // -- Cut Wave Length to Limit Vector --
            // linePoint1 = nearStream.Inlet[^1], lineVec1 = wave
            // linePoint2 = nearStream.Inlet[0], lineVec2 = limVec
            Vector3 limVec = nearStream.Outlet[0] - nearStream.Inlet[0];
            Vector3 lineVec3 = nearStream.Inlet[0] - nearStream.Inlet[^1];
            Vector3 crossVec1and2 = Vector3.Cross(wave, limVec);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, limVec);
            float length = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            Vector3 intersection = nearStream.Inlet[^1] + wave * length;

            // Thickness
            Vector3 walldir = (nearStream.Outlet[0] - nearStream.Inlet[0]).normalized;
            Vector3 waveThickness = walldir * thickness / Mathf.Sin(Vector3.Angle(walldir, wave) * Mathf.Deg2Rad);

            //mesh.vertices = new Vector3[4] { nearStream.Inlet[^1], nearStream.Inlet[^1] + waveThickness, intersection, intersection + waveThickness };
            mesh.vertices = new Vector3[4] { intersection, nearStream.Inlet[^1], nearStream.Inlet[^1] + waveThickness, intersection + waveThickness };
            mesh.uv = new Vector2[4] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
            mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };

            if (_debug)
            {
                Debug.DrawRay(nearStream.Inlet[^1], wave * length, Color.red);
            }

            return mesh;
        }
        else
        {
            return null;
        }
    }
}
