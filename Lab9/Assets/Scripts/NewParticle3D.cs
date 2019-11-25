using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewParticle3D : MonoBehaviour
{
    [SerializeField]
    float xPos;

    [SerializeField]
    float yPos;

    [SerializeField]
    float zPos;

    [SerializeField]
    float mass;

    // Start is called before the first frame update
    void Start()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
        zPos = transform.position.z;
        mass = 2;

        PhysicsDLLAccess.AddParticle3D(ref mass, ref xPos, ref yPos, ref zPos);
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsDLLAccess.AddForce(5.0f, 0.0f, 0.0f);
        transform.position = new Vector3(xPos, yPos, zPos);
    }
}
