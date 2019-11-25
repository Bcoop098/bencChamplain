#include "PhysicsWorld.h"

PhysicsWorld::PhysicsWorld()
{
	/*int particlePoolSize = 1024;
	for (int i = 0; i < particlePoolSize; ++i)
	{
		particle3DPool.push_back(new Particle3D());
	}*/
}

PhysicsWorld::~PhysicsWorld()
{
}

void PhysicsWorld::Update(float deltaTime)
{
	for (int i = 0; i < particle3DPool.size(); ++i)
	{
		particle3DPool[i].updateData(deltaTime);
	}
}

void PhysicsWorld::AddParticle3D(float& mass, float& xPos, float& yPos, float& zPos)
{
	particle3DPool.push_back(Particle3D(mass, xPos, yPos, zPos));
}

void PhysicsWorld::applyForce(float xForce, float yForce, float zForce)
{
	for (int i = 0; i < particle3DPool.size(); ++i)
	{
		particle3DPool[i].addForce(xForce, yForce, zForce);
	}
}

void PhysicsWorld::reset()
{
	particle3DPool.clear();
	particle3DPool.shrink_to_fit();
}

void Particle3D::updateData(float deltaTime)
{
	positionX += velocityX * deltaTime;
	positionY += velocityY * deltaTime;
	positionZ += velocityZ * deltaTime;

	velocityX += accelerationX * deltaTime;
	velocityY += accelerationY * deltaTime;
	velocityZ += accelerationZ * deltaTime;

	accelerationX = invMass * forceX;
	accelerationY = invMass * forceY;
	accelerationZ = invMass * forceZ;

	forceX = 0.0;
	forceY = 0.0;
	forceZ = 0.0;
}

void Particle3D::addForce(float xForce, float yForce, float zForce)
{
	forceX = xForce;
	forceY = yForce;
	forceZ = zForce;


}
