using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
public class ParticleSystem : JobComponentSystem
{

    struct ParticleJob : IJobForEach<ParticleData>
    {
        public float deltaTime;

        public void Execute(ref ParticleData data)
        {
            float3 posVal = data.position;
            quaternion rotVal = data.rotation;
            float3 velVal = data.velocity;
            float3 accVal = data.acceleration;
            float3 angVel = data.angularVelocity;
            float3 angAcc = data.angularAcceleration;

            posVal += velVal * deltaTime;
            velVal += accVal * deltaTime;

            Quaternion angularVelocityQuat = new Quaternion(angVel.x, angVel.y, angVel.z, 0f);
            Quaternion angularTimesRotation = angularVelocityQuat * rotVal;
            angularTimesRotation.x = angularTimesRotation.x * deltaTime * 0.5f;
            angularTimesRotation.y = angularTimesRotation.y * deltaTime * 0.5f;
            angularTimesRotation.z = angularTimesRotation.z * deltaTime * 0.5f;
            angularTimesRotation.w = angularTimesRotation.w * deltaTime * 0.5f;

            rotVal = new Quaternion((rotVal.value.x + angularTimesRotation.x), (rotVal.value.y + angularTimesRotation.y),
                                     (rotVal.value.z + angularTimesRotation.z), (rotVal.value.w + angularTimesRotation.w)).normalized;


            angVel = angAcc * deltaTime;

            data.position = posVal;
            data.velocity = velVal;
            data.rotation = rotVal;
            data.angularVelocity = angVel;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        ParticleJob particleJob = new ParticleJob
        {
            deltaTime = Time.deltaTime
        };

        JobHandle jobHandle = particleJob.Schedule(this,inputDeps);

        return jobHandle;
    }
}
