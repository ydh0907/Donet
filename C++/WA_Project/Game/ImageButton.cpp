#include "pch.h"
#include "ImageButton.h"
#include "Button.h"

ImageButton::ImageButton(Vector2 size, Vector2 offset, void* sender, void(*action)(void*))
{
	AddComponent(new Button(size, offset, sender, action));
}

ImageButton::~ImageButton()
{
}

void ImageButton::Update()
{
}

void ImageButton::Render(HDC hdc)
{
}
