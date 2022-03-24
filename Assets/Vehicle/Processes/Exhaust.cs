using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhaust : Expansion
{
    Parcel Exit;
    float NozzleAngle;
    float NozzleRadius;
    Vector3[] jetBoundary;
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
        float dr = 0.1f;
        int n = 10;

        jetBoundary = new Vector3[n];
        jetBoundary[0] = new Vector3(0f, 1f);
        float M = Exit.M;

        float ARe = SonicArea(i.Gamma, Exit.M);
        float alpha = PrandtlMeyerAngle(Exit.Gamma, Exit.M) - PrandtlMeyerAngle(i.Gamma, i.M) + NozzleAngle;

        for (int seg = 1; seg < n; seg++)
        {
            jetBoundary[seg] = new Vector3(0f, jetBoundary[seg - 1].y + dr);
            float AR = jetBoundary[seg].y * jetBoundary[seg].y * ARe;
            M = SonicAreaMach(i.Gamma, AR);
            float theta = PrandtlMeyerAngle(i.Gamma, M) - PrandtlMeyerAngle(Exit.Gamma, Exit.M);
            jetBoundary[seg].x = dr / Mathf.Tan(alpha - theta) + jetBoundary[seg - 1].x;
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
        featureVertices = new Vector3[jetBoundary.Length];
        Mesh[] meshes = new Mesh[jetBoundary.Length - 1];

        for (int vert = 0; vert < jetBoundary.Length; vert++)
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
