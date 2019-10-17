using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public Vector2 position;
    private Vector2 localSpace;
    public Vector2 velocity = new Vector2(0, 0);
    public Vector2 acceleration = new Vector2(0, 0);
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
    public float fuel = 1000;

    public bool isPlayer = false;

    public float radius;
    public float radius2;
    public float torque;
    private float inertia;
    private float inverseInertia;
    public float dXdimension;
    public float dYdimension;
    public float rodLength;
    public float restitution;

    Vector2 force;

    private float Mass
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
        Rod,
        None
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
       
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPlayer) // only runs for the player
        {
            AddForce(ForceGenerator.GenerateForce_Gravity(-0.5f, transform.up, mass));
            /*if (Input.GetAxis("Vertical") < 0) //down
            {
                AddForce(ForceGenerator.GenerateForce_Gravity(-2.0f, transform.up, mass));
                //AddForce(ForceGenerator.GenerateForce_Normal(new Vector2(0, 0.5f), new Vector2(0.0f, -1.0f).normalized));
            }
            else if (Input.GetAxis("Vertical") > 0) //up
            {
                AddForce(ForceGenerator.GenerateForce_Gravity(2.0f, transform.up, mass));
                //AddForce(ForceGenerator.GenerateForce_Normal(new Vector2(0, 0.5f), new Vector2(0.0f, 1.0f).normalized));
            }
            else if (Input.GetAxis("Horizontal") > 0) //right
            {
                ApplyTorque(transform.position, ForceGenerator.GenerateForce_Gravity(2.0f, Vector2.up, mass));

                //ApplyTorque(new Vector2(transform.position.x - 0.25f, transform.position.y), ForceGenerator.GenerateForce_Gravity(2.0f, Vector2.up, mass));
            }
            else if (Input.GetAxis("Horizontal") < 0) //left
            {
                ApplyTorque(transform.position, ForceGenerator.GenerateForce_Gravity(-2.0f, Vector2.up, mass));
                //ApplyTorque(new Vector2(transform.position.x + 0.25f,transform.position.y), ForceGenerator.GenerateForce_Gravity(-2.0f, Vector2.up, mass));
            }*/
            if (transform.eulerAngles.z >= 90.0f && transform.eulerAngles.z < 180f && !(Input.GetAxis("Horizontal") < 0.0f))
            {
                transform.eulerAngles = new Vector3(0f, 0f, 90f);
                StopTorque();
            }
            if (transform.eulerAngles.z <= 270.0f && transform.eulerAngles.z > 180f && !(Input.GetAxis("Horizontal") < 0.0f))
            {
                transform.eulerAngles = new Vector3(0f, 0f, 270f);
                StopTorque();
            }
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 && fuel > 0)
            {
                AddForce(ForceGenerator.GenerateForce_Gravity(2.0f, transform.up * Input.GetAxis("Vertical"), mass));
                --fuel;
            }
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
            {
                ApplyTorque(transform.position, ForceGenerator.GenerateForce_Gravity(2.0f, (Vector2.up * Input.GetAxis("Horizontal")), mass));
            }
            else
            {
                StopTorque();
            }
            

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
        UpdateAngularAcceleration();
        UpdateAcceleration();
    }

    void StopTorque()
    {
        torque = 0.0f;
        angularAcceleration = 0.0f;
        angularVelocity = 0f;

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

    public Vector2 GetPosition()
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
    }

    public string GetTag()
    {
        return gameObject.tag;
    }
}
