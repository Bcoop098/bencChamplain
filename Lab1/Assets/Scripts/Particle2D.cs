using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;
    public float rotation;
    public float angularVelocity;
    public float angularAcceleration;
    public Physics calculationType;
    public float startTime;

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
            acceleration.x = -Mathf.Sin(Time.time - startTime);
        }

        //if Kinematic is selected, do this
        else if(calculationType == Physics.Kinematic)
        {
            UpdatePositionKinematic(Time.fixedDeltaTime);
            transform.position = position; //set the new position
            UpdateRotationKinematic(Time.fixedDeltaTime);
            transform.eulerAngles = new Vector3(0f, 0f, rotation); //set the new rotation
            //move the cubes in a circle
            acceleration.x = Mathf.Sin(Time.time - startTime);
            acceleration.y = Mathf.Cos(Time.time - startTime);
        }
        
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
