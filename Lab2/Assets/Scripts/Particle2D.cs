using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public Vector2 position;
    public Vector2 velocity;
    private Vector2 acceleration;
    public float rotation;
    public float angularVelocity;
    public float angularAcceleration;
    public Physics calculationType;
    public Forces forceType;
    public float startTime;
    [Range(0,Mathf.Infinity)]
    public float mass;
    private float inverseMass;

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

    public enum Forces
    {
        Gravity,
        NormalForce,
        SlidingForce,
        StaticFriction,
        KinematicFriction,
        Drag,
        Spring
    }

    //drop down menu
    public enum Physics
    {
        Kinematic,
        Euler
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;//needed to ensure everything stays in the correct positions
        Mass = mass;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (forceType == Forces.Gravity)
        {
            AddForce(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass));
        }

        else if (forceType == Forces.NormalForce)
        {
            AddForce(ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2 (1.0f,1.0f).normalized));
        }

        else if (forceType == Forces.SlidingForce)
        {
            AddForce(ForceGenerator.GenerateForce_Sliding(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f,1f).normalized)));
            //AddForce(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass));
        }

        else if (forceType == Forces.StaticFriction)
        {
            AddForce(ForceGenerator.GenerateForce_friction_static((ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)),-(ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)),0.8f));
            //AddForce(ForceGenerator.GenerateForce_Sliding(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)));
        }

        else if (forceType == Forces.KinematicFriction)
        {
            AddForce(ForceGenerator.GenerateForce_friction_kinetic((ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)), velocity, 0.5f));
            //AddForce(ForceGenerator.GenerateForce_Sliding(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), ForceGenerator.GenerateForce_Normal(ForceGenerator.GenerateForce_Gravity(-10.0f, Vector2.up, mass), new Vector2(1f, 1f).normalized)));
        }

        else if (forceType == Forces.Drag)
        {

        }

        else if (forceType == Forces.Spring)
        {

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
        }

        UpdateAcceleration();
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
