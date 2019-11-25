using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        PhysicsDLLAccess.CreatePhysicsWorld();
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsDLLAccess.UpdatePhysicsWorld(Time.deltaTime);
    }
}
