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

        foreach (var col in activeCollisions)
        {
            foreach (var col2 in activeCollisions)
            {
                if (col != col2)
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
            if (colInfo.RigidBodyA.tag == "Player" && colInfo.RigidBodyB.tag == "Ground")
            {
                if ((colInfo.RigidBodyA.velocity.x < 0.8f) && (colInfo.RigidBodyA.velocity.y > -1f))
                {
                    GameManager.manager.win = true;
                    colInfo.RigidBodyA.velocity = new Vector2(0f, 0f);
                    colInfo.RigidBodyA.acceleration = new Vector2(0f, 0f);
                    //colInfo.RigidBodyA.rotation = 0f;
                }
                else if (colInfo.RigidBodyA.velocity.x >= 0.8f || colInfo.RigidBodyA.velocity.y <= -1f)
                {
                    GameManager.manager.lose = true;
                    colInfo.RigidBodyA.velocity = new Vector2(0f, 0f);
                    colInfo.RigidBodyA.acceleration = new Vector2(0f, 0f);
                    //colInfo.RigidBodyA.rotation = 0f;
                }
            }
            else if (GameManager.manager.win == false && GameManager.manager.lose == false)
            {
                ResolveVelocity(colInfo);
                ResolvePenetration(colInfo);
            }
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

        float newSeparatingVelocity = -separatingVelocity * collisionInfo.contacts[0].restitution;

        Vector2 accCausedVelocity = collisionInfo.RigidBodyA.acceleration - collisionInfo.RigidBodyB.acceleration;

        float accCausedSepVelocity = Vector2.Dot(accCausedVelocity,collisionInfo.contacts[0].normal) * Time.fixedDeltaTime;
        if (accCausedSepVelocity < 0)
        {
            newSeparatingVelocity += collisionInfo.contacts[0].restitution * accCausedSepVelocity;
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

        collisionInfo.RigidBodyA.position -= collisionInfo.RigidBodyA.GetInverseMass() * movePerMass;
        collisionInfo.RigidBodyB.position += collisionInfo.RigidBodyB.GetInverseMass() * movePerMass;

    }
}
