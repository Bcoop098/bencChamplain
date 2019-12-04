using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://github.com/idmillington/cyclone-physics/blob/master/src/collide_fine.cpp
public class WorldPhysics : MonoBehaviour
{
    public static WorldPhysics Instance = new WorldPhysics();

    public List<GameObject> activeCollisions;

    private List<CollisionHull3D.CollisionInfo> allCollisionInfo = new List<CollisionHull3D.CollisionInfo>();

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        foreach (var col in activeCollisions)
        {
            col.GetComponent<Renderer>().material.color = Color.blue;
        }

        for (var i = 0; i < activeCollisions.Count; i++)
        {
            var col = activeCollisions[i];
            for (var g = i + 1; g < activeCollisions.Count; g++)
            {
                var col2 = activeCollisions[g];
                {
                    var collisionInfo = col.GetComponent<CollisionHull3D>().TestCollision(col2.GetComponent<CollisionHull3D>());
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

        /*
         * foreach (var colInfo in allCollisionInfo)
        {
            if (colInfo.RigidBodyA.tag == "Player" && colInfo.RigidBodyB.tag == "Ground")
            {
                if ((Mathf.Abs(colInfo.RigidBodyA.velocity.x) < 1f) && (colInfo.RigidBodyA.velocity.y > -1f))
                {
                    GameManager.manager.win = true;
                    colInfo.RigidBodyA.velocity = new Vector2(0f, 0f);
                    colInfo.RigidBodyA.acceleration = new Vector2(0f, 0f);
                    //colInfo.RigidBodyA.rotation = 0f;
                }
                else if ((Mathf.Abs(colInfo.RigidBodyA.velocity.x) >= 1f || colInfo.RigidBodyA.velocity.y <= -1f))
                {
                    GameManager.manager.lose = true;
                    colInfo.RigidBodyA.velocity = new Vector2(0f, 0f);
                    colInfo.RigidBodyA.acceleration = new Vector2(0f, 0f);
                    //colInfo.RigidBodyA.rotation = 0f;
                }
            }

            else if (GameManager.manager.win == false && GameManager.manager.lose == false)
            {
                if (colInfo.RigidBodyA.tag == "Player" && colInfo.RigidBodyB.tag == "Wall")
                {
                    colInfo.RigidBodyA.fuel -= 50;
                }
                ResolveVelocity(colInfo);
                ResolvePenetration(colInfo);
            }
        }
        allCollisionInfo.Clear();
         */
    }

    void ResolveVelocity(CollisionHull3D.CollisionInfo collisionInfo)
    {
        
        float separatingVelocity = Vector3.Dot(collisionInfo.RelativeVelocity, collisionInfo.contacts[0].normal);
        if (separatingVelocity > 0.0f)
        {
            return;
        }

        float newSeparatingVelocity = -separatingVelocity * collisionInfo.contacts[0].restitution;

        Vector3 accCausedVelocity = collisionInfo.RigidBodyA.acceleration - collisionInfo.RigidBodyB.acceleration;

        float accCausedSepVelocity = Vector3.Dot(accCausedVelocity, collisionInfo.contacts[0].normal) * Time.fixedDeltaTime;
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
        Vector3 impulsePerMass = collisionInfo.contacts[0].normal * impulse;

        //collisionInfo.RigidBodyA.velocity -= collisionInfo.RigidBodyA.GetInverseMass() * impulsePerMass;
        //collisionInfo.RigidBodyB.velocity += collisionInfo.RigidBodyB.GetInverseMass() * impulsePerMass;
    }

    void ResolvePenetration(CollisionHull3D.CollisionInfo collisionInfo)
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

        Vector3 movePerMass = collisionInfo.contacts[0].normal * (collisionInfo.contacts[0].penetration / totalInverseMass);

        //collisionInfo.RigidBodyA.position -= collisionInfo.RigidBodyA.GetInverseMass() * movePerMass;
        //collisionInfo.RigidBodyB.position += collisionInfo.RigidBodyB.GetInverseMass() * movePerMass;
        Debug.Log(collisionInfo.contacts[0].point);
    }
}

