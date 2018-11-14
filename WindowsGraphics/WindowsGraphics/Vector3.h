#ifndef _VECTOR3_H_
#define _VECTOR3_H_

class Vector3 
{
public:
    Vector3();
    Vector3(double _x, double _y, double _z);
    Vector3(const Vector3& _other);
    ~Vector3();
    Vector3& operator=(const Vector3& _other);
    double Dot(const Vector3& _other);//点积
    Vector3 CrossProduct(const Vector3& _other) const;//叉积
    double GetX() const;
    double GetY() const;
    double GetZ() const;
	void SetX(double _x);
	void SetY(double _y);
	void SetZ(double _z);
	double GetLength() const;
	Vector3& Normalize();

private:
    double mX;
    double mY;
    double mZ;
};

Vector3 operator+(const Vector3& _lhs, const Vector3& _rhs);
Vector3 operator-(const Vector3& _lhs, const Vector3& _rhs);

#endif