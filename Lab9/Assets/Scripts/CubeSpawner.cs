using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ParticleData : IComponentData
{
    public Vector3 position;
    public Vector3 velocity;
    public Quaternion rotation;
    public Vector3 acceleration;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;
}
public class CubeSpawner : ComponentSystem
{
    protected override void OnUpdate()


    {

        Entities.ForEach((Entity entity, ref cubeSpawnerData spawnerData) =>
        {
            for (int x = 0; x < spawnerData.NumXCubes; ++x)
            {
                float posX = x - (spawnerData.NumXCubes / 2);

                for (int z = 0; z < spawnerData.NumZCubes; ++z)
                {
                    float posZ = z - (spawnerData.NumZCubes / 2);

                    var cubeEntity = EntityManager.Instantiate(spawnerData.cubePrefabEntity);
                    spawnerData.positionVal = new Vector3(posX, 0.0f, posZ);
                    EntityManager.SetComponentData(cubeEntity, new Translation { Value = new float3(posX, 0.0f, posZ) });
                    EntityManager.AddComponentData(cubeEntity, new ParticleData { velocity = spawnerData.velocityVal, rotation = spawnerData.rotationValue, position = spawnerData.positionVal,
                    acceleration = spawnerData.accVal, angularAcceleration = spawnerData.angAcc, angularVelocity = spawnerData.angVel});
                }
            }
            EntityManager.DestroyEntity(entity);
        });
    }


    
}
