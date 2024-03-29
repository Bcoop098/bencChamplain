﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        foreach (var col in activeCollisions)
        {
            foreach (var col2 in activeCollisions)
            {
                if (col != col2)
                {
                    if (col.GetComponent<CollisionHull2D>().TestCollision(col2.GetComponent<CollisionHull2D>()))
                    {
                        col.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
            }
        }
    }
}
