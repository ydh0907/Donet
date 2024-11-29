#pragma once
#include<assert.h>
struct Vector2
{
public:
	Vector2() = default;
	Vector2(float _x, float _y) : x(_x), y(_y) {}
	Vector2(POINT _pt) : x((float)_pt.x), y((float)_pt.y) {}
	Vector2(int _x, int _y) : x((float)_x), y((float)_y) {}
	Vector2(const Vector2& _other) : x(_other.x), y(_other.y) {}
public:
	Vector2 operator + (const Vector2& _vOther)
	{
		return Vector2(x + _vOther.x, y + _vOther.y);
	}
	Vector2 operator - (const Vector2& _vOther)
	{
		return Vector2(x - _vOther.x, y - _vOther.y);
	}
	Vector2 operator * (const Vector2& _vOther)
	{
		return Vector2(x * _vOther.x, y * _vOther.y);
	}
	Vector2 operator * (float _val)
	{
		return Vector2(x * _val, y * _val);
	}
	Vector2 operator / (const Vector2& _vOther)
	{
		assert(!(0.f == _vOther.x || 0.f == _vOther.y));
		return Vector2(x / _vOther.x, y / _vOther.y);
	}
	void operator+=(const Vector2& _other)
	{
		x += _other.x;
		y += _other.y;
	}
	void operator-=(const Vector2& _other)
	{
		x -= _other.x;
		y -= _other.y;
	}
	float LengthSquared()
	{
		return x * x + y * y;
	}
	float Length()
	{
		return ::sqrt(LengthSquared());
	}
	void Normalize()
	{
		float len = Length();
		// 0이면 안돼.
		if (len < FLT_EPSILON)
			return;
		x /= len;
		y /= len;
	}
	float Dot(Vector2 _other)
	{
		return x * _other.x + y * _other.y;
	}
	float Cross(Vector2 _other)
	{
		// z축이 나온다고 가정
		return x * _other.y - y * _other.x;
	}
public:
	float x = 0.f;
	float y = 0.f;
};