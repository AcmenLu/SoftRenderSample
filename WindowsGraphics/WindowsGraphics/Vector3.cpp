#include <math.h>
#include "Vector3.h"

Vector3::Vector3()
    : mX(0), mY(0), mZ(0)
{/*empty*/}

Vector3::Vector3(double _x, double _y, double _z)
    : mX(_x), mY(_y), mZ(_z)
{/*empty*/}

Vector3::Vector3(const Vector3& _other)
    : mX(_other.GetX()),
      mY(_other.GetY()),
      mZ(_other.GetZ())    
{/*empty*/}

Vector3::~Vector3()
{/*empty*/}

Vector3& Vector3::operator=(const Vector3& _other)
{
    if (this == &_other)
    {
        return *this;
    }
    else
    {
        mX = _other.GetX();
        mY = _other.GetY();
        mZ = _other.GetZ();
    }
    return *this;
}

Vector3 operator+(const Vector3& _lhs, const Vector3& _rhs)
{
	Vector3 newVec;
	newVec.SetX(_lhs.GetX() + _rhs.GetX());
	newVec.SetY(_lhs.GetY() + _rhs.GetY());
	newVec.SetZ(_lhs.GetZ() + _rhs.GetZ());
    return newVec;
}

Vector3 operator-(const Vector3& _lhs, const Vector3& _rhs)
{
	Vector3 newVec;
	newVec.SetX(_lhs.GetX() - _rhs.GetX());
	newVec.SetY(_lhs.GetY() - _rhs.GetY());
	newVec.SetZ(_lhs.GetZ() - _rhs.GetZ());
    return newVec;
}

double Vector3::Dot(const Vector3& _other)
{
    return mX * _other.GetX() + mY * _other.GetY() + mZ * _other.GetZ();
}

Vector3 Vector3::CrossProduct(const Vector3& _other) const
{
    return Vector3(
        mY * _other.GetZ() - mZ * _other.GetY(),
        mZ * _other.GetX() - mX * _other.GetZ(),
        mX * _other.GetY() - mY * _other.GetX()
    );
}

double Vector3::GetX() const
{
    return mX;
}

double Vector3::GetY() const
{
    return mY;
}

double Vector3::GetZ() const
{
    return mZ;
}

void Vector3::SetX(double _x)
{
	mX = _x;
}

void Vector3::SetY(double _y)
{
	mY = _y;
}

void Vector3::SetZ(double _z)
{
	mZ = _z;
}

double Vector3::GetLength() const
{
	return sqrt(mX * mX + mY * mY + mZ * mZ);
}

Vector3& Vector3::Normalize()
{
	double fLength = GetLength();
	if (fLength > 0.0)
	{
		double fInvLength = 1.0 / fLength;
		mX *= fInvLength;
		mY *= fInvLength;
		mZ *= fInvLength;
	}
	return *this;
}