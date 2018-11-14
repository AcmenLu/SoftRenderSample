// WindowsGraphics.cpp : ����Ӧ�ó������ڵ㡣
// Title:  ͼ��ѧ��ѧ����
// Author: �޺�
// Date:   2017��9��

#include "stdafx.h" //��������include
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

// ȫ�ֱ���:
HINSTANCE hInst;								// ��ǰʵ��
TCHAR szTitle[MAX_LOADSTRING];					// �������ı�
TCHAR szWindowClass[MAX_LOADSTRING];			// ����������

// �˴���ģ���а����ĺ�����ǰ������:
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

 	// TODO: �ڴ˷��ô��롣

	MSG msg;
	HACCEL hAccelTable;

	// ��ʼ��ȫ���ַ���
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_WINDOWSGRAPHICS, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	// ִ��Ӧ�ó����ʼ��:
	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_WINDOWSGRAPHICS));

	int nPassedFrame = 0;
	time_t lastTime = time(NULL);
	// ����Ϣѭ����������ʱʵ�ǵȴ���Ϸѭ��
	while (TRUE)
	{
		// ���������Ƿ�����Ϣ������У���ȡ��
		if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			// ����Ƿ����˳���Ϣ
			if (msg.message == WM_QUIT)
				break;
			// ת�����ټ�
			TranslateMessage(&msg);

			// ����Ϣ���� window proc
			DispatchMessage(&msg);
		} // end if

		// ����Ϸ�߼�
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
		camPos, //�����λ��
		up, //������Ϸ���
		right, //������ҷ���
		60, //�����ǣ��Ƕ�
		1, //��߱�
		1, //����
		5); //Զ��
	pScene->SetCamera(camera); //�����������

	ViewPortParam viewPortParam(
		0.0, //�ӿ����x
		0.0, //�ӿ����y
		SCREEN_WIDTH, //�ӿڿ��
		SCREEN_HEIGHT, //�ӿڸ߶�
		0.0, //������С���
		1.0); //����������

	pScene->SetViewPort(viewPortParam); //�����ӿڲ���

	//���û�����
	pScene->SetAmbt(AmbtLight(254, 67, 101));
	//ƽ�йⷽ����z��������
	pScene->SetParal(ParalLight(255, 255, 255, Vector4(0, 0, 1, 0))); 

	//�ڴ˴����������
	Triangle triangle1( //��������ģ������ϵ��
		Vertex(Vector4(1, 0, 0, 1), //λ��
			   Vector4(0, 0, -1, 0), //����
			   Material(0.2, 0.4, 0.6, 2), //����
			   TextCoord(0, 1)), //��������
		Vertex(Vector4(0, 1, 0, 1),
			   Vector4(0, 0, -1, 0),
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0)), //��������
		Vertex(Vector4(0, 0, 0, 1),
			   Vector4(0, 0, -1, 0), 
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0, 0))); //��������

	Triangle triangle2( //��������ģ������ϵ��
		Vertex(Vector4(0, 1, 0, 1), //λ��
			   Vector4(1, 1, 1, 0).Normalize(), //����
			   Material(0.2, 0.4, 0.6, 2), //����
			   TextCoord(0, 0)), //��������
		Vertex(Vector4(1, 0, 0, 1),
		       Vector4(1, 1, 1, 0).Normalize(), //����
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0.5, 1.7320508 / 2)), //��������
		Vertex(Vector4(0, 0, 1, 1),
			   Vector4(1, 1, 1, 0).Normalize(), //����
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0))); //��������


	Triangle triangle3( //��������ģ������ϵ��
		Vertex(Vector4(0, 0, 1, 1), //λ��
			   Vector4(-1, 0, 0, 0), //����
			   Material(0.2, 0.4, 0.6, 2), //����
			   TextCoord(0, 1)), //��������
		Vertex(Vector4(0, 0, 0, 1),
			   Vector4(-1, 0, 0, 0), //����
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0, 0)), //��������
		Vertex(Vector4(0, 1, 0, 1),
			   Vector4(-1, 0, 0, 0), //���� 
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0))); //��������


	Triangle triangle4( //��������ģ������ϵ��
		Vertex(Vector4(1, 0, 0, 1), //λ��
			   Vector4(0, -1, 0, 0), //����
			   Material(0.2, 0.4, 0.6, 2), //����
			   TextCoord(0, 1)), //��������
		Vertex(Vector4(0, 0, 0, 1),
			   Vector4(0, -1, 0, 0), //����
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(0, 0)), //��������
		Vertex(Vector4(0, 0, 1, 1),
			   Vector4(0, -1, 0, 0), //���� 
			   Material(0.2, 0.4, 0.6, 2),
			   TextCoord(1, 0))); //��������

	Model model;
	//��������Ƭ��ӵ�ģ��
	model.AddTriangle(triangle1);
	model.AddTriangle(triangle2);
	model.AddTriangle(triangle3);
	model.AddTriangle(triangle4);

	//����ģ�͵�world transform
	Vector4 ModlePos(1, 0, 0, 1);
	Vector4 scale(1, 1, 1, 1);
	double xAngle = 0.0f;
	double yAngle = 90.0f;
	double zAngle = 0.0f;
	Matrix4x4 worldMatrix = Matrix4x4::BuildWorldTransform(
		ModlePos, //ģ��λ��
		scale, //���� 
		xAngle, //��x����ת�Ƕ�
		yAngle, //��y����ת�Ƕ�
		zAngle); //��z����ת�Ƕ�
	model.SetWorldMatrix(worldMatrix);

	pScene->AddModel(model); //���ģ��
	pScene->ReadTexture();
}

//
//  ����: MyRegisterClass()
//
//  Ŀ��: ע�ᴰ���ࡣ
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
//   ����: InitInstance(HINSTANCE, int)
//
//   Ŀ��: ����ʵ�����������������
//
//   ע��:
//
//        �ڴ˺����У�������ȫ�ֱ����б���ʵ�������
//        ��������ʾ�����򴰿ڡ�
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance; // ��ʵ������洢��ȫ�ֱ�����

   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
	   CW_USEDEFAULT, 0, SCREEN_WIDTH, SCREEN_HEIGHT, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   //�޸Ĵ��ڱ�����ɫΪ��ɫ
   HBRUSH brush;
   brush = CreateSolidBrush(RGB(0, 0, 0));
   SetClassLong(hWnd, GCL_HBRBACKGROUND, (long)brush);

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);
   //��ӿ���̨����
   //AllocConsole();
   //freopen("conout$", "w", stdout);
   //std::cout << "Hello World!" << std::endl;

   //��ʼ����Ϸ��Դ
   Init(hWnd);

   return TRUE;
}

//
//  ����: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  Ŀ��: ���������ڵ���Ϣ��
//
//  WM_COMMAND	- ����Ӧ�ó���˵�
//  WM_PAINT	- ����������
//  WM_DESTROY	- �����˳���Ϣ������
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
		// �����˵�ѡ��:
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

// �����ڡ������Ϣ�������
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
