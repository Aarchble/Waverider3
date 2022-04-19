using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhaust : Expand
{
    Parcel Exit;
    float NozzleAngle;
    float NozzleRadius;
    List<Vector3> jetBoundary;
    bool Upper;

    public Exhaust(Parcel e, float nozzleAngle, float nozzleRadius, bool upper = false)
    {
        Exit = e;
        NozzleAngle = nozzleAngle;
        NozzleRadius = nozzleRadius;
        Upper = upper;
    }

    public override Parcel GetParcel(Parcel i)
    {
        // Mn = i.M
        float dr = 0.2f;
        int n = 20;
        float lenLim = 10f;

        jetBoundary = new();
        jetBoundary.Add(new Vector3(0f, 1f));
        float M = Exit.M;

        float ARe = SonicArea(i.Gamma, Exit.M);
        float alpha = PrandtlMeyerAngle(Exit.Gamma, Exit.M) - PrandtlMeyerAngle(i.Gamma, i.M) + NozzleAngle;

        for (int seg = 1; seg < n; seg++)
        {
            Vector3 newPoint = new Vector3(0f, jetBoundary[seg - 1].y + dr);
            float AR = newPoint.y * newPoint.y * ARe;
            M = SonicAreaMach(i.Gamma, AR);
            float theta = PrandtlMeyerAngle(i.Gamma, M) - PrandtlMeyerAngle(Exit.Gamma, Exit.M);
            newPoint.x = dr / Mathf.Tan(alpha - theta) + jetBoundary[seg - 1].x;
            if (newPoint.x < jetBoundary[seg - 1].x || newPoint.x > lenLim)
            {
                break;
            }
            else
            {
                jetBoundary.Add(newPoint);
            }
        }

        float Tratio = (1f + (i.Gamma - 1f) / 2f * i.M * i.M) / (1f + (i.Gamma - 1f) / 2f * M * M);
        float Pratio = Mathf.Pow(Tratio, i.Gamma / (i.Gamma - 1f));

        Parcel final = new(i.R, i.Gamma, 1f / Pratio * i.P, Tratio * i.T);
        final.SetMach(M);

        return final;
    }

    public Mesh[] GetExhaustMesh(NearStream nearStream, float thickness)
    {
        float invert = Upper ? 1f : -1f;

        Vector3 outletOrigin = nearStream.Outlet[0] + (nearStream.Outlet[^1] - nearStream.Outlet[0]) * 0.5f;
        featureVertices = new Vector3[jetBoundary.Count];
        Mesh[] meshes = new Mesh[jetBoundary.Count - 1];

        for (int vert = 0; vert < jetBoundary.Count; vert++)
        {
            featureVertices[vert] = outletOrigin + jetBoundary[vert].x * NozzleRadius * nearStream.FlowDir + invert * jetBoundary[vert].y * NozzleRadius * Vector3.Cross(nearStream.FlowDir, Vector3.forward);

            if (vert > 0)
            {
                meshes[vert - 1] = new ThickLine(featureVertices[vert - 1], featureVertices[vert], thickness).GetMesh();
            }
        }
        return meshes;
    }
}
