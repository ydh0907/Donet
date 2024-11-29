#pragma once

class Collider;
class Component;

class Object
{
public:
	Object();
	virtual ~Object();
public:
	virtual void Update() abstract;
	virtual void LateUpdate();
	virtual void Render(HDC hdc) abstract;
	void ComponentRender(HDC hdc);
public:
	void SetPos(Vector2 newPos) { pos = newPos; }
	void SetSize(Vector2 newSize) { size = newSize; }
	const Vector2& GetPos() const { return pos; }
	const Vector2& GetSize() const { return size; }
public:
	virtual void EnterCollision(Collider* other);
	virtual void StayCollision(Collider* other);
	virtual void ExitCollision(Collider* other);
	const bool& GetIsDead() const { return isDie; }
	void SetDead() { isDie = true; }
	void SetName(wstring newName) { name = newName; }
	const wstring& GetName() const { return name; }

private:
	bool isDie;
	wstring name;
public:
	template<typename T>
	void AddComponent()
	{
		T* com = new T;
		com->SetOwner(this);
		components.push_back(com);
	}
	template<typename T>
	T* GetComponent()
	{
		T* component = nullptr;
		for (Component* com : components)
		{
			component = dynamic_cast<T*>(com);
			if (component)
				break;
		}
		return component;
	}
private:
	Vector2 pos;
	Vector2 size;
	vector<Component*> components;
};

