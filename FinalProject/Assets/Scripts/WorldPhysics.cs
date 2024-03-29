﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://github.com/idmillington/cyclone-physics/blob/master/src/collide_fine.cpp
public class WorldPhysics : MonoBehaviour
{
    public static WorldPhysics Instance;// = new WorldPhysics();

    [SerializeField]
    GameObject Cannon;

    public List<GameObject> activeCollisions;

    private List<CollisionHull3D.CollisionInfo> allCollisionInfo = new List<CollisionHull3D.CollisionInfo>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void addToList(GameObject item)
    {
        activeCollisions.Add(item);
    }

    public void removeFromList(GameObject item)
    {
        for (int i = 0; i < activeCollisions.Count; i++)
        {
            if (activeCollisions[i] == item)
            {
                activeCollisions.RemoveAt(i);
                break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        /*foreach (var col in activeCollisions)
        {
            col.GetComponent<Renderer>().material.color = Color.blue;
        }*/

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
                        //col.GetComponent<Renderer>().material.color = Color.red;
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
            if (colInfo.RigidBodyA.tag == "CannonBall" && colInfo.RigidBodyB.tag == "OuterWall")
            {
                colInfo.RigidBodyB.RemoveObject();
                colInfo.RigidBodyA.resetAngularValues();
                colInfo.RigidBodyB.resetAngularValues();
                colInfo.RigidBodyA.hasHit = true;
                colInfo.RigidBodyB.hasHit = true;
                ResolveVelocity(colInfo);
                ResolvePenetration(colInfo);
                colInfo.RigidBodyA.addToBounceCount();
            }
            else if (colInfo.RigidBodyA.tag == "CannonBall" && colInfo.RigidBodyB.tag == "Ground")
            {
                colInfo.RigidBodyA.resetAngularValues();
                colInfo.RigidBodyB.resetAngularValues();
                colInfo.RigidBodyA.hasHit = true;
                colInfo.RigidBodyB.hasHit = true;
                colInfo.RigidBodyA.addFriction();
                ResolveVelocity(colInfo);
                ResolvePenetration(colInfo);
            }
            else if (colInfo.RigidBodyA.tag == "CannonBall" && colInfo.RigidBodyB.tag == "PowerUp")
            {
                colInfo.RigidBodyB.RemoveObject();
                Cannon.GetComponent<CannonMove>().addAmmo();
            }
            else if (colInfo.RigidBodyA.tag == "CannonBall" && colInfo.RigidBodyB.tag == "Complex")
            {
                if (colInfo.RigidBodyB.transform.parent.tag == "Holder")
                    colInfo.RigidBodyB.RemoveChildren();
                else if (colInfo.RigidBodyB.transform.parent.tag == "Holder2")
                    colInfo.RigidBodyB.RemoveChildren2();
                colInfo.RigidBodyA.addToBounceCount();
                colInfo.RigidBodyA.resetAngularValues();
                colInfo.RigidBodyB.resetAngularValues();
                colInfo.RigidBodyA.hasHit = true;
                colInfo.RigidBodyB.hasHit = true;
                ResolveVelocity(colInfo);
                ResolvePenetration(colInfo);
            }

            else if (colInfo.RigidBodyA.tag == "EndGame" && colInfo.RigidBodyB.tag == "EndGame")
            {
                colInfo.RigidBodyB.RemoveObject();
                colInfo.RigidBodyA.RemoveObject();
                GameManager.manager.Fireworks1();
            }

            else if (colInfo.RigidBodyA.tag == "EndGame2" && colInfo.RigidBodyB.tag == "EndGame2")
            {
                colInfo.RigidBodyB.RemoveObject();
                colInfo.RigidBodyA.RemoveObject();
                GameManager.manager.Fireworks2();
            }

            else if (colInfo.RigidBodyA.tag == "EndGame3" && colInfo.RigidBodyB.tag == "EndGame3")
            {
                colInfo.RigidBodyB.RemoveObject();
                colInfo.RigidBodyA.RemoveObject();
                GameManager.manager.Fireworks3();
            }

            else if (colInfo.RigidBodyA.tag == "EndGame4" && colInfo.RigidBodyB.tag == "EndGame4")
            {
                colInfo.RigidBodyB.RemoveObject();
                colInfo.RigidBodyA.RemoveObject();
                GameManager.manager.Fireworks4();
            }

            else if (colInfo.RigidBodyA.tag == "CannonBall" && colInfo.RigidBodyB.tag == "CannonBall")
            {
                ResolveVelocity(colInfo);
                ResolvePenetration(colInfo);
            }

        }
        allCollisionInfo.Clear();
    }

    void ResolveVelocity(CollisionHull3D.CollisionInfo collisionInfo)
    {
        float separatingVelocity = Vector3.Dot(collisionInfo.RelativeVelocity, collisionInfo.contacts[0].normal);
        if (separatingVelocity > 0.0f)
        {
            return;
        }

        const float velLimit = 1f;
        float appliedRestitution = collisionInfo.RelativeVelocity.sqrMagnitude < velLimit * velLimit ? 0.0f : collisionInfo.contacts[0].restitution;
        float newSeparatingVelocity = -separatingVelocity * appliedRestitution;

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

        collisionInfo.RigidBodyA.velocity -= collisionInfo.RigidBodyA.GetInverseMass() * impulsePerMass;
        collisionInfo.RigidBodyB.velocity += collisionInfo.RigidBodyB.GetInverseMass() * impulsePerMass;
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

        collisionInfo.RigidBodyA.position -= collisionInfo.RigidBodyA.GetInverseMass() * movePerMass;
        collisionInfo.RigidBodyB.position += collisionInfo.RigidBodyB.GetInverseMass() * movePerMass;

    }
}

