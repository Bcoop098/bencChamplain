using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
public struct MovementJob : IJobParallelForTransform
{
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        Vector3 pos = transform.position;

        pos += velocity * deltaTime;
        transform.position = pos;
    }
}