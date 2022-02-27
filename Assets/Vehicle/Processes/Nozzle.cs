using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nozzle : Process
{
    float Aratio;

    public Nozzle(float aratio)
    {
        Aratio = aratio;
    }

    public override Parcel GetParcel(Parcel i)
    {
        float gratio = (i.Gamma + 1f) / (i.Gamma - 1f);
        float aSonic1 = 1f / i.M * Mathf.Pow(2f / (i.Gamma + 1f), 0.5f * gratio) * Mathf.Pow(1f + i.M * i.M * (i.Gamma - 1f) / 2f, 0.5f * gratio);
        float aSonic2 = Aratio * aSonic1;

        // Method Ref: Dennis Yoder, Nasa b4wind_guide
        float P = 2f / (i.Gamma + 1f);
        float Q = 1f - P;
        float R = Mathf.Pow(aSonic2, 2f * Q / P);

        float a = Mathf.Pow(Q, 1f / P);
        float r = (R - 1f) / (2f * a);
        float X = 1f / ((1f + r) + Mathf.Sqrt(r * (r + 2)));

        float f = Mathf.Pow(P * X + Q, 1f / P) - R * X;
        float df = Mathf.Pow(P * X + Q, 1f / P - 1f) - R;
        float ddf = (1f - P) * Mathf.Pow(P * X + Q, 1f / P - 2f);

        float M2 = 1f / Mathf.Sqrt(X - 2f * f / (df - Mathf.Sqrt(df * df - 2f * f * ddf)));
        float Tratio = (1f + (i.Gamma - 1f) / 2f * i.M * i.M) / (1f + (i.Gamma - 1f) / 2f * M2 * M2);
        float Pratio = Mathf.Pow(Tratio, i.Gamma / (i.Gamma - 1f));

        // -- !verified --
        Parcel final = new(i.R, i.Gamma, 1f / Pratio * i.P, Tratio * i.T);
        final.SetMach(M2);

        return final;
    }
}
