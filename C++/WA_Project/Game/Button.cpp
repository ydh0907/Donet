#include "pch.h"
#include "Button.h"
#include "Object.h"
#include "InputManager.h"
#include "GDISelector.h"

void Button::LateUpdate()
{
	if (Single(InputManager)->GetKey(KEY_TYPE::LBUTTON) == KEY_STATE::UP) {
		Vector2 mPos = Single(InputManager)->GetMousePos();
		Vector2 center = GetOwner()->GetPos() + offset;

		bool x = center.x - size.x / 2 < mPos.x && mPos.x < center.x + size.x / 2;
		bool y = center.y - size.y / 2 < mPos.y && mPos.y < center.y + size.y / 2;

		if (x && y) {
			action(sender);
		}
	}
}

void Button::Render(HDC _hdc)
{
	PEN_TYPE ePen = PEN_TYPE::RED;

	Vector2 mPos = Single(InputManager)->GetMousePos();
	Vector2 center = GetOwner()->GetPos() + offset;

	bool x = center.x - size.x / 2 < mPos.x && mPos.x < center.x + size.x / 2;
	bool y = center.y - size.y / 2 < mPos.y && mPos.y < center.y + size.y / 2;

	isin = x && y;
	if (isin)
		ePen = PEN_TYPE::GREEN;

	GDISelector pen(_hdc, ePen);
	GDISelector brush(_hdc, BRUSH_TYPE::HOLLOW);

	RECT_RENDER(_hdc, center.x, center.y, size.x, size.y);
}
