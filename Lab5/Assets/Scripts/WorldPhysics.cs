using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPhysics : MonoBehaviour
{
    public static WorldPhysics Instance = new WorldPhysics();

    public List<GameObject> activeCollisions;

    private List<CollisionHull2D.CollisionInfo> allCollisionInfo = new List<CollisionHull2D.CollisionInfo>();

    //List to store all collision infos in

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame

    void FixedUpdate()
    {
        foreach (var col in activeCollisions)
        {
            col.GetComponent<Renderer>().material.color = Color.blue;
        }

        for(var i = 0; i < activeCollisions.Count; i++)
        {
            var col = activeCollisions[i];
            for(var g = i+1; g < activeCollisions.Count; g++)
            {
                var col2 = activeCollisions[g];
                {
                    var collisionInfo = col.GetComponent<CollisionHull2D>().TestCollision(col2.GetComponent<CollisionHull2D>());
                    if (collisionInfo != null)
                    {
                        allCollisionInfo.Add(collisionInfo);
                        col.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }
        }
        ResolveCollision();
    }

    void ResolveCollision()
    {
        foreach (var colInfo in allCollisionInfo)
        {
            ResolveVelocity(colInfo);
            ResolvePenetration(colInfo);
        }
        allCollisionInfo.Clear();
    }

    void ResolveVelocity(CollisionHull2D.CollisionInfo collisionInfo)
    {
        float separatingVelocity = Vector2.Dot(collisionInfo.RelativeVelocity, collisionInfo.contacts[0].normal);
        if (separatingVelocity > 0.0f)
        {
            return;
        }
        Vector2 accCausedVelocity = collisionInfo.RigidBodyA.previousVelocity - collisionInfo.RigidBodyB.previousVelocity;
        const float velLimit = 1f;
        float appliedRestitution = collisionInfo.RelativeVelocity.sqrMagnitude < velLimit * velLimit ? 0.0f : collisionInfo.contacts[0].restitution;

        float newSeparatingVelocity = -separatingVelocity * appliedRestitution;

        

        float accCausedSepVelocity = Vector2.Dot(accCausedVelocity, collisionInfo.contacts[0].normal);
        if (accCausedSepVelocity < 0)
        {
            newSeparatingVelocity += appliedRestitution * accCausedSepVelocity;
            if (newSeparatingVelocity < 0)
                newSeparatingVelocity = 0;
        }

        float deltaVelocity = newSeparatingVelocity - separatingVelocity;
        float totalInverseMass = collisionInfo.RigidBodyA.GetInverseMass() + collisionInfo.RigidBodyB.GetInverseMass();
        if (totalInverseMass <= 0.0f)
        {
            return;
        }

        float impulse = deltaVelocity / totalInverseMass;
        Vector2 impulsePerMass = collisionInfo.contacts[0].normal * impulse;

        collisionInfo.RigidBodyA.velocity -= collisionInfo.RigidBodyA.GetInverseMass() * impulsePerMass;
        collisionInfo.RigidBodyB.velocity += collisionInfo.RigidBodyB.GetInverseMass() * impulsePerMass;
    }

    void ResolvePenetration(CollisionHull2D.CollisionInfo collisionInfo)
    {
        if (collisionInfo.contacts[0].penetration <= 0.0f)
        {
            return;
        }

        float totalInverseMass = collisionInfo.RigidBodyA.GetInverseMass() + collisionInfo.RigidBodyB.GetInverseMass();
        if (totalInverseMass <= 0.0f)
        {
            return;
        }

        Vector2 movePerMass = collisionInfo.contacts[0].normal * (collisionInfo.contacts[0].penetration / totalInverseMass);
        if (movePerMass.y > 0.0f)
        {
            Debug.Log(movePerMass.y);
        }
        collisionInfo.RigidBodyA.position -= collisionInfo.RigidBodyA.GetInverseMass() * movePerMass;
        collisionInfo.RigidBodyB.position += collisionInfo.RigidBodyB.GetInverseMass() * movePerMass;
        
    }
}
