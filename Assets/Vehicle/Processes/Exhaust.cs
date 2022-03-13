using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhaust : Expansion
{
    Parcel Exit;
    float NozzleAngle;
    float NozzleRadius;
    Vector3[] jetBoundary;

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
}
