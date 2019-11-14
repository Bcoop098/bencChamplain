#include "PhysicsPlugin.h"
#include "PhysicsWorld.h"

PhysicsWorld* PhysicsWorldInstance = nullptr;
 void CreatePhysicsWorld()
{
	if (PhysicsWorldInstance == nullptr)
	{
		PhysicsWorldInstance = new PhysicsWorld;
	}
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
