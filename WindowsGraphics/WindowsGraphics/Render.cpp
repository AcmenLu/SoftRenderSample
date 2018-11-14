#include <afxwin.h>
#include <stdio.h>
#include "CommonDef.h"
#include "Render.h"
Render* Render::mInstance = NULL;

void Render::Clear()
{
	mPixels.clear();
}

void Render::SetFps(int _nFps)
{
	mFps = _nFps;
}

void Render::Draw()
{
	if (mHandle == NULL) return ;

	//SetDIBits to improve SetPixel
	CDC *pDC = CDC::FromHandle(mHdc);
	CDC memDC;
	memDC.CreateCompatibleDC(pDC);

	CBitmap bmp;
	bmp.CreateCompatibleBitmap(pDC, SCREEN_WIDTH, SCREEN_HEIGHT);
	memDC.SelectObject(&bmp);
	//todo: 可以把一副图存bmp中以节省一半的空间
	BITMAPINFO bmpInfo;
	bmpInfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
	bmpInfo.bmiHeader.biWidth = SCREEN_WIDTH;
	bmpInfo.bmiHeader.biHeight = SCREEN_HEIGHT;
	bmpInfo.bmiHeader.biPlanes = 1;
	bmpInfo.bmiHeader.biBitCount = 24;
	bmpInfo.bmiHeader.biCompression = BI_RGB;
	bmpInfo.bmiHeader.biSizeImage = 0;
	bmpInfo.bmiHeader.biXPelsPerMeter = 3000;
	bmpInfo.bmiHeader.biYPelsPerMeter = 3000;
	bmpInfo.bmiHeader.biClrUsed = 0;
	bmpInfo.bmiHeader.biClrImportant = 0;

	//绘制 实现双缓冲
	BYTE red;
	BYTE green;
	BYTE blue;
	memset(mDataCompute, 0, mLen);
	//画背景
	for (int i = 0; i < mLen; i += 3)
	{
		mDataCompute[i + 2] = 118;
		mDataCompute[i] = 77;
		mDataCompute[i + 1] = 57;
	}
	for (auto iter = mPixels.begin();
		 iter != mPixels.end(); ++iter)
	{
		//iter是第n个像素点 行优先 todo: 这里为什么要-y
		int n = (SCREEN_HEIGHT - iter->y) * SCREEN_WIDTH + iter->x; 
		int beginPos = n * 3; //mData中颜色开始的位置
		COLORREF color = iter->color; //当前点的颜色
		BYTE* pByte = (BYTE*)&color;
		//第一个字节为0 其它三个字节分别表示蓝色、绿色和红色
		red = *(pByte + 2);
		blue = *(pByte + 0);
		green = *(pByte + 1);
		//bmp图像中是R、G、B排列
		mDataCompute[beginPos] = red;
		mDataCompute[beginPos + 1] = green;
		mDataCompute[beginPos + 2] = blue;
	}
	SetDIBits(pDC->m_hDC, bmp, 0, SCREEN_HEIGHT,
		mDataDraw, &bmpInfo, DIB_RGB_COLORS);
	pDC->BitBlt(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, 
		&memDC, 0, 0, SRCCOPY);
	mDataDraw = mDataCompute;

	// render fps
	COLORREF color = RGB(255, 255, 255);
	SetTextColor(mHdc, color);
	// int fps to LPCWSTR
	wchar_t buffer[16];
	wsprintf(buffer, L"FPS: %d", mFps);
	SetBkMode(mHdc, TRANSPARENT);
	TextOut(mHdc, 100, 100, buffer, wcslen(buffer));    
}

void Render::AddPixel(Pixel& _pix)
{
	if (_pix.x > SCREEN_WIDTH ||
		_pix.x < 0 ||
		_pix.y > SCREEN_HEIGHT ||
		_pix.y < 0) return ;
	mPixels.push_back(_pix);
}

void Render::SetHandle(HWND _handle)
{
	mHandle = _handle;
	mHdc = GetWindowDC(mHandle);
}

Render* Render::GetInstance()
{
	if (mInstance == NULL)
	{
		mInstance = new Render();
	}
	return mInstance;
}

Render::Render() : mFps(0)
{
	//一个像素占3个字节
	mLen = (SCREEN_WIDTH + 1) * (SCREEN_HEIGHT + 1) * 3;
	mDataDraw = new BYTE[mLen];
	mDataCompute = new BYTE[mLen];
	memset(mDataDraw, 0, mLen);
	memset(mDataCompute, 0, mLen);
}

Render::~Render()
{
	if (mDataDraw != NULL)
	{
		delete [] mDataDraw;
	}
	if (mDataCompute != NULL)
	{
		delete [] mDataCompute;
	}
}