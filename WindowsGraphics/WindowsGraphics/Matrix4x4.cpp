#include <math.h>
#include <string.h>
#include "Matrix4x4.h"
#include "Vector3.h"
#include "Vector4.h"
//都是基于左手坐标系的变换
//向量与矩阵乘法都是向量在左，矩阵在右
Matrix4x4::Matrix4x4()
{
    memset(&this->mElem, 0, sizeof(this->mElem));
	mElem[0] = 1;
	mElem[5] = 1;
	mElem[10] = 1;
	mElem[15] = 1;
}

Matrix4x4::Matrix4x4(
	double _f11, double _f12, double _f13, double _f14,
	double _f21, double _f22, double _f23, double _f24,
	double _f31, double _f32, double _f33, double _f34,
	double _f41, double _f42, double _f43, double _f44)
{
	mElem[0] = _f11;
	mElem[1] = _f12;
	mElem[2] = _f13;
	mElem[3] = _f14;
	mElem[4] = _f21;
	mElem[5] = _f22;
	mElem[6] = _f23;
	mElem[7] = _f24;
	mElem[8] = _f31;
	mElem[9] = _f32;
	mElem[10] = _f33;
	mElem[11] = _f34;
	mElem[12] = _f41;
	mElem[13] = _f42;
	mElem[14] = _f43;
	mElem[15] = _f44;
}

Matrix4x4::Matrix4x4(const Matrix4x4& _other)
{
	*this = _other;
}

Matrix4x4& Matrix4x4::operator=(const Matrix4x4& _other)
{
	if (this != &_other)
	{
		memmove(mElem, _other.mElem, 16 * sizeof(double));
	}
	return *this;
}

Matrix4x4::~Matrix4x4()
{/*empty*/}

//清空数组
void Matrix4x4::Zero()
{
	memset(mElem, 0, 16 * sizeof(double));
}

//将矩阵设为单位矩阵
void Matrix4x4::Identity()
{
	Zero();
	mElem[0] = 1;
	mElem[5] = 1;
	mElem[10] = 1;
	mElem[15] = 1;
}

Matrix4x4 operator+(Matrix4x4& _lhs, Matrix4x4& _rhs)
{
    Matrix4x4 newMatrix;
	for (int i = 0; i < 16; ++i)
	{
		newMatrix[i] = _lhs[i] + _rhs[i];
	}
    return newMatrix;
}

Matrix4x4 operator-(Matrix4x4& _lhs, Matrix4x4& _rhs)
{
    Matrix4x4 newMatrix;
	for (int i = 0; i < 16; ++i)
	{
		newMatrix[i] = _lhs[i] - _rhs[i];
	}
    return newMatrix;
}

Matrix4x4 Matrix4x4::operator*(Matrix4x4& _rMat)
{
	//Shorten the syntax a bit
	const double *m1 = mElem;
	const double *m2 = _rMat.mElem;

	return Matrix4x4(
		(m1[0] * m2[0] + m1[4] * m2[1] + m1[8] * m2[2] + m1[12] * m2[3]),
		(m1[1] * m2[0] + m1[5] * m2[1] + m1[9] * m2[2] + m1[13] * m2[3]),
		(m1[2] * m2[0] + m1[6] * m2[1] + m1[10] * m2[2] + m1[14] * m2[3]),
		(m1[3] * m2[0] + m1[7] * m2[1] + m1[11] * m2[2] + m1[15] * m2[3]),

		(m1[0] * m2[4] + m1[4] * m2[5] + m1[8] * m2[6] + m1[12] * m2[7]),
		(m1[1] * m2[4] + m1[5] * m2[5] + m1[9] * m2[6] + m1[13] * m2[7]),
		(m1[2] * m2[4] + m1[6] * m2[5] + m1[10] * m2[6] + m1[14] * m2[7]),
		(m1[3] * m2[4] + m1[7] * m2[5] + m1[11] * m2[6] + m1[15] * m2[7]),

		(m1[0] * m2[8] + m1[4] * m2[9] + m1[8] * m2[10] + m1[12] * m2[11]),
		(m1[1] * m2[8] + m1[5] * m2[9] + m1[9] * m2[10] + m1[13] * m2[11]),
		(m1[2] * m2[8] + m1[6] * m2[9] + m1[10] * m2[10] + m1[14] * m2[11]),
		(m1[3] * m2[8] + m1[7] * m2[9] + m1[11] * m2[10] + m1[15] * m2[11]),

		(m1[0] * m2[12] + m1[4] * m2[13] + m1[8] * m2[14] + m1[12] * m2[15]),
		(m1[1] * m2[12] + m1[5] * m2[13] + m1[9] * m2[14] + m1[13] * m2[15]),
		(m1[2] * m2[12] + m1[6] * m2[13] + m1[10] * m2[14] + m1[14] * m2[15]),
		(m1[3] * m2[12] + m1[7] * m2[13] + m1[11] * m2[14] + m1[15] * m2[15])
	);
}

void Matrix4x4::operator*=(Matrix4x4& _rMat)
{
	double fNewMat[16];
	//Shorten the syntax a bit
	const double *m1 = mElem;
	const double *m2 = _rMat.mElem;

	fNewMat[0] = m1[0] * m2[0] + m1[4] * m2[1] + m1[8] * m2[2] + m1[12] * m2[3];
	fNewMat[1] = m1[1] * m2[0] + m1[5] * m2[1] + m1[9] * m2[2] + m1[13] * m2[3];
	fNewMat[2] = m1[2] * m2[0] + m1[6] * m2[1] + m1[10] * m2[2] + m1[14] * m2[3];
	fNewMat[3] = m1[3] * m2[0] + m1[7] * m2[1] + m1[11] * m2[2] + m1[15] * m2[3];

	fNewMat[4] = m1[0] * m2[4] + m1[4] * m2[5] + m1[8] * m2[6] + m1[12] * m2[7];
	fNewMat[5] = m1[1] * m2[4] + m1[5] * m2[5] + m1[9] * m2[6] + m1[13] * m2[7];
	fNewMat[6] = m1[2] * m2[4] + m1[6] * m2[5] + m1[10] * m2[6] + m1[14] * m2[7];
	fNewMat[7] = m1[3] * m2[4] + m1[7] * m2[5] + m1[11] * m2[6] + m1[15] * m2[7];

	fNewMat[8] = m1[0] * m2[8] + m1[4] * m2[9] + m1[8] * m2[10] + m1[12] * m2[11];
	fNewMat[9] = m1[1] * m2[8] + m1[5] * m2[9] + m1[9] * m2[10] + m1[13] * m2[11];
	fNewMat[10] = m1[2] * m2[8] + m1[6] * m2[9] + m1[10] * m2[10] + m1[14] * m2[11];
	fNewMat[11] = m1[3] * m2[8] + m1[7] * m2[9] + m1[11] * m2[10] + m1[15] * m2[11];

	fNewMat[12] = m1[0] * m2[12] + m1[4] * m2[13] + m1[8] * m2[14] + m1[12] * m2[15];
	fNewMat[13] = m1[1] * m2[12] + m1[5] * m2[13] + m1[9] * m2[14] + m1[13] * m2[15];
	fNewMat[14] = m1[2] * m2[12] + m1[6] * m2[13] + m1[10] * m2[14] + m1[14] * m2[15];
	fNewMat[15] = m1[3] * m2[12] + m1[7] * m2[13] + m1[11] * m2[14] + m1[15] * m2[15];

	memcpy(mElem, fNewMat, 16 * sizeof(double));
}

double& Matrix4x4::operator[](int _n)
{
	return mElem[_n];
}

//todo: 旋转改成四元数，由方向向量和旋转角生成四元数
//再由四元数生成旋转矩阵
//生成由模型坐标系到世界坐标系的变换矩阵
//WorldTransform
Matrix4x4 Matrix4x4::BuildWorldTransform(
	const Vector4& _position, //在世界坐标系中的位置
	const Vector4& _scale, //缩放系数
	double _xAngle, //绕x轴旋转的角度
	double _yAngle, //绕y轴旋转的角度
	double _zAngle) //绕z轴旋转的角度
{
	Matrix4x4 newMatrix;
	//注意顺序：先旋转，再缩放，最后平移
	newMatrix *= CreateRotateX(_xAngle);
	newMatrix *= CreateRotateX(_xAngle);
	newMatrix *= CreateRotateX(_xAngle);
	newMatrix *= CreateScale(_scale);
	newMatrix *= CreateTrans(_position);
	return newMatrix;
}

//生成从世界坐标系到观察坐标系的变换矩阵
//ViewTransform 左手坐标系
Matrix4x4 Matrix4x4::BuildLookAtLH(
	const Vector4& _position, //摄像机在世界坐标系中的位置
	const Vector4& _upVector, //相机上方向
	const Vector4& _right) //相机右方向
{
	Vector4 zAxis = _right.CrossProduct(_upVector);
	zAxis.Normalize();
	Vector4 xAxis = _right;
	xAxis.Normalize();
	Vector4 yAxis = zAxis.CrossProduct(xAxis);

	Matrix4x4 newMatrix;
	newMatrix[0] = xAxis.mX; 
	newMatrix[1] = yAxis.mX;
	newMatrix[2] = zAxis.mX;
	newMatrix[4] = xAxis.mY;
	newMatrix[5] = yAxis.mY;
	newMatrix[6] = zAxis.mY;
	newMatrix[8] = xAxis.mZ;
	newMatrix[9] = yAxis.mZ;
	newMatrix[10] = zAxis.mZ; 
	newMatrix[12] = -xAxis.Dot(_position);
	newMatrix[13] = -yAxis.Dot(_position);
	newMatrix[14] = -zAxis.Dot(_position);
	return newMatrix;
}

//生成透视变换矩阵LH 投影窗口与z = 1的平面重合
//ProjectTransform
Matrix4x4 Matrix4x4::BuildPerspectiveLH(
	double _fovY, //垂直方向的视角的一半，角度
	double _aspect, //投影平面宽高比
	double _zNear, //近裁剪面离视点距离
	double _zFar) //远裁剪面离视点距离
{
	Matrix4x4 newMatrix;
	newMatrix.Zero();
	newMatrix[0] = 1 / (tan(_fovY * PIOver180) * _aspect);
	newMatrix[5] = 1 / tan(_fovY * PIOver180);
	newMatrix[10] = _zFar / (_zFar - _zNear);
	newMatrix[11] = 1.0f;
	newMatrix[14] = (_zNear * _zFar) / (_zNear - _zFar);
	return newMatrix;
}

//生成视口变换矩阵
//ViewPortTransform
Matrix4x4 Matrix4x4::BuildViewPort(
	double _x, //视口的起点的x轴坐标，x轴向右为正
	double _y, //视口的起点的y轴坐标，y轴向下为正
	double _width, //视口的宽度
	double _height, //视口的高度
	double _minZ, //场景的最小深度值，通常取0.0f
	double _maxZ) //场景的最大深度值，通常取1.0f
{
	Matrix4x4 newMatrix;
	newMatrix[0] = _width / 2;
	newMatrix[5] = -_height / 2;
	newMatrix[10] = _maxZ - _minZ;
	newMatrix[12] = _x + _width / 2;
	newMatrix[13] = _y + _height / 2;
	newMatrix[14] = _minZ;
	return newMatrix;
}

Matrix4x4 Matrix4x4::CreateScale(const Vector4& _scale)
{
    Matrix4x4 newMatrix;
    newMatrix.mElem[0] = _scale.mX;
    newMatrix.mElem[5] = _scale.mY;
    newMatrix.mElem[10] = _scale.mZ;
    return newMatrix;
}

//向量在左，矩阵在右
Matrix4x4 Matrix4x4::CreateTrans(const Vector4& _trans)
{
    Matrix4x4 newMatrix;
	newMatrix.mElem[12] = _trans.mX;
    newMatrix.mElem[13] = _trans.mY;
    newMatrix.mElem[14] = _trans.mZ;
    return newMatrix;
}

Matrix4x4 Matrix4x4::CreateRotateX(double _angle)
{
    Matrix4x4 newMatrix;
    newMatrix.mElem[5] = cos(_angle * PIOver180);
    newMatrix.mElem[6] = sin(_angle * PIOver180);
    newMatrix.mElem[9] = -sin(_angle * PIOver180);
    newMatrix.mElem[10] = cos(_angle * PIOver180);
    return newMatrix;
}

Matrix4x4 Matrix4x4::CreateRotateY(double _angle)
{
    Matrix4x4 newMatrix;
    newMatrix.mElem[0] = cos(_angle * PIOver180);
    newMatrix.mElem[2] = -sin(_angle * PIOver180);
    newMatrix.mElem[8] = sin(_angle * PIOver180);
    newMatrix.mElem[10] = cos(_angle * PIOver180);
    return newMatrix;
}

Matrix4x4 Matrix4x4::CreateRotateZ(double _angle)
{
    Matrix4x4 newMatrix;
    newMatrix.mElem[0] = cos(_angle * PIOver180);
    newMatrix.mElem[1] = sin(_angle * PIOver180);
    newMatrix.mElem[4] = -sin(_angle * PIOver180);
    newMatrix.mElem[5] = cos(_angle * PIOver180);
    return newMatrix;
}

Matrix4x4 Matrix4x4::RotateOnAxis(Vector4& _axis, double _angle)
{
    Matrix4x4 newMatrix;
	_axis.Normalize();
	double u = _axis.mX;
    double v = _axis.mY;
    double w = _axis.mZ;

    newMatrix[0] = cosf(_angle * PIOver180) + (u * u) * (1 - cosf(_angle * PIOver180));
    newMatrix[1] = u * v * (1 - cosf(_angle * PIOver180)) + w * sinf(_angle * PIOver180);
    newMatrix[2] = u * w * (1 - cosf(_angle * PIOver180)) - v * sinf(_angle * PIOver180);
    newMatrix[3] = 0;

    newMatrix[4] = u * v * (1 - cosf(_angle * PIOver180)) - w * sinf(_angle * PIOver180);
    newMatrix[5] = cosf(_angle * PIOver180) + v * v * (1 - cosf(_angle * PIOver180));
    newMatrix[6] = w * v * (1 - cosf(_angle * PIOver180)) + u * sinf(_angle * PIOver180);
    newMatrix[7] = 0;

    newMatrix[8] = u * w * (1 - cosf(_angle * PIOver180)) + v * sinf(_angle * PIOver180);
    newMatrix[9] = v * w * (1 - cosf(_angle * PIOver180)) - u * sinf(_angle * PIOver180);
    newMatrix[10] = cosf(_angle * PIOver180) + w * w * (1 - cosf(_angle * PIOver180));
    newMatrix[11] = 0;

    newMatrix[12] = 0;
    newMatrix[13] = 0;
    newMatrix[14] = 0;
    newMatrix[15] = 1;
	return newMatrix;
}