#include<crtdbg.h>

#include "pch.h"
#include "BaseWindow.h"
#include "Scene.h"

int APIENTRY wWinMain(
	_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPWSTR lpCmdLine,
	_In_ int nCmdShow)
{
#pragma region memory checking
	//_CrtSetBreakAlloc(306);
	//_CRTDBG_ALLOC_MEM_DF; // 할당
	_CrtSetDbgFlag(_CRTDBG_LEAK_CHECK_DF | _CRTDBG_ALLOC_MEM_DF);
	//_CrtSetBreakAlloc(번호);  
#pragma endregion

	BaseWindow game;
	game.Run(hInstance, lpCmdLine, nCmdShow);
}