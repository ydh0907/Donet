#include "pch.h"
#include "InitializeScene.h"
#include "ImageButton.h"

void InitializeScene::Init()
{
	ImageButton* btn = new ImageButton(Vector2(100, 100), Vector2::zero, this, Matching);
	btn->SetPos({ SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 });
	AddObject(btn, LAYER::UI);
}
