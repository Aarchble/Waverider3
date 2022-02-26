using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeInputs : MonoBehaviour
{
    // -- GameObjects --
    public GameObject Fuselage;
    public GameObject Nacelle;
    public MeshFilter FuselageStructureFilter;
    public MeshFilter NacelleStructureFilter;

    // -- Inputs --
    public float Length = 5f;
    public float Width = 1f;

    // Make these sliders in GUI
    float Length_Inlet = 0.4f; // inlet length / position
    float Length_Engine = 0.3f; // engine length
    float Length_Nozzle;
    float Length_Lip = 1.3f; // nacelle lip position

    float Angle_Inlet = 10f;
    float Angle_Engine = -10f;
    float Angle_Nozzle = 0f;

    float NozzleRatio = 10f;
    float EngineHeight = 0.025f;

    public Mesh FuselageMesh;
    public Mesh NacelleMesh;

    Vector3 Centroid;

    // -- Outputs --
    // External Flows
    public NearStream InletRamp;
    public NearStream UpperRamp;
    public NearStream NacelleRamp;
    // Internal Flows
    public NearStream Engine;
    public NearStream Nozzle;

    void Start()
    {
        // -- Prepare Inputs --
        // Prepare GameObject Pointers(ish)
        FuselageStructureFilter = Fuselage.GetComponent<MeshFilter>();
        NacelleStructureFilter = Nacelle.GetComponent<MeshFilter>();

        // Length Fractions
        Length_Inlet *= Length;
        Length_Engine *= Length;
        Length_Nozzle = Length - Length_Inlet - Length_Engine;

        // Angles (now relative to eachother)
        Angle_Inlet *= Mathf.Deg2Rad;
        Angle_Engine = Angle_Inlet + Angle_Engine * Mathf.Deg2Rad;
        Angle_Nozzle = Angle_Engine + Angle_Nozzle * Mathf.Deg2Rad;

        // Miscellaneous
        EngineHeight *= Length;


        // -- Initialise Variables --
        FuselageMesh = new Mesh();
        NacelleMesh = new Mesh();

        Vector3[] fuselage = new Vector3[4];
        Vector3[] nacelle = new Vector3[3];
        
        int[] fuselageTriangles;
        int[] nacelleTriangles;


        // -- Building Points --
        // Inlet
        fuselage[0] = Vector3.zero; // Leading Edge
        fuselage[1] = fuselage[0] + new Vector3(Length_Inlet * Mathf.Cos(Angle_Inlet + Mathf.PI), Length_Inlet * Mathf.Sin(Angle_Inlet + Mathf.PI), 0f); // Engine Inlet

        // Engine and Nacelle
        fuselage[2] = fuselage[1] + new Vector3(Length_Engine * Mathf.Cos(Angle_Engine + Mathf.PI), Length_Engine * Mathf.Sin(Angle_Engine + Mathf.PI), 0f); // Engine Outlet
        Vector3 nozzleAxis = new Vector3(Length_Nozzle * Mathf.Cos(Angle_Engine + Mathf.PI), Length_Nozzle * Mathf.Sin(Angle_Engine + Mathf.PI), 0f);
        nacelle[1] = fuselage[2] + new Vector3(EngineHeight * Mathf.Cos(Angle_Engine + 3f * Mathf.PI / 2f), EngineHeight * Mathf.Sin(Angle_Engine + 3f * Mathf.PI / 2f), 0f); // Nacelle Engine Outlet
        nacelle[0] = nacelle[1] + (fuselage[1] - fuselage[2]) * Length_Lip; // Engine Lip (other)

        // Nozzle
        Vector3 nozzleEntryPlane = (nacelle[1] - fuselage[2]);
        Vector3 nozzleExitCentre = fuselage[2] + 0.5f * nozzleEntryPlane + nozzleAxis;
        fuselage[3] = nozzleExitCentre + 0.5f * nozzleEntryPlane.magnitude * NozzleRatio * Vector3.Cross(nozzleAxis, Vector3.forward).normalized;
        nacelle[2] = nozzleExitCentre + 0.5f * nozzleEntryPlane.magnitude * NozzleRatio * Vector3.Cross(nozzleAxis, Vector3.back).normalized; // Internal Nozzle Outlet


        // -- Centroids --
        // Fuselage
        Vector3 fuselageCentroid = new Vector3((fuselage[0].x + fuselage[2].x + fuselage[3].x) / 3f, (fuselage[0].y + fuselage[2].y + fuselage[3].y) / 3f);
        float fuselageArea = TriangleArea(fuselage[0], fuselage[2], fuselage[3]);

        // Nacelle
        Vector3 nacelleCentroid = new Vector3((nacelle[0].x + nacelle[1].x + nacelle[2].x) / 3f, (nacelle[0].y + nacelle[1].y + nacelle[2].y) / 3f);
        float nacelleArea = TriangleArea(nacelle[0], nacelle[2], nacelle[1]);

        // Combined
        Centroid = (fuselageCentroid * fuselageArea + nacelleCentroid * nacelleArea) / (fuselageArea + nacelleArea);

        // Append centroids to vertices
        fuselage = new Vector3[5] { fuselage[0] - Centroid, fuselage[1] - Centroid, fuselage[2] - Centroid, fuselage[3] - Centroid, fuselageCentroid - Centroid };
        nacelle = new Vector3[4] { nacelle[0] - Centroid, nacelle[1] - Centroid, nacelle[2] - Centroid, nacelleCentroid - Centroid };

        // -- Triangles --
        // Fuselage
        fuselageTriangles = TrianglesAboutCentroid(fuselage);

        // Triangles
        nacelleTriangles = TrianglesAboutCentroid(nacelle);


        // -- Fill Meshes --
        FuselageMesh.vertices = fuselage;
        NacelleMesh.vertices = nacelle;

        FuselageMesh.triangles = fuselageTriangles;
        NacelleMesh.triangles = nacelleTriangles;

        // -- Set UVs --
        FuselageMesh.uv3 = new Vector2[5] { fuselage[0] - fuselage[4], fuselage[1] - fuselage[4], fuselage[2] - fuselage[4], fuselage[3] - fuselage[4], Vector2.zero };
        NacelleMesh.uv3 = new Vector2[4] { nacelle[0] - nacelle[3], nacelle[1] - nacelle[3], nacelle[2] - nacelle[3], Vector2.zero };


        // -- Build Streams --
        // External Flows
        InletRamp = new ExternalStream(fuselage[0], fuselage[1], false);
        UpperRamp = new ExternalStream(fuselage[0], fuselage[^2], true);
        NacelleRamp = new ExternalStream(nacelle[0], nacelle[^2], false);
        // Internal Flows
        Engine = new InternalStream(new Vector3[] { fuselage[1], nacelle[0] }, new Vector3[] { fuselage[2], nacelle[1] });
        Nozzle = new InternalStream(new Vector3[] { fuselage[2], nacelle[1] }, new Vector3[] { fuselage[3], nacelle[2] });
    }

    public int[] TrianglesAboutCentroid(Vector3[] vertices)
    {
        int[] triangles = new int[vertices.Length * 3];
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            int j = 3 * i; // triangle iterator
            triangles[j] = vertices.Length - 1; // centre point
            triangles[j + 1] = i;

            if (i + 1 == vertices.Length - 1)
            {
                triangles[j + 2] = 0; // return to origin point
            }
            else
            {
                triangles[j + 2] = i + 1;
            }
        }
        return triangles;
    }

    float TriangleArea(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return 0.5f * Mathf.Abs(p0.x * (p1.y - p2.y) + p1.x * (p2.y - p1.y) + p2.x * (p0.y - p1.y));
    }

}
