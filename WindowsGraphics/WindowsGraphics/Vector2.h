#ifndef _VECTOR2_H_
#define _VECTOR2_H_

class Vector2
{
public:
	Vector2();
    Vector2(double _x, double _y);
    ~Vector2();
    Vector2 Rotate(double _phi) const;
    double GetX() const;
    double GetY() const;
	void SetX(double _x);
	void SetY(double _y);

private:
    double mX;
    double mY;
};

Vector2 operator+(const Vector2& _lhs, const Vector2& _rhs);
Vector2 operator-(const Vector2& _lhs, const Vector2& _rhs);

#endif