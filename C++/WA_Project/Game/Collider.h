#pragma once
#include "Component.h"
class Collider : public Component
{
public:
    Collider();
    ~Collider();
public:
    // Component을(를) 통해 상속됨
    void LateUpdate() override;
    void Render(HDC _hdc) override;
public:
    void EnterCollision(Collider* _other); // 충돌진입
    void StayCollision(Collider* _other); // 충돌중
    void ExitCollision(Collider* _other); // 충돌해제
    const UINT& GetID() const { return m_ID; }
public:
    void SetSize(Vector2 _vSize) { m_vSize = _vSize; }
    const Vector2& GetSize() const { return m_vSize; }
    void SetOffSetPos(Vector2 _vOffsetPos)
    {
        m_vOffsetPos = _vOffsetPos;
    }
    const Vector2& GetOffSetPos() const { return m_vOffsetPos; }
    const Vector2& GetLatedUpatedPos() const
    {
        return m_vLatePos;
    }
private:
    bool m_showDebug = false;
    UINT m_ID; // 충돌체 고유 ID값
    static UINT m_sNextID;

    Vector2 m_vSize; // 콜라이더 크기
    // Object로부터 상대적인 위치
    Vector2 m_vOffsetPos;
    // LateUpdate에서 매 프레임마다 오브젝트로부터 계산되는 위치
    Vector2 m_vLatePos;

};

