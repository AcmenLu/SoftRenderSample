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

//���� ÿ֡����Draw
//���Ļ���һ��˫������
class Render
{
public:
	void Clear();
	void Draw();
	//todo: ����ȡ���ɫ���� z���弼��,x y����-������
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
	HWND			   mHandle; //�����ھ��
	static Render*     mInstance;
	HDC				   mHdc;
	BYTE*			   mDataDraw; //��¼��Ļÿ�����ص������ R��G��B����
	BYTE*			   mDataCompute; 
	int				   mLen; //mData�ĳ��ȣ���BYTE��
	int				   mFps; //֡��
};

#endif