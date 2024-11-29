#pragma once

class Collider;

union COLLIDER_ID
{
	struct
	{
		UINT left_ID;
		UINT right_ID;
	};
	ULONGLONG ID;
};

class CollisionManager
{
	DECLARE_SINGLE(CollisionManager);
public:
	void Update();
	void CheckLayer(LAYER _left, LAYER _right);
	void CheckReset();
private:
	void CollisionLayerUpdate(LAYER _left, LAYER _right);
	bool IsCollision(Collider* _left, Collider* _right);
private:
	// 그룹 사이의 충돌 정의
	UINT m_arrLayer[(UINT)LAYER::END];
	map<ULONGLONG, bool> m_mapCollisionInfo;
};