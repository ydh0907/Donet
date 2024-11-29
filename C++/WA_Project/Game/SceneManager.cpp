#include "pch.h"
#include "SceneManager.h"
#include "Scene.h"
#include "InitializeScene.h"

void SceneManager::Init()
{
	currentScene = nullptr;

	// ¾À µî·Ï
	RegisterScene(L"InitializeScene", std::make_shared<InitializeScene>());

	// ¾À ·Îµå
	LoadScene(L"InitializeScene");
}

void SceneManager::Update()
{
	if (currentScene == nullptr)
		return;
	currentScene->Update();
	currentScene->LateUpdate();
}

void SceneManager::Render(HDC _hdc)
{
	if (currentScene == nullptr)
		return;
	currentScene->Render(_hdc);
}

void SceneManager::RegisterScene(const wstring& sceneName, std::shared_ptr<Scene> scene)
{
	if (sceneName.empty() || scene == nullptr)
		return;
	sceneMap.insert(sceneMap.end(), { sceneName, scene });
}

void SceneManager::LoadScene(const wstring& _sceneName)
{
	// ¾ÀÀÌ ÀÖÀ¸¸é
	if (currentScene != nullptr)
	{
		currentScene->Release();
		currentScene = nullptr;
	}

	auto iter = sceneMap.find(_sceneName);
	if (iter != sceneMap.end())
	{
		currentScene = iter->second;
		currentScene->Init();
	}
}
