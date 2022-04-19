using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel
{
    public float Rb;
    public float Gammab;
    public float Cpb;
    public float Fst;
    public float H;

    public Fuel(float rb, float gammab, float fst, float h)
    {
        Rb = rb;
        Gammab = gammab;
        Fst = fst;
        H = h;
        Cpb = Gammab / (Gammab - 1f) * Rb;
    }
}
