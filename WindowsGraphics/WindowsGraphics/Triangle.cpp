#include <Windows.h>
#include <iostream>
#include <math.h>
#include <stdio.h>
#include <string.h>
#include "Camera.h"
#include "Scene.h"
#include "Triangle.h"

using namespace std;
//Span::Span() : y(0), xLeft(0), xRight(0) 
//{/*empty*/}
//
//Span::~Span()
//{/*empty*/}
//
//Span::Span(const Span& _other) :
//	y(_other.y),
//	xLeft(_other.xLeft),
//	xRight(_other.xRight) 
//{/*empty*/}
//
//Span::Span(int _y, int _xLeft, int _xRight)
//	: y(_y), xLeft(_xLeft), xRight(_xRight)
//{/*empty*/}
//
//Span& Span::operator=(const Span& _other)
//{
//	if (this == &_other)
//	{
//		return *this;
//	}
//	else
//	{
//		y = _other.y;
//		xLeft = _other.xLeft;
//		xRight = _other.xRight;
//	}
//}

Triangle::Triangle()
{
    memset(&this->mPoints, 0, sizeof(this->mPoints));
	Init();
}

void Triangle::Init()
{
	//int nResolution = SCREEN_WIDTH * SCREEN_HEIGHT;
	//mEdgePoints = new int*[SCREEN_WIDTH];
	//for (int i = 0; i < SCREEN_WIDTH; ++i)
	//{
	//	mEdgePoints[i] = new int[SCREEN_HEIGHT];
	//	memset(mEdgePoints[i], 0, sizeof(int) * SCREEN_HEIGHT);
	//}
	//�����������ʱ��
	LARGE_INTEGER lv;
	QueryPerformanceFrequency(&lv);
	mSecondPerTick = 1.0 / lv.QuadPart;

	mRender = Render::GetInstance();
}

Triangle::Triangle(const Triangle& _other)
{
	*this = _other;
	Init();
}

Triangle& Triangle::operator=(const Triangle& _other)
{
	if (this != &_other)
	{
		mPoints[0] = _other.mPoints[0];
		mPoints[1] = _other.mPoints[1];
		mPoints[2] = _other.mPoints[2];
	}
	Init();
	return *this;
}

Triangle::Triangle(
	const Vertex& _ver0,
	const Vertex& _ver1,
	const Vertex& _ver2)
{
	mPoints[0] = _ver0;
	mPoints[1] = _ver1;
	mPoints[2] = _ver2;
	Init();
}

Triangle::~Triangle()
{
	//if (mEdgePoints)
	//{
	//	for (int i = 0; i < SCREEN_WIDTH; ++i)
	//	{
	//		if (mEdgePoints[i])
	//		{
	//			delete [] mEdgePoints[i];
	//			mEdgePoints[i] = NULL;
	//		}
	//	}
	//	delete [] mEdgePoints;
	//	mEdgePoints = NULL;
	//}
}

//Phong����ģ��
COLORREF Triangle::CaculateLight(const Vertex& _ver)
{
	AmbtLight Ia = Scene::GetInstance()->GetAmbt(); //������
	ParalLight Ip = Scene::GetInstance()->GetParal(); //ƽ�й�

	Vector4 L(Ip.direction.Reverse());
	L.Normalize();
	Vector4 N = _ver.mNormal;
	N.Normalize();

	Camera cam = Scene::GetInstance()->GetCamera();
	Vector4 V = cam.mPostion - _ver.mPos;
	V.Normalize();
	Vector4 H = (L + V) / 2;
	H.Normalize();

	int Ir;
	int Ig;
	int Ib;

	Material m = _ver.mMaterial;
	//�����估���淴������
	double factor = m.Kd * (L * N) + m.Ks * pow((H * N), m.n);
	factor = factor < 0 ? 0 : factor; 

	Ir = m.Ka * Ia.r + Ip.r * factor;
	if (Ir < 0) Ir = 0;
	else if (Ir > 255) Ir = 255;
	Ig = m.Ka * Ia.g + Ip.g * factor;
	if (Ig < 0) Ig = 0;
	else if (Ig > 255) Ig = 255;
	Ib = m.Ka * Ia.b + Ip.b * factor;
	if (Ib < 0) Ib = 0;
	else if (Ib > 255) Ib = 255;

	return COLORREF(RGB(Ir, Ig, Ib));
}

void Triangle::CutW(
	const sTriangle& _tri, 
	vector<sTriangle>& _outTris)
{
	if (_tri.ver[0].mPos.mW > 0 &&
		_tri.ver[1].mPos.mW > 0 &&
		_tri.ver[2].mPos.mW > 0)
	{
		_outTris.push_back(_tri);
		return ;
	}

	else if (_tri.ver[0].mPos.mW < 0 &&
			 _tri.ver[1].mPos.mW < 0 &&
			 _tri.ver[2].mPos.mW < 0)
	{
		return ;
	}

	else
	{
		// w = 0
		Vertex ver0 = _tri.ver[0];
		Vertex ver1 = _tri.ver[1];
		Vertex ver2 = _tri.ver[2];
		// �������㰴��wС��������
		if (ver0.mPos.mW > ver1.mPos.mW)
		{
			SwapVertex(ver0, ver1);
		}
		if (ver0.mPos.mW > ver2.mPos.mW)
		{
			SwapVertex(ver0, ver2);
		}
		if (ver1.mPos.mW > ver2.mPos.mW)
		{
			SwapVertex(ver1, ver2);
		}

		//��������������� �ü�������ؿ�
		if (abs(ver2.mPos.mW - ver0.mPos.mW) < 0.0001f) return ;

		Vertex v0; //v0��w0w2��ƽ��w = 0�Ľ���
		//λ��
		v0.mPos.mW = 0.000001f;
		v0.mPos.mX = ver0.mPos.mX +
			(ver2.mPos.mX - ver0.mPos.mX) / 
			(ver2.mPos.mW - ver0.mPos.mW) * 
			(0.000001f - ver0.mPos.mW);
		v0.mPos.mY = ver0.mPos.mY +
			(ver2.mPos.mY - ver0.mPos.mY) / 
			(ver2.mPos.mW - ver0.mPos.mW) * 
			(0.000001f - ver0.mPos.mW);
		v0.mPos.mZ = ver0.mPos.mZ +
			(ver2.mPos.mZ - ver0.mPos.mZ) / 
			(ver2.mPos.mW - ver0.mPos.mW) * 
			(0.000001f - ver0.mPos.mW);
		//����
		v0.mNormal = ver0.mNormal;
		//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
		double newR = GetRValue(ver0.mColor) +  
			(GetRValue(ver2.mColor) - GetRValue(ver0.mColor)) / 
			(ver2.mPos.mW - ver0.mPos.mW) *
			(0.000001f - ver0.mPos.mW);
		double newG = GetGValue(ver0.mColor) +  
			(GetGValue(ver2.mColor) - GetGValue(ver0.mColor)) / 
			(ver2.mPos.mW - ver0.mPos.mW) *
			(0.000001f - ver0.mPos.mW);
		double newB = GetBValue(ver0.mColor) +  
			(GetBValue(ver2.mColor) - GetBValue(ver0.mColor)) / 
			(ver2.mPos.mW - ver0.mPos.mW) *
			(0.000001f - ver0.mPos.mW);
		v0.mColor = RGB(newR, newG, newB);
		//�������� ��͸�ӽ���
		double s = (0.000001f - ver0.mPos.mW) / 
			(ver2.mPos.mW - ver0.mPos.mW);
		double Z1 = 0.0;
		double Z2 = 0.0;
		double Zt = 0.0;
		double t = s;
		Z1 = ver0.mZView;
		Z2 = ver2.mZView;
		if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
		if (Zt != 0) Zt = 1 / Zt;
		if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
		v0.mZView = Zt;
		v0.mTexture.u = ver0.mTexture.u + 
			t * (ver2.mTexture.u - ver0.mTexture.u);
		v0.mTexture.v = ver0.mTexture.v + 
			t * (ver2.mTexture.v - ver0.mTexture.v);

		//y0 w < 0 y1 y2 w > 0
		if (ver0.mPos.mW < 0.000001f && ver1.mPos.mW > 0.000001f)
		{
			Vertex v1; //v1��w0w1��ƽ��w = 0�Ľ���
			// todo: ����v1 ע��͸�ӽ���			
			//λ��
			v1.mPos.mW = 0.000001f;
			v1.mPos.mX = ver0.mPos.mX +
				(ver1.mPos.mX - ver0.mPos.mX) / 
				(ver1.mPos.mW - ver0.mPos.mW) * 
				(0.000001f - ver0.mPos.mW);
			v1.mPos.mY = ver0.mPos.mY +
				(ver1.mPos.mY - ver0.mPos.mY) / 
				(ver1.mPos.mW - ver0.mPos.mW) * 
				(0.000001f - ver0.mPos.mW);
			v1.mPos.mZ = ver0.mPos.mZ +
				(ver1.mPos.mZ - ver0.mPos.mZ) / 
				(ver1.mPos.mW - ver0.mPos.mW) * 
				(0.000001f - ver0.mPos.mW);
			//����
			v1.mNormal = ver0.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver0.mColor) +  
				(GetRValue(ver1.mColor) - GetRValue(ver0.mColor)) / 
				(ver1.mPos.mW - ver0.mPos.mW) *
				(0.000001f - ver0.mPos.mW);
			double newG = GetGValue(ver0.mColor) +  
				(GetGValue(ver1.mColor) - GetGValue(ver0.mColor)) / 
				(ver1.mPos.mW - ver0.mPos.mW) *
				(0.000001f - ver0.mPos.mW);
			double newB = GetBValue(ver0.mColor) +  
				(GetBValue(ver1.mColor) - GetBValue(ver0.mColor)) / 
				(ver1.mPos.mW - ver0.mPos.mW) *
				(0.000001f - ver0.mPos.mW);
			v1.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (0.000001f - ver0.mPos.mW) / 
				(ver1.mPos.mW - ver0.mPos.mW);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver0.mZView;
			Z2 = ver1.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v1.mZView = Zt;
			v1.mTexture.u = ver0.mTexture.u + 
				t * (ver1.mTexture.u - ver0.mTexture.u);
			v1.mTexture.v = ver0.mTexture.v + 
				t * (ver1.mTexture.v - ver0.mTexture.v);
			// todo: v0 v1 ver1 ver2�ֳ�����������װ��outTris
			_outTris.push_back(sTriangle(v0, ver2, v1));
			_outTris.push_back(sTriangle(ver2, ver1, v1));
		}
			
		//y0 y1 w < 0 y2 w > 0
		else if (ver0.mPos.mW < 0.000001f && ver1.mPos.mW < 0.000001f)
		{
			Vertex v2; //v2��y1y2��ƽ��y = 1�Ľ���
			// todo: ����v2 ע��͸�ӽ���
			//λ��
			v2.mPos.mW = 0.000001f;
			v2.mPos.mX = ver1.mPos.mX +
				(ver2.mPos.mX - ver1.mPos.mX) / 
				(ver2.mPos.mW - ver1.mPos.mW) * 
				(0.000001f - ver1.mPos.mW);
			v2.mPos.mY = ver1.mPos.mY +
				(ver2.mPos.mY - ver1.mPos.mY) / 
				(ver2.mPos.mW - ver1.mPos.mW) * 
				(0.000001f - ver1.mPos.mW);
			v2.mPos.mZ = ver1.mPos.mZ +
				(ver2.mPos.mZ - ver1.mPos.mZ) / 
				(ver2.mPos.mW - ver1.mPos.mW) * 
				(0.000001f - ver1.mPos.mW);
			//����
			v2.mNormal = ver1.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver1.mColor) +  
				(GetRValue(ver2.mColor) - GetRValue(ver1.mColor)) / 
				(ver2.mPos.mW - ver1.mPos.mW) *
				(0.000001f - ver1.mPos.mW);
			double newG = GetGValue(ver1.mColor) +  
				(GetGValue(ver2.mColor) - GetGValue(ver1.mColor)) / 
				(ver2.mPos.mW - ver1.mPos.mW) *
				(0.000001f - ver1.mPos.mW);
			double newB = GetBValue(ver1.mColor) +  
				(GetBValue(ver2.mColor) - GetBValue(ver1.mColor)) / 
				(ver2.mPos.mW - ver1.mPos.mW) *
				(0.000001f - ver1.mPos.mW);
			v2.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (0.000001f - ver1.mPos.mW) / 
				(ver2.mPos.mW - ver1.mPos.mW);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver1.mZView;
			Z2 = ver2.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v2.mZView = Zt;
			v2.mTexture.u = ver1.mTexture.u + 
				t * (ver2.mTexture.u - ver1.mTexture.u);
			v2.mTexture.v = ver1.mTexture.v + 
				t * (ver2.mTexture.v - ver1.mTexture.v);
			// todo: v0 v2 ver2���һ��������װ��outTris
			_outTris.push_back(sTriangle(v0, ver2, v2));
		}
	}
}

//��ƽ�� y = 1  ��ƽ�� x = 1  ��ƽ�� z = -1
//��ƽ�� y = -1  ��ƽ�� x = -1  Զƽ�� z = 1
//����һ�������κ�һ���棬����ü���0��1��2�����������
//��ʱ��֤�����κ�ƽ���Ȼ�н���
void Triangle::Cut(
	const sTriangle& _tri, 
	FACE_TYPE _face,
	vector<sTriangle>& _outTris)
{
	switch (_face)
	{
	case FACE_TOP:
		{
			// y = 1
			Vertex ver0 = _tri.ver[0];
			Vertex ver1 = _tri.ver[1];
			Vertex ver2 = _tri.ver[2];
			// �������㰴���϶�������
			if (ver0.mPos.mY < ver1.mPos.mY)
			{
				SwapVertex(ver0, ver1);
			}
			if (ver0.mPos.mY < ver2.mPos.mY)
			{
				SwapVertex(ver0, ver2);
			}
			if (ver1.mPos.mY < ver2.mPos.mY)
			{
				SwapVertex(ver1, ver2);
			}

			//��������������� �ü�������ؿ�
			if (abs(ver2.mPos.mY - ver0.mPos.mY) < 0.0001f) return ;

			Vertex v0; //v0��y0y2��ƽ��y = 1�Ľ���
			//λ��
			v0.mPos.mY = 1.0;
			v0.mPos.mX = ver0.mPos.mX +
				(ver2.mPos.mX - ver0.mPos.mX) / 
				(ver2.mPos.mY - ver0.mPos.mY) * 
				(1.0 - ver0.mPos.mY);
			v0.mPos.mZ = ver0.mPos.mZ +
				(ver2.mPos.mZ - ver0.mPos.mZ) / 
				(ver2.mPos.mY - ver0.mPos.mY) * 
				(1.0 - ver0.mPos.mY);
			//����
			v0.mNormal = ver0.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver0.mColor) +  
				(GetRValue(ver2.mColor) - GetRValue(ver0.mColor)) / 
				(ver2.mPos.mY - ver0.mPos.mY) *
				(1.0 - ver0.mPos.mY);
			double newG = GetGValue(ver0.mColor) +  
				(GetGValue(ver2.mColor) - GetGValue(ver0.mColor)) / 
				(ver2.mPos.mY - ver0.mPos.mY) *
				(1.0 - ver0.mPos.mY);
			double newB = GetBValue(ver0.mColor) +  
				(GetBValue(ver2.mColor) - GetBValue(ver0.mColor)) / 
				(ver2.mPos.mY - ver0.mPos.mY) *
				(1.0 - ver0.mPos.mY);
			v0.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (1.0 - ver0.mPos.mY) / 
				(ver2.mPos.mY - ver0.mPos.mY);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver0.mZView;
			Z2 = ver2.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v0.mZView = Zt;
			v0.mTexture.u = ver0.mTexture.u + 
				t * (ver2.mTexture.u - ver0.mTexture.u);
			v0.mTexture.v = ver0.mTexture.v + 
				t * (ver2.mTexture.v - ver0.mTexture.v);

			//y0��ƽ���� y1 y2��ƽ����
			if (ver0.mPos.mY > 1.0f && ver1.mPos.mY < 1.0f)
			{
				Vertex v1; //v1��y0y1��ƽ��y = 1�Ľ���
				// todo: ����v1 ע��͸�ӽ���			
				//λ��
				v1.mPos.mY = 1.0;
				v1.mPos.mX = ver0.mPos.mX +
					(ver1.mPos.mX - ver0.mPos.mX) / 
					(ver1.mPos.mY - ver0.mPos.mY) * 
					(1.0 - ver0.mPos.mY);
				v1.mPos.mZ = ver0.mPos.mZ +
					(ver1.mPos.mZ - ver0.mPos.mZ) / 
					(ver1.mPos.mY - ver0.mPos.mY) * 
					(1.0 - ver0.mPos.mY);
				//����
				v1.mNormal = ver0.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver0.mColor) +  
					(GetRValue(ver1.mColor) - GetRValue(ver0.mColor)) / 
					(ver1.mPos.mY - ver0.mPos.mY) *
					(1.0 - ver0.mPos.mY);
				double newG = GetGValue(ver0.mColor) +  
					(GetGValue(ver1.mColor) - GetGValue(ver0.mColor)) / 
					(ver1.mPos.mY - ver0.mPos.mY) *
					(1.0 - ver0.mPos.mY);
				double newB = GetBValue(ver0.mColor) +  
					(GetBValue(ver1.mColor) - GetBValue(ver0.mColor)) / 
					(ver1.mPos.mY - ver0.mPos.mY) *
					(1.0 - ver0.mPos.mY);
				v1.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (1.0 - ver0.mPos.mY) / 
					(ver1.mPos.mY - ver0.mPos.mY);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver0.mZView;
				Z2 = ver1.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v1.mZView = Zt;
				v1.mTexture.u = ver0.mTexture.u + 
					t * (ver1.mTexture.u - ver0.mTexture.u);
				v1.mTexture.v = ver0.mTexture.v + 
					t * (ver1.mTexture.v - ver0.mTexture.v);
				// todo: v0 v1 ver1 ver2�ֳ�����������װ��outTris
				_outTris.push_back(sTriangle(v0, ver2, v1));
				_outTris.push_back(sTriangle(ver2, ver1, v1));
			}
			
			//y0 y1��ƽ���� y2��ƽ����
			else if (ver0.mPos.mY > 1.0f && ver1.mPos.mY > 1.0f)
			{
				Vertex v2; //v2��y1y2��ƽ��y = 1�Ľ���
				// todo: ����v2 ע��͸�ӽ���
				//λ��
				v2.mPos.mY = 1.0;
				v2.mPos.mX = ver1.mPos.mX +
					(ver2.mPos.mX - ver1.mPos.mX) / 
					(ver2.mPos.mY - ver1.mPos.mY) * 
					(1.0 - ver1.mPos.mY);
				v2.mPos.mZ = ver1.mPos.mZ +
					(ver2.mPos.mZ - ver1.mPos.mZ) / 
					(ver2.mPos.mY - ver1.mPos.mY) * 
					(1.0 - ver1.mPos.mY);
				//����
				v2.mNormal = ver1.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver1.mColor) +  
					(GetRValue(ver2.mColor) - GetRValue(ver1.mColor)) / 
					(ver2.mPos.mY - ver1.mPos.mY) *
					(1.0 - ver1.mPos.mY);
				double newG = GetGValue(ver1.mColor) +  
					(GetGValue(ver2.mColor) - GetGValue(ver1.mColor)) / 
					(ver2.mPos.mY - ver1.mPos.mY) *
					(1.0 - ver1.mPos.mY);
				double newB = GetBValue(ver1.mColor) +  
					(GetBValue(ver2.mColor) - GetBValue(ver1.mColor)) / 
					(ver2.mPos.mY - ver1.mPos.mY) *
					(1.0 - ver1.mPos.mY);
				v2.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (1.0 - ver1.mPos.mY) / 
					(ver2.mPos.mY - ver1.mPos.mY);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver1.mZView;
				Z2 = ver2.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v2.mZView = Zt;
				v2.mTexture.u = ver1.mTexture.u + 
					t * (ver2.mTexture.u - ver1.mTexture.u);
				v2.mTexture.v = ver1.mTexture.v + 
					t * (ver2.mTexture.v - ver1.mTexture.v);
				// todo: v0 v2 ver2���һ��������װ��outTris
				_outTris.push_back(sTriangle(v0, ver2, v2));
			}
		}
		break;
	case FACE_DOWN:
		{
			// y = -1
			Vertex ver0 = _tri.ver[0];
			Vertex ver1 = _tri.ver[1];
			Vertex ver2 = _tri.ver[2];
			// �������㰴���϶�������
			if (ver0.mPos.mY < ver1.mPos.mY)
			{
				SwapVertex(ver0, ver1);
			}
			if (ver0.mPos.mY < ver2.mPos.mY)
			{
				SwapVertex(ver0, ver2);
			}
			if (ver1.mPos.mY < ver2.mPos.mY)
			{
				SwapVertex(ver1, ver2);
			}

			//��������������� �ü�������ؿ�
			if (abs(ver2.mPos.mY - ver0.mPos.mY) < 0.0001f) return ;

			Vertex v0; //v0��y0y2��ƽ��y = 1�Ľ���
			//λ��
			v0.mPos.mY = -1.0;
			v0.mPos.mX = ver0.mPos.mX +
				(ver2.mPos.mX - ver0.mPos.mX) / 
				(ver2.mPos.mY - ver0.mPos.mY) * 
				(-1.0 - ver0.mPos.mY);
			v0.mPos.mZ = ver0.mPos.mZ +
				(ver2.mPos.mZ - ver0.mPos.mZ) / 
				(ver2.mPos.mY - ver0.mPos.mY) * 
				(-1.0 - ver0.mPos.mY);
			//����
			v0.mNormal = ver0.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver0.mColor) +  
				(GetRValue(ver2.mColor) - GetRValue(ver0.mColor)) / 
				(ver2.mPos.mY - ver0.mPos.mY) *
				(-1.0 - ver0.mPos.mY);
			double newG = GetGValue(ver0.mColor) +  
				(GetGValue(ver2.mColor) - GetGValue(ver0.mColor)) / 
				(ver2.mPos.mY - ver0.mPos.mY) *
				(-1.0 - ver0.mPos.mY);
			double newB = GetBValue(ver0.mColor) +  
				(GetBValue(ver2.mColor) - GetBValue(ver0.mColor)) / 
				(ver2.mPos.mY - ver0.mPos.mY) *
				(-1.0 - ver0.mPos.mY);
			v0.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (-1.0 - ver0.mPos.mY) / 
				(ver2.mPos.mY - ver0.mPos.mY);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver0.mZView;
			Z2 = ver2.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v0.mZView = Zt;
			v0.mTexture.u = ver0.mTexture.u + 
				t * (ver2.mTexture.u - ver0.mTexture.u);
			v0.mTexture.v = ver0.mTexture.v + 
				t * (ver2.mTexture.v - ver0.mTexture.v);

			//y0��ƽ���� y1 y2��ƽ����
			if (ver0.mPos.mY > -1.0f && ver1.mPos.mY < -1.0f)
			{
				Vertex v1; //v1��y0y1��ƽ��y = 1�Ľ���
				// todo: ����v1 ע��͸�ӽ���			
				//λ��
				v1.mPos.mY = -1.0;
				v1.mPos.mX = ver0.mPos.mX +
					(ver1.mPos.mX - ver0.mPos.mX) / 
					(ver1.mPos.mY - ver0.mPos.mY) * 
					(-1.0 - ver0.mPos.mY);
				v1.mPos.mZ = ver0.mPos.mZ +
					(ver1.mPos.mZ - ver0.mPos.mZ) / 
					(ver1.mPos.mY - ver0.mPos.mY) * 
					(-1.0 - ver0.mPos.mY);
				//����
				v1.mNormal = ver0.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver0.mColor) +  
					(GetRValue(ver1.mColor) - GetRValue(ver0.mColor)) / 
					(ver1.mPos.mY - ver0.mPos.mY) *
					(-1.0 - ver0.mPos.mY);
				double newG = GetGValue(ver0.mColor) +  
					(GetGValue(ver1.mColor) - GetGValue(ver0.mColor)) / 
					(ver1.mPos.mY - ver0.mPos.mY) *
					(-1.0 - ver0.mPos.mY);
				double newB = GetBValue(ver0.mColor) +  
					(GetBValue(ver1.mColor) - GetBValue(ver0.mColor)) / 
					(ver1.mPos.mY - ver0.mPos.mY) *
					(-1.0 - ver0.mPos.mY);
				v1.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (-1.0 - ver0.mPos.mY) / 
					(ver1.mPos.mY - ver0.mPos.mY);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver0.mZView;
				Z2 = ver1.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v1.mZView = Zt;
				v1.mTexture.u = ver0.mTexture.u + 
					t * (ver1.mTexture.u - ver0.mTexture.u);
				v1.mTexture.v = ver0.mTexture.v + 
					t * (ver1.mTexture.v - ver0.mTexture.v);
				// v0 v1 ver0���������װ��outTris
				_outTris.push_back(sTriangle(v0, v1, ver0));
			}
			
			//y0 y1��ƽ���� y2��ƽ����
			else if (ver0.mPos.mY > -1.0f && ver1.mPos.mY > -1.0f)
			{
				Vertex v2; //v2��y1y2��ƽ��y = 1�Ľ���
				// todo: ����v2 ע��͸�ӽ���
				//λ��
				v2.mPos.mY = -1.0;
				v2.mPos.mX = ver1.mPos.mX +
					(ver2.mPos.mX - ver1.mPos.mX) / 
					(ver2.mPos.mY - ver1.mPos.mY) * 
					(-1.0 - ver1.mPos.mY);
				v2.mPos.mZ = ver1.mPos.mZ +
					(ver2.mPos.mZ - ver1.mPos.mZ) / 
					(ver2.mPos.mY - ver1.mPos.mY) * 
					(-1.0 - ver1.mPos.mY);
				//����
				v2.mNormal = ver1.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver1.mColor) +  
					(GetRValue(ver2.mColor) - GetRValue(ver1.mColor)) / 
					(ver2.mPos.mY - ver1.mPos.mY) *
					(-1.0 - ver1.mPos.mY);
				double newG = GetGValue(ver1.mColor) +  
					(GetGValue(ver2.mColor) - GetGValue(ver1.mColor)) / 
					(ver2.mPos.mY - ver1.mPos.mY) *
					(-1.0 - ver1.mPos.mY);
				double newB = GetBValue(ver1.mColor) +  
					(GetBValue(ver2.mColor) - GetBValue(ver1.mColor)) / 
					(ver2.mPos.mY - ver1.mPos.mY) *
					(-1.0 - ver1.mPos.mY);
				v2.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (-1.0 - ver1.mPos.mY) / 
					(ver2.mPos.mY - ver1.mPos.mY);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver1.mZView;
				Z2 = ver2.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v2.mZView = Zt;
				v2.mTexture.u = ver1.mTexture.u + 
					t * (ver2.mTexture.u - ver1.mTexture.u);
				v2.mTexture.v = ver1.mTexture.v + 
					t * (ver2.mTexture.v - ver1.mTexture.v);
				// v0 v2 ver1 ver0�������������װ��outTris
				_outTris.push_back(sTriangle(v0, v2, ver1));
				_outTris.push_back(sTriangle(v0, ver1, ver0));
			}
		}
		break;
	case FACE_LEFT:
		{
			// x = -1
			Vertex ver0 = _tri.ver[0];
			Vertex ver1 = _tri.ver[1];
			Vertex ver2 = _tri.ver[2];
			// �������㰴�����������
			if (ver0.mPos.mX > ver1.mPos.mX)
			{
				SwapVertex(ver0, ver1);
			}
			if (ver0.mPos.mX > ver2.mPos.mX)
			{
				SwapVertex(ver0, ver2);
			}
			if (ver1.mPos.mX > ver2.mPos.mX)
			{
				SwapVertex(ver1, ver2);
			}

			//��������������� �ü�������ؿ�
			if (abs(ver2.mPos.mX - ver0.mPos.mX) < 0.0001f) return ;

			Vertex v0; //v0��x0x2��ƽ��x = -1�Ľ���
			//λ��
			v0.mPos.mX = -1.0;
			v0.mPos.mY = ver0.mPos.mY +
				(ver2.mPos.mY - ver0.mPos.mY) / 
				(ver2.mPos.mX - ver0.mPos.mX) * 
				(-1.0 - ver0.mPos.mX);
			v0.mPos.mZ = ver0.mPos.mZ +
				(ver2.mPos.mZ - ver0.mPos.mZ) / 
				(ver2.mPos.mX - ver0.mPos.mX) * 
				(-1.0 - ver0.mPos.mX);
			//����
			v0.mNormal = ver0.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver0.mColor) +  
				(GetRValue(ver2.mColor) - GetRValue(ver0.mColor)) / 
				(ver2.mPos.mX - ver0.mPos.mX) *
				(-1.0 - ver0.mPos.mX);
			double newG = GetGValue(ver0.mColor) +  
				(GetGValue(ver2.mColor) - GetGValue(ver0.mColor)) / 
				(ver2.mPos.mX - ver0.mPos.mX) *
				(-1.0 - ver0.mPos.mX);
			double newB = GetBValue(ver0.mColor) +  
				(GetBValue(ver2.mColor) - GetBValue(ver0.mColor)) / 
				(ver2.mPos.mX - ver0.mPos.mX) *
				(-1.0 - ver0.mPos.mX);
			v0.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (-1.0 - ver0.mPos.mX) / 
				(ver2.mPos.mX - ver0.mPos.mX);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver0.mZView;
			Z2 = ver2.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v0.mZView = Zt;
			v0.mTexture.u = ver0.mTexture.u + 
				t * (ver2.mTexture.u - ver0.mTexture.u);
			v0.mTexture.v = ver0.mTexture.v + 
				t * (ver2.mTexture.v - ver0.mTexture.v);

			//x0��ƽ���� x1 x2��ƽ����
			if (ver0.mPos.mX < -1.0f && ver1.mPos.mX > -1.0f)
			{
				Vertex v1; //v1��x0x1��ƽ��x = -1�Ľ���
				// todo: ����v1 ע��͸�ӽ���			
				//λ��
				v1.mPos.mX = -1.0;
				v1.mPos.mY = ver0.mPos.mY +
					(ver1.mPos.mY - ver0.mPos.mY) / 
					(ver1.mPos.mX - ver0.mPos.mX) * 
					(-1.0 - ver0.mPos.mX);
				v1.mPos.mZ = ver0.mPos.mZ +
					(ver1.mPos.mZ - ver0.mPos.mZ) / 
					(ver1.mPos.mX - ver0.mPos.mX) * 
					(-1.0 - ver0.mPos.mX);
				//����
				v1.mNormal = ver0.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver0.mColor) +  
					(GetRValue(ver1.mColor) - GetRValue(ver0.mColor)) / 
					(ver1.mPos.mX - ver0.mPos.mX) *
					(-1.0 - ver0.mPos.mX);
				double newG = GetGValue(ver0.mColor) +  
					(GetGValue(ver1.mColor) - GetGValue(ver0.mColor)) / 
					(ver1.mPos.mX - ver0.mPos.mX) *
					(-1.0 - ver0.mPos.mX);
				double newB = GetBValue(ver0.mColor) +  
					(GetBValue(ver1.mColor) - GetBValue(ver0.mColor)) / 
					(ver1.mPos.mX - ver0.mPos.mX) *
					(-1.0 - ver0.mPos.mX);
				v1.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (-1.0 - ver0.mPos.mX) / 
					(ver1.mPos.mX - ver0.mPos.mX);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver0.mZView;
				Z2 = ver1.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v1.mZView = Zt;
				v1.mTexture.u = ver0.mTexture.u + 
					t * (ver1.mTexture.u - ver0.mTexture.u);
				v1.mTexture.v = ver0.mTexture.v + 
					t * (ver1.mTexture.v - ver0.mTexture.v);
				// v0 v1 ver1 ver2�������������װ��outTris
				_outTris.push_back(sTriangle(v1, ver1, ver2));
				_outTris.push_back(sTriangle(v1, ver2, v0));
			}
			
			//x0 x1��ƽ���� x2��ƽ����
			else if (ver0.mPos.mX < -1.0f && ver1.mPos.mX < -1.0f)
			{
				Vertex v2; //v2��x1x2��ƽ��x = -1�Ľ���
				// todo: ����v2 ע��͸�ӽ���
				//λ��
				v2.mPos.mX = -1.0;
				v2.mPos.mY = ver1.mPos.mY +
					(ver2.mPos.mY - ver1.mPos.mY) / 
					(ver2.mPos.mX - ver1.mPos.mX) * 
					(-1.0 - ver1.mPos.mX);
				v2.mPos.mZ = ver1.mPos.mZ +
					(ver2.mPos.mZ - ver1.mPos.mZ) / 
					(ver2.mPos.mX - ver1.mPos.mX) * 
					(-1.0 - ver1.mPos.mX);
				//����
				v2.mNormal = ver1.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver1.mColor) +  
					(GetRValue(ver2.mColor) - GetRValue(ver1.mColor)) / 
					(ver2.mPos.mX - ver1.mPos.mX) *
					(-1.0 - ver1.mPos.mX);
				double newG = GetGValue(ver1.mColor) +  
					(GetGValue(ver2.mColor) - GetGValue(ver1.mColor)) / 
					(ver2.mPos.mX - ver1.mPos.mX) *
					(-1.0 - ver1.mPos.mX);
				double newB = GetBValue(ver1.mColor) +  
					(GetBValue(ver2.mColor) - GetBValue(ver1.mColor)) / 
					(ver2.mPos.mX - ver1.mPos.mX) *
					(-1.0 - ver1.mPos.mX);
				v2.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (-1.0 - ver1.mPos.mX) / 
					(ver2.mPos.mX - ver1.mPos.mX);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver1.mZView;
				Z2 = ver2.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v2.mZView = Zt;
				v2.mTexture.u = ver1.mTexture.u + 
					t * (ver2.mTexture.u - ver1.mTexture.u);
				v2.mTexture.v = ver1.mTexture.v + 
					t * (ver2.mTexture.v - ver1.mTexture.v);
				// v0 v2 ver2���������װ��outTris
				_outTris.push_back(sTriangle(v0, v2, ver2));
			}
		}
		break;
	case FACE_RIGHT:
		{
			// x = 1
			Vertex ver0 = _tri.ver[0];
			Vertex ver1 = _tri.ver[1];
			Vertex ver2 = _tri.ver[2];
			// �������㰴�����������
			if (ver0.mPos.mX > ver1.mPos.mX)
			{
				SwapVertex(ver0, ver1);
			}
			if (ver0.mPos.mX > ver2.mPos.mX)
			{
				SwapVertex(ver0, ver2);
			}
			if (ver1.mPos.mX > ver2.mPos.mX)
			{
				SwapVertex(ver1, ver2);
			}

			//��������������� �ü�������ؿ�
			if (abs(ver2.mPos.mX - ver0.mPos.mX) < 0.0001f) return ;

			Vertex v0; //v0��x0x2��ƽ��x = -1�Ľ���
			//λ��
			v0.mPos.mX = 1.0;
			v0.mPos.mY = ver0.mPos.mY +
				(ver2.mPos.mY - ver0.mPos.mY) / 
				(ver2.mPos.mX - ver0.mPos.mX) * 
				(1.0 - ver0.mPos.mX);
			v0.mPos.mZ = ver0.mPos.mZ +
				(ver2.mPos.mZ - ver0.mPos.mZ) / 
				(ver2.mPos.mX - ver0.mPos.mX) * 
				(1.0 - ver0.mPos.mX);
			//����
			v0.mNormal = ver0.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver0.mColor) +  
				(GetRValue(ver2.mColor) - GetRValue(ver0.mColor)) / 
				(ver2.mPos.mX - ver0.mPos.mX) *
				(1.0 - ver0.mPos.mX);
			double newG = GetGValue(ver0.mColor) +  
				(GetGValue(ver2.mColor) - GetGValue(ver0.mColor)) / 
				(ver2.mPos.mX - ver0.mPos.mX) *
				(1.0 - ver0.mPos.mX);
			double newB = GetBValue(ver0.mColor) +  
				(GetBValue(ver2.mColor) - GetBValue(ver0.mColor)) / 
				(ver2.mPos.mX - ver0.mPos.mX) *
				(1.0 - ver0.mPos.mX);
			v0.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (1.0 - ver0.mPos.mX) / 
				(ver2.mPos.mX - ver0.mPos.mX);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver0.mZView;
			Z2 = ver2.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v0.mZView = Zt;
			v0.mTexture.u = ver0.mTexture.u + 
				t * (ver2.mTexture.u - ver0.mTexture.u);
			v0.mTexture.v = ver0.mTexture.v + 
				t * (ver2.mTexture.v - ver0.mTexture.v);

			//x0��ƽ���� x1 x2��ƽ����
			if (ver0.mPos.mX < 1.0f && ver1.mPos.mX > 1.0f)
			{
				Vertex v1; //v1��x0x1��ƽ��x = 1�Ľ���
				// todo: ����v1 ע��͸�ӽ���			
				//λ��
				v1.mPos.mX = 1.0;
				v1.mPos.mY = ver0.mPos.mY +
					(ver1.mPos.mY - ver0.mPos.mY) / 
					(ver1.mPos.mX - ver0.mPos.mX) * 
					(1.0 - ver0.mPos.mX);
				v1.mPos.mZ = ver0.mPos.mZ +
					(ver1.mPos.mZ - ver0.mPos.mZ) / 
					(ver1.mPos.mX - ver0.mPos.mX) * 
					(1.0 - ver0.mPos.mX);
				//����
				v1.mNormal = ver0.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver0.mColor) +  
					(GetRValue(ver1.mColor) - GetRValue(ver0.mColor)) / 
					(ver1.mPos.mX - ver0.mPos.mX) *
					(1.0 - ver0.mPos.mX);
				double newG = GetGValue(ver0.mColor) +  
					(GetGValue(ver1.mColor) - GetGValue(ver0.mColor)) / 
					(ver1.mPos.mX - ver0.mPos.mX) *
					(1.0 - ver0.mPos.mX);
				double newB = GetBValue(ver0.mColor) +  
					(GetBValue(ver1.mColor) - GetBValue(ver0.mColor)) / 
					(ver1.mPos.mX - ver0.mPos.mX) *
					(1.0 - ver0.mPos.mX);
				v1.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (1.0 - ver0.mPos.mX) / 
					(ver1.mPos.mX - ver0.mPos.mX);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver0.mZView;
				Z2 = ver1.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v1.mZView = Zt;
				v1.mTexture.u = ver0.mTexture.u + 
					t * (ver1.mTexture.u - ver0.mTexture.u);
				v1.mTexture.v = ver0.mTexture.v + 
					t * (ver1.mTexture.v - ver0.mTexture.v);
				// v0 v1 ver0���������װ��outTris
				_outTris.push_back(sTriangle(v0, v1, ver0));
			}
			
			//x0 x1��ƽ���� x2��ƽ����
			else if (ver0.mPos.mX < 1.0f && ver1.mPos.mX < 1.0f)
			{
				Vertex v2; //v2��x1x2��ƽ��x = 1�Ľ���
				// todo: ����v2 ע��͸�ӽ���
				//λ��
				v2.mPos.mX = 1.0;
				v2.mPos.mY = ver1.mPos.mY +
					(ver2.mPos.mY - ver1.mPos.mY) / 
					(ver2.mPos.mX - ver1.mPos.mX) * 
					(1.0 - ver1.mPos.mX);
				v2.mPos.mZ = ver1.mPos.mZ +
					(ver2.mPos.mZ - ver1.mPos.mZ) / 
					(ver2.mPos.mX - ver1.mPos.mX) * 
					(1.0 - ver1.mPos.mX);
				//����
				v2.mNormal = ver1.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver1.mColor) +  
					(GetRValue(ver2.mColor) - GetRValue(ver1.mColor)) / 
					(ver2.mPos.mX - ver1.mPos.mX) *
					(1.0 - ver1.mPos.mX);
				double newG = GetGValue(ver1.mColor) +  
					(GetGValue(ver2.mColor) - GetGValue(ver1.mColor)) / 
					(ver2.mPos.mX - ver1.mPos.mX) *
					(1.0 - ver1.mPos.mX);
				double newB = GetBValue(ver1.mColor) +  
					(GetBValue(ver2.mColor) - GetBValue(ver1.mColor)) / 
					(ver2.mPos.mX - ver1.mPos.mX) *
					(1.0 - ver1.mPos.mX);
				v2.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (1.0 - ver1.mPos.mX) / 
					(ver2.mPos.mX - ver1.mPos.mX);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver1.mZView;
				Z2 = ver2.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v2.mZView = Zt;
				v2.mTexture.u = ver1.mTexture.u + 
					t * (ver2.mTexture.u - ver1.mTexture.u);
				v2.mTexture.v = ver1.mTexture.v + 
					t * (ver2.mTexture.v - ver1.mTexture.v);
				// v0 v2 ver1 ver0�������������װ��outTris
				_outTris.push_back(sTriangle(v0, v2, ver1));
				_outTris.push_back(sTriangle(v0, ver1, ver0));
			}
		}
		break;
	case FACE_NEAR:
		{
			// z = -1
			Vertex ver0 = _tri.ver[0];
			Vertex ver1 = _tri.ver[1];
			Vertex ver2 = _tri.ver[2];
			// �������㰴�Խ���Զ����
			if (ver0.mPos.mZ > ver1.mPos.mZ)
			{
				SwapVertex(ver0, ver1);
			}
			if (ver0.mPos.mZ > ver2.mPos.mZ)
			{
				SwapVertex(ver0, ver2);
			}
			if (ver1.mPos.mZ > ver2.mPos.mZ)
			{
				SwapVertex(ver1, ver2);
			}

			//��������������� �ü�������ؿ�
			if (abs(ver2.mPos.mZ - ver0.mPos.mZ) < 0.0001f) return ;

			Vertex v0; //v0��z0z2��ƽ��z = -1�Ľ���
			//λ��
			v0.mPos.mZ = -1.0;
			v0.mPos.mY = ver0.mPos.mY +
				(ver2.mPos.mY - ver0.mPos.mY) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) * 
				(-1.0 - ver0.mPos.mZ);
			v0.mPos.mX = ver0.mPos.mX +
				(ver2.mPos.mX - ver0.mPos.mX) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) * 
				(-1.0 - ver0.mPos.mZ);
			//����
			v0.mNormal = ver0.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver0.mColor) +  
				(GetRValue(ver2.mColor) - GetRValue(ver0.mColor)) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) *
				(-1.0 - ver0.mPos.mZ);
			double newG = GetGValue(ver0.mColor) +  
				(GetGValue(ver2.mColor) - GetGValue(ver0.mColor)) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) *
				(-1.0 - ver0.mPos.mZ);
			double newB = GetBValue(ver0.mColor) +  
				(GetBValue(ver2.mColor) - GetBValue(ver0.mColor)) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) *
				(-1.0 - ver0.mPos.mZ);
			v0.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (-1.0 - ver0.mPos.mZ) / 
				(ver2.mPos.mZ - ver0.mPos.mZ);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver0.mZView;
			Z2 = ver2.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v0.mZView = Zt;
			v0.mTexture.u = ver0.mTexture.u + 
				t * (ver2.mTexture.u - ver0.mTexture.u);
			v0.mTexture.v = ver0.mTexture.v + 
				t * (ver2.mTexture.v - ver0.mTexture.v);

			//z0��ƽ���� z1 z2��ƽ����
			if (ver0.mPos.mZ < -1.0f && ver1.mPos.mZ > -1.0f)
			{
				Vertex v1; //v1��z0z1��ƽ��z = -1�Ľ���
				// todo: ����v1 ע��͸�ӽ���			
				//λ��
				v1.mPos.mZ = -1.0;
				v1.mPos.mY = ver0.mPos.mY +
					(ver1.mPos.mY - ver0.mPos.mY) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) * 
					(-1.0 - ver0.mPos.mZ);
				v1.mPos.mX = ver0.mPos.mX +
					(ver1.mPos.mX - ver0.mPos.mX) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) * 
					(-1.0 - ver0.mPos.mZ);
				//����
				v1.mNormal = ver0.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver0.mColor) +  
					(GetRValue(ver1.mColor) - GetRValue(ver0.mColor)) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) *
					(-1.0 - ver0.mPos.mZ);
				double newG = GetGValue(ver0.mColor) +  
					(GetGValue(ver1.mColor) - GetGValue(ver0.mColor)) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) *
					(-1.0 - ver0.mPos.mZ);
				double newB = GetBValue(ver0.mColor) +  
					(GetBValue(ver1.mColor) - GetBValue(ver0.mColor)) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) *
					(-1.0 - ver0.mPos.mZ);
				v1.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (-1.0 - ver0.mPos.mZ) / 
					(ver1.mPos.mZ - ver0.mPos.mZ);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver0.mZView;
				Z2 = ver1.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v1.mZView = Zt;
				v1.mTexture.u = ver0.mTexture.u + 
					t * (ver1.mTexture.u - ver0.mTexture.u);
				v1.mTexture.v = ver0.mTexture.v + 
					t * (ver1.mTexture.v - ver0.mTexture.v);
				// v0 v1 ver1 ver2�������������װ��outTris
				_outTris.push_back(sTriangle(v1, ver1, ver2));
				_outTris.push_back(sTriangle(v1, ver2, v0));
			}
			
			//z0 z1��ƽ���� z2��ƽ����
			else if (ver0.mPos.mZ < -1.0f && ver1.mPos.mZ < -1.0f)
			{
				Vertex v2; //v2��z1z2��ƽ��z = -1�Ľ���
				// todo: ����v2 ע��͸�ӽ���
				//λ��
				v2.mPos.mZ = -1.0;
				v2.mPos.mY = ver1.mPos.mY +
					(ver2.mPos.mY - ver1.mPos.mY) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) * 
					(-1.0 - ver1.mPos.mZ);
				v2.mPos.mX = ver1.mPos.mX +
					(ver2.mPos.mX - ver1.mPos.mX) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) * 
					(-1.0 - ver1.mPos.mZ);
				//����
				v2.mNormal = ver1.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver1.mColor) +  
					(GetRValue(ver2.mColor) - GetRValue(ver1.mColor)) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) *
					(-1.0 - ver1.mPos.mZ);
				double newG = GetGValue(ver1.mColor) +  
					(GetGValue(ver2.mColor) - GetGValue(ver1.mColor)) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) *
					(-1.0 - ver1.mPos.mZ);
				double newB = GetBValue(ver1.mColor) +  
					(GetBValue(ver2.mColor) - GetBValue(ver1.mColor)) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) *
					(-1.0 - ver1.mPos.mZ);
				v2.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (-1.0 - ver1.mPos.mZ) / 
					(ver2.mPos.mZ - ver1.mPos.mZ);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver1.mZView;
				Z2 = ver2.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v2.mZView = Zt;
				v2.mTexture.u = ver1.mTexture.u + 
					t * (ver2.mTexture.u - ver1.mTexture.u);
				v2.mTexture.v = ver1.mTexture.v + 
					t * (ver2.mTexture.v - ver1.mTexture.v);
				// v0 v2 ver2���������װ��outTris
				_outTris.push_back(sTriangle(v0, v2, ver2));
			}
		}
		break;
	case FACE_FAR:
		{
			// z = 1
			Vertex ver0 = _tri.ver[0];
			Vertex ver1 = _tri.ver[1];
			Vertex ver2 = _tri.ver[2];
			// �������㰴�Խ���Զ����
			if (ver0.mPos.mZ > ver1.mPos.mZ)
			{
				SwapVertex(ver0, ver1);
			}
			if (ver0.mPos.mZ > ver2.mPos.mZ)
			{
				SwapVertex(ver0, ver2);
			}
			if (ver1.mPos.mZ > ver2.mPos.mZ)
			{
				SwapVertex(ver1, ver2);
			}

			//��������������� �ü�������ؿ�
			if (abs(ver2.mPos.mZ - ver0.mPos.mZ) < 0.0001f) return ;

			Vertex v0; //v0��z0z2��ƽ��z = -1�Ľ���
			//λ��
			v0.mPos.mZ = 1.0;
			v0.mPos.mY = ver0.mPos.mY +
				(ver2.mPos.mY - ver0.mPos.mY) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) * 
				(1.0 - ver0.mPos.mZ);
			v0.mPos.mX = ver0.mPos.mX +
				(ver2.mPos.mX - ver0.mPos.mX) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) * 
				(1.0 - ver0.mPos.mZ);
			//����
			v0.mNormal = ver0.mNormal;
			//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
			double newR = GetRValue(ver0.mColor) +  
				(GetRValue(ver2.mColor) - GetRValue(ver0.mColor)) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) *
				(1.0 - ver0.mPos.mZ);
			double newG = GetGValue(ver0.mColor) +  
				(GetGValue(ver2.mColor) - GetGValue(ver0.mColor)) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) *
				(1.0 - ver0.mPos.mZ);
			double newB = GetBValue(ver0.mColor) +  
				(GetBValue(ver2.mColor) - GetBValue(ver0.mColor)) / 
				(ver2.mPos.mZ - ver0.mPos.mZ) *
				(1.0 - ver0.mPos.mZ);
			v0.mColor = RGB(newR, newG, newB);
			//�������� ��͸�ӽ���
			double s = (1.0 - ver0.mPos.mZ) / 
				(ver2.mPos.mZ - ver0.mPos.mZ);
			double Z1 = 0.0;
			double Z2 = 0.0;
			double Zt = 0.0;
			double t = s;
			Z1 = ver0.mZView;
			Z2 = ver2.mZView;
			if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
			v0.mZView = Zt;
			v0.mTexture.u = ver0.mTexture.u + 
				t * (ver2.mTexture.u - ver0.mTexture.u);
			v0.mTexture.v = ver0.mTexture.v + 
				t * (ver2.mTexture.v - ver0.mTexture.v);

			//z0��ƽ���� z1 z2��ƽ����
			if (ver0.mPos.mZ < 1.0f && ver1.mPos.mZ > 1.0f)
			{
				Vertex v1; //v1��z0z1��ƽ��z = 1�Ľ���
				// todo: ����v1 ע��͸�ӽ���			
				//λ��
				v1.mPos.mZ = 1.0;
				v1.mPos.mY = ver0.mPos.mY +
					(ver1.mPos.mY - ver0.mPos.mY) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) * 
					(1.0 - ver0.mPos.mZ);
				v1.mPos.mX = ver0.mPos.mX +
					(ver1.mPos.mX - ver0.mPos.mX) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) * 
					(1.0 - ver0.mPos.mZ);
				//����
				v1.mNormal = ver0.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver0.mColor) +  
					(GetRValue(ver1.mColor) - GetRValue(ver0.mColor)) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) *
					(1.0 - ver0.mPos.mZ);
				double newG = GetGValue(ver0.mColor) +  
					(GetGValue(ver1.mColor) - GetGValue(ver0.mColor)) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) *
					(1.0 - ver0.mPos.mZ);
				double newB = GetBValue(ver0.mColor) +  
					(GetBValue(ver1.mColor) - GetBValue(ver0.mColor)) / 
					(ver1.mPos.mZ - ver0.mPos.mZ) *
					(1.0 - ver0.mPos.mZ);
				v1.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (1.0 - ver0.mPos.mZ) / 
					(ver1.mPos.mZ - ver0.mPos.mZ);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver0.mZView;
				Z2 = ver1.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v1.mZView = Zt;
				v1.mTexture.u = ver0.mTexture.u + 
					t * (ver1.mTexture.u - ver0.mTexture.u);
				v1.mTexture.v = ver0.mTexture.v + 
					t * (ver1.mTexture.v - ver0.mTexture.v);
				// v0 v1 ver0���������װ��outTris
				_outTris.push_back(sTriangle(v0, v1, ver0));
			}
			
			//z0 z1��ƽ���� z2��ƽ����
			else if (ver0.mPos.mZ < 1.0f && ver1.mPos.mZ < 1.0f)
			{
				Vertex v2; //v2��z1z2��ƽ��z = 1�Ľ���
				// todo: ����v2 ע��͸�ӽ���
				//λ��
				v2.mPos.mZ = 1.0;
				v2.mPos.mY = ver1.mPos.mY +
					(ver2.mPos.mY - ver1.mPos.mY) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) * 
					(1.0 - ver1.mPos.mZ);
				v2.mPos.mX = ver1.mPos.mX +
					(ver2.mPos.mX - ver1.mPos.mX) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) * 
					(1.0 - ver1.mPos.mZ);
				//����
				v2.mNormal = ver1.mNormal;
				//���ʵ���ɫ ���Բ�ֵ ����͸�ӽ���
				double newR = GetRValue(ver1.mColor) +  
					(GetRValue(ver2.mColor) - GetRValue(ver1.mColor)) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) *
					(1.0 - ver1.mPos.mZ);
				double newG = GetGValue(ver1.mColor) +  
					(GetGValue(ver2.mColor) - GetGValue(ver1.mColor)) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) *
					(1.0 - ver1.mPos.mZ);
				double newB = GetBValue(ver1.mColor) +  
					(GetBValue(ver2.mColor) - GetBValue(ver1.mColor)) / 
					(ver2.mPos.mZ - ver1.mPos.mZ) *
					(1.0 - ver1.mPos.mZ);
				v2.mColor = RGB(newR, newG, newB);
				//�������� ��͸�ӽ���
				double s = (1.0 - ver1.mPos.mZ) / 
					(ver2.mPos.mZ - ver1.mPos.mZ);
				double Z1 = 0.0;
				double Z2 = 0.0;
				double Zt = 0.0;
				double t = s;
				Z1 = ver1.mZView;
				Z2 = ver2.mZView;
				if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
				if (Zt != 0) Zt = 1 / Zt;
				if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
				v2.mZView = Zt;
				v2.mTexture.u = ver1.mTexture.u + 
					t * (ver2.mTexture.u - ver1.mTexture.u);
				v2.mTexture.v = ver1.mTexture.v + 
					t * (ver2.mTexture.v - ver1.mTexture.v);
				// v0 v2 ver1 ver0���������װ��outTris
				_outTris.push_back(sTriangle(v0, v2, ver1));
				_outTris.push_back(sTriangle(v0, ver1, ver2));
			}
		}
		break;
	}
}

void Triangle::CalAreaCode(Vertex& _ver)
{
	BYTE areaCode = 0;
	if (_ver.mPos.mX < -1.0f) areaCode |= 1;
	if (_ver.mPos.mX > 1.0f) areaCode |= 2;
	if (_ver.mPos.mY < -1.0f) areaCode |= 4;
	if (_ver.mPos.mY > 1.0f) areaCode |= 8;
	if (_ver.mPos.mZ < -1.0f) areaCode |= 16;
	if (_ver.mPos.mZ > 1.0f) areaCode |= 32;
	_ver.mAreaCode = areaCode;
}

//�����߶δ�������Щ��
void Triangle::LinePunctureFaces(
	const Vertex& _ver0, 
	const Vertex& _ver1, 
	vector<FACE_TYPE>& _outFaces)
{
	auto code0 = _ver0.mAreaCode;
	auto code1 = _ver1.mAreaCode;
	//0  0  0  0  0  0
	//Զ �� �� �� �� ��
	//���һ���˵��ĳλΪ0����һ���˵����ͬλΪ1
	//����߶δ�����Ӧ�Ĳü��߽�
	BYTE a = (code0 ^ code1);
	int p = 0; //�� 
	while (a)
	{
		if (a & 1 == 1)
		{
			_outFaces.push_back((FACE_TYPE)p);
		}
		++p;
		a >>= 1;
	}
}

//���������δ�������Щ��
void Triangle::TriPunctureFaces(
	const sTriangle& _tri,
	queue<FACE_TYPE>& _outFaces)
{
	vector<FACE_TYPE> tempFaces;
	LinePunctureFaces(
		_tri.ver[0],
		_tri.ver[1],
		tempFaces);

	LinePunctureFaces(
		_tri.ver[0],
		_tri.ver[2],
		tempFaces);

	LinePunctureFaces(
		_tri.ver[1],
		_tri.ver[2],
		tempFaces);

	int a[6];
	memset(a, 0, sizeof(int) * 6);
	for (auto iter = tempFaces.begin();
		iter != tempFaces.end(); ++iter)
	{
		a[(int)(*iter)] = 1;
	}
	for (int i = 0; i < 6; ++i)
	{
		if (a[i] == 1)
		{
			_outFaces.push((FACE_TYPE)i);
		}
	}
}

//�ù淶�Ӽ���ȥ�ü�һ�������� �õ��ܶ�������
void Triangle::Cut(
	const sTriangle& _tri, 
	std::vector<sTriangle>& _outTris)
{
	//����������㶼��ĳ�ü�ƽ��֮�� �򷵻ؿ�
	if ((_tri.ver[0].mAreaCode &
		_tri.ver[1].mAreaCode &
		_tri.ver[2].mAreaCode) == 1)
	{
		return ;
	}

	//����������㶼���ڲ� �򷵻����������
	else if ((_tri.ver[0].mAreaCode |
		_tri.ver[1].mAreaCode |
		_tri.ver[2].mAreaCode) == 0)
	{
		_outTris.push_back(_tri);
	}

	else 
	{
		//��������Ҫ�ü������
		queue<FACE_TYPE> faces;
		TriPunctureFaces(_tri, faces);

		vector<sTriangle> inTris;
		inTris.push_back(_tri);

		while (!faces.empty())
		{
			auto currFace = faces.front();
			faces.pop();
			for (auto iter = inTris.begin();
				iter != inTris.end(); ++iter)
			{
				auto currTri = *iter;
				Cut(currTri, currFace, _outTris);
			}
			if (faces.empty()) break;
			inTris.clear();
			for (auto iter = _outTris.begin();
				iter != _outTris.end(); ++iter)
			{
				inTris.push_back(*iter);
			}
			_outTris.clear();
		}
	}
}

void Triangle::Draw(Matrix4x4& _worldMatrix)
{
	//for (int i = 0; i < SCREEN_WIDTH; ++i)
	//{
	//	memset(mEdgePoints[i], 0, sizeof(int) * SCREEN_HEIGHT);
	//}
	
	Scene* pScene = Scene::GetInstance();
	Camera& camera = pScene->GetCamera();

	Matrix4x4 viewMatrix = camera.GetViewTransform();
	Matrix4x4 projectMatrix = camera.GetProjectTransform();

	ViewPortParam viewPortParam = pScene->GetViewPort();
	Matrix4x4 viewPortMatrix = Matrix4x4::BuildViewPort(
		viewPortParam.x,
		viewPortParam.y,
		viewPortParam.width,
		viewPortParam.height,
		viewPortParam.minZ,
		viewPortParam.maxZ);

	//mPoints�����λ�ò��䣬����tempVertex��
	Vertex tempVertex[3];
	for (int i = 0; i < 3; ++i)
	{
		tempVertex[i] = mPoints[i];
		//world transform ģ�ͱ任
		tempVertex[i].MatrixMulti(_worldMatrix);
		//Phong����ģ��
		tempVertex[i].SetColor(CaculateLight(tempVertex[i]));
		//view transform ȡ���任
		tempVertex[i].MatrixMulti(viewMatrix);
		tempVertex[i].mZView = tempVertex[i].mPos.mZ;
		//project transform ͶӰ�任
		tempVertex[i].MatrixMulti(projectMatrix);
	}

	// W�ü� �õ�����w < 0�ĵ�
	sTriangle wInTri;
	vector<sTriangle> wOutTris;
	for (int i = 0; i < 3; ++i)
	{
		wInTri.ver[i] = tempVertex[i];
	}
	CutW(wInTri, wOutTris);

	Vertex wTempVertex[3];
	for (auto iter = wOutTris.begin();
		iter != wOutTris.end(); ++iter)
	{
		wTempVertex[0] = iter->ver[0];
		wTempVertex[1] = iter->ver[1];
		wTempVertex[2] = iter->ver[2];

		for (int i = 0; i < 3; ++i)
		{
			//project divide ͸�ӳ���
			wTempVertex[i].DivideW();
		}

		//�����޳�(������ͶӰ�任֮��)
		Vector4 camDirecton(0, 0, 1, 0);
		Vector4 N1 = wTempVertex[1].mPos - wTempVertex[0].mPos;
		Vector4 N2 = wTempVertex[2].mPos - wTempVertex[0].mPos;
		Vector4 faceNormalNeg =  N1.CrossProduct(N2);
		faceNormalNeg.Normalize();
		if (faceNormalNeg.Dot(camDirecton) < 0)
		{
			continue ;
		}

		//���ڹ淶�Ӽ���Ĳü�
		sTriangle inTri;
		for (int i = 0; i < 3; ++i)
		{
			inTri.ver[i] = wTempVertex[i];
			CalAreaCode(inTri.ver[i]);
		}
		vector<sTriangle> outTris;
		Cut(inTri, outTris);

		for (auto iter_ = outTris.begin();
			iter_ != outTris.end(); ++iter_)
		{
			for (int i = 0; i < 3; ++i)
			{ 
				// viewport transform �ӿڱ任
				iter_->ver[i].MatrixMulti(viewPortMatrix);
			}
			DrawTriangle2D(iter_->ver[0], 
						   iter_->ver[1], 
						   iter_->ver[2]);
		}
	}

	//���Գ�������ʱ��
	//LARGE_INTEGER lv, lv_b;
	
	//��������� todo: ���Ϸ���
	//Gouraud��ɫ��
	//��color��ʱ����������Ӱ������
	//������� + ������������ => ��������ÿ�������������(u, v) u 0-1 v 0-1
	//DrawTriangle2D(tempVertex[0], tempVertex[1], tempVertex[2]);
	//���Գ�������ʱ��
	//QueryPerformanceCounter(&lv);
	//QueryPerformanceCounter(&lv_b);
	//std::cout.precision(6);
	//LONGLONG duration = lv_b.QuadPart - lv.QuadPart;
	//double timeElapsedTotal = mSecondPerTick * duration;
	//cout << fixed << showpoint << timeElapsedTotal << endl;
}

//DDA�����㷨
void Triangle::DDALine(int _xa, int _ya, int _xb, int _yb,
	std::vector<Pixel>& _outPixels)
{
	double deltaX;
	double deltaY;
	double x;
	double y;
	int dx;
	int dy;
	int steps;

	dx = _xb - _xa;
	dy = _yb - _ya;
	if (abs(dx) > abs(dy))
	{
		steps = abs(dx);
	}
	else
	{
		steps = abs(dy);
	}
	deltaX = ((double)dx) / ((double)steps);
	deltaY = ((double)dy) / ((double)steps);

	x = _xa;
	y = _ya;
	for (int k = 1; k <= steps; ++k)
	{
		x += deltaX;
		y += deltaY;
		_outPixels.push_back(Pixel(x, y));
	}
}

void Triangle::Swap(int& _x, int& _y)
{
	int temp = _y;
	_y = _x;
	_x = temp;
}

void Triangle::SwapColor(COLORREF& _x, COLORREF& _y)
{
	COLORREF temp = _y;
	_y = _x;
	_x = temp;
}

void Triangle::SwapVertex(Vertex& _x, Vertex& _y)
{
	Vertex temp = _y;
	_y = _x;
	_x = temp;
}

void Triangle::DrawTopTriangle(Vertex& _ver0, Vertex& _ver1, Vertex& _ver2)
{
	//�ж������������
	if (abs(_ver0.mPos.mY - _ver1.mPos.mY) < 0.000001f) 
	{/*empty*/}
	else if (abs(_ver0.mPos.mY - _ver2.mPos.mY) < 0.000001f)
	{
		SwapVertex(_ver2, _ver1);
	}
	else if (abs(_ver1.mPos.mY - _ver2.mPos.mY) < 0.000001f)
	{
		SwapVertex(_ver0, _ver2);
	}
	else
	{ //����ƽ��������
		return ;
	}
	if (_ver1.mPos.mX < _ver0.mPos.mX)
	{
		SwapVertex(_ver1, _ver0);
	}
	else if (abs(_ver1.mPos.mX - _ver0.mPos.mX) < 0.000001f)
	{ //����������
		return ; 
	}

	double _x0 = _ver0.mPos.mX;
	double _x1 = _ver1.mPos.mX;
	double _x2 = _ver2.mPos.mX;
	double _y0 = _ver0.mPos.mY;
	double _y1 = _ver1.mPos.mY;
	double _y2 = _ver2.mPos.mY;
	COLORREF _color0 = _ver0.mColor;
	COLORREF _color1 = _ver1.mColor;
	COLORREF _color2 = _ver2.mColor;

	//�����������
	double dxy_left = (_x2 - _x0) * 1.0 / (_y2 - _y0);
	double dxy_right = (_x1 - _x2) * 1.0 / (_y1 - _y2);
	double dr_left = (GetRValue(_color2) - GetRValue(_color0)) * 1.0 / (_y2 - _y0);
	double dr_right = (GetRValue(_color1) - GetRValue(_color2)) * 1.0 / (_y1 - _y2);
	double dg_left = (GetGValue(_color2) - GetGValue(_color0)) * 1.0 / (_y2 - _y0);
	double dg_right = (GetGValue(_color1) - GetGValue(_color2)) * 1.0 / (_y1 - _y2);
	double db_left = (GetBValue(_color2) - GetBValue(_color0)) * 1.0 / (_y2 - _y0);
	double db_right = (GetBValue(_color1) - GetBValue(_color2)) * 1.0 / (_y1 - _y2);


	//��ʼ�������
	double xs = _x0;
	double xe = _x1;
	double rs = GetRValue(_color0);
	double gs = GetGValue(_color0);
	double bs = GetBValue(_color0);
	double re = GetRValue(_color1);
	double ge = GetGValue(_color1);
	double be = GetBValue(_color1);

	if (abs(_ver2.mPos.mY - _ver0.mPos.mY) < 0.0001f)
		return ;

	for (double y = _y0; y <= _y2; y += 1.0f)
	{
		double s = (double)(y - _ver0.mPos.mY) / 
			(_ver2.mPos.mY - _ver0.mPos.mY);
		double Zt = 0.0;
		double t = s;
		double Z0 = _ver0.mZView;
		double Z2 = _ver2.mZView;
		if (Z0 != 0.0 && Z2 != 0.0) Zt = 1 / Z0 + s * (1 / Z2 - 1 / Z0);
		if (Zt != 0) Zt = 1 / Zt;
		if (Z2 != Z0) t = (Zt - Z0) / (Z2 - Z0);
		double lz = Zt;
		double lu = _ver0.mTexture.u + 
			t * (_ver2.mTexture.u - _ver0.mTexture.u);
		double lv = _ver0.mTexture.v + 
			t * (_ver2.mTexture.v - _ver0.mTexture.v);

		t = s;
		double Z1 = _ver1.mZView;
		if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
		if (Zt != 0) Zt = 1 / Zt;
		if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);
		double rz = Zt;
		double ru = _ver1.mTexture.u + 
			t * (_ver2.mTexture.u - _ver1.mTexture.u);
		double rv = _ver1.mTexture.v + 
			t * (_ver2.mTexture.v - _ver1.mTexture.v);

		DrawSimpleLine(xs, xe, y,
			rs, gs, bs, re, ge, be,
			lz, lu, lv, rz, ru, rv);
		xs += dxy_left;
		xe += dxy_right;
		rs += dr_left;
		gs += dg_left;
		bs += db_left;
		re += dr_right;
		ge += dg_right;
		be += db_right;
	}
}

void Triangle::DrawBottomTriangle(Vertex& _ver0, Vertex& _ver1, Vertex& _ver2)
{
	//���ж������������
	if (abs(_ver2.mPos.mY - _ver1.mPos.mY) < 0.000001f)
	{/*empty*/}
	else if (abs(_ver2.mPos.mY - _ver0.mPos.mY) < 0.000001f)
	{
		SwapVertex(_ver0, _ver1);
	}
	else if (abs(_ver0.mPos.mY - _ver1.mPos.mY) < 0.000001f)
	{
		SwapVertex(_ver0, _ver2);
	}
	else
	{ //��ƽ��������
		return ;
	}
	if (_ver1.mPos.mX < _ver2.mPos.mX)
	{
		SwapVertex(_ver1, _ver2);
	}
	else if (abs(_ver1.mPos.mX - _ver2.mPos.mX) < 0.000001f) 
	{
		return ; //����������
	}

	double _x0 = _ver0.mPos.mX;
	double _x1 = _ver1.mPos.mX;
	double _x2 = _ver2.mPos.mX;
	double _y0 = _ver0.mPos.mY;
	double _y1 = _ver1.mPos.mY;
	double _y2 = _ver2.mPos.mY;
	COLORREF _color0 = _ver0.mColor;
	COLORREF _color1 = _ver1.mColor;
	COLORREF _color2 = _ver2.mColor;

	//�����������
	double dxy_left = (_x2 - _x0) * 1.0 / (_y2 - _y0);
	double dxy_right = (_x1 - _x0) * 1.0 / (_y1 - _y0);
	double dr_left = (GetRValue(_color2) - GetRValue(_color0)) * 1.0 / (_y2 - _y0);
	double dr_right = (GetRValue(_color1) - GetRValue(_color0)) * 1.0 / (_y1 - _y0);
	double dg_left = (GetGValue(_color2) - GetGValue(_color0)) * 1.0 / (_y2 - _y0);
	double dg_right = (GetGValue(_color1) - GetGValue(_color0)) * 1.0 / (_y1 - _y0);
	double db_left = (GetBValue(_color2) - GetBValue(_color0)) * 1.0 / (_y2 - _y0);
	double db_right = (GetBValue(_color1) - GetBValue(_color0)) * 1.0 / (_y1 - _y0);

	//��ʼ�������
	double xs = _x0;
	double xe = _x0;
	double rs = GetRValue(_color0);
	double gs = GetGValue(_color0);
	double bs = GetBValue(_color0);
	double re = GetRValue(_color0);
	double ge = GetGValue(_color0);
	double be = GetBValue(_color0);

	if (abs(_ver2.mPos.mY - _ver0.mPos.mY) < 0.0001f)
		return ;

	for (double y = _y0; y <= _y2; y += 1.0f)
	{

		double s = (double)(y - _ver0.mPos.mY) / 
			(_ver2.mPos.mY - _ver0.mPos.mY);
		double Zt = 0.0;
		double t = s;
		double Z0 = _ver0.mZView;
		double Z2 = _ver2.mZView;
		if (Z0 != 0.0 && Z2 != 0.0) Zt = 1 / Z0 + s * (1 / Z2 - 1 / Z0);
		if (Zt != 0) Zt = 1 / Zt;
		if (Z2 != Z0) t = (Zt - Z0) / (Z2 - Z0);
		double lz = Zt;
		double lu = _ver0.mTexture.u + 
			t * (_ver2.mTexture.u - _ver0.mTexture.u);
		double lv = _ver0.mTexture.v + 
			t * (_ver2.mTexture.v - _ver0.mTexture.v);

		t = s;
		double Z1 = _ver1.mZView;
		if (Z0 != 0.0 && Z1 != 0.0) Zt = 1 / Z0 + s * (1 / Z1 - 1 / Z0);
		if (Zt != 0) Zt = 1 / Zt;
		if (Z1 != Z0) t = (Zt - Z0) / (Z1 - Z0);
		double rz = Zt;
		double ru = _ver0.mTexture.u + 
			t * (_ver1.mTexture.u - _ver0.mTexture.u);
		double rv = _ver0.mTexture.v + 
			t * (_ver1.mTexture.v - _ver0.mTexture.v);

		DrawSimpleLine(xs, xe, y,
			rs, gs, bs, re, ge, be,
			lz, lu, lv, rz, ru, rv);
		xs += dxy_left;
		xe += dxy_right;
		rs += dr_left;
		gs += dg_left;
		bs += db_left;
		re += dr_right;
		ge += dg_right;
		be += db_right;
	}
}

void Triangle::DrawTriangle2D(Vertex& _ver0, Vertex& _ver1, Vertex& _ver2)
{
	int _x0 = _ver0.mPos.mX;
	int _x1 = _ver1.mPos.mX;
	int _x2 = _ver2.mPos.mX;
	int _y0 = _ver0.mPos.mY;
	int _y1 = _ver1.mPos.mY;
	int _y2 = _ver2.mPos.mY;
	
	if ((_x0 == _x1 && _x1 == _x2)
		|| (_y0 == _y1 && _y1 == _y2))
	{
		return ; //����ĵ��޷�����������
	}

	//�������㰴���϶�������
	if (_ver0.mPos.mY > _ver1.mPos.mY)
	{
		SwapVertex(_ver0, _ver1);
	}
	if (_ver0.mPos.mY > _ver2.mPos.mY)
	{
		SwapVertex(_ver0, _ver2);
	}
	if (_ver1.mPos.mY > _ver2.mPos.mY)
	{
		SwapVertex(_ver1, _ver2);
	}

	//���л���
	if (abs(_ver0.mPos.mY - _ver1.mPos.mY) < 0.000001f) //ƽ��������
	{
		DrawTopTriangle(_ver0, _ver1, _ver2);
	}
	else if (abs(_ver1.mPos.mY - _ver2.mPos.mY) < 0.000001f) //ƽ��������
	{
		DrawBottomTriangle(_ver0, _ver1, _ver2);
	}
	else
	{ //�ֳ�һ��ƽ�������κ�һ��ƽ��������
		int newX = _ver0.mPos.mX + 0.5 + 
			(double)1.0 * (_ver1.mPos.mY - _ver0.mPos.mY) * 
			(_ver2.mPos.mX - _ver0.mPos.mX) / (_ver2.mPos.mY - _ver0.mPos.mY);
		double newR = GetRValue(_ver0.mColor) + 
			(double)1.0 * (_ver1.mPos.mY - _ver0.mPos.mY) * 
			(GetRValue(_ver2.mColor) - GetRValue(_ver0.mColor)) / 
			(_ver2.mPos.mY - _ver0.mPos.mY);
		double newG = GetGValue(_ver0.mColor) + 
			(double)1.0 * (_ver1.mPos.mY - _ver0.mPos.mY) * 
			(GetGValue(_ver2.mColor) - GetGValue(_ver0.mColor)) / 
			(_ver2.mPos.mY - _ver0.mPos.mY);
		double newB = GetBValue(_ver0.mColor) + 
			(double)1.0 * (_ver1.mPos.mY - _ver0.mPos.mY) * 
			(GetBValue(_ver2.mColor) - GetBValue(_ver0.mColor)) / 
			(_ver2.mPos.mY - _ver0.mPos.mY);
		//�µ���ʵ���ɫ ���Բ�ֵ δ��͸�ӽ���
		COLORREF newColor = RGB((int)newR, (int)newG, (int)newB);
		Vertex newVertex = _ver1;
		newVertex.mPos.mX = newX;
		newVertex.mColor = newColor;
		//todo: ��ֵ�õ��µ����Ⱥ��������� ��͸�ӽ���
		//newVertex.mZView = ?
		//newVertex.mTexture = ?
		double s = (_ver1.mPos.mY - _ver0.mPos.mY) / 
			(_ver2.mPos.mY - _ver0.mPos.mY);

		double Z1 = 0.0;
		double Z2 = 0.0;
		double Zt = 0.0;
		double t = s;
		Z1 = _ver0.mZView;
		Z2 = _ver2.mZView;
		if (Z1 != 0.0 && Z2 != 0.0) Zt = 1 / Z1 + s * (1 / Z2 - 1 / Z1);
		if (Zt != 0) Zt = 1 / Zt;
		if (Z2 != Z1) t = (Zt - Z1) / (Z2 - Z1);

		newVertex.mZView = Zt;
		newVertex.mTexture.u = _ver0.mTexture.u + 
			t * (_ver2.mTexture.u - _ver0.mTexture.u);
		newVertex.mTexture.v = _ver0.mTexture.v + 
			t * (_ver2.mTexture.v - _ver0.mTexture.v);
		
		DrawBottomTriangle(_ver0, newVertex, _ver1);
		DrawTopTriangle(newVertex, _ver1, _ver2);
	}
}

//����ƽ����
void Triangle::DrawSimpleLine(double _x0, double _x1, double _y,
	double _rs, double _gs, double _bs, double _re, double _ge, double _be,
	double _lz, double _lu, double _lv, double _rz, double _ru, double _rv)
{
	double deltaR = 0.0;
	double deltaG = 0.0;
	double deltaB = 0.0;
	if (abs(_x0 - _x1) > 0.0001f)
	{
		deltaR = (double)(_re - _rs) * 1.0 / (double)(_x1 - _x0);
		deltaG = (double)(_ge - _gs) * 1.0 / (double)(_x1 - _x0);	
		deltaB = (double)(_be - _bs) * 1.0 / (double)(_x1 - _x0);	
	}
	double rs = _rs;
	double gs = _gs;
	double bs = _bs;
	int height = Scene::GetInstance()->mTextHeight;
	int width = Scene::GetInstance()->mTextWidth;
	for (double x = _x0; x <= _x1; x += 1.0f)
	{
		if (abs(_x1 - _x0) > 0.0001f)
		{
			double s = (double)(x - _x0) / (double)(_x1 - _x0);
			double Zt = 0.0;
			double t = s;
			double Z0 = _lz;
			double Z2 = _rz;
			if (Z0 != 0.0 && Z2 != 0.0) Zt = 1 / Z0 + s * (1 / Z2 - 1 / Z0);
			if (Zt != 0) Zt = 1 / Zt;
			if (Z2 != Z0) t = (Zt - Z0) / (Z2 - Z0);
			double u = _lu + t * (_ru - _lu);
			double v = _lv + t * (_rv - _lv);
			//���������colorЧ��
			if (v < 0.0) v = 0.0;
			else if (v > 1.0) v = 1.0;
			if (u < 0.0) u = 0.0;
			else if (u > 1.0) u = 1.0;
			MRGB textColor = Scene::GetInstance()->mTexture\
				[(int)((height - 1) * v)][(int)((width - 1) * u)];

			int r = rs * ((double)(textColor.r) / 255.0);
			int g = gs * ((double)(textColor.g) / 255.0);
			int b = bs * ((double)(textColor.b) / 255.0);

			mRender->AddPixel(Pixel(x, _y, RGB(r, g, b)));
		}
		else
		{
			double u = _lu;
			double v = _lv;
			//���������colorЧ��
			if (v < 0.0) v = 0.0;
			else if (v > 1.0) v = 1.0;
			if (u < 0.0) u = 0.0;
			else if (u > 1.0) u = 1.0;
			MRGB textColor = Scene::GetInstance()->mTexture\
				[(int)((height - 1) * v)][(int)((width - 1) * u)];

			int r = rs * ((double)(textColor.r) / 255.0);
			int g = gs * ((double)(textColor.g) / 255.0);
			int b = bs * ((double)(textColor.b) / 255.0);

			mRender->AddPixel(Pixel(x, _y, RGB(r, g, b)));
		}
		rs += deltaR;
		gs += deltaG;
		bs += deltaB;
	}
}

//ScanLine����㷨
//void Triangle::ScanLineFill(int _x, int _y, int _oldColor,
//	int _newColor, std::vector<Pixel>& _outPixels)
//{
//	int i;
//	Span span;
//	
//	//��䲢ȷ�����ӵ�(x, y)���ڵ�����
//	i = _x;
//	while (mEdgePoints[i][_y] == _oldColor) //�������
//	{
//		_outPixels.push_back(Pixel(i, _y));
//		mEdgePoints[i][_y] = _newColor;
//		++i;
//	}
//	span.xRight = i - 1; //ȷ�������ұ߽�
//
//	i = _x - 1;
//	while (mEdgePoints[i][_y] == _oldColor) //�������
//	{
//		_outPixels.push_back(Pixel(i, _y));
//		mEdgePoints[i][_y] = _newColor;
//		--i;
//	}
//	span.xLeft = i + 1; //ȷ��������߽�
//	//��ʼ��
//	while (!mSpanStack.empty()) mSpanStack.pop();
//	span.y = _y;
//	mSpanStack.push(span); //��ǰ�����ɵ�����ѹ���ջ
//
//	while (!mSpanStack.empty())
//	{
//		span = mSpanStack.top();
//		mSpanStack.pop();
//		//�ֱ�������ɨ����
//		ScanLine(span.y + 1, span.xLeft, span.xRight, _oldColor, _newColor, _outPixels);
//		ScanLine(span.y - 1, span.xLeft, span.xRight, _oldColor, _newColor, _outPixels);
//	}
//}
//
//void Triangle::ScanLine(int _y, int _xLeft, int _xRight, 
//	int _oldColor, int _newColor, std::vector<Pixel>& _outPixels)
//{
//	int xLeft;
//	int xRight;
//	int i;
//	bool isLeftEndSet;
//	bool spanNeedFill;
//
//	xRight = _xRight;
//	i = _xLeft;
//	isLeftEndSet = false;
//	while (mEdgePoints[i][_y] == _oldColor) //�������
//	{
//		_outPixels.push_back(Pixel(i, _y));
//		mEdgePoints[i][_y] = _newColor;
//		--i;
//	}
//
//	if (i != _xLeft - 1) //ȷ��������߽�
//	{
//		isLeftEndSet = true;
//		xLeft = i + 1;
//	}
//	i = _xLeft;
//	while (i < xRight)
//	{
//		spanNeedFill = false;
//		while (mEdgePoints[i][_y] == _oldColor) //�������
//		{
//			if (!spanNeedFill)
//			{
//				spanNeedFill = true;
//				if (!isLeftEndSet)
//				{
//					isLeftEndSet = true;
//					xLeft = i;
//				}
//			}
//			_outPixels.push_back(Pixel(i, _y));
//			mEdgePoints[i][_y] = _newColor;
//			++i;
//		}
//		if (spanNeedFill)
//		{
//			Span newSpan(_y, xLeft, i - 1);
//			mSpanStack.push(newSpan);
//			isLeftEndSet = false;
//			spanNeedFill = false;
//		}
//		while (mEdgePoints[i][_y] != _oldColor) ++i;
//	}
//}