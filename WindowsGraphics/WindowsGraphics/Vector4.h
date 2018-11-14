#ifndef _VECTOR4_H_
#define _VECTOR4_H_

#include "Matrix4x4.h"

class Vector4 
{
public:
    Vector4();
    Vector4(double _x, double _y, double _z, double _w);
    Vector4(const Vector4& _other);
    ~Vector4();
    Vector4& operator=(const Vector4& _other);
	Vector4& operator+=(double _f);
    Vector4& operator+=(const Vector4& _other);
	Vector4 operator*(double _f);
	Vector4& Reverse();
    double Dot(const Vector4& _other);//点积
    Vector4 CrossProduct(const Vector4& _other) const;//叉积  
    Vector4 MatrixMulti(Matrix4x4& _matrix);//左乘一个矩阵  
	void AngleToRadian();
	void DivideW();
	double GetLength() const;
	Vector4& Normalize();
	Vector4 operator/(double _f);

    double mX;
    double mY;
    double mZ;
    double mW;
};

Vector4 operator+(const Vector4& _lhs, const Vector4& _rhs);
Vector4 operator-(const Vector4& _lhs, const Vector4& _rhs);
double operator*(const Vector4& _lhs, const Vector4& _rhs);

#endif