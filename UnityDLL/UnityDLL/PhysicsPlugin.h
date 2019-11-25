#pragma once

extern "C"
{
	__declspec(dllexport) void CreatePhysicsWorld();
	__declspec(dllexport) void DestroyPhysicsWorld();

	__declspec(dllexport) void UpdatePhysicsWorld(float deltaTime);
	__declspec(dllexport) void AddParticle3D(float& mass, float& xPos, float& yPos, float& zPos);
	__declspec(dllexport) void AddForce(float xForce, float yForce, float zForce);
}