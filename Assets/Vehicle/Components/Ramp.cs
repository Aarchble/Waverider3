using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : Component
{
    // Streams
    //Stream Up;
    //NearStream[] Current;
    //Stream Down;

    // Processes
    Deflect[] Surfaces;

    public Ramp(ExternalStream[] streams, Stream inStream, float width)
    {
        DeflectMeshes = new();
        ExhaustMeshes = new();

        Width = width;
        Up = new Stream[] { inStream };
        Current = streams;

        Surfaces = new Deflect[streams.Length];
        Surfaces[0] = new(inStream, streams[0]);

        for (int s = 1; s < streams.Length; s++)
        {
            Surfaces[s] = new(streams[s - 1], streams[s]);
        }
    }

    public override void Operate()
    {
        for (int i = 0; i < Surfaces.Length; i++)
        {
            // If first, use Up.Fluid; else use Current[i-1].Fluid (previous)
            Current[i].Fluid = i < 1 ? Surfaces[i].GetParcel(Up[0].Fluid) : Surfaces[i].GetParcel(Current[i - 1].Fluid);
            PressureForceAndMoment(Current[i].WallPoints(0.5f)[0], Current[i].WallNormals()[0], Current[i].Fluid.P);
            AddDrawnMesh(DeflectMeshes, Surfaces[i].GetDeflectMesh(Current[i]));
        }
    }
}
