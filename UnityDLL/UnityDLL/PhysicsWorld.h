#pragma once
#include <vector>

class Particle3D 
{
public:
	float& mass;
	float &positionX;
	float &positionY;
	float &positionZ;

	float velocityX = 0.0;
	float velocityY = 0.0;
	float velocityZ = 0.0;

	float accelerationX = 0.0;
	float accelerationY = 0.0;
	float accelerationZ = 0.0;

	float forceX = 0.0;
	float forceY = 0.0;
	float forceZ = 0.0;
	float invMass = 0.0;

	Particle3D(float& mass, float& xPos, float& yPos, float& zPos) : mass(mass), positionX(xPos), positionY(yPos), positionZ(zPos)
	{
		invMass = mass > 0.0f ? 1.0f / mass : 0.0f;
	}

	void updateData(float deltaTime);
	void addForce(float xForce, float yForce, float zForce);
};

class PhysicsWorld
{
public:
	explicit PhysicsWorld();
	~PhysicsWorld();

	void Update(float deltaTime);

	void AddParticle3D(float& mass, float& xPos, float& yPos, float& zPos);

	void applyForce(float xForce,float yForce, float zForce);

	void reset();

private:

	std::vector<Particle3D> particle3DPool;

};