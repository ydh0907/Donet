#pragma once

class Scene;

class SceneManager
{
	DECLARE_SINGLE(SceneManager);
public:
	void Init();
	void Update();
	void Render(HDC _hdc);
	void Release();
public:
	void RegisterScene(const wstring& sceneName,
		std::shared_ptr<Scene> scene);
	void LoadScene(const wstring& sceneName);
public:
	const std::shared_ptr<Scene>& GetCurrentScene() const
	{
		return currentScene;
	}
private:
	map<wstring, std::shared_ptr<Scene>> sceneMap;
	std::shared_ptr<Scene> currentScene;
};
