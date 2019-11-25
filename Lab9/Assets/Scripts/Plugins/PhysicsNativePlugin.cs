using System.Collections;
using System.Collections.Generic;

using System.Runtime.InteropServices;
public class PhysicsDLLAccess
{
	[DllImport("UnityDLL")]
	public static extern void CreatePhysicsWorld();
	[DllImport("UnityDLL")]
	public static extern void DestroyPhysicsWorld();
	[DllImport("UnityDLL")]
	public static extern void UpdatePhysicsWorld(float deltaTime);

    [DllImport("UnityDLL")]
    public static extern void AddParticle3D(ref float mass, ref float xPos, ref float yPos, ref float zPos);

    [DllImport("UnityDLL")]
    public static extern void AddForce(float xForce, float yForce, float zForce);

}