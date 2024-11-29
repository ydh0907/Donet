#include "pch.h"
#include "Object.h"
#include "TimeManager.h"
#include "InputManager.h"
#include "Component.h"

Object::Object()
	: pos{}
	, size{}
	, isDie(false)
{
}

Object::~Object()
{
	for (Component* com : components)
	{
		if (com != nullptr)
			delete com;
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

//void Object::Update()
//{
//	if (GET_KEY(KEY_TYPE::LEFT))
//		m_vPos.x -= 100.f * fDT;
//	if (GET_KEY(KEY_TYPE::RIGHT))
//		m_vPos.x += 100.f * fDT;
//}
//
//void Object::Render(HDC _hdc)
//{
//	RECT_RENDER(_hdc, m_vPos.x, m_vPos.y
//		, m_vSize.x, m_vSize.y);
//}
