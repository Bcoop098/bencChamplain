using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://github.com/idmillington/cyclone-physics/blob/master/src/collide_fine.cpp
public class WorldPhysics : MonoBehaviour
{
    public static WorldPhysics Instance = new WorldPhysics();

    public List<GameObject> activeCollisions;

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
                        //allCollisionInfo.Add(collisionInfo);
                        col.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }
        }
    }
}
