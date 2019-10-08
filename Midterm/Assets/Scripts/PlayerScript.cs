using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Vector2 position;
    private Vector2 localSpace;
    public Vector2 velocity = new Vector2(0, 0);
    private Vector2 acceleration = new Vector2(0, 0);
    [Range(-Mathf.PI, Mathf.PI)]
    private float rotation = 0f;
    private float angularVelocity = 0f;
    private float angularAcceleration;
    [Range(0, Mathf.Infinity)]
    public float mass;
    private float inverseMass;
    public float startTime;
    private float inertia;
    private float inverseInertia;

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

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        localSpace = new Vector2(transform.localScale.x, transform.localScale.y);
        startTime = Time.time;//needed to ensure everything stays in the correct positions
        Mass = mass;
        if (mass == 0f)
        {
            inertia = 0f;
            inverseInertia = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rotation -= 0.05f;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rotation += 0.05f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector2 gravity = ForceGenerator.GenerateForce_Gravity(-5.0f, Vector2.up, mass);
            AddForce(ForceGenerator.GenerateForce_Normal(gravity, new Vector2(0.0f, -1.0f).normalized));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector2 gravity = ForceGenerator.GenerateForce_Gravity(-5.0f, Vector2.up, mass);
            AddForce(ForceGenerator.GenerateForce_Normal(gravity, new Vector2(0.0f, 1.0f).normalized));
        }

        UpdateRotationKinematic(Time.fixedDeltaTime);
        transform.eulerAngles = new Vector3(0f, 0f, rotation);
        UpdatePositionKinematic(Time.fixedDeltaTime);
        transform.position = position;
        UpdateVelocityEuler(Time.fixedDeltaTime);
        /*
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
        else if (calculationType == Physics.Kinematic)
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
        UpdateAcceleration();
    }

    void UpdateVelocityEuler(float deltaTime)
    {
        velocity += acceleration * deltaTime;
    }

    void UpdatePositionKinematic(float deltaTime)
    {
        position += velocity * deltaTime + (acceleration * (deltaTime * deltaTime)) * 0.5f;
    }
    void UpdateRotationKinematic(float deltaTime)
    {
        rotation += angularVelocity * deltaTime + (angularAcceleration * (deltaTime * deltaTime)) * 0.5f;

        angularVelocity += angularAcceleration * deltaTime;
    }

    public Vector2 GetPosition()
    {
        return position;
    }
}
