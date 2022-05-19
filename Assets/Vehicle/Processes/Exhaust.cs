using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhaust : Expand
{
    Stream Exit;
    float NozzleAngle;
    float NozzleRadius;
    List<Vector3> jetBoundary;
    bool Upper;

    public Exhaust(NearStream nozzle, Stream ambient)
    {
        Exit = ambient;
        NozzleAngle = Vector3.Angle(nozzle.FlowDir, nozzle.WallVectors()[0]) * Mathf.Deg2Rad;
        NozzleRadius = (nozzle.Outlet[1] - nozzle.Outlet[0]).magnitude / 2f;
        Upper = Vector3.Dot(ambient.WallNormals()[0], nozzle.WallNormals()[0]) < 0f;
    }

    public override Parcel GetParcel(Parcel i)
    {
        // Mn = i.M
        float dr = 0.2f;
        int n = 20;
        float lenLim = 10f;

        jetBoundary = new();
        jetBoundary.Add(new Vector3(0f, 1f));
        float M = Exit.Fluid.M;

        float ARe = SonicArea(i.Gamma, Exit.Fluid.M);
        float alpha = PrandtlMeyerAngle(Exit.Fluid.Gamma, Exit.Fluid.M) - PrandtlMeyerAngle(i.Gamma, i.M) + NozzleAngle;

        for (int seg = 1; seg < n; seg++)
        {
            Vector3 newPoint = new Vector3(0f, jetBoundary[seg - 1].y + dr);
            float AR = newPoint.y * newPoint.y * ARe;
            M = SonicAreaMach(i.Gamma, AR);
            float theta = PrandtlMeyerAngle(i.Gamma, M) - PrandtlMeyerAngle(Exit.Fluid.Gamma, Exit.Fluid.M);
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

    public Mesh[] GetExhaustMesh(NearStream nearStream)
    {
        float thickness = 0.01f;

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
