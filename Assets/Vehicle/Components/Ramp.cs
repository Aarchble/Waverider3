using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : Processor
{
    // Streams
    Stream Up;

    // Processes
    Deflect[] Surfaces;

    bool Upper;

    public Ramp(List<GameObject> points, bool upper, float width)
    {
        Upper = upper;
        ExternalStream[] streams = new ExternalStream[points.Count - 1];

        for (int pt = 0; pt < points.Count - 1; pt++)
        {
            streams[pt] = new ExternalStream(points[pt].transform.localPosition, points[pt + 1].transform.localPosition, Upper);
        }

        operated = false;
        DeflectMeshes = VehiclePhysics.Instance.Dmesh;

        Width = width;
        Current = streams;

        Surfaces = new Deflect[streams.Length];

        for (int s = 1; s < streams.Length; s++)
        {
            // Surfaces[0] is set in Operate() (since it interfaces with the inStream)
            Surfaces[s] = new(streams[s - 1], streams[s]);
        }
    }

    public override void Operate(Stream inStream)
    {
        Up = inStream;
        Surfaces[0] = new(Up, Current[0]);

        for (int i = 0; i < Surfaces.Length; i++)
        {
            // If first, use Up.Fluid; else use Current[i-1].Fluid (previous)
            Current[i].Fluid = i < 1 ? Surfaces[i].GetParcel(Up.Fluid) : Surfaces[i].GetParcel(Current[i - 1].Fluid);
            PressureForceAndMoment(Current[i].WallPoints(0.5f)[0], Current[i].WallNormals()[0], Current[i].Fluid.P);
            AddDrawnMesh(DeflectMeshes, Surfaces[i].GetDeflectMesh(Current[i]));
        }

        operated = true;
    }

    public override Stream GetOutput(Processor down)
    {
        Stream outStream = Up;

        for (int i = 0; i < Current.Length; i++)
        {
            Vector3 shockNormal = Vector3.Cross(Surfaces[i].featureVertices[^1] - Surfaces[i].featureVertices[0], Upper ? Vector3.back : Vector3.forward).normalized;
            if (Vector3.Dot(down.Current[0].Inlet[0] - Current[i].Inlet[0], shockNormal) > 0f)
            {
                // Included
                outStream = Current[i];
            }
            else
            {
                // Not included
            }
        }

        return outStream;
    }
}
