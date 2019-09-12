using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGenerator : MonoBehaviour
{
    public static Vector2 GenerateForce_Gravity(float gravityConst, Vector2 worldUp, float particleMass)
    {
        Vector2 f_gravity = worldUp * gravityConst * particleMass;
        return f_gravity;
    }

    public static Vector2 GenerateForce_Normal(Vector2 f_gravity, Vector2 surfaceNormal_unit)
    {
        Vector2 f_normal = Vector3.Project(f_gravity, surfaceNormal_unit.normalized);
        return f_normal;
    }

    public static Vector2 GenerateForce_Sliding(Vector2 f_gravity, Vector2 f_normal)
    {
        Vector2 f_sliding = f_gravity + f_normal;
        return f_sliding;
    }

    public static Vector2 GenerateForce_friction_static(Vector2 f_normal, Vector2 f_opposing, float frictionCoefficient_static)
    {
        float max = frictionCoefficient_static * f_normal.magnitude;
        Vector2 f_friction_s = new Vector2(0.0f,0.0f);
        if (-f_opposing.magnitude < max)
        {
            f_friction_s = -f_opposing;
        }
        else
        {
            f_friction_s = f_normal * frictionCoefficient_static;
        }

        return f_friction_s;
    }

    public static Vector2 GenerateForce_friction_kinetic(Vector2 f_normal, Vector2 particleVelocity, float frictionCoefficient_kinetic)
    {
        Vector2 f_friction_k = -frictionCoefficient_kinetic * f_normal.magnitude * particleVelocity;
        return f_friction_k;
    }

    public static Vector2 GenerateForce_drag(Vector2 particleVelocity, Vector2 fluidVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
    {
        Vector2 f_drag = (0.5f *(particleVelocity - fluidVelocity) * (particleVelocity - fluidVelocity) * objectArea_crossSection * objectDragCoefficient);
        return f_drag;
    }

    public static Vector2 GenerateForce_spring(Vector2 particlePosition, Vector2 anchorPosition, float springRestingLength, float springStiffnessCoefficient)
    {
        Vector2 f_spring = -springStiffnessCoefficient * (Vector2.Distance(particlePosition,anchorPosition) - springRestingLength) * (anchorPosition - particlePosition).normalized;
        return f_spring;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
