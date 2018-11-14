#ifndef _TRIANGLE_H_
#define _TRIANGLE_H_

#include <queue>
#include <stack>
#include <vector>
#include "Render.h"
#include "Vertex.h"

//struct Span
//{
//	int y;
//	int xLeft;
//	int xRight;
//
//	Span();
//	Span(int _y, int _xLeft, int _xRight);
//	~Span();
//	Span(const Span& _other);
//	Span& operator=(const Span& _other);
//};

enum FACE_TYPE 
{
	FACE_LEFT = 0,
	FACE_RIGHT,
	FACE_DOWN,
	FACE_TOP,
	FACE_NEAR,
	FACE_FAR
};

class sTriangle
{
public:
	Vertex ver[3];
	sTriangle() {}
	sTriangle(const Vertex& _ver0,
		const Vertex& _ver1,
		const Vertex& _ver2) 
	{
		ver[0] = _ver0;
		ver[1] = _ver1;
		ver[2] = _ver2;
	}
};

class Triangle
{
public:
	Vertex mPoints[3]; //初始化为世界坐标系中的三个点
	Triangle();
	Triangle(const Triangle& _other);
	Triangle& operator=(const Triangle& _other);
	Triangle(
		const Vertex& _ver0, 
		const Vertex& _ver1, 
		const Vertex& _ver2);
	~Triangle();
	void DDALine(int _xa, int _ya, int _xb, int _yb,
		std::vector<Pixel>& _outPixels);
	//void ScanLineFill(int _x, int _y, int _oldColor,
	//	int _newColor, std::vector<Pixel>& _outPixels);
	//void ScanLine(int _y, int _xLeft, int _xRight, 
	//	int _oldColor, int newColor, std::vector<Pixel>& _outPixels);
	//裁剪算法 一个三角形被一个面裁剪
	void Cut(
		const sTriangle& _tri, 
		FACE_TYPE _face,
		std::vector<sTriangle>& _outTris);

	//裁剪算法 一个三角形被规范化视见体裁剪
	void Cut(
		const sTriangle& _tri, 
		std::vector<sTriangle>& _outTris);

	void CutW(
		const sTriangle& _tri, 
		std::vector<sTriangle>& _outTris);

	void Draw(Matrix4x4& _worldMatrix);
	void Init();
	void Swap(int& _x, int& _y);
	void SwapColor(COLORREF& _x, COLORREF& _y);
	void SwapVertex(Vertex& _x, Vertex& _y);
	void DrawTopTriangle(Vertex& _ver0, Vertex& _ver1, Vertex& _ver2);
	void DrawBottomTriangle(Vertex& _ver0, Vertex& _ver1, Vertex& _ver2);
	void DrawTriangle2D(Vertex& _ver0, Vertex& _ver1, Vertex& _ver2);
	void DrawSimpleLine(double _x0, double _x1, double _y, 
		double _rs, double _gs, double _bs, double _re, double _ge, double _be,
		double _lz, double _lu, double _lv, double _rz, double _ru, double _rv);
	//int **mEdgePoints; //记录投影完成后三角形边缘上的点供ScaneLine使用
	//std::stack<Span> mSpanStack;
	COLORREF CaculateLight(const Vertex& _ver);

	void CalAreaCode(Vertex& _ver);

	void LinePunctureFaces(
		const Vertex& _ver0, 
		const Vertex& _ver1, 
		std::vector<FACE_TYPE>& _outFaces);

	void TriPunctureFaces(
		const sTriangle& _tri,
		std::queue<FACE_TYPE>& _outFaces);

	double mSecondPerTick;
	Render* mRender;
};

#endif