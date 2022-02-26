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

    public Mesh GetNozzleMesh(NearStream nearStream, bool _debug = false)
    {
        if (nearStream is InternalStream)
        {
            // InternalStream
            Mesh mesh = new();
            mesh.vertices = new Vector3[5] { nearStream.Inlet[0], nearStream.Inlet[1], nearStream.Outlet[1], nearStream.Outlet[0] + (nearStream.Outlet[1] - nearStream.Outlet[0]) * 0.5f, nearStream.Outlet[0]  };
            mesh.uv = new Vector2[5] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), new Vector2(1f, 0f) };
            mesh.triangles = new int[9] { 0, 1, 3, 1, 2, 3, 0, 3, 4 };

            if (_debug)
            {
                Debug.DrawLine(mesh.vertices[0], mesh.vertices[1], Color.yellow);
                Debug.DrawLine(mesh.vertices[1], mesh.vertices[2], Color.yellow);
                Debug.DrawLine(mesh.vertices[2], mesh.vertices[3], Color.yellow);
                Debug.DrawLine(mesh.vertices[3], mesh.vertices[0], Color.yellow);
            }

            return mesh;
        }
        else
        {
            // ExternalStream
            return null;
        }
    }

    //public Mesh GetNozzleMesh(NearStream internalStream, NearStream externalStream, bool _debug = false)
    //{
    //    if (internalStream is InternalStream && externalStream is ExternalStream)
    //    {
    //        // Assumes that internalStream.Outlet[0] is coincident with externalStream.Inlet[0]
    //        Mesh mesh = new Mesh();
    //        Vector3 wallVector = externalStream.WallVectors()[0];
    //        mesh.vertices = new Vector3[4] { internalStream.Outlet[0], internalStream.Outlet[1], internalStream.Outlet[1] + new Vector3(wallVector.x, -wallVector.y), externalStream.Outlet[0] };
    //        mesh.uv = new Vector2[4] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f) };
    //        mesh.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };

    //        if (_debug)
    //        {
    //            Debug.DrawLine(mesh.vertices[0], mesh.vertices[1], Color.yellow);
    //            Debug.DrawLine(mesh.vertices[1], mesh.vertices[2], Color.yellow);
    //            Debug.DrawLine(mesh.vertices[2], mesh.vertices[3], Color.yellow);
    //            Debug.DrawLine(mesh.vertices[3], mesh.vertices[0], Color.yellow);
    //        }

    //        return mesh;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
}
