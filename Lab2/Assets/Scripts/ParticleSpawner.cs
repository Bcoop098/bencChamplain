using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public GameObject particle;
    public PhysicsSpawner physicsType;

    //drop down menu
    public enum PhysicsSpawner
    {
        Kinematic,
        Euler
    }

    // Start is called before the first frame update
    void Start()
    {
        //start spawning objects
        InvokeRepeating("SpawnObject",0f,5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObject()
    {
        //if the spawner is selected to be Kinematic in the drop down menu, do this
        if (physicsType == PhysicsSpawner.Kinematic)
        {
            GameObject clone = Instantiate(particle, new Vector3(2f, 0f, 0f), Quaternion.identity); //create a new cube
            clone.GetComponent<Particle2D>().velocity = new Vector2(-1.0f, 0.0f); //set velocity
            clone.GetComponent<Particle2D>().calculationType = Particle2D.Physics.Kinematic; //set equation type
            
        }

        //if the spawner is selected to be Euler in the drop down menu, do this
        if (physicsType == PhysicsSpawner.Euler)
        {
            GameObject clone = Instantiate(particle, new Vector3(-2f, 0f, 0f), Quaternion.identity); //create a new cube
            clone.GetComponent<Particle2D>().velocity = new Vector2(1.0f, 0.0f); //set velocity
            clone.GetComponent<Particle2D>().calculationType = Particle2D.Physics.Euler; //set equation type
        }
    }
}
