using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePhysics : MonoBehaviour
{
    public Material structMat;
    public Material heatMat;
    public Material shockMat;
    //public Material expansionMat;
    public Material combustionMat;
    public Material exhaustMat;

    ShapeInputs shp;
    Rigidbody2D rb;
    FlightControls fcs;

    Vector3 Force;
    float Moment;
    Vector3 Velocity;

    Mesh InletDeflectMesh;
    Mesh UpperDeflectMesh;
    Mesh EngineShockMesh;
    Mesh EngineCombustionMesh;
    Mesh InternalNozzleMesh;
    Mesh ExternalNozzleMesh;
    Mesh NacelleDeflectMesh;
    Mesh UpperTrailDeflectMesh;
    Mesh LowerTrailDeflectMesh;

    //Material inletMat;
    //Material upperMat;
    //Material preEngineMat;
    //Material engineMat;
    //Material internalNozzleMat;
    //Material externalNozzleMat;
    //Material nacelleMat;
    //Material upperTrailMat;
    //Material lowerTrailMat;

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
        shp = GetComponent<ShapeInputs>();
        rb = GetComponent<Rigidbody2D>();
        fcs = GetComponent<FlightControls>();

        // ! Set up dynamic params
        float height = Mathf.Abs(shp.Nozzle.Outlet[^1].y) + Mathf.Abs(shp.UpperRamp.Outlet[0].y); // -- verified --
        rb.inertia = 1f / 12f * rb.mass * (shp.Length * shp.Length + height * height);

        //inletMat = new(shockMat);
        //upperMat = new(shockMat);
        //preEngineMat = new(shockMat);
        //engineMat = new(exhaustMat);
        //internalNozzleMat = new(exhaustMat);
        //externalNozzleMat = new(exhaustMat);
        //nacelleMat = new(shockMat);
        //upperTrailMat = new(shockMat);
        //lowerTrailMat = new(shockMat);

        paused = false;
        wasPaused = true;
        Velocity = new Vector3(Speed, 0f, 0f);

        _debug = true;
    }

    void Update()
    {
        // -- Vehicle --
        // Structure
        shp.FuselageStructureFilter.mesh = shp.FuselageMesh;
        shp.NacelleStructureFilter.mesh = shp.NacelleMesh;
        // Heating
        //shp.FuselageTemperatureRenderer.positionCount = 5;
        //shp.FuselageTemperatureRenderer.startWidth = shp.Length / 100f;
        //shp.FuselageTemperatureRenderer.endWidth = shp.Length / 100f;
        //shp.FuselageTemperatureRenderer.SetPositions(new Vector3[] { shp.FuselageMesh.vertices[0], shp.FuselageMesh.vertices[1], shp.FuselageMesh.vertices[2], shp.FuselageMesh.vertices[3], shp.FuselageMesh.vertices[4] });

        //shp.NacelleTemperatureRenderer.positionCount = 3;
        //shp.NacelleTemperatureRenderer.startWidth = shp.Length / 100f;
        //shp.NacelleTemperatureRenderer.endWidth = shp.Length / 100f;
        //shp.NacelleTemperatureRenderer.SetPositions(new Vector3[] { shp.NacelleMesh.vertices[0], shp.NacelleMesh.vertices[1], shp.NacelleMesh.vertices[2] });


        // -- Flow Effects --
        //int flowVisLayer = 1;
        //shockMat.SetVector("_velocityVector", rb.GetRelativeVector(Velocity).normalized);

        // Draw InletRamp effects
        // ! Shock / Expansion Fan
        //Graphics.DrawMesh(InletDeflectMesh, transform.position, transform.rotation, shockMat, flowVisLayer); // InletRamp [Shock / Expansion Fan]


        // Draw UpperRamp effects
        // ! Shock / Expansion Fan
        //Graphics.DrawMesh(UpperDeflectMesh, transform.position, transform.rotation, shockMat, flowVisLayer); // UpperRamp [Shock / Expansion Fan]


        // Draw Engine effects
        // ! PreShock
        //Graphics.DrawMesh(EngineShockMesh, transform.position, transform.rotation, shockMat, flowVisLayer); // Engine PreShock [Shock]
        // ! Combustion
        //Graphics.DrawMesh(EngineCombustionMesh, transform.position, transform.rotation, exhaustMat, flowVisLayer); // Engine Combustion [Combustion]


        // Draw InternalNozzle effects
        // ! Exhaust
        //Graphics.DrawMesh(InternalNozzleMesh, transform.position, transform.rotation, exhaustMat, flowVisLayer); // Internal Nozzle [Exhaust]


        // Draw Nacelle effects
        // ! Shock / Expansion Fan
        //Graphics.DrawMesh(NacelleDeflectMesh, transform.position, transform.rotation, shockMat, flowVisLayer); // Nacelle Ramp [Shock / Expansion Fan]


        // Draw Trailing effects
        // ! Shock / Expansion Fan
        //Graphics.DrawMesh(UpperTrailDeflectMesh, transform.position, transform.rotation, shockMat, flowVisLayer); // Upper Trailing [Shock / Expansion Fan]
        //Graphics.DrawMesh(LowerTrailDeflectMesh, transform.position, transform.rotation, shockMat, flowVisLayer); // Lower Trailing [Shock / Expansion Fan]


        // Combined Internal Flow effects
        //CombineInstance[] internalFlowCombine = new CombineInstance[2];
        //internalFlowCombine[0].mesh = EngineCombustionMesh;
        //internalFlowCombine[0].transform = transform.localToWorldMatrix;
        //internalFlowCombine[1].mesh = InternalNozzleMesh;
        //internalFlowCombine[1].transform = transform.localToWorldMatrix;
        //Mesh internalFlowMesh = new Mesh();
        //internalFlowMesh.CombineMeshes(internalFlowCombine);
        //Graphics.DrawMesh(internalFlowMesh, transform.position, transform.rotation, internalNozzleMat, flowVisLayer);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

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
        //Debug.Log("Inlet Deflect Angle " + freeStream.AngleTo(shp.InletRamp));

        // -- Flow Streams --
        // Freestream -> InletRamp => DEFLECT
        Deflect InletDeflect = new Deflect(freeStream.AngleTo(shp.InletRamp));
        shp.InletRamp.Fluid = InletDeflect.GetParcel(freeStream.Fluid);
        // ! Pressure Forces
        PresureForceAndMoment(shp.InletRamp.WallPoints(0.5f)[0], shp.InletRamp.WallNormals()[0], shp.InletRamp.Fluid.P);
        InletDeflectMesh = InletDeflect.GetDeflectMesh(shp.InletRamp, effectLength * shp.Length, effectThickness, shp.Engine.Inlet[^1], shp.Engine.Outlet[^1] - shp.Engine.Inlet[^1], _debug);


        // Freestream -> UpperRamp => DEFLECT
        Deflect UpperDeflect = new Deflect(freeStream.AngleTo(shp.UpperRamp), upper:true);
        shp.UpperRamp.Fluid = UpperDeflect.GetParcel(freeStream.Fluid);
        // ! Pressure Forces
        PresureForceAndMoment(shp.UpperRamp.WallPoints(0.5f)[0], shp.UpperRamp.WallNormals()[0], shp.UpperRamp.Fluid.P); // negative wall vector for upper
        UpperDeflectMesh = UpperDeflect.GetDeflectMesh(shp.UpperRamp, effectLength * shp.Length, effectThickness, _debug);

        
        // InletRamp -> Engine => DEFLECT (SHOCK)
        Deflect EngineShock = new(shp.InletRamp.AngleTo(shp.Engine), internalFlow:true); // Abs forces deflect into shock
        Parcel preEngine = EngineShock.GetParcel(shp.InletRamp.Fluid); // engine fluid pre-combustion
        // ! This pressure doesn't act anywhere significant
        EngineShockMesh = EngineShock.GetDeflectMesh(shp.Engine, effectThickness, _debug);


        // Engine -> Nozzle => COMBUST
        float minCombustionLength = 0.0f; // not dimensionless
        // ! Check both upper and lower lengths of engine
        if (Vector3.Dot(shp.Engine.Outlet[0] - EngineShockMesh.vertices[0], shp.Engine.FlowDir) < minCombustionLength || Vector3.Dot(shp.Engine.Outlet[^1] - EngineShockMesh.vertices[1], shp.Engine.FlowDir) < minCombustionLength)
        {
            // ! Engine shock exits the combustion chamber through the nozzle, combustion unlikely
            Debug.Log("Insufficient Combustion!");
            shp.Engine.Fluid = preEngine;
        }
        else
        {
            Combustion EngineCombustion = new(290.3f, 1.238f, 0.0291f, 119.95e6f);
            shp.Engine.Fluid = EngineCombustion.GetParcel(preEngine); // update to engine fluid post-combustion
        }
        
        // ! Pressure Forces
        PresureForceAndMoment(shp.Engine.WallPoints(0.5f)[0], shp.Engine.WallNormals()[0], shp.Engine.Fluid.P);
        PresureForceAndMoment(shp.Engine.WallPoints(0.5f)[1], shp.Engine.WallNormals()[1], shp.Engine.Fluid.P);
        // ! Stream Thrust
        float massFlow = shp.Engine.Fluid.Rho * shp.Engine.Fluid.V * (shp.Engine.Inlet[1] - shp.Engine.Inlet[0]).magnitude * shp.Width;
        StreamForceAndMoment(Vector3.Lerp(shp.Engine.Inlet[0], shp.Engine.Inlet[^1], 0.5f), shp.Engine.FlowDir, (shp.Engine.Fluid.V - preEngine.V) * massFlow);


        // Nozzle -> Exhaust => NOZZLE
        Nozzle Nozzle = new(shp.NozzleRatio);
        shp.Nozzle.Fluid = Nozzle.GetParcel(shp.Engine.Fluid);
        // ! Pressure Forces
        PresureForceAndMoment(shp.Nozzle.WallPoints(0.2809f)[0], shp.Nozzle.WallNormals()[0], 0.3167f * shp.Nozzle.Fluid.P);
        PresureForceAndMoment(shp.Nozzle.WallPoints(0.2809f)[1], shp.Nozzle.WallNormals()[1], 0.3167f * shp.Nozzle.Fluid.P);
        // ! Stream Thrust
        StreamForceAndMoment(Vector3.Lerp(shp.Nozzle.Inlet[0], shp.Nozzle.Inlet[^1], 0.5f), shp.Nozzle.FlowDir, (shp.Nozzle.Fluid.V - shp.Engine.Fluid.V) * massFlow);


        // NacelleRamp
        // ! This does not handle expansion fans around the inlet
        if (InletDeflect.GetAngles()[^1] > shp.InletRamp.AngleToPoint(shp.InletRamp.Inlet[0], shp.Engine.Inlet[^1]))
        {
            // InletRamp -> NacelleRamp => DEFLECT
            Deflect NacelleDeflect = new Deflect(shp.InletRamp.AngleTo(shp.NacelleRamp));
            shp.NacelleRamp.Fluid = NacelleDeflect.GetParcel(shp.InletRamp.Fluid);
            NacelleDeflectMesh = NacelleDeflect.GetDeflectMesh(shp.NacelleRamp, effectLength * shp.Length, effectThickness, InletDeflectMesh.vertices[0], InletDeflectMesh.vertices[^2] - InletDeflectMesh.vertices[0], _debug);
            // NacelleDeflectMesh encapsulated by inletDeflectMesh
        }
        else
        {
            // Freestream -> NacelleRamp => DEFLECT
            Deflect NacelleDeflect = new Deflect(freeStream.AngleTo(shp.NacelleRamp));
            shp.NacelleRamp.Fluid = NacelleDeflect.GetParcel(freeStream.Fluid);
            NacelleDeflectMesh = NacelleDeflect.GetDeflectMesh(shp.NacelleRamp, effectLength * shp.Length, effectThickness, _debug);
        }
        // ! Pressure Forces
        PresureForceAndMoment(shp.NacelleRamp.WallPoints(0.5f)[0], shp.NacelleRamp.WallNormals()[0], shp.NacelleRamp.Fluid.P);


        // Exhaust -> UpperRamp => EXHAUST
        Exhaust UpperExhaust = new(shp.UpperRamp.Fluid, shp.NozzleExpansionAngle, shp.NozzleExitRadius);


        // Exhaust -> NacelleRamp => EXHAUST
        Exhaust NacelleExhaust = new(shp.NacelleRamp.Fluid, shp.NozzleExpansionAngle, shp.NozzleExitRadius);


        // -- Forces --
        if (!paused)
        {
            //Debug.Log(Force);
            //Debug.Log(Moment);
            rb.AddRelativeForce(Force);
            //rb.AddTorque(Moment);
        }

        rb.rotation = fcs.PitchInceptor;


        // -- Temperature UVs (TUVs) --
        shp.FuselageMesh.uv2 = new Vector2[] { TUV(shp.InletRamp.Fluid.T), TUV(preEngine.T), TUV(shp.Engine.Fluid.T), TUV(shp.Nozzle.Fluid.T), Vector2.zero};
        shp.NacelleMesh.uv2 = new Vector2[] { TUV(preEngine.T), TUV(shp.Engine.Fluid.T), TUV(shp.Nozzle.Fluid.T), Vector2.zero};

        Debug.Log("Inlet: " + shp.InletRamp.Fluid.M);
        Debug.Log("Engine: " + shp.Engine.Fluid.M);
        Debug.Log("Nozzle: " + shp.Nozzle.Fluid.P);
        Debug.Log("Nacelle: " + shp.NacelleRamp.Fluid.M);
        Debug.Log("Upper: " + shp.UpperRamp.Fluid.M);
    }

    float LeverArm3(Vector3 point, Vector3 force)
    {
        //moments += Mathf.Abs(Force.magnitude) * LeverArm(Surface.SubPoint, Force);
        return Vector3.Dot(point, Vector3.Cross(force, Vector3.forward).normalized); //seems right
    }

    void PresureForceAndMoment(Vector3 point, Vector3 vector, float pressure)
    {
        //point: shp.NacelleRamp.WallPoints(0.5f)[0]
        //force: +/-shp.NacelleRamp.WallVectors()[0]
        Vector3 force = pressure * shp.Width * -vector;
        Force += force;
        Moment += Mathf.Abs(force.magnitude) * LeverArm3(point, force);
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

}
