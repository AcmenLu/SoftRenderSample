#ifndef _MATRIX4X4_H_
#define _MATRIX4X4_H_

class Vector3;
class Vector4;
const double PIOver180 = 3.141592653 / 180.0;

struct Matrix4x4 
{
    double mElem[16];
    Matrix4x4(); //默认生成单位阵
	Matrix4x4(const Matrix4x4& _other);
    Matrix4x4(double _f11, double _f12, double _f13, double _f14,
			  double _f21, double _f22, double _f23, double _f24,
			  double _f31, double _f32, double _f33, double _f34,
			  double _f41, double _f42, double _f43, double _f44);
    ~Matrix4x4();
	double& operator[](int _n);
	Matrix4x4& operator=(const Matrix4x4& _other);
	void Zero(); 
	void Identity(); 

    static Matrix4x4 CreateScale(const Vector4& _scale);
    static Matrix4x4 CreateTrans(const Vector4& _trans);
	//旋转均用角度表示
    static Matrix4x4 CreateRotateX(double _angle);
    static Matrix4x4 CreateRotateY(double _angle);
    static Matrix4x4 CreateRotateZ(double _angle);    

	static Matrix4x4 BuildWorldTransform(
		const Vector4& _postion, 
		const Vector4& _scale, 
		double _xAngle,
		double _yAngle,
		double _zAngle);

	static Matrix4x4 BuildLookAtLH(
		const Vector4& _position,
		const Vector4& _upVector,
		const Vector4& _right);

	static Matrix4x4 BuildPerspectiveLH(
		double _fovY,
		double _aspect,
		double _zNear,
		double _zFar);

	static Matrix4x4 BuildViewPort(
		double _x,
		double _y,
		double _width,
		double _height,
		double _minZ,
		double _maxZ);

	void operator*=(Matrix4x4& _rMaT);
	Matrix4x4 operator*(Matrix4x4& _rMat);

	static Matrix4x4 RotateOnAxis(Vector4& _axis, double _angle);
};

Matrix4x4 operator+(Matrix4x4& _lhs, Matrix4x4& _rhs);
Matrix4x4 operator-(Matrix4x4& _lhs, Matrix4x4& _rhs);

#endif