using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpansionFan : Process
{
    float Theta;
    public float[] FanAngles;

    public ExpansionFan(float theta)
    {
        Theta = theta;
        FanAngles = new float[2];
    }

    public override Parcel GetParcel(Parcel i)
    {
        float m2 = i.M; // initial guess
        for (int j = 0; j < 4; j++)
        {
            float dnudM = (Mathf.Sqrt(m2 * m2 - 1f)) / (m2 * (1f + (i.Gamma - 1f) / 2f * m2 * m2));
            float m2new = (Theta + PrandtlMeyerAngle(i.M, i.Gamma) - PrandtlMeyerAngle(m2, i.Gamma)) / dnudM + m2;
            m2 = m2new;
        }

        float M2 = m2;
        float Tratio = (1f + (i.Gamma - 1f) / 2f * i.M * i.M) / (1f + (i.Gamma - 1f) / 2f * M2 * M2);
        float Pratio = Mathf.Pow(Tratio, i.Gamma / (i.Gamma - 1f));

        FanAngles[0] = Mathf.Asin(1f / i.M); //upstream wave (relative to pre-deflected flow direction)
        FanAngles[1] = Mathf.Asin(1f / M2);  //downstream wave (relative to pre-deflected flow direction)

        // -- verified --
        Parcel f = new(i.R, i.Gamma, Pratio * i.P, Tratio * i.T);
        f.SetMach(M2);

        return f;
    }

    private float PrandtlMeyerAngle(float M, float gamma)
    {
        // returns radians
        float omega = Mathf.Sqrt((gamma + 1f) / (gamma - 1f)) * Mathf.Atan(Mathf.Sqrt((M * M - 1f) / ((gamma + 1f) / (gamma - 1f)))) - Mathf.Atan(Mathf.Sqrt(M * M - 1f));
        float max = Mathf.PI / 2f * (Mathf.Sqrt((gamma + 1f) / (gamma - 1f)) - 1f);
        if (omega > max)
        {
            return max;
        }
        else
        {
            return omega;
        }
    }
}
