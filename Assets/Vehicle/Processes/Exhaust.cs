using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhaust : Expansion
{
    Parcel Exit;
    float NozzleAngle;
    float NozzleRadius;

    public Exhaust(Parcel e, float nozzleAngle, float nozzleRadius)
    {
        Exit = e;
        NozzleAngle = nozzleAngle;
        NozzleRadius = nozzleRadius;
    }

    public override Parcel GetParcel(Parcel i)
    {
        // Mn = i.M
        float dr = 0.1f;
        int n = 5;

        featureVertices = new Vector3[n];
        featureVertices[0] = new Vector3(0f, 1f);
        float M = Exit.M;

        float ARe = SonicArea(i.Gamma, Exit.M);
        float alpha = PrandtlMeyerAngle(Exit.Gamma, Exit.M) - PrandtlMeyerAngle(i.Gamma, i.M) + NozzleAngle;

        for (int seg = 1; seg < n; seg++)
        {
            featureVertices[seg] = new Vector3(0f, featureVertices[seg - 1].y + dr);
            float AR = featureVertices[seg].y * featureVertices[seg].y * ARe;
            M = SonicAreaMach(i.Gamma, AR);
            float theta = PrandtlMeyerAngle(i.Gamma, M) - PrandtlMeyerAngle(Exit.Gamma, Exit.M);
            featureVertices[seg].x = dr / Mathf.Tan(alpha - theta) + featureVertices[seg - 1].x;
        }

        float Tratio = (1f + (i.Gamma - 1f) / 2f * i.M * i.M) / (1f + (i.Gamma - 1f) / 2f * M * M);
        float Pratio = Mathf.Pow(Tratio, i.Gamma / (i.Gamma - 1f));

        Parcel final = new(i.R, i.Gamma, 1f / Pratio * i.P, Tratio * i.T);
        final.SetMach(M);

        return final;
    }

    //public Mesh GetExhaustMesh()
    //{

    //}
}
