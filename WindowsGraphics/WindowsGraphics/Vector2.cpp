#include <math.h>
#include "Vector2.h"

Vector2::Vector2()
    : mX(0), mY(0)
{/*empty*/}

Vector2::Vector2(double _x, double _y)
    : mX(_x),
      mY(_y) 
{/*empty*/}

Vector2::~Vector2()
{/*empty*/}

double Vector2::GetX() const
{
    return mX;
}

double Vector2::GetY() const
{
    return mY;
}

void Vector2::SetX(double _x)
{
	mX = _x;
}

void Vector2::SetY(double _y)
{
	mY = _y;
}

Vector2 operator+(const Vector2& _lhs, const Vector2& _rhs)
{
	Vector2 newVec;
	newVec.SetX(_lhs.GetX() + _rhs.GetX());
	newVec.SetY(_lhs.GetY() + _rhs.GetY());
    return newVec;
}

Vector2 operator-(const Vector2& _lhs, const Vector2& _rhs)
{
	Vector2 newVec;
	newVec.SetX(_lhs.GetX() - _rhs.GetX());
	newVec.SetY(_lhs.GetY() - _rhs.GetY());
    return newVec;
}

Vector2 Vector2::Rotate(double _phi) const
{
    return Vector2(
        cos(_phi) * mX - sin(_phi) * mY,
        sin(_phi) * mX + cos(_phi) * mY
    );
}