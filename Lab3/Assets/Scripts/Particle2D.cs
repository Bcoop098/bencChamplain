using UnityEngine;

public class Particle2D : MonoBehaviour
{
    private Vector2 position;
    private Vector2 localSpace;
    public Vector2 velocity = new Vector2(0, 0);
    private Vector2 acceleration = new Vector2(0, 0);
    private float rotation = 0f;
    private float angularVelocity = 0f;
    private float angularAcceleration;
    public Physics calculationType;
    public Forces forceType;
    public Torque torqueType;
    public float startTime;
    [Range(0,Mathf.Infinity)]
    public float mass;
    private float inverseMass;

    public float radius;
    public float radius2;
    public float torque;
    private float inertia;
    private float inverseInertia;
    public float dXdimension;
    public float dYdimension;
    public float rodLength;

    Vector2 force;

    private float Mass
    {
        set
        {
            mass = mass > 0.0f ? mass : 0.0f;
            inverseMass = mass > 0.0f ? 1.0f / mass : 0.0f;
        }

        get
        {
            return mass;
        }
    }

    private void AddForce(Vector2 newForce)
    {
        force += newForce;
    }

    private void UpdateAcceleration()
    {
        acceleration = force * inverseMass;

        force.Set(0.0f, 0.0f);
    }

    private void UpdateAngularAcceleration()
    {
        angularAcceleration = inverseInertia * torque;
        torque = 0.0f;
    }

    

    public enum Forces
    {
        Gravity,
        NormalForce,
        SlidingForce,
        StaticFriction,
        KinematicFriction,
        Drag,
        Spring,
        None,
        Lab3Bonus,
        TorqueTest
    }

    //drop down menu
    public enum Physics
    {
        Kinematic,
        Euler
    }

    public enum Torque
    {
        Disk,
        Ring,
        Rectangle,
        Rod
    }

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        localSpace = new Vector2(transform.localScale.x,transform.localScale.y);
        startTime = Time.time;//needed to ensure everything stays in the correct positions
        Mass = mass;
        if (mass == 0f)
        {
            inertia = 0f;
            inverseInertia = 0f;
        }
        else
        {
            if (torqueType == Torque.Disk)
            {
                inertia = 0.5f * mass * (radius * radius);
                inverseInertia = 1.0f / inertia;
            }

            if (torqueType == Torque.Ring)
            {
                inertia = 0.5f * mass * ((radius * radius) + (radius2 * radius2));
                inverseInertia = 1.0f / inertia;
            }
            if (torqueType == Torque.Rectangle)
            {
                inertia = 0.083f * mass * ((dXdimension * dXdimension) + (dYdimension * dYdimension));
                inverseInertia = 1.0f / inertia;
            }
            if (torqueType == Torque.Rod)
            {
                inertia = 0.083f * mass * (rodLength * rodLength);
                inverseInertia = 1.0f / inertia;
            }
        }
    }

    private void Update()
    {
        if (forceType == Forces.TorqueTest)
        {
            ApplyTorque(localSpace, new Vector2(-5, 0));
        }
        if (forceType == Forces.Gravity)
        {
            AddForce(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass));
        }

        else if (forceType == Forces.NormalForce)
        {
            AddForce(ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1.0f, 0.0f).normalized));
        }

        else if (forceType == Forces.SlidingForce)
        {
            AddForce(ForceGenerator.GenerateForce_Sliding(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)));
        }

        else if (forceType == Forces.StaticFriction)
        {
            Vector2 Normal = ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1.0f, 1.0f).normalized);
            AddForce(ForceGenerator.GenerateForce_friction_static(Normal, new Vector2(5, 5), 0.5f));
        }

        else if (forceType == Forces.KinematicFriction)
        {
            AddForce(ForceGenerator.GenerateForce_friction_kinetic((ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)), new Vector2(1, 1), 0.5f));
        }

        else if (forceType == Forces.Drag)
        {
            AddForce(ForceGenerator.GenerateForce_drag(new Vector2(1, 1), new Vector2(0.5f, 1), 1.0f, 2.0f, 0.2f));
        }

        else if (forceType == Forces.Spring)
        {
            AddForce(ForceGenerator.GenerateForce_spring(position, new Vector2(0, -2), 10.0f, 1.0f));
        }

        else if (forceType == Forces.Lab3Bonus)
        {
            Vector2 gravity = ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass);
            //AddForce(gravity);
            Vector2 Normal = ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1.0f, 1.0f).normalized);
            Vector2 Friction = ForceGenerator.GenerateForce_friction_kinetic((ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)), new Vector2(1, 1), 0.5f);
            //AddForce(Normal);
            AddForce(Friction);
            //ApplyTorque(localSpace, -Friction);
            ApplyTorque(localSpace, gravity);

        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        

        //if Euler is selected in the drop down menu, do this
        if (calculationType == Physics.Euler)
        {
            UpdatePositionEulerExplicit(Time.fixedDeltaTime);
            transform.position = position;//set the new position
            UpdateRotationEulerExplicit(Time.fixedDeltaTime);
            transform.eulerAngles = new Vector3(0f, 0f, rotation);//set the new rotation
            //move cubes back and forth
            //acceleration.x = -Mathf.Sin(Time.time - startTime);
        }

        //if Kinematic is selected, do this
        else if(calculationType == Physics.Kinematic)
        {
            UpdatePositionKinematic(Time.fixedDeltaTime);
            transform.position = position; //set the new position
            UpdateRotationKinematic(Time.fixedDeltaTime);
            transform.eulerAngles = new Vector3(0f, 0f, rotation); //set the new rotation
            //move the cubes in a circle
            //acceleration.x = Mathf.Sin(Time.time - startTime);
            //acceleration.y = Mathf.Cos(Time.time - startTime);
        }
        UpdateAngularAcceleration();
        UpdateAcceleration();
    }

    void ApplyTorque(Vector2 locationOfForce, Vector2 appliedForce)
    {
        torque += Vector3.Cross(locationOfForce,appliedForce).z;
    }

    void UpdatePositionEulerExplicit(float deltaTime)
    {
        position += velocity * deltaTime;

        velocity += acceleration * deltaTime;
    }

    void UpdatePositionKinematic(float deltaTime)
    {
        position += velocity * deltaTime + (acceleration*(deltaTime*deltaTime))*0.5f;

        velocity += acceleration * deltaTime;
    }
    void UpdateRotationEulerExplicit(float deltaTime)
    {
        rotation += angularVelocity * deltaTime;

        angularVelocity += angularAcceleration * deltaTime;
    }
    void UpdateRotationKinematic(float deltaTime)
    {
        rotation += angularVelocity * deltaTime + (angularAcceleration * (deltaTime * deltaTime)) * 0.5f;

        angularVelocity += angularAcceleration * deltaTime;
    }

}
