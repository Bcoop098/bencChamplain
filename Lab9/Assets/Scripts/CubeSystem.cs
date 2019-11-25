using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;


public class CubeSystem : JobComponentSystem
{
    //[BurstCompile] fails with quaternions
    public struct cubeJob : IJobForEach<Rotation, ParticleData, Translation>
    {
        public float deltaTime;
        public void Execute(ref Rotation rotation, ref ParticleData particleData, ref Translation translation)
        {
            particleData.position += particleData.velocity * deltaTime;
            particleData.velocity += particleData.acceleration * deltaTime;

            Quaternion angularVelocityQuat = new Quaternion(particleData.angularVelocity.x, particleData.angularVelocity.y, particleData.angularVelocity.z, 0f);
            Quaternion angularTimesRotation = angularVelocityQuat * particleData.rotation;
            angularTimesRotation.x = angularTimesRotation.x * deltaTime * 0.5f;
            angularTimesRotation.y = angularTimesRotation.y * deltaTime * 0.5f;
            angularTimesRotation.z = angularTimesRotation.z * deltaTime * 0.5f;
            angularTimesRotation.w = angularTimesRotation.w * deltaTime * 0.5f;

            particleData.rotation = new Quaternion((particleData.rotation.x + angularTimesRotation.x), (particleData.rotation.y + angularTimesRotation.y),
                                         (particleData.rotation.z + angularTimesRotation.z), (particleData.rotation.w + angularTimesRotation.w)).normalized;


            particleData.angularVelocity = particleData.angularAcceleration * deltaTime;

            translation.Value = particleData.position;
            rotation.Value = particleData.rotation;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new cubeJob { deltaTime = Time.deltaTime }.Schedule(this, inputDeps);
    }
}
