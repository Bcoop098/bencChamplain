#pragma once
#include <vector>

class Particle3D {};

class PhysicsWorld
{
public:
	explicit PhysicsWorld();
	~PhysicsWorld();

	void Update(float deltaTime);

	void AddParticle3D(float mass, float boxHeight, float boxWidth, float boxLength);

private:

	std::vector<Particle3D*> particle3DPool;

};