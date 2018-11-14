#include "Model.h"
#include "Scene.h"

void Model::Draw()
{
	for (auto iter = mTriangles.begin();
		 iter != mTriangles.end();
		 ++iter)
	{
		iter->Draw(mWorldMatrix);
	}
	//ÿ��draw���Ժ���תһ���Ƕ�
	Matrix4x4 rotateX = Matrix4x4::CreateRotateX(0.1);
	Matrix4x4 rotateY = Matrix4x4::CreateRotateY(0.2);
	mWorldMatrix *= rotateX;
	mWorldMatrix *= rotateY;

	//�������
	//Scene::GetInstance()->GetCamera().Yaw(0.01);
}

void Model::AddTriangle(const Triangle& _triangle)
{
	mTriangles.push_back(_triangle);
}

void Model::SetWorldMatrix(const Matrix4x4& _matrix)
{
	mWorldMatrix = _matrix;
}