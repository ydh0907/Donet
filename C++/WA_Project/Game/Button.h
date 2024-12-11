#pragma once
#include "Component.h"

class Button : public Component
{
private:
	Vector2 offset;
	Vector2 size;
	void* sender;
	void(*action)(void*);
	bool isin = false;
public:
	Button(Vector2 size, Vector2 offset, void* sender, void(*action)(void*)) {
		this->size = size;
		this->offset = offset;
		this->sender = sender;
		this->action = action;
	}
	~Button() {

	}
public:
	void LateUpdate() override;
	void Render(HDC _hdc) override;
};