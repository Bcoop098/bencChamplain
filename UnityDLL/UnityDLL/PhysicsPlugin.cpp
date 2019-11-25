#include "PhysicsPlugin.h"
#include "PhysicsWorld.h"

PhysicsWorld* PhysicsWorldInstance = nullptr;
 void CreatePhysicsWorld()
{
	if (PhysicsWorldInstance == nullptr)
	{
		PhysicsWorldInstance = new PhysicsWorld;
	}
	PhysicsWorldInstance->reset();
}

 void DestroyPhysicsWorld()
{
	if (PhysicsWorldInstance != nullptr)
	{
		delete PhysicsWorldInstance;
		PhysicsWorldInstance = nullptr;
	}
}

 void UpdatePhysicsWorld(float deltaTime)
{
	if (PhysicsWorldInstance != nullptr)
	{
		PhysicsWorldInstance->Update(deltaTime);
	}
}

 void AddParticle3D(float& mass, float& xPos, float& yPos, float& zPos)
 {
	 if (PhysicsWorldInstance != nullptr)
	 {
		 PhysicsWorldInstance->AddParticle3D(mass, xPos, yPos, zPos);
	 }
 }

 void AddForce(float xForce, float yForce, float zForce)
 {
	 if (PhysicsWorldInstance != nullptr)
	 {
		 PhysicsWorldInstance->applyForce(xForce, yForce, zForce);
	 }
 }
