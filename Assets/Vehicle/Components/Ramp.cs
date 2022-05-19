using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : Processor
{
    // Streams
    Stream Up;

    // Processes
    Deflect[] Surfaces;

    public Ramp(ExternalStream[] streams, float width, List<Mesh> deflectMeshes = null)
    {
        operated = false;
        DeflectMeshes = deflectMeshes == null ? new() : deflectMeshes;

        Width = width;
        Current = streams;

        Surfaces = new Deflect[streams.Length];

        for (int s = 1; s < streams.Length; s++)
        {
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
            // Down is ExternalStream:
            if (Surfaces[i].GetAngles()[^1] > Current[i].AngleToPoint(Current[i].Inlet[0], down.Current[0].Inlet[0]))
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
