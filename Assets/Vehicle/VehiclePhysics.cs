using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePhysics : MonoBehaviour
{
    public static VehiclePhysics Instance;
    public Material structMat;
    public Material internalsMat;
    public Material wingMat;
    public Material shockMat;

    VehicleStatic veh;
    Wing wng;
    Rigidbody2D rb;
    Fuel ful;

    Vector3 Force;
    float Moment;
    Vector3 Velocity;

    public List<Mesh> Dmesh;
    public List<Mesh> Emesh;

    public float effectLength;
    public float effectThickness;

    // -- DEBUG PARAMS --
    bool _debug;
    [Range(1000f, 4000f)]
    public float Speed;
    [Range(-90f, 90f)]
    public float AoA;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        veh = GetComponent<VehicleStatic>();
        wng = new(new Vector3(-2.5f, 0.0f), veh.Length / 2f, 0.5f * veh.Width, 3f, 0f);
        rb = GetComponent<Rigidbody2D>();
        ful = new(290.3f, 1.238f, 0.0291f, 119.95e6f);

        // ! Set up dynamic params
        rb.inertia = 1f / 12f * rb.mass * (veh.Length * veh.Length + veh.Height * veh.Height);

        Velocity = new Vector3(Speed, 0f, 0f);
        rb.velocity = rb.GetRelativeVector(Velocity);

        _debug = true;
    }

    void Update()
    {
        // -- Vehicle --
        // Structure
        foreach (Mesh structure in veh.GetMeshes())
        {
            Graphics.DrawMesh(structure, transform.position, transform.rotation, structMat, 0);
        }
        //Graphics.DrawMesh(wng.WingMesh, transform.position, transform.rotation, wingMat, 0);


        // -- Flow Effects --
        foreach (Mesh defl in Dmesh)
        {
            Graphics.DrawMesh(defl, transform.position, transform.rotation, shockMat, 1);
        }

        foreach (Mesh exh in Emesh)
        {
            Graphics.DrawMesh(exh, transform.position, transform.rotation, shockMat, 1);
        }

        Vector3 leverArm = (Moment / Force.magnitude) * Vector3.Cross(Force, Vector3.forward).normalized;
        ThickLine drawCoP = new(leverArm, leverArm + Force / 10000f, effectThickness * 2f);
        Graphics.DrawMesh(drawCoP.GetMesh(), transform.position, transform.rotation, shockMat, 1);
        ThickLine drawLvr = new(Vector3.zero, leverArm, effectThickness);
        Graphics.DrawMesh(drawLvr.GetMesh(), transform.position, transform.rotation, shockMat, 1);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log("-- Fixed Update Start --");
        Velocity = rb.GetVector(rb.velocity);

        // Need to use the following equation to keep calculating velocity while paused
        //Vector3 velocity = new(Velocity * Mathf.Cos(-AoA * Mathf.Deg2Rad), Velocity * Mathf.Sin(-AoA * Mathf.Deg2Rad), 0f);

        Dmesh = new();
        Emesh = new();

        // -- RESET DYNAMICS --
        Force = Vector3.zero;
        Moment = 0f;

        // Environment
        FreeStream freeStream = new(-Velocity);
        Parcel Atmosphere = new(287f, 1.4f, 1013f, 220f);
        Atmosphere.SetVelocity(Mathf.Abs(Velocity.magnitude));
        freeStream.Fluid = Atmosphere;
        //Debug.Log("Atmosphere " + freeStream.Fluid.M);
        //Debug.Log("Velocity " + rb.velocity);
        //Debug.Log("Inlet Deflect Angle " + freeStream.AngleTo(afm.InletRamp));

        // -- Flow Streams --
        foreach (Processor[] line in veh.FlowLines)
        {
            for (int c = 0; c < line.Length; c++)
            {
                if (line[c].operated)
                {
                    // Don't recalculate
                }
                else
                {
                    if (c < 1)
                    {
                        line[c].Operate(freeStream);
                    }
                    else
                    {
                        line[c].Operate(line[c - 1].GetOutput(line[c]));
                    }
                    Force += line[c].Force;
                    Moment += line[c].Moment;
                }
            }
        }


        // WING
        //Processor[] wing = new Processor[] { new Ramp(wng.Lower, wng.Width), new Ramp(wng.Upper, wng.Width) };
        //foreach (Processor c in wing)
        //{
        //    c.Operate(freeStream);
        //    Force += c.Force * 2f;
        //    Moment += c.Moment * 2f;
        //}


        // -- Forces --
        Debug.Log("Force: " + Force);
        Debug.Log("Moment: " + Moment);
        rb.AddRelativeForce(Force);
        rb.AddTorque(Moment);

        //rb.rotation = fcs.PitchInceptor;

        //Debug.Log("Inlet: " + afm.InletRamp.Fluid.P);
        //Debug.Log("Engine: " + afm.Engine.Fluid.P);
        //Debug.Log("Nozzle: " + afm.Nozzle.Fluid.P);
        //Debug.Log("Nacelle: " + afm.NacelleRamp.Fluid.P);
        //Debug.Log("Upper: " + afm.UpperRamp.Fluid.P);
    }

}
