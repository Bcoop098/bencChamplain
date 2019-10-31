using UnityEngine;

public class Particle3D : MonoBehaviour
{

    private Vector2 localSpace;

    public Vector3 position;
    public Vector3 velocity = new Vector3(0, 0,0);
    public Vector3 acceleration = new Vector3(0, 0,0);


    public Quaternion rotation;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;

    public Physics calculationType;
    public Shape shapeType;

    //Force
    private Vector3 force;


    //Inertia
    private Matrix4x4 invInertiaLocalSpace;
    private Matrix4x4 invInertiaWorldSpace;

    //Center of Masses
    public Vector3 centerOfMassLocalSpace = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 centerOfMassWorldSpace;

    //Transform
    private Matrix4x4 objectToWorldTransform;
    private Matrix4x4 worldToObjectTransform;

    //Torque
    private Vector3 torque;

    private float inverseMass;

    //drop down menu
    public enum Physics
    {
        Kinematic,
        Euler
    }

    public enum Shape
    {
        SolidSphere,
        HollowSphere,
        SolidBox,
        HollowBox,
        SolidCylinder,
        SolidCone
    }

    // Start is called before the first frame update
    void Start()
    {
        if (shapeType == Shape.SolidSphere)
        {
            invInertiaLocalSpace = InertiaTensor.tensor.SolidSphere().inverse;
        }
        else if (shapeType == Shape.HollowSphere)
        {
            invInertiaLocalSpace = InertiaTensor.tensor.HollowSphere().inverse;
        }
        else if (shapeType == Shape.SolidBox)
        {
            invInertiaLocalSpace = InertiaTensor.tensor.SolidBox().inverse;
        }
        else if (shapeType == Shape.HollowBox)
        {
            invInertiaLocalSpace = InertiaTensor.tensor.HollowBox().inverse;
        }
        else if (shapeType == Shape.SolidCylinder)
        {
            invInertiaLocalSpace = InertiaTensor.tensor.SolidCylinder().inverse;
        }
        else if (shapeType == Shape.SolidCone)
        {
            invInertiaLocalSpace = InertiaTensor.tensor.SolidCone().inverse;
        }

        inverseMass = 1f / InertiaTensor.tensor.getMass();

        invInertiaWorldSpace = Matrix4x4.zero;
        centerOfMassWorldSpace = Vector3.zero;
        torque = Vector3.zero;
        force = Vector3.zero;
        acceleration = Vector3.zero;
        objectToWorldTransform = Matrix4x4.zero;
        worldToObjectTransform = Matrix4x4.zero;

        position = transform.position;

        
       
    }

    private void Update()
    {
        ApplyForce(new Vector3(0, 1, 0), new Vector3(2, 0, 0));
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        
        //UpdateRotationKinematic(Time.fixedDeltaTime);
        

        if (calculationType == Physics.Euler)
        {
            UpdatePositionEulerExplicit(Time.fixedDeltaTime);
            //transform.position = position;//set the new position
            UpdateRotationEulerExplicit(Time.fixedDeltaTime);
            //transform.rotation = rotation; ;//set the new rotation
        }

        //if Kinematic is selected, do this
        else if (calculationType == Physics.Kinematic)
        {
            UpdatePositionKinematic(Time.fixedDeltaTime);
            //transform.position = position; //set the new position
            UpdateRotationKinematic(Time.fixedDeltaTime);
            //transform.rotation = rotation; //set the new rotation
        }

        objectToWorldTransform = Matrix4x4.TRS(position, rotation, Vector3.one);
        worldToObjectTransform = objectToWorldTransform.transpose;

        //Useful for bonus
        invInertiaWorldSpace = objectToWorldTransform * invInertiaLocalSpace * worldToObjectTransform;

        updateAngularAcceleration();
        UpdateAcceleration();
        transform.SetPositionAndRotation(position, rotation);
        centerOfMassWorldSpace = objectToWorldTransform * centerOfMassLocalSpace;

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
        Quaternion angularVelocityQuat = new Quaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z,0f);
        Quaternion angularTimesRotation = angularVelocityQuat * rotation;
        angularTimesRotation.x = angularTimesRotation.x * deltaTime * 0.5f;
        angularTimesRotation.y = angularTimesRotation.y * deltaTime * 0.5f;
        angularTimesRotation.z = angularTimesRotation.z * deltaTime * 0.5f;
        angularTimesRotation.w = angularTimesRotation.w * deltaTime * 0.5f;

        rotation = new Quaternion((rotation.x + angularTimesRotation.x), (rotation.y + angularTimesRotation.y), 
                                 (rotation.z + angularTimesRotation.z), (rotation.w + angularTimesRotation.w)).normalized;


        angularVelocity = angularAcceleration * deltaTime;
    }
    void UpdateRotationKinematic(float deltaTime)
    {
        Quaternion angularVelocityQuat = new Quaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0f);
        Quaternion angularTimesRotation = angularVelocityQuat * rotation;
        angularTimesRotation.x = angularTimesRotation.x * deltaTime * 0.5f;
        angularTimesRotation.y = angularTimesRotation.y * deltaTime * 0.5f;
        angularTimesRotation.z = angularTimesRotation.z * deltaTime * 0.5f;
        angularTimesRotation.w = angularTimesRotation.w * deltaTime * 0.5f;

        rotation = new Quaternion((rotation.x + angularTimesRotation.x), (rotation.y + angularTimesRotation.y),
                                 (rotation.z + angularTimesRotation.z), (rotation.w + angularTimesRotation.w));

        Quaternion angularAccelQuat = new Quaternion(angularAcceleration.x, angularAcceleration.y, angularAcceleration.z, 0f);
        angularAccelQuat.x = angularAccelQuat.x * (deltaTime * deltaTime) * 0.5f;
        angularAccelQuat.y = angularAccelQuat.y * (deltaTime * deltaTime) * 0.5f;
        angularAccelQuat.z = angularAccelQuat.z * (deltaTime * deltaTime) * 0.5f;
        angularAccelQuat.w = angularAccelQuat.w * (deltaTime * deltaTime) * 0.5f;

        rotation = new Quaternion((rotation.x + angularAccelQuat.x), (rotation.y + angularAccelQuat.y),
                                 (rotation.z + angularAccelQuat.z), (rotation.w + angularAccelQuat.w)).normalized;

        angularVelocity = angularAcceleration * deltaTime;
    }


    void ApplyForce(Vector3 locationOfForce, Vector3 appliedForce)
    {
        Vector3 momentArm = locationOfForce - centerOfMassWorldSpace;
        force += appliedForce;
        torque += Vector3.Cross(momentArm, appliedForce);
    }

    void ApplyForceLocal(Vector3 locationOfForceLocal, Vector3 appliedForce)
    {
        Vector3 localToWorld = objectToWorldTransform * locationOfForceLocal;
        Vector3 forceToWorld = objectToWorldTransform * appliedForce;
        ApplyForce(localToWorld, forceToWorld);
    }

    void updateAngularAcceleration()
    {
        angularAcceleration = invInertiaWorldSpace * torque;
        torque = Vector3.zero;
    }

    void UpdateAcceleration()
    {
        acceleration = inverseMass * force;
        force = Vector3.zero;
    }


    //Code for later Labs

    /*public Vector2 GetPosition()
    {
        return position;
    }

    public float GetRadius()
    {
        return radius;
    }

    public float GetInverseMass()
    {
        return inverseMass;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }*/

    /*if (forceType == Forces.TorqueTest)
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
        }*/
    //UpdateAngularAcceleration();
    //UpdateAcceleration();
    //UpdatePositionEulerExplicit(Time.fixedDeltaTime);
    //transform.position = position;//set the new position
    //UpdateRotationEulerExplicit(Time.fixedDeltaTime);

    /*position = new Vector2(transform.position.x, transform.position.y);
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
       }*/

    /*

   public Forces forceType;
   public Torque torqueType;
   public float startTime;
   [Range(0,Mathf.Infinity)]
   public float mass;
   private float inverseMass;
   public Vector2 previousVelocity = new Vector2(0, 0);

   public float radius;
   public float radius2;
   public float torque;
   private float inertia;
   private float inverseInertia;
   public float dXdimension;
   public float dYdimension;
   public float rodLength;
   public float restitution;

   Vector2 force;*/

    /*private float Mass
    {
        set
        {
            mass = value > 0.0f ? value : 0.0f;
            inverseMass = value > 0.0f ? 1.0f / value : 0.0f;
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
    }*/

    /*void ApplyTorque(Vector2 locationOfForce, Vector2 appliedForce)
    {
        torque += Vector3.Cross(locationOfForce,appliedForce).z;
    }*/
}
