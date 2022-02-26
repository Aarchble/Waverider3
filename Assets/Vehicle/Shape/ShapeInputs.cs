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
    public LineRenderer FuselageTemperatureRenderer;
    public LineRenderer NacelleTemperatureRenderer;

    // -- Inputs --
    public float Length = 5f;
    public float Width = 1f;

    // Make these sliders in GUI
    float Length_Inlet = 0.4f;
    float Length_Engine = 0.3f;
    float Length_InternalNozzle = 0.1f;
    float Length_LipExtension = 1.3f;

    float Angle_Inlet = 10f;
    float Angle_Engine = -10f;
    float Angle_InternalNozzle = -20f;
    float Angle_InternalNozzlePose = 0f;
    //float Angle_Nacelle = 5f;
    float Angle_Upper = 3f;

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
    public NearStream InternalNozzle;
    public NearStream ExternalNozzle;

    void Start()
    {
        // -- Prepare Inputs --
        // Prepare GameObject Pointers(ish)
        FuselageStructureFilter = Fuselage.GetComponent<MeshFilter>();
        NacelleStructureFilter = Nacelle.GetComponent<MeshFilter>();
        FuselageTemperatureRenderer = Fuselage.GetComponent<LineRenderer>();
        NacelleTemperatureRenderer = Nacelle.GetComponent<LineRenderer>();

        // Length Fractions
        Length_Inlet *= Length;
        Length_Engine *= Length;
        Length_InternalNozzle *= Length;
        // Do not multiply Length_LipExtension by Length

        // Angles (now relative to eachother)
        Angle_Inlet *= Mathf.Deg2Rad;
        Angle_Engine = Angle_Inlet + Angle_Engine * Mathf.Deg2Rad;
        Angle_InternalNozzle = Angle_Engine + Angle_InternalNozzle * Mathf.Deg2Rad;
        Angle_InternalNozzlePose *= Mathf.Deg2Rad;
        //Angle_Nacelle = Angle_Engine + Angle_Nacelle * Mathf.Deg2Rad;
        Angle_Upper *= Mathf.Deg2Rad;

        // Miscellaneous
        EngineHeight *= Length;


        // -- Initialise Variables --
        FuselageMesh = new Mesh();
        NacelleMesh = new Mesh();

        Vector3[] fuselage = new Vector3[5];
        Vector3[] nacelle = new Vector3[3];
        
        int[] fuselageTriangles;
        int[] nacelleTriangles;


        // -- Building Points --
        // Fuselage
        fuselage[0] = Vector3.zero; // Leading Edge
        fuselage[1] = fuselage[0] + new Vector3(Length_Inlet * Mathf.Cos(Angle_Inlet + Mathf.PI), Length_Inlet * Mathf.Sin(Angle_Inlet + Mathf.PI), 0f); // Engine Inlet
        fuselage[2] = fuselage[1] + new Vector3(Length_Engine * Mathf.Cos(Angle_Engine + Mathf.PI), Length_Engine * Mathf.Sin(Angle_Engine + Mathf.PI), 0f); // Engine Outlet
        fuselage[3] = fuselage[2] + new Vector3(Length_InternalNozzle * Mathf.Cos(Angle_InternalNozzlePose + Angle_InternalNozzle + Mathf.PI), Length_InternalNozzle * Mathf.Sin(Angle_InternalNozzlePose + Angle_InternalNozzle + Mathf.PI), 0f); // Internal Nozzle Outlet
        fuselage[4] = fuselage[0] + new Vector3(Length * Mathf.Cos(-Angle_Upper + Mathf.PI), Length * Mathf.Sin(-Angle_Upper + Mathf.PI), 0f); // Trailing Edge

        // Nacelle
        nacelle[1] = fuselage[2] + new Vector3(EngineHeight * Mathf.Cos(Angle_Engine + 3f * Mathf.PI / 2f), EngineHeight * Mathf.Sin(Angle_Engine + 3f * Mathf.PI / 2f), 0f); // Nacelle Engine Outlet
        nacelle[0] = nacelle[1] + (fuselage[1] - fuselage[2]) * Length_LipExtension; // Engine Lip (other)
        nacelle[2] = nacelle[1] + new Vector3(Length_InternalNozzle * Mathf.Cos(Angle_InternalNozzlePose - Angle_InternalNozzle + Mathf.PI), Length_InternalNozzle * Mathf.Sin(Angle_InternalNozzlePose - Angle_InternalNozzle + Mathf.PI), 0f); // Internal Nozzle Outlet
        //nacelle[2] = Vector3.zero; // Nacelle Tailing Edge (do not update this point for moving lip extension)
        //float nozzleExitPlaneGradient = Mathf.Sin(Angle_Engine + 3f * Mathf.PI / 2f) / Mathf.Cos(Angle_Engine + 3f * Mathf.PI / 2f);
        //float nacelleLowerPlaneGradient = Mathf.Sin(Angle_Nacelle + Mathf.PI) / Mathf.Cos(Angle_Nacelle + Mathf.PI);
        //nacelle[2].x = (fuselage[3].x * nozzleExitPlaneGradient - nacelle[0].x * nacelleLowerPlaneGradient - fuselage[3].y + nacelle[0].y) / (nozzleExitPlaneGradient - nacelleLowerPlaneGradient);
        //nacelle[2].y = nozzleExitPlaneGradient * (nacelle[2].x - fuselage[3].x) + fuselage[3].y;


        // -- Centroids --
        // Fuselage
        Vector3 fuselageCentroid = new Vector3((fuselage[0].x + fuselage[2].x + fuselage[4].x) / 3f, (fuselage[0].y + fuselage[2].y + fuselage[4].y) / 3f);
        float fuselageArea = TriangleArea(fuselage[0], fuselage[2], fuselage[4]);

        // Nacelle
        Vector3 nacelleCentroid = new Vector3((nacelle[0].x + nacelle[1].x + nacelle[2].x) / 3f, (nacelle[0].y + nacelle[1].y + nacelle[2].y) / 3f);
        float nacelleArea = TriangleArea(nacelle[0], nacelle[2], nacelle[1]);

        // Combined
        Centroid = (fuselageCentroid * fuselageArea + nacelleCentroid * nacelleArea) / (fuselageArea + nacelleArea);

        // Append centroids to vertices
        fuselage = new Vector3[6] { fuselage[0] - Centroid, fuselage[1] - Centroid, fuselage[2] - Centroid, fuselage[3] - Centroid, fuselage[4] - Centroid, fuselageCentroid - Centroid };
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
        FuselageMesh.uv3 = new Vector2[6] { fuselage[0] - fuselage[5], fuselage[1] - fuselage[5], fuselage[2] - fuselage[5], fuselage[3] - fuselage[5], fuselage[4] - fuselage[5], Vector2.zero };
        NacelleMesh.uv3 = new Vector2[4] { nacelle[0] - nacelle[3], nacelle[1] - nacelle[3], nacelle[2] - nacelle[3], Vector2.zero };


        // -- Build Streams --
        // External Flows
        InletRamp = new ExternalStream(fuselage[0], fuselage[1], false);
        UpperRamp = new ExternalStream(fuselage[0], fuselage[^2], true);
        NacelleRamp = new ExternalStream(nacelle[0], nacelle[^2], false);
        // Internal Flows
        Engine = new InternalStream(new Vector3[] { fuselage[1], nacelle[0] }, new Vector3[] { fuselage[2], nacelle[1] });
        InternalNozzle = new InternalStream(new Vector3[] { fuselage[2], nacelle[1] }, new Vector3[] { fuselage[3], nacelle[2] });
        ExternalNozzle = new ExternalStream(fuselage[3], fuselage[4], false, InternalNozzle.FlowDir);
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
