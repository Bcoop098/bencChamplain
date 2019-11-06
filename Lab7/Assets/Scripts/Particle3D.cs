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


    
}
