using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Expansion : Process
{
    public float PrandtlMeyerAngle(float gamma, float M)
    {
        float beta = Mathf.Sqrt(M * M - 1f);
        float gratio = (gamma + 1f) / (gamma - 1f);

        return Mathf.Sqrt(gratio) * Mathf.Atan(Mathf.Sqrt(beta / gratio)) - Mathf.Atan(beta);
    }

    public float SonicArea(float gamma, float M)
    {
        float gratio = (gamma + 1f) / (gamma - 1f);

        return 1f / M * Mathf.Pow(2f / (gamma + 1f), 0.5f * gratio) * Mathf.Pow(1f + M * M * (gamma - 1f) / 2f, 0.5f * gratio);
    }

    public float SonicAreaMach(float gamma, float Aratio)
    {
        // Method Ref: Dennis Yoder, Nasa b4wind_guide
        float P = 2f / (gamma + 1f);
        float Q = 1f - P;
        float R = Mathf.Pow(Aratio, 2f * Q / P);

        float a = Mathf.Pow(Q, 1f / P);
        float r = (R - 1f) / (2f * a);
        float X = 1f / ((1f + r) + Mathf.Sqrt(r * (r + 2)));

        float f = Mathf.Pow(P * X + Q, 1f / P) - R * X;
        float df = Mathf.Pow(P * X + Q, 1f / P - 1f) - R;
        float ddf = (1f - P) * Mathf.Pow(P * X + Q, 1f / P - 2f);

        return 1f / Mathf.Sqrt(X - 2f * f / (df - Mathf.Sqrt(df * df - 2f * f * ddf)));
    }
}
