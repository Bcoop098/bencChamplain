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
}