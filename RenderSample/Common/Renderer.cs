using System;
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
		Wireframe,
		/// <summary>
		/// 纹理
		/// </summary>
		Textured,
		/// <summary>
		/// 顶点色
		/// </summary>
		VertexColor
	}

	public class Renderer
	{
		private List<Mesh> mRenderList;
		private List<Light> mLightList;
		private Color mBgColor;
		private Camera mCamera;
		private Vector2 mSize;
		private Matrix4X4 mProjection;
		private float mFov;
		private float mNear;
		private float mFar;

		private Graphics mGraphics;
		private FrameBuffer mFrameBuffer;
		private RenderMode mRenderMode;

		public List<Mesh> RenderList
		{
			get { return mRenderList; }
		}

		public List<Light> LightList
		{
			get { return mLightList; }
		}

		public Color BgColor
		{
			get { return mBgColor; }
			set { mBgColor = value; }
		}

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

		public Vector2 Size
		{
			get { return mSize; }
			set
			{
				mSize = value;
				CreateFrameBuffer();
				ResetProjection();
			}
		}

		public Matrix4X4 Projection
		{
			get { return mProjection; }
		}

		public float Fov
		{
			get { return mFov; }
			set
			{
				mFov = value;
				ResetProjection();
			}
		}

		public float Near
		{
			get { return mNear; }
			set
			{
				mNear = value;
				ResetProjection();
			}
		}

		public float Far
		{
			get { return mFar; }
			set
			{
				mFar = value;
				ResetProjection();
			}
		}

		public FrameBuffer FrameBuffer
		{
			get { return mFrameBuffer; }
		}

		public RenderMode RenderMode
		{
			get { return mRenderMode; }
			set { mRenderMode = value; }
		}
		/// <summary>
		/// 初始化的大小为指定大小
		/// </summary>
		/// <param name="size"></param>
		public Renderer(int width, int height, float fov, float near, float far)
		{
			mRenderList = new List<Mesh>();
			mSize = new Vector2(width, height);
			mBgColor = Color.Black;
			mFov = fov;
			mNear = near;
			mFar = far;
			mRenderMode = RenderMode.Wireframe;
			mLightList = new List<Light>();
			CreateFrameBuffer();
			ResetProjection();
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
				mLightList.Add(light);
		}

		/// <summary>
		/// 清除渲染对象
		/// </summary>
		public void ClearLight()
		{
			if (mLightList.Count() > 0)
				mLightList.Clear();
		}

		/// <summary>
		/// 删除某个渲染对象
		/// </summary>
		/// <param name="light"></param>
		public void RemoveLight(Light light)
		{
			if (mLightList.Count() <= 0)
				return;

			int index = mLightList.FindIndex(x => x == light);
			if (index >= 0 && index < mLightList.Count())
			{
				mLightList.RemoveAt(index);
			}
		}

		/// <summary>
		/// 设置透视矩阵
		/// </summary>
		private void ResetProjection()
		{
			float aspect = mSize.x / mSize.y;
			mProjection = Matrix4X4.Projection(mFov, aspect, mNear, mFar);
		}

		private void CreateFrameBuffer()
		{
			mFrameBuffer = new FrameBuffer(mSize.x, mSize.y);
		}

		public void BindGraphics(Graphics graphics)
		{
			mGraphics = graphics;
		}

		/// <summary>
		/// 渲染在渲染队列中的对象
		/// </summary>
		public void OnRender()
		{
			if (mGraphics == null || mFrameBuffer == null)
				return;

			lock (mFrameBuffer.ColorBuffer)
			{

				if (mFrameBuffer != null)
					mFrameBuffer.ClearBuffer();

				foreach (Mesh obj in mRenderList)
				{
					obj.OnRender(this);
				}

				if (mGraphics != null)
					mGraphics.Clear(System.Drawing.Color.Black);

				mGraphics.DrawImage(mFrameBuffer.ColorBuffer, 0, 0);
			}
		}
	}
}
