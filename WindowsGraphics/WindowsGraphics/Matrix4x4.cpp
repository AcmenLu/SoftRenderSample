#include <math.h>
#include <string.h>
#include "Matrix4x4.h"
#include "Vector3.h"
#include "Vector4.h"
//���ǻ�����������ϵ�ı任
//���������˷������������󣬾�������
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

//�������
void Matrix4x4::Zero()
{
	memset(mElem, 0, 16 * sizeof(double));
}

//��������Ϊ��λ����
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

//todo: ��ת�ĳ���Ԫ�����ɷ�����������ת��������Ԫ��
//������Ԫ��������ת����
//������ģ������ϵ����������ϵ�ı任����
//WorldTransform
Matrix4x4 Matrix4x4::BuildWorldTransform(
	const Vector4& _position, //����������ϵ�е�λ��
	const Vector4& _scale, //����ϵ��
	double _xAngle, //��x����ת�ĽǶ�
	double _yAngle, //��y����ת�ĽǶ�
	double _zAngle) //��z����ת�ĽǶ�
{
	Matrix4x4 newMatrix;
	//ע��˳������ת�������ţ����ƽ��
	newMatrix *= CreateRotateX(_xAngle);
	newMatrix *= CreateRotateX(_xAngle);
	newMatrix *= CreateRotateX(_xAngle);
	newMatrix *= CreateScale(_scale);
	newMatrix *= CreateTrans(_position);
	return newMatrix;
}

//���ɴ���������ϵ���۲�����ϵ�ı任����
//ViewTransform ��������ϵ
Matrix4x4 Matrix4x4::BuildLookAtLH(
	const Vector4& _position, //���������������ϵ�е�λ��
	const Vector4& _upVector, //����Ϸ���
	const Vector4& _right) //����ҷ���
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

//����͸�ӱ任����LH ͶӰ������z = 1��ƽ���غ�
//ProjectTransform
Matrix4x4 Matrix4x4::BuildPerspectiveLH(
	double _fovY, //��ֱ������ӽǵ�һ�룬�Ƕ�
	double _aspect, //ͶӰƽ���߱�
	double _zNear, //���ü������ӵ����
	double _zFar) //Զ�ü������ӵ����
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

//�����ӿڱ任����
//ViewPortTransform
Matrix4x4 Matrix4x4::BuildViewPort(
	double _x, //�ӿڵ�����x�����꣬x������Ϊ��
	double _y, //�ӿڵ�����y�����꣬y������Ϊ��
	double _width, //�ӿڵĿ��
	double _height, //�ӿڵĸ߶�
	double _minZ, //��������С���ֵ��ͨ��ȡ0.0f
	double _maxZ) //������������ֵ��ͨ��ȡ1.0f
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

//�������󣬾�������
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