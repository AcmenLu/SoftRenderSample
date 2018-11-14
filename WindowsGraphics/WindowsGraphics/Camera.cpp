#include "Camera.h"

Camera::Camera(
	Vector4 _postion,
	Vector4 _upVector,
	Vector4 _right,
	double _fovY,
	double _aspect,
	double _zNear,
	double _zFar) : 
	mPostion(_postion),
	mUpVector(_upVector),
	mRight(_right),
	mFovY(_fovY),
	mAspect(_aspect),
	mZNear(_zNear),
	mZFar(_zFar) 
{
	GenViewTransform();
	GenProjectTransform();
}
const Matrix4x4& Camera::GetViewTransform()
{
	GenViewTransform();
	return mViewTransform;
}

const Matrix4x4& Camera::GetProjectTransform()
{
	return mProjectTransform;
}

Camera::Camera(const Camera& _other) :
		mPostion(_other.mPostion),
		mUpVector(_other.mUpVector),
		mRight(_other.mRight),
		mFovY(_other.mFovY),
		mAspect(_other.mAspect),
		mZNear(_other.mZNear),
		mZFar(_other.mZFar),
		mViewTransform(_other.mViewTransform),
		mProjectTransform(_other.mProjectTransform)
		{/*empty*/}

Camera& Camera::operator=(const Camera& _other)
{
	mPostion = _other.mPostion;
	mUpVector = _other.mUpVector;
	mRight = _other.mRight;
	mFovY = _other.mFovY;
	mAspect = _other.mAspect;
	mZNear = _other.mZNear;
	mZFar = _other.mZFar;
	mViewTransform = _other.mViewTransform;
	mProjectTransform = _other.mProjectTransform;
	return *this;
}

void Camera::GenProjectTransform()
{
	mProjectTransform = Matrix4x4::BuildPerspectiveLH(
		mFovY,
		mAspect,
		mZNear,
		mZFar);
}

void Camera::GenViewTransform()
{
	mViewTransform = Matrix4x4::BuildLookAtLH(
		mPostion,
		mUpVector,
		mRight);
}

Camera::~Camera()
{/*empty*/}

Camera::Camera()
{/*empty*/}

void Camera::Strafe(double _units) //left right 扫视
{
	mPostion += mRight * _units;
}

void Camera::Fly(double _units) //up down 升降
{
	mPostion += mUpVector * _units;
}

void Camera::Walk(double _units) //forward backward 行走
{
	mPostion += mRight.CrossProduct(mUpVector) * _units;
}

void Camera::Pitch(double _angle) // rotate on right vector 俯仰
{
	Matrix4x4 t = Matrix4x4::RotateOnAxis(mRight, _angle);
	mUpVector = mUpVector.MatrixMulti(t);
}

void Camera::Yaw(double _angle) // rotate on up vector 偏航
{
	Matrix4x4 t = Matrix4x4::RotateOnAxis(mUpVector, _angle);
	mRight = mRight.MatrixMulti(t);
}

void Camera::Roll(double _angle) // rotate on look vector 滚动
{
	// todo: 暂不实现
}