#ifndef _VERTEX_H_
#define _VERTEX_H_

#include <Windows.h>
#include "Matrix4x4.h"
#include "Material.h"
#include "Vector4.h"
// todo: Z Buffer Algorithm

class TextCoord
{
public:
	double u;
	double v;
	TextCoord();
	TextCoord(double _u, double _v);
	TextCoord(const TextCoord& _other);
	TextCoord& operator=(const TextCoord& _other);
	~TextCoord();
};

class Vertex 
{
public:
    Vertex();
    Vertex(const Vector4& _pos);
	Vertex(const Vector4& _pos,
		   const Vector4& _normal, 
		   Material& _material,
		   TextCoord& _texture);
    Vertex(const Vertex& _other);
    ~Vertex();
    Vertex& operator=(const Vertex& _other);
    void Trans(Vector4& _trans);
    void Scale(Vector4& _scale);
    void RotateX(double _angle);
    void RotateY(double _angle);
    void RotateZ(double _angle);
	void MatrixMulti(Matrix4x4& _matrix);
	void DivideW(); //͸�ӳ���
	void SetColor(COLORREF _color);
	COLORREF GetColor();

    Vector4		mPos;
	Vector4		mNormal;
	Material	mMaterial; //todo: ���ʷŵ�ģ����ȥ
	COLORREF	mColor;
	TextCoord   mTexture; //������������
	BYTE		mAreaCode; //������

	//�������ֵ��������ӳ����͸�ӽ�����
	double		mZView; //�۲�����ϵ�е�zֵ ���ֵ
};

#endif