#ifndef _RENDER_H_
#define _RENDER_H_

#include <vector>
#include "stdafx.h"

struct Pixel
{
	int x;
	int y;
	COLORREF color;
	Pixel(int _x, int _y) :
		x(_x), y(_y) {};
	Pixel(int _x, int _y, COLORREF _color) :
		x(_x), y(_y), color(_color) {};
};

//单例 每帧调用Draw
//闪的话做一个双缓冲区
class Render
{
public:
	void Clear();
	void Draw();
	//todo: 加深度、颜色属性 z缓冲技术,x y索引-开数组
	void AddPixel(Pixel& _pix);
	static Render* GetInstance();
	void SetHandle(HWND _handle);
	void SetFps(int _nFps);

private:
	Render();
	~Render();
	Render(const Render&);
	Render& operator=(const Render&);

private:
	std::vector<Pixel> mPixels;
	HWND			   mHandle; //主窗口句柄
	static Render*     mInstance;
	HDC				   mHdc;
	BYTE*			   mDataDraw; //记录屏幕每个像素点的数据 R、G、B排列
	BYTE*			   mDataCompute; 
	int				   mLen; //mData的长度，按BYTE计
	int				   mFps; //帧率
};

#endif