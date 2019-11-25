using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseParticle : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Quaternion rotation;
    public Vector3 acceleration;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*UpdatePositionEulerExplicit(Time.fixedDeltaTime);
        UpdateRotationEulerExplicit(Time.fixedDeltaTime);
        transform.SetPositionAndRotation(position, rotation);*/
    }

    void UpdatePositionEulerExplicit(float deltaTime)
    {
        position += velocity * deltaTime;
        velocity += acceleration * deltaTime;
    }

    void UpdateRotationEulerExplicit(float deltaTime)
    {
        Quaternion angularVelocityQuat = new Quaternion(angularVelocity.x, angularVelocity.y, angularVelocity.z, 0f);
        Quaternion angularTimesRotation = angularVelocityQuat * rotation;
        angularTimesRotation.x = angularTimesRotation.x * deltaTime * 0.5f;
        angularTimesRotation.y = angularTimesRotation.y * deltaTime * 0.5f;
        angularTimesRotation.z = angularTimesRotation.z * deltaTime * 0.5f;
        angularTimesRotation.w = angularTimesRotation.w * deltaTime * 0.5f;

        rotation = new Quaternion((rotation.x + angularTimesRotation.x), (rotation.y + angularTimesRotation.y),
                                 (rotation.z + angularTimesRotation.z), (rotation.w + angularTimesRotation.w)).normalized;


        angularVelocity = angularAcceleration * deltaTime;
    }

}
