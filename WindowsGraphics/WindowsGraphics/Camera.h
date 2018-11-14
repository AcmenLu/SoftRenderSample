#ifndef _CAMERA_H_
#define _CAMERA_H_

#include "Matrix4x4.h"
#include "Vertex.h"
#include "Vector3.h"

class Camera 
{
public:
	Camera();
    Camera(
		Vector4 _postion,
		Vector4 _upVector,
		Vector4 _right,
		double _fovY,
		double _aspect,
		double _zNear,
		double _zFar); 
    Camera(const Camera& _other);
	Camera& operator=(const Camera& _other);
    ~Camera();
	Vector4 mPostion;
	Vector4 mUpVector;
	Vector4 mRight;
	double  mFovY;
	double  mAspect;
	double  mZNear;
	double  mZFar;

	void GenViewTransform();
	void GenProjectTransform();

	const Matrix4x4& GetViewTransform();
	const Matrix4x4& GetProjectTransform();

	Matrix4x4 mViewTransform;
	Matrix4x4 mProjectTransform;

	//Camera Controll
	void Strafe(double _units); //left right
	void Fly(double _units); //up down
	void Walk(double _units); //forward backward

	void Pitch(double _angle); // rotate on right vector
	void Yaw(double _angle); // rotate on up vector
	void Roll(double _angle); // rotate on look vector
};

#endif