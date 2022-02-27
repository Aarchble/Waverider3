using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combustion : Process
{
    float Rb;
    float Gammab;
    float Cpb;
    float Fst;
    float H;

    public Combustion(float rb, float gammab, float fst, float h)
    {
        Rb = rb;
        Gammab = gammab;
        Fst = fst;
        H = h;
        Cpb = Gammab / (Gammab - 1f) * Rb;
    }

    public override Parcel GetParcel(Parcel i)
    {
        float G = i.Rho * i.V; // Mass Flux
        float K = i.Rho * i.V * i.V + i.P; // Momentum Flux
        float ht = i.Cp * i.T + 0.5f * i.V * i.V; // Stagnation Enthalpy

        float V3b = Gammab / (Gammab + 1f) * (K / G + Mathf.Sqrt(K / G * K / G - 2f * ht * ((Gammab * Gammab - 1f) / (Gammab * Gammab))));
        float rhob = G / V3b;
        float T3b = (ht - (V3b * V3b) / 2f) / Cpb;
        float p3b = rhob * Rb * T3b;
        float M3b = V3b / Mathf.Sqrt(Gammab * Rb * T3b);
        float Tt3b = T3b * (1f + (Gammab - 1f) / 2f * M3b * M3b);

        float taub = (Fst * H) / (Cpb * Tt3b) + 1f;
        float X = (taub * M3b * M3b * (1f + (Gammab - 1f) / 2f * M3b * M3b)) / (Mathf.Pow(1f + Gammab * M3b * M3b, 2f));

        // State 4
        float M2 = Mathf.Sqrt((2f * X) / (1f - 2f * X * Gammab - Mathf.Sqrt(1f - 2f * X * (Gammab + 1f))));
        float P2 = p3b * (1f + Gammab * M3b * M3b) / (1f + Gammab * M2 * M2);
        float T2 = (taub * Tt3b) / (1f + (Gammab - 1f) / 2f * M2 * M2);

        // -- verified --

        if (float.IsNaN(M2) || float.IsNaN(P2) || float.IsNaN(T2))
        {
            //Debug.Log("Flameout!");
            return i;
        }
        else
        {
            Parcel f = new(Rb, Gammab, P2, T2);
            f.SetMach(M2);
            return f;
        }
    }
}
