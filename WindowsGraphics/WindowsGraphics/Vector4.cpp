#include <math.h>
#include "Vector4.h"
//w = 0时表示向量 w = 1时表示点
Vector4::Vector4()
    : mX(0), mY(0), mZ(0), mW(0)
{/*empty*/}

Vector4::Vector4(double _x, double _y, double _z, double _w)
    : mX(_x), mY(_y), mZ(_z), mW(_w)
{/*empty*/}

Vector4::Vector4(const Vector4& _other)
    : mX(_other.mX),
      mY(_other.mY),
      mZ(_other.mZ),
      mW(_other.mW)    
{/*empty*/}

Vector4::~Vector4()
{/*empty*/}

Vector4& Vector4::Reverse()
{
	mX = -mX;
	mY = -mY;
	mZ = -mZ;
	return *this;
}

Vector4& Vector4::operator=(const Vector4& _other)
{
    if (this == &_other)
    {
        return *this;
    }
    else
    {
        mX = _other.mX;
        mY = _other.mY;
        mZ = _other.mZ;
        mW = _other.mW;
    }
    return *this;
}

Vector4& Vector4::operator+=(const Vector4& _other)
{
    mX += _other.mX;
    mY += _other.mY;
    mZ += _other.mZ;
    mW += _other.mW;
    return *this;
}

Vector4& Vector4::operator+=(double _f)
{
	mX += _f;
	mY += _f;
	mZ += _f;
	mW += _f;
	return *this;
}

Vector4 Vector4::operator*(double _f)
{
	return Vector4(
		mX * _f,
		mY * _f,
		mZ * _f,
		mW * _f);
}

Vector4 operator+(const Vector4& _lhs, const Vector4& _rhs)
{
	Vector4 newVec;
	newVec.mX = _lhs.mX + _rhs.mX;
	newVec.mY = _lhs.mY + _rhs.mY;
	newVec.mZ = _lhs.mZ + _rhs.mZ;
	newVec.mW = _lhs.mW + _rhs.mZ;
    return newVec;
}

Vector4 operator-(const Vector4& _lhs, const Vector4& _rhs)
{
	Vector4 newVec;
	newVec.mX = _lhs.mX - _rhs.mX;
	newVec.mY = _lhs.mY - _rhs.mY;
	newVec.mZ = _lhs.mZ - _rhs.mZ;
	newVec.mW = _lhs.mW - _rhs.mW;
    return newVec;
}

double Vector4::Dot(const Vector4& _other)
{
    return mX * _other.mX + 
           mY * _other.mY + 
           mZ * _other.mZ;
}

double operator*(const Vector4& _lhs, const Vector4& _rhs)
{
	return _lhs.mX * _rhs.mX +
		   _lhs.mY * _rhs.mY +
		   _lhs.mZ * _rhs.mZ;
}

//实现三维叉乘算法，W不参与计算
//叉乘一个Vector4
Vector4 Vector4::CrossProduct(const Vector4& _other) const
{
    return Vector4(
        mY * _other.mZ - mZ * _other.mY,
        mZ * _other.mX - mX * _other.mZ,
        mX * _other.mY - mY * _other.mX,
        mW
    );
}

Vector4 Vector4::operator/(double _f)
{
	return Vector4(mX / _f, mY / _f, mZ / _f, mW / _f);
}

void Vector4::DivideW()
{
	if (mW == 0) return ;
	mX /= mW;
	mY /= mW;
	mZ /= mW;
	mW = 1;
}

//乘一个4x4矩阵，横向量向量在左，矩阵在右
Vector4 Vector4::MatrixMulti(Matrix4x4& _matrix)
{
    return Vector4(
        mX * _matrix[0] + mY * _matrix[4] + mZ * _matrix[8] + mW * _matrix[12],
        mX * _matrix[1] + mY * _matrix[5] + mZ * _matrix[9] + mW * _matrix[13],
        mX * _matrix[2] + mY * _matrix[6] + mZ * _matrix[10] + mW * _matrix[14],
        mX * _matrix[3] + mY * _matrix[7] + mZ * _matrix[11] + mW * _matrix[15]        
    );
}

void Vector4::AngleToRadian()
{
	mX *= PIOver180;
	mY *= PIOver180;
	mZ *= PIOver180;
	mW *= PIOver180;
}

double Vector4::GetLength() const
{
	return sqrt(mX * mX + mY * mY + mZ * mZ);
}

Vector4& Vector4::Normalize()
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