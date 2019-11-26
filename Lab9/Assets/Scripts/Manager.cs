using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


public struct cubeSpawnerData : IComponentData
{
    public int NumXCubes;
    public int NumZCubes;
    public Quaternion rotationValue;
    public Vector3 velocityVal;
    public Vector3 accVal;
    public Vector3 angAcc;
    public Vector3 angVel;
    public Entity cubePrefabEntity;
    public Vector3 positionVal;


}
[RequiresEntityConversion]
public class Manager : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject cube;

    public Vector3 acceleration;

    public Vector3 velocity;

    public Vector3 angularAcceleration;

    public Vector3 angularVelocity;

    public Quaternion rotation;

    public Vector3 position;

    public int NumXCubes;
    public int NumZCubes;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(cube);
    }
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var cubePrefabEntity = conversionSystem.GetPrimaryEntity(cube);

        var cubeSpawner = new cubeSpawnerData
        {
            NumXCubes = NumXCubes,
            NumZCubes = NumZCubes,
            rotationValue = rotation,
            velocityVal = velocity,
            accVal = acceleration,
            angAcc = angularAcceleration,
            angVel = angularVelocity,
            positionVal = position,
            cubePrefabEntity = cubePrefabEntity
        };

        dstManager.AddComponentData(entity, cubeSpawner);
    }

    

    
}
