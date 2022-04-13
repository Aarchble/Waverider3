using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : Process
{
    float Theta;
    public float ShockAngle;

    public Shock(float theta)
    {
        Theta = theta;
    }

    public override Parcel GetParcel(Parcel i) // i: initial
    {
        // Explicit Solution (From Anderson, pp. 142,143)
        float lambda = Mathf.Sqrt(Mathf.Pow((i.M * i.M - 1f), 2f) - 3f * (1f + (i.Gamma - 1f) / 2f * i.M * i.M) * (1f + (i.Gamma + 1f) / 2f * i.M * i.M) * Mathf.Pow(Mathf.Tan(Theta), 2f));
        float chi = (Mathf.Pow((i.M * i.M - 1f), 3f) - 9f * (1f + (i.Gamma - 1f) / 2f * i.M * i.M) * (1f + (i.Gamma - 1f) / 2f * i.M * i.M + (i.Gamma + 1f) / 4f * i.M * i.M * i.M * i.M) * Mathf.Pow(Mathf.Tan(Theta), 2f)) / (lambda * lambda * lambda);

        float beta;
        if (chi > 0)
        {
            beta = Mathf.Atan(((i.M * i.M - 1f) + 2f * lambda * Mathf.Cos((4f * Mathf.PI + Mathf.Acos(chi)) / 3f)) / (3f * (1f + (i.Gamma - 1f) / 2f * i.M * i.M) * Mathf.Tan(Theta)));
            ShockAngle = beta - Theta;
        }
        else
        {
            // Normal Shock
            beta = Mathf.PI / 2f;
            ShockAngle = Mathf.PI / 2f;
        }

        float M1n = i.M * Mathf.Sin(beta);
        float M2n = Mathf.Pow((1f + (i.Gamma - 1f) / 2f * M1n * M1n) / (i.Gamma * M1n * M1n - (i.Gamma - 1f) / 2f), 0.5f);
        float M2 = M2n / Mathf.Sin(ShockAngle);

        float Pratio = (2f * i.Gamma * M1n * M1n - i.Gamma + 1f) / (i.Gamma + 1f);
        float Tratio = ((2f * i.Gamma * M1n * M1n - i.Gamma + 1f) * ((i.Gamma - 1f) * M1n * M1n + 2f)) / ((i.Gamma + 1f) * (i.Gamma + 1f) * M1n * M1n);

        // -- verified --
        Parcel f = new(i.R, i.Gamma, Pratio * i.P, Tratio * i.T); // f: final
        f.SetMach(M2);

        return f;
    }
}
