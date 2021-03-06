﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace SampleCommon
{
	/// <summary>
	/// 渲染模式
	/// </summary>
	public enum RenderMode
	{
		/// <summary>
		/// 线框模式
		/// </summary>
		WIREFRAME,
		/// <summary>
		/// 顶点色
		/// </summary>
		VERTEXCOLOR,
		/// <summary>
		/// 单张纹理
		/// </summary>
		TEXTURED,
		/// <summary>
		/// 多张纹理
		/// </summary>
		CUBETEXTURED,
	}

	/// <summary>
	/// 裁剪模式
	/// </summary>
	public enum CullMode
	{
		CULL_NONE = 0,
		CULL_CAMERA = 1,
		CULL_CVV = 2,
		CULL_ALL = 3
	}

	/// <summary>
	/// 渲染器，用来渲染所有需要渲染的物体
	/// </summary>
	public class Renderer
	{
		private List<Mesh> mRenderList;
		private List<Light> mLightList;
		private Color mBgColor;
		private Camera mCamera;
		private Vector2 mSize;
		private FrameBuffer mFrameBuffer;
		private RenderMode mRenderMode;
		private CullMode mCullMode;
		private Matrix4X4 mViewPoreMat;

		/// <summary>
		/// 所有需要被渲染的物体
		/// </summary>
		public List<Mesh> RenderList
		{
			get { return mRenderList; }
		}

		/// <summary>
		/// 场景中的光的列表
		/// </summary>
		public List<Light> LightList
		{
			get { return mLightList; }
		}

		/// <summary>
		/// 用来清空的背景色
		/// </summary>
		public Color BgColor
		{
			get { return mBgColor; }
			set { mBgColor = value; }
		}

		/// <summary>
		/// 渲染器对应的摄像机
		/// </summary>
		public Camera Camera
		{
			get
			{
				if (mCamera == null)
					mCamera = new Camera();
				return mCamera;
			}
			set { mCamera = value; }
		}

		/// <summary>
		/// 渲染窗口的大小
		/// </summary>
		public Vector2 Size
		{
			get { return mSize; }
			set
			{
				mSize = value;
				CreateFrameBuffer();
			}
		}

		/// <summary>
		/// 缓冲用的Buffer
		/// </summary>
		public FrameBuffer FrameBuffer
		{
			get { return mFrameBuffer; }
		}

		/// <summary>
		/// 渲染模式
		/// </summary>
		public RenderMode RenderMode
		{
			get { return mRenderMode; }
			set { mRenderMode = value; }
		}

		/// <summary>
		/// 裁剪模式
		/// </summary>
		public CullMode CullMode
		{
			get { return mCullMode; }
			set { mCullMode = value; }
		}

		/// <summary>
		/// 是否需要开启深度测试
		/// </summary>
		public bool EnableDepthTest
		{
			get { return mFrameBuffer.EnableDepthTest; }
			set { mFrameBuffer.EnableDepthTest = value; }
		}

		/// <summary>
		/// 视口矩阵
		/// </summary>
		public Matrix4X4 ViewPort
		{
			get { return mViewPoreMat; }
		}
		
		/// <summary>
		/// 初始化的大小为指定大小
		/// </summary>
		/// <param name="size"></param>
		public Renderer(int width, int height)
		{
			mRenderList = new List<Mesh>();
			mSize = new Vector2(width, height);
			mBgColor = Color.Black;
			mRenderMode = RenderMode.WIREFRAME;
			mCullMode = CullMode.CULL_ALL;
			mLightList = new List<Light>();
			CreateFrameBuffer();
			InitViewPortMat();
		}

		/// <summary>
		/// 初始化ViewPort矩阵
		/// </summary>
		public void InitViewPortMat()
		{
			if (mViewPoreMat == null)
				mViewPoreMat = new Matrix4X4();
			Matrix4X4.BuildViewPortLH(0, 0, mSize.x, mSize.y, 0, 1, ref mViewPoreMat);
		}

		/// <summary>
		/// 向渲染列表中插入一条数据
		/// </summary>
		/// <param name="obj"></param>
		public void AddRenderObject(Mesh obj)
		{
			if (mRenderList.Exists(x => x == obj) == false)
				mRenderList.Add(obj);
		}

		/// <summary>
		/// 清除渲染对象
		/// </summary>
		public void ClearRenderObjects()
		{
			if (mRenderList.Count() > 0)
				mRenderList.Clear();
		}

		/// <summary>
		/// 删除某个渲染对象
		/// </summary>
		/// <param name="obj"></param>
		public void RemoveRenderObject(Mesh obj)
		{
			if (mRenderList.Count() <= 0)
				return;

			int index = mRenderList.FindIndex(x => x == obj);
			if (index >= 0 && index < mRenderList.Count())
			{
				mRenderList.RemoveAt(index);
			}
		}

		/// <summary>
		/// 向渲染列表中插入一条数据
		/// </summary>
		/// <param name="light"></param>
		public void AddLight(Light light)
		{
			if (mLightList.Exists(x => x == light) == false)
			{
				lock (mLightList)
				{
					mLightList.Add(light);
				}
			}
		}

		/// <summary>
		/// 清除渲染对象
		/// </summary>
		public void ClearLight()
		{
			if (mLightList.Count() > 0)
			{
				lock (mLightList)
				{
					mLightList.Clear();
				}
			}
		}

		/// <summary>
		/// 删除某个光
		/// </summary>
		/// <param name="light"></param>
		public void RemoveLight(Light light)
		{
			if (mLightList.Count() <= 0)
				return;

			int index = mLightList.FindIndex(x => x == light);
			if (index >= 0 && index < mLightList.Count())
			{
				lock (mLightList)
				{
					mLightList.RemoveAt(index);
				}
			}
		}

		/// <summary>
		/// 创建一个新的FramBuffer
		/// </summary>
		private void CreateFrameBuffer()
		{
			mFrameBuffer = new FrameBuffer((int)mSize.x, (int)mSize.y);
		}

		/// <summary>
		/// 渲染在渲染队列中的对象
		/// </summary>
		public void OnRender(Graphics graphics)
		{
			if (mFrameBuffer == null)
				return;

			mFrameBuffer.LockBuffer();
			if (mFrameBuffer != null)
				mFrameBuffer.ClearBuffer();

			foreach (Mesh obj in mRenderList)
			{
				obj.OnRender(this);
			}

			mFrameBuffer.UnLockBuffer();
			graphics.DrawImage(mFrameBuffer.ColorBuffer, 0, 0);
		}
	}
}
