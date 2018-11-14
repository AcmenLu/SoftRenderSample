// WindowsGraphics.cpp : 定义应用程序的入口点。
// Title:  图形学自学任务
// Author: 罗浩
// Date:   2017年9月

#include "stdafx.h" //必须首先include
#include "Camera.h"
#include "CommonDef.h"
#include "Render.h"
#include "Scene.h"
#include "Vector3.h"
#include "Vector4.h"
#include "WindowsGraphics.h"

#include <ctime>
#include <iostream>

#define MAX_LOADSTRING 100

// 全局变量:
HINSTANCE hInst;								// 当前实例
TCHAR szTitle[MAX_LOADSTRING];					// 标题栏文本
TCHAR szWindowClass[MAX_LOADSTRING];			// 主窗口类名

// 此代码模块中包含的函数的前向声明:
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

void Game_Main()
{
	Scene* pScene = Scene::GetInstance();
	pScene->Draw();
}

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

 	// TODO: 在此放置代码。

	MSG msg;
	HACCEL hAccelTable;

	// 初始化全局字符串
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_WINDOWSGRAPHICS, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	// 执行应用程序初始化:
	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_WINDOWSGRAPHICS));

	int nPassedFrame = 0;
	time_t lastTime = time(NULL);
	// 主消息循环——构建时实非等待游戏循环
	while (TRUE)
	{
		// 检测队列中是否有消息，如果有，读取它
		if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			// 检测是否是退出消息
			if (msg.message == WM_QUIT)
				break;
			// 转换加速键
			TranslateMessage(&msg);

			// 将消息发给 window proc
			DispatchMessage(&msg);
		} // end if

		// 主游戏逻辑
		Game_Main();
		++nPassedFrame;

		time_t deltaTime = time(NULL) - lastTime;
		if (deltaTime > 0)
		{
			//gdi show frame rate	
			lastTime = time(NULL);
			Render::GetInstance()->SetFps(nPassedFrame);
			nPassedFrame = 0;
		}

	} // end while

	return (int) msg.wParam;
}

void Init(HWND hWnd)
{
	Scene* pScene = Scene::GetInstance();
	Render* pRender = Render::GetInstance();

	pScene->SetHandle(hWnd);
	pRender->SetHandle(hWnd);
	Vector4 camPos(0, 0, -2, 1);
	Vector4 right(1, 0, 0, 1);
	Vector4 up(0, 1, 0, 0);
	Camera camera(
		camPos, //摄像机位置
		up, //相机向上方向
		right, //相机向右方向
		60, //俯仰角，角度
		1, //宽高比
		1, //近裁
		5); //远裁
	pScene->SetCamera(camera); //设置相机参数

	ViewPortParam viewPortParam(
		0.0, //视口起点x
		0.0, //视口起点y
		SCREEN_WIDTH, //视口宽度
		SCREEN_HEIGHT, //视口高度
		0.0, //场景最小深度
		1.0); //场景最大深度

	pScene->SetViewPort(viewPortParam); //设置视口参数

	//设置环境光
	pScene->SetAmbt(AmbtLight(254, 67, 101));
	//平行光方向朝向z轴正方向
	pScene->SetParal(ParalLight(255, 255, 255, Vector4(0, 0, 1, 0))); 

	//在此处添加三角形
	Triangle triangle1( //三角形在模型坐标系中
		Vertex(Vector4(1, 0, 0, 1), //位置
			   Vector4(0, 0, -1, 0), //法向
			   Material(0.2, 0.4, 0.6, 2), //材质
			   TextCoord(0, 1)), //纹理坐标
		Vertex(Vector4(0, 1, 0, 1),
			   Vector4(0, 0, -1, 0),
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0)), //纹理坐标
		Vertex(Vector4(0, 0, 0, 1),
			   Vector4(0, 0, -1, 0), 
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0, 0))); //纹理坐标

	Triangle triangle2( //三角形在模型坐标系中
		Vertex(Vector4(0, 1, 0, 1), //位置
			   Vector4(1, 1, 1, 0).Normalize(), //法向
			   Material(0.2, 0.4, 0.6, 2), //材质
			   TextCoord(0, 0)), //纹理坐标
		Vertex(Vector4(1, 0, 0, 1),
		       Vector4(1, 1, 1, 0).Normalize(), //法向
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0.5, 1.7320508 / 2)), //纹理坐标
		Vertex(Vector4(0, 0, 1, 1),
			   Vector4(1, 1, 1, 0).Normalize(), //法向
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0))); //纹理坐标


	Triangle triangle3( //三角形在模型坐标系中
		Vertex(Vector4(0, 0, 1, 1), //位置
			   Vector4(-1, 0, 0, 0), //法向
			   Material(0.2, 0.4, 0.6, 2), //材质
			   TextCoord(0, 1)), //纹理坐标
		Vertex(Vector4(0, 0, 0, 1),
			   Vector4(-1, 0, 0, 0), //法向
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0, 0)), //纹理坐标
		Vertex(Vector4(0, 1, 0, 1),
			   Vector4(-1, 0, 0, 0), //法向 
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0))); //纹理坐标


	Triangle triangle4( //三角形在模型坐标系中
		Vertex(Vector4(1, 0, 0, 1), //位置
			   Vector4(0, -1, 0, 0), //法向
			   Material(0.2, 0.4, 0.6, 2), //材质
			   TextCoord(0, 1)), //纹理坐标
		Vertex(Vector4(0, 0, 0, 1),
			   Vector4(0, -1, 0, 0), //法向
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0, 0)), //纹理坐标
		Vertex(Vector4(0, 0, 1, 1),
			   Vector4(0, -1, 0, 0), //法向 
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0))); //纹理坐标

	Model model;
	//将三角面片添加到模型
	model.AddTriangle(triangle1);
	model.AddTriangle(triangle2);
	model.AddTriangle(triangle3);
	model.AddTriangle(triangle4);

	//设置模型的world transform
	Vector4 ModlePos(1, 0, 0, 1);
	Vector4 scale(1, 1, 1, 1);
	double xAngle = 0.0f;
	double yAngle = 90.0f;
	double zAngle = 0.0f;
	Matrix4x4 worldMatrix = Matrix4x4::BuildWorldTransform(
		ModlePos, //模型位置
		scale, //缩放 
		xAngle, //绕x轴旋转角度
		yAngle, //绕y轴旋转角度
		zAngle); //绕z轴旋转角度
	model.SetWorldMatrix(worldMatrix);

	pScene->AddModel(model); //添加模型
	pScene->ReadTexture();
}

//
//  函数: MyRegisterClass()
//
//  目的: 注册窗口类。
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_WINDOWSGRAPHICS));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
	wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_WINDOWSGRAPHICS);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassEx(&wcex);
}

//
//   函数: InitInstance(HINSTANCE, int)
//
//   目的: 保存实例句柄并创建主窗口
//
//   注释:
//
//        在此函数中，我们在全局变量中保存实例句柄并
//        创建和显示主程序窗口。
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance; // 将实例句柄存储在全局变量中

   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
	   CW_USEDEFAULT, 0, SCREEN_WIDTH, SCREEN_HEIGHT, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   //修改窗口背景颜色为黑色
   HBRUSH brush;
   brush = CreateSolidBrush(RGB(0, 0, 0));
   SetClassLong(hWnd, GCL_HBRBACKGROUND, (long)brush);

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);
   //添加控制台窗口
   //AllocConsole();
   //freopen("conout$", "w", stdout);
   //std::cout << "Hello World!" << std::endl;

   //初始化游戏资源
   Init(hWnd);

   return TRUE;
}

//
//  函数: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  目的: 处理主窗口的消息。
//
//  WM_COMMAND	- 处理应用程序菜单
//  WM_PAINT	- 绘制主窗口
//  WM_DESTROY	- 发送退出消息并返回
//
//

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	int wmId, wmEvent;

	switch (message)
	{
	case WM_COMMAND:
		wmId    = LOWORD(wParam);
		wmEvent = HIWORD(wParam);
		// 分析菜单选择:
		switch (wmId)
		{
		case IDM_ABOUT:
			DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
			break;
		case IDM_EXIT:
			DestroyWindow(hWnd);
			break;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
		break;
	case WM_PAINT:
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	case WM_KEYDOWN:
		switch (wParam)
		{
		case 'A':
			Scene::GetInstance()->GetCamera().Strafe(0.1);
			break;
		case 'D':
			Scene::GetInstance()->GetCamera().Strafe(-0.1);
			break;
		case 'W':
			Scene::GetInstance()->GetCamera().Fly(-0.1);
			break;
		case 'S':
			Scene::GetInstance()->GetCamera().Fly(0.1);
			break;
		case 'J':
			Scene::GetInstance()->GetCamera().Walk(0.1);
			break;
		case 'K':
			Scene::GetInstance()->GetCamera().Walk(-0.1);
			break;
		case VK_LEFT:
			Scene::GetInstance()->GetCamera().Yaw(-0.4);
			break;
		case VK_RIGHT:
			Scene::GetInstance()->GetCamera().Yaw(0.4);
			break;
		case VK_UP:
			Scene::GetInstance()->GetCamera().Pitch(-0.4);
			break;
		case VK_DOWN:
			Scene::GetInstance()->GetCamera().Pitch(0.4);
			break;
		}
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

// “关于”框的消息处理程序。
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, LOWORD(wParam));
			return (INT_PTR)TRUE;
		}
		break;
	}
	return (INT_PTR)FALSE;
}
