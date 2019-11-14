#include "PhysicsWorld.h"

PhysicsWorld::PhysicsWorld()
{
	int particlePoolSize = 1024;
	for (int i = 0; i < particlePoolSize; ++i)
	{
		particle3DPool.push_back(new Particle3D());
	}
}

PhysicsWorld::~PhysicsWorld()
{
}

void PhysicsWorld::Update(float deltaTime)
{
}

void PhysicsWorld::AddParticle3D(float mass, float boxHeight, float boxWidth, float boxLength)
{
	//create a new particle3D
}
