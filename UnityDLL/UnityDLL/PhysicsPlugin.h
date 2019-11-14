#pragma once

extern "C"
{
	__declspec(dllexport) void CreatePhysicsWorld();
	__declspec(dllexport) void DestroyPhysicsWorld();

	__declspec(dllexport) void UpdatePhysicsWorld(float deltaTime);
}