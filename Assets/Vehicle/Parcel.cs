using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parcel
{
    // Parcel of fluid, contains thermodynamic properties
    public float R;
    public float Gamma;

    public float P; //pressure
    public float T; //temperature
    public float Rho; //density
    public float Mu; //dynamic viscosity

    public float M; //mach number
    public float A; //local speed of sound
    public float V; //velocity

    public float Cp; //heat capacity

    public Parcel(float r, float gamma, float p, float t)
    {
        R = r;
        Gamma = gamma;

        P = p;
        T = t;
        Rho = P / (R * T);
        Mu = 1.789e-5f * Mathf.Pow(T / 288f, 3f / 2f) * ((288f + 110f) / (T + 110f));

        A = Mathf.Sqrt(Gamma * R * T);

        Cp = Gamma / (Gamma - 1f) * R;
    }

    public void SetMach(float m)
    {
        M = m;
        V = M * A;
    }

    public void SetVelocity(float v)
    {
        V = v;
        M = V / A;
    }
}
