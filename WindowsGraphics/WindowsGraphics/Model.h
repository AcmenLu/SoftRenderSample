#ifndef _MODEL_H_
#define _MODEL_H_

#include <vector>
#include "Matrix4x4.h"
#include "Triangle.h"
#include "Vector4.h"
//Triangle按照冗余存放，不按索引

class Model
{
public:
	std::vector<Triangle> mTriangles; //模型的三角面片，冗余保存
	Matrix4x4 mWorldMatrix;

public:
	void Draw();
	void AddTriangle(const Triangle& _triangle);
	void SetWorldMatrix(const Matrix4x4& _matrix);
};

#endif