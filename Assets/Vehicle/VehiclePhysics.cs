using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePhysics : MonoBehaviour
{
    public Material structMat;
    public Material internalsMat;
    public Material wingMat;
    public Material shockMat;

    Airframe afm;
    Wing wng;
    Rigidbody2D rb;
    FlightControls fcs;

    Vector3 Force;
    float Moment;
    Vector3 Velocity;

    List<Mesh> DeflectMeshes;
    List<Mesh> ExhaustMeshes;

    public float effectLength;
    public float effectThickness;

    public bool paused;
    private bool wasPaused;

    // -- DEBUG PARAMS --
    bool _debug;
    [Range(1000f, 4000f)]
    public float Speed;
    [Range(-90f, 90f)]
    public float AoA;

    // Start is called before the first frame update
    void Start()
    {
        afm = new();
        wng = new(new Vector3(-2.5f, 0.1f), afm.Length / 2f, 0.5f * afm.Width, 3f, 5f);
        rb = GetComponent<Rigidbody2D>();
        fcs = GetComponent<FlightControls>();

        // ! Set up dynamic params
        float height = Mathf.Abs(afm.Nozzle.Outlet[^1].y) + Mathf.Abs(afm.UpperRamp.Outlet[0].y); // -- verified --
        rb.inertia = 1f / 12f * rb.mass * (afm.Length * afm.Length + height * height);

        paused = false;
        wasPaused = true;
        Velocity = new Vector3(Speed, 0f, 0f);

        _debug = true;
    }

    void Update()
    {
        // -- Vehicle --
        // Structure
        Graphics.DrawMesh(afm.InternalsMesh, transform.position, transform.rotation, internalsMat, 0);
        Graphics.DrawMesh(afm.FuselageMesh, transform.position, transform.rotation, structMat, 0);
        Graphics.DrawMesh(afm.NacelleMesh, transform.position, transform.rotation, structMat, 0);
        Graphics.DrawMesh(wng.WingMesh, transform.position, transform.rotation, wingMat, 0);

        // Heating



        // -- Flow Effects --
        foreach (Mesh defl in DeflectMeshes)
        {
            Graphics.DrawMesh(defl, transform.position, transform.rotation, shockMat, 1);
        }

        foreach (Mesh exh in ExhaustMeshes)
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
        if (paused && !wasPaused)
        {
            // Stop
            wasPaused = true;
            Velocity = rb.GetVector(rb.velocity);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }
        else if (!paused && wasPaused)
        {
            // Restart
            wasPaused = false;
            rb.velocity = rb.GetRelativeVector(Velocity);
            rb.gravityScale = 1f;
        }
        else if (!paused && !wasPaused)
        {
            Velocity = rb.GetVector(rb.velocity);
        }
        // Need to use the following equation to keep calculating velocity while paused
        //Vector3 velocity = new(Velocity * Mathf.Cos(-AoA * Mathf.Deg2Rad), Velocity * Mathf.Sin(-AoA * Mathf.Deg2Rad), 0f);

        DeflectMeshes = new();
        ExhaustMeshes = new();

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

        // FUSELAGE
        // Freestream -> InletRamp => DEFLECT
        Deflect InletDeflect = new Deflect(freeStream, afm.InletRamp);
        afm.InletRamp.Fluid = InletDeflect.GetParcel(freeStream.Fluid);
        // ! Pressure Forces
        PressureForceAndMoment(afm.InletRamp.WallPoints(0.5f)[0], afm.InletRamp.WallNormals()[0], afm.InletRamp.Fluid.P);
        AddDrawnMesh(DeflectMeshes, InletDeflect.GetDeflectMesh(afm.InletRamp, effectLength * afm.Length, effectThickness, afm.Engine.Inlet[^1], afm.Engine.Outlet[^1] - afm.Engine.Inlet[^1], _debug));


        // Freestream -> UpperRamp => DEFLECT
        Deflect UpperDeflect = new Deflect(freeStream, afm.UpperRamp);
        afm.UpperRamp.Fluid = UpperDeflect.GetParcel(freeStream.Fluid);
        // ! Pressure Forces
        PressureForceAndMoment(afm.UpperRamp.WallPoints(0.5f)[0], afm.UpperRamp.WallNormals()[0], afm.UpperRamp.Fluid.P); // negative wall vector for upper
        AddDrawnMesh(DeflectMeshes, UpperDeflect.GetDeflectMesh(afm.UpperRamp, effectLength * afm.Length, effectThickness, _debug));

        
        // InletRamp -> Engine => DEFLECT (SHOCK)
        Deflect EngineShock = new(afm.InletRamp, afm.Engine); // Abs forces deflect into shock
        Parcel preEngine = EngineShock.GetParcel(afm.InletRamp.Fluid); // engine fluid pre-combustion
        // ! This pressure doesn't act anywhere significant
        AddDrawnMesh(DeflectMeshes, EngineShock.GetDeflectMesh(afm.Engine, effectThickness, _debug)); // This will always be a shock, hence only adds one Mesh to list. 


        // Engine -> Nozzle => COMBUST
        float minCombustionLength = 0.0f; // not dimensionless
        // ! Check both upper and lower lengths of engine
        if (Vector3.Dot(afm.Engine.Outlet[0] - EngineShock.featureVertices[0], afm.Engine.FlowDir) < minCombustionLength || Vector3.Dot(afm.Engine.Outlet[^1] - EngineShock.featureVertices[1], afm.Engine.FlowDir) < minCombustionLength)
        {
            // ! Engine shock exits the combustion chamber through the nozzle, combustion unlikely
            Debug.Log("Insufficient Combustion!");
            afm.Engine.Fluid = preEngine;
        }
        else
        {
            Combust EngineCombustion = new(290.3f, 1.238f, 0.0291f, 119.95e6f);
            afm.Engine.Fluid = EngineCombustion.GetParcel(preEngine); // update to engine fluid post-combustion
        }
        
        // ! Pressure Forces
        PressureForceAndMoment(afm.Engine.WallPoints(0.5f)[0], afm.Engine.WallNormals()[0], afm.Engine.Fluid.P);
        PressureForceAndMoment(afm.Engine.WallPoints(0.5f)[1], afm.Engine.WallNormals()[1], afm.Engine.Fluid.P);
        // ! Stream Thrust
        float massFlow = afm.Engine.Fluid.Rho * afm.Engine.Fluid.V * (afm.Engine.Inlet[1] - afm.Engine.Inlet[0]).magnitude * afm.Width;
        StreamForceAndMoment(Vector3.Lerp(afm.Engine.Inlet[0], afm.Engine.Inlet[^1], 0.5f), afm.Engine.FlowDir, (afm.Engine.Fluid.V - preEngine.V) * massFlow);


        // Nozzle -> Exhaust => NOZZLE
        AreaChange Nozzle = new(afm.NozzleRatio);
        afm.Nozzle.Fluid = Nozzle.GetParcel(afm.Engine.Fluid);
        // ! Pressure Forces
        PressureForceAndMoment(afm.Nozzle.WallPoints(0.2809f)[0], afm.Nozzle.WallNormals()[0], 0.3167f * afm.Nozzle.Fluid.P);
        PressureForceAndMoment(afm.Nozzle.WallPoints(0.2809f)[1], afm.Nozzle.WallNormals()[1], 0.3167f * afm.Nozzle.Fluid.P);
        // ! Stream Thrust
        StreamForceAndMoment(Vector3.Lerp(afm.Nozzle.Inlet[0], afm.Nozzle.Inlet[^1], 0.5f), afm.Nozzle.FlowDir, (afm.Nozzle.Fluid.V - afm.Engine.Fluid.V) * massFlow);


        // NacelleRamp
        // ! This does not handle expansion fans around the inlet
        if (InletDeflect.GetAngles()[^1] > afm.InletRamp.AngleToPoint(afm.InletRamp.Inlet[0], afm.Engine.Inlet[^1]))
        {
            // InletRamp -> NacelleRamp => DEFLECT
            Deflect NacelleDeflect = new Deflect(afm.InletRamp, afm.NacelleRamp);
            afm.NacelleRamp.Fluid = NacelleDeflect.GetParcel(afm.InletRamp.Fluid);
            AddDrawnMesh(DeflectMeshes, NacelleDeflect.GetDeflectMesh(afm.NacelleRamp, effectLength * afm.Length, effectThickness, InletDeflect.featureVertices[0], InletDeflect.featureVertices[^1] - InletDeflect.featureVertices[0], _debug));
            // NacelleDeflectMesh encapsulated by inletDeflectMesh
        }
        else
        {
            // Freestream -> NacelleRamp => DEFLECT
            Deflect NacelleDeflect = new Deflect(freeStream, afm.NacelleRamp);
            afm.NacelleRamp.Fluid = NacelleDeflect.GetParcel(freeStream.Fluid);
            AddDrawnMesh(DeflectMeshes, NacelleDeflect.GetDeflectMesh(afm.NacelleRamp, effectLength * afm.Length, effectThickness, _debug));
        }
        // ! Pressure Forces
        PressureForceAndMoment(afm.NacelleRamp.WallPoints(0.5f)[0], afm.NacelleRamp.WallNormals()[0], afm.NacelleRamp.Fluid.P);


        // Exhaust -> UpperRamp => EXHAUST
        Exhaust UpperExhaust = new(afm.UpperRamp.Fluid, afm.NozzleExpansionAngle, afm.NozzleExitRadius, upper:true);
        Parcel upperPlume = UpperExhaust.GetParcel(afm.Nozzle.Fluid);
        AddDrawnMesh(ExhaustMeshes, UpperExhaust.GetExhaustMesh(afm.Nozzle, effectThickness));


        // Exhaust -> NacelleRamp => EXHAUST
        Exhaust NacelleExhaust = new(afm.NacelleRamp.Fluid, afm.NozzleExpansionAngle, afm.NozzleExitRadius);
        Parcel NacellePlume = NacelleExhaust.GetParcel(afm.Nozzle.Fluid);
        AddDrawnMesh(ExhaustMeshes, NacelleExhaust.GetExhaustMesh(afm.Nozzle, effectThickness));


        // WING
        Deflect UpperLeadWingDeflect = new Deflect(freeStream, wng.UpperLead);
        Deflect LowerLeadWingDeflect = new Deflect(freeStream, wng.LowerLead);
        Deflect UpperTrailWingDeflect = new Deflect(wng.UpperLead, wng.UpperTrail);
        Deflect LowerTrailWingDeflect = new Deflect(wng.LowerLead, wng.LowerTrail);

        wng.UpperLead.Fluid = UpperLeadWingDeflect.GetParcel(freeStream.Fluid);
        wng.LowerLead.Fluid = LowerLeadWingDeflect.GetParcel(freeStream.Fluid);
        wng.UpperTrail.Fluid = UpperTrailWingDeflect.GetParcel(wng.UpperLead.Fluid);
        wng.LowerTrail.Fluid = LowerTrailWingDeflect.GetParcel(wng.LowerLead.Fluid);

        PressureForceAndMoment(wng.UpperLead.WallPoints(0.5f)[0], wng.UpperLead.WallNormals()[0], wng.UpperLead.Fluid.P);
        PressureForceAndMoment(wng.LowerLead.WallPoints(0.5f)[0], wng.LowerLead.WallNormals()[0], wng.LowerLead.Fluid.P);
        PressureForceAndMoment(wng.UpperTrail.WallPoints(0.5f)[0], wng.UpperTrail.WallNormals()[0], wng.UpperTrail.Fluid.P);
        PressureForceAndMoment(wng.LowerTrail.WallPoints(0.5f)[0], wng.LowerTrail.WallNormals()[0], wng.LowerTrail.Fluid.P);

        AddDrawnMesh(DeflectMeshes, UpperLeadWingDeflect.GetDeflectMesh(wng.UpperLead, effectThickness, _debug));


        // -- Forces --
        if (!paused)
        {
            //Debug.Log("Force: " + Force);
            //Debug.Log("Moment: " + Moment);
            rb.AddRelativeForce(Force);
            rb.AddTorque(Moment);
        }

        //rb.rotation = fcs.PitchInceptor;


        // -- Temperature UVs (TUVs) --
        //afm.FuselageMesh.uv2 = new Vector2[] { TUV(afm.InletRamp.Fluid.T), TUV(preEngine.T), TUV(afm.Engine.Fluid.T), TUV(afm.Nozzle.Fluid.T), Vector2.zero};
        //afm.NacelleMesh.uv2 = new Vector2[] { TUV(preEngine.T), TUV(afm.Engine.Fluid.T), TUV(afm.Nozzle.Fluid.T), Vector2.zero};

        //Debug.Log("Inlet: " + afm.InletRamp.Fluid.P);
        //Debug.Log("Engine: " + afm.Engine.Fluid.P);
        //Debug.Log("Nozzle: " + afm.Nozzle.Fluid.P);
        //Debug.Log("Nacelle: " + afm.NacelleRamp.Fluid.P);
        //Debug.Log("Upper: " + afm.UpperRamp.Fluid.P);
    }

    void Ramp()
    {

    }

    float LeverArm3(Vector3 point, Vector3 force)
    {
        //moments += Mathf.Abs(Force.magnitude) * LeverArm(Surface.SubPoint, Force);
        return Vector3.Dot(point, Vector3.Cross(force, Vector3.forward).normalized); //seems right
    }

    void PressureForceAndMoment(Vector3 point, Vector3 vector, float pressure)
    {
        //point: afm.NacelleRamp.WallPoints(0.5f)[0]
        //force: +/-afm.NacelleRamp.WallVectors()[0]
        Vector3 force = pressure * afm.Width * -vector;
        Force += force;
        Moment += Mathf.Abs(force.magnitude) * LeverArm3(point, force);
        Debug.Log(force + ", " + LeverArm3(point, force));
        if (_debug)
        {
            Debug.DrawRay(rb.GetRelativePoint(point), rb.GetRelativeVector(force.normalized));
        }
    }

    void StreamForceAndMoment(Vector3 point, Vector3 vector, float thrust)
    {
        Vector3 force = thrust * -vector;
        Force += force;
        Moment += Mathf.Abs(force.magnitude) * LeverArm3(point, force);
        if (_debug)
        {
            Debug.DrawRay(rb.GetRelativePoint(point), rb.GetRelativeVector(force.normalized));
        }
    }

    Vector2 TUV(float temperature)
    {
        // Edge Temperature UV
        return new Vector2(temperature, 1f);
    }

    void AddDrawnMesh(List<Mesh> drawnMeshes, Mesh[] meshes)
    {
        if (meshes != null)
        {
            drawnMeshes.AddRange(meshes);
        }
    }

}
