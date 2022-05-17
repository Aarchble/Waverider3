using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Component
{
    // Streams
    //public Stream[] Up;
    public NearStream[] Current;
    //public Stream[] Down;

    // Properties
    public Vector3 Force;
    public float Moment;
    public float Width;
    public List<Mesh> DeflectMeshes;
    public List<Mesh> ExhaustMeshes;

    public abstract void Operate(Stream inStream);
    public abstract Stream GetOutput(Component down);
    //public abstract void GetInput();

    float LeverArm3(Vector3 point, Vector3 force)
    {
        //moments += Mathf.Abs(Force.magnitude) * LeverArm(Surface.SubPoint, Force);
        return Vector3.Dot(point, Vector3.Cross(force, Vector3.forward).normalized); //seems right
    }

    public void PressureForceAndMoment(Vector3 point, Vector3 vector, float pressure)
    {
        //point: afm.NacelleRamp.WallPoints(0.5f)[0]
        //force: +/-afm.NacelleRamp.WallVectors()[0]
        Vector3 force = pressure * Width * -vector;
        Force += force;
        Moment += Mathf.Abs(force.magnitude) * LeverArm3(point, force);
    }

    public void StreamForceAndMoment(Vector3 point, Vector3 vector, float thrust)
    {
        Vector3 force = thrust * -vector;
        Force += force;
        Moment += Mathf.Abs(force.magnitude) * LeverArm3(point, force);
    }

    public void AddDrawnMesh(List<Mesh> drawnMeshes, Mesh[] meshes)
    {
        if (meshes != null)
        {
            drawnMeshes.AddRange(meshes);
        }
    }
}
