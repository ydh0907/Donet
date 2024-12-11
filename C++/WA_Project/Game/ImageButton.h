#pragma once
#include "Object.h"

class ImageButton : public Object
{
public:
	ImageButton(Vector2 size, Vector2 offset, void* sender, void(*action)(void*));
	~ImageButton() override;
public:
	void Update() override;
	void Render(HDC hdc) override;
};
