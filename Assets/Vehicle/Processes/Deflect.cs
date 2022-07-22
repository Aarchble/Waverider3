using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflect : Process
{
    public float Theta;
    public Process Chosen;
    float[] Angles;
    float Tol;
    bool InternalFlow;

    public Deflect(Stream up, Stream down)
    {
        if (up is FreeStream)
        {
            Theta = Mathf.PI / 2f - Mathf.Acos(Vector3.Dot(up.WallNormals(down)[0], down.FlowDir) / (up.WallNormals(down)[0].magnitude * down.FlowDir.magnitude));
        }
        else
        {
            Theta = Mathf.PI / 2f - Mathf.Acos(Vector3.Dot(up.WallNormals()[0], down.FlowDir) / (up.WallNormals()[0].magnitude * down.FlowDir.magnitude));
        }
        Tol = 0.5f * Mathf.Deg2Rad;
        InternalFlow = down is InternalStream;
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
            Fan e = new(Mathf.Abs(Theta));
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

    public Mesh[] GetDeflectMesh(NearStream nearStream)
    {
        float length = 5f;
        float thickness = 0.01f;

        // NO MASK
        if (InternalFlow)
        {
            // -- Internal --
            if (Theta > Tol)
            {
                // Shock
                Vector3 wave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);
                featureVertices = new Vector3[] { nearStream.Inlet[0], nearStream.Inlet[0] + wave * length };
                ThickLine waveLine = new(featureVertices[0], featureVertices[1], thickness);

                return new Mesh[] { waveLine.GetMesh() };
            }
            else if (Theta < -Tol)
            {
                // Shock
                Vector3 wave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);
                featureVertices = new Vector3[] { nearStream.Inlet[^1], nearStream.Inlet[^1] + wave * length };
                ThickLine waveLine = new(featureVertices[0], featureVertices[1], thickness);

                return new Mesh[] { waveLine.GetMesh() };
            }
            else
            {
                return null;
            }
        }
        else
        {
            // -- External --
            if (Theta > Tol)
            {
                // Shock
                Vector3 wave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);
                featureVertices = new Vector3[] { nearStream.Inlet[0], nearStream.Inlet[0] + wave * length };
                ThickLine waveLine = new(featureVertices[0], featureVertices[1], thickness);

                return new Mesh[] { waveLine.GetMesh() };
            }
            else if (Theta < -Tol)
            {
                // Expansion Fan
                Vector3 upstreamWave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[0]);
                Vector3 downstreamWave = nearStream.FlowDir + nearStream.WallNormals()[0] * nearStream.FlowDir.magnitude * Mathf.Tan(Angles[1]);
                featureVertices = new Vector3[] { nearStream.Inlet[0], nearStream.Inlet[0] + upstreamWave * length, nearStream.Inlet[0] + downstreamWave * length };
                ThickLine upstreamWaveLine = new(featureVertices[0], featureVertices[1], thickness);
                ThickLine downstreamWaveLine = new(featureVertices[0], featureVertices[2], thickness);

                return new Mesh[] { upstreamWaveLine.GetMesh(), downstreamWaveLine.GetMesh() };
            }
            else
            {
                return null;
            }
        }
    }
}
