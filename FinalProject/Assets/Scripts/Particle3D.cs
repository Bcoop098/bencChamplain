using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Particle3D : MonoBehaviour
{
    public bool isCannonBall;
    public float timeToDisable;
    public bool onGround;

    public bool powerUp;

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

    public float mass;

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

    public float inverseMass;

    public Vector3 forceLocation;
    public Vector3 forceAmount;

    public float restitution;

    public float gravity = -10f;

    public bool hasHit = false;

    [SerializeField]
    int bounceCount;


    [SerializeField]
    bool isPartOfEnd;

    //drop down menu
    public enum Physics
    {
        Kinematic,
        Euler,
        None
    }

    public enum Shape
    {
        SolidSphere,
        HollowSphere,
        SolidBox,
        HollowBox,
        SolidCylinder,
        SolidCone,
        None
    }

    // Start is called before the first frame update
    void Start()
    {
        onGround = false;
        if(isCannonBall)
        {
            StartCoroutine(Disable(timeToDisable));
        }
        WorldPhysics.Instance.addToList(gameObject);
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

        if (mass == 0)
        {
            inverseMass = 0.0f;
        }
        else
        {
            inverseMass = 1f /mass;
        }

        invInertiaWorldSpace = Matrix4x4.zero;
        centerOfMassWorldSpace = Vector3.zero;
        torque = Vector3.zero;
        force = Vector3.zero;
        acceleration = Vector3.zero;
        objectToWorldTransform = Matrix4x4.zero;
        worldToObjectTransform = Matrix4x4.zero;
        //angularVelocity = new Vector3(5, 0, 0);


        position = transform.position;

        if (isCannonBall)
        {
            ApplyForce(Vector3.down, new Vector3(0, 200, 0));
        }
        
        
       
    }

    private void Update()
    {
        if (bounceCount >= 2)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            WorldPhysics.Instance.removeFromList(gameObject);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isCannonBall && !onGround)
        {
            Vector3 dragLocation = new Vector3(position.x + 2, position.y, position.z);
            float cannonBallArea = Mathf.PI * (InertiaTensor.tensor.radius * InertiaTensor.tensor.radius);
            ApplyForce(dragLocation, newDrag());
        }
        if (!hasHit)
        {
            ApplyForce(forceLocation, forceAmount);
        }
        else if(hasHit && !onGround)
        {   
            ApplyForce(forceLocation, -forceAmount);
        }

        if (!isPartOfEnd)
        {
            ApplyForce(position, generateGravityForce(gravity, Vector3.up, mass));
        }
        

        if (onGround )
        {
            Vector3 inFront = new Vector3(position.x +5, position.y, position.z);
            ApplyForce(inFront, generateFriction(Vector3.up, velocity*2, 3f));
        }



        if (calculationType == Physics.Euler)
        {
            UpdatePositionEulerExplicit(Time.fixedDeltaTime);
            UpdateRotationEulerExplicit(Time.fixedDeltaTime);
        }

        //if Kinematic is selected, do this
        else if (calculationType == Physics.Kinematic)
        {
            UpdatePositionKinematic(Time.fixedDeltaTime);
            UpdateRotationKinematic(Time.fixedDeltaTime);
        }

        objectToWorldTransform = Matrix4x4.TRS(position, rotation, Vector3.one);
        worldToObjectTransform = objectToWorldTransform.transpose;

        //Useful for bonus
        //invInertiaWorldSpace = objectToWorldTransform * invInertiaLocalSpace * worldToObjectTransform;

        updateAngularAcceleration();
        UpdateAcceleration();
        transform.SetPositionAndRotation(position, rotation);
        centerOfMassWorldSpace = objectToWorldTransform.MultiplyPoint(centerOfMassLocalSpace);

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

        if(!powerUp)
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

    public Matrix4x4 getObjectToWorld()
    {
        //return objectToWorldTransform; used with forces
        return transform.localToWorldMatrix;
    }
    public Matrix4x4 getWorldToObject()
    {
        return transform.worldToLocalMatrix;
        //return worldToObjectTransform;used with forces
    }


    public float GetInverseMass()
    {
        return inverseMass;
    }

    public Vector3 generateGravityForce(float gravityConst, Vector3 worldUp, float particleMass)
    {
        Vector3 gravityVal = worldUp * gravityConst * particleMass;
        return gravityVal;
    }

    public Vector3 generateFriction(Vector3 normal, Vector3 velocity, float coefficientKinetic)
    {
        Vector3 kinetic = -coefficientKinetic * normal.magnitude * velocity.normalized;
        return kinetic;
    }

    public Vector3 newDrag()
    {
        Vector3 force = velocity;
        float dragCoeff = force.magnitude;
        dragCoeff = 0.2f * dragCoeff + 0.15f * (dragCoeff * dragCoeff);
        force.Normalize();
        force *= -dragCoeff;
        return force;
    }

    public void addFriction()
    {
        onGround = true;
    }

    public void resetAngularValues()
    {
        angularAcceleration = Vector3.zero;
        angularVelocity = Vector3.zero;
    }
    public void RemoveObject()
    {
        
        if (!isCannonBall && !powerUp)
        {
            GameManager.manager.removeCastlePart(gameObject);
        }
        gameObject.SetActive(false);
        WorldPhysics.Instance.removeFromList(gameObject);
    }

    private IEnumerator Disable(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        WorldPhysics.Instance.removeFromList(gameObject);
    }

    public void RemoveChildren()
    {
        GameObject complex;
        complex = GameObject.FindGameObjectWithTag("Holder");
        for(int i = 0; i < complex.transform.childCount; i++)
        {
            WorldPhysics.Instance.removeFromList(complex.transform.GetChild(i).gameObject);
            complex.transform.GetChild(i).gameObject.SetActive(false);
        }
        GameManager.manager.removeCastlePart(complex);
        //complex.SetActive(false);
    }

    public void RemoveChildren2()
    {
        GameObject complex;
        complex = GameObject.FindGameObjectWithTag("Holder2");
        for (int i = 0; i < complex.transform.childCount; i++)
        {
            WorldPhysics.Instance.removeFromList(complex.transform.GetChild(i).gameObject);
            complex.transform.GetChild(i).gameObject.SetActive(false);
        }
        GameManager.manager.removeCastlePart(complex);
        //complex.SetActive(false);
    }

    public void addToBounceCount()
    {
        bounceCount++;
    }
}
