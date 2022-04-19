using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaChange : Expand
{
    float Aratio;

    public AreaChange(float aratio)
    {
        Aratio = aratio;
    }

    public override Parcel GetParcel(Parcel i)
    {
        // Apply Area Ratio
        float gratio = (i.Gamma + 1f) / (i.Gamma - 1f);
        float aSonic1 = 1f / i.M * Mathf.Pow(2f / (i.Gamma + 1f), 0.5f * gratio) * Mathf.Pow(1f + i.M * i.M * (i.Gamma - 1f) / 2f, 0.5f * gratio);
        float aSonic2 = Aratio * aSonic1;

        // Get Downstream Flow Properties
        float M2 = SonicAreaMach(i.Gamma, aSonic2);
        float Tratio = (1f + (i.Gamma - 1f) / 2f * i.M * i.M) / (1f + (i.Gamma - 1f) / 2f * M2 * M2);
        float Pratio = Mathf.Pow(Tratio, i.Gamma / (i.Gamma - 1f));

        // -- !verified --
        Parcel final = new(i.R, i.Gamma, Pratio * i.P, Tratio * i.T);
        final.SetMach(M2);

        return final;
    }
}
