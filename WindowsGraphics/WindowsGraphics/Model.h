#ifndef _MODEL_H_
#define _MODEL_H_

#include <vector>
#include "Matrix4x4.h"
#include "Triangle.h"
#include "Vector4.h"
//Triangle���������ţ���������

class Model
{
public:
	std::vector<Triangle> mTriangles; //ģ�͵�������Ƭ�����ౣ��
	Matrix4x4 mWorldMatrix;

public:
	void Draw();
	void AddTriangle(const Triangle& _triangle);
	void SetWorldMatrix(const Matrix4x4& _matrix);
};

#endif