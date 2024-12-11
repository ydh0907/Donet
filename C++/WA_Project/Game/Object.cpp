#include "pch.h"
#include "Object.h"
#include "TimeManager.h"
#include "InputManager.h"
#include "Component.h"

Object::Object() : isDie(false)
{
}

Object::~Object()
{
	for (Component* com : components)
	{
		if (com != nullptr) {
			delete com;
		}
	}
	components.clear();
}

void Object::LateUpdate()
{
	for (Component* com : components)
	{
		if (com)
		{
			com->LateUpdate();
		}
	}
}

void Object::ComponentRender(HDC _hdc)
{
	for (Component* com : components)
	{
		if (com)
		{
			com->Render(_hdc);
		}
	}
}

void Object::EnterCollision(Collider* _other)
{
}

void Object::StayCollision(Collider* _other)
{
}

void Object::ExitCollision(Collider* _other)
{
}
