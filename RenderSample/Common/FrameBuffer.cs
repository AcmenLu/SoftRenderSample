using System;
using System.Drawing;

namespace SampleCommon
{
	/// <summary>
	/// 颜色缓存和深度缓存清除时的标志
	/// </summary>
	enum CLEARFLAG
	{
		DEPTHBUFFER = 1,
		COLORBUFFER = 2,
	}

	/// <summary>
	/// 缓存类，包括颜色缓存和深度缓存。
	/// </summary>
	public class FrameBuffer
	{
		private Bitmap mColorBuffer;
		private float[,] mDepthBuffer;
		private Graphics mGraphicsframe;
		private Vector2 mSize;
		private bool mDepthTest;

		/// <summary>
		/// 颜色缓存，外部只读
		/// </summary>
		public Bitmap ColorBuffer
		{
			get { return mColorBuffer; }
		}

		/// <summary>
		/// 开启/关闭深度测试
		/// </summary>
		public bool EnableDepthTest
		{
			get { return mDepthTest; }
			set { mDepthTest = value; }
		}

		/// <summary>
		/// 构造一个指定大小的缓存区
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public FrameBuffer(int width, int height)
		{
			mSize = new Vector2(width, height);
			CreateBuffer();
		}

		/// <summary>
		/// 创建一个指定大小的图片作为颜色缓存，创建一个指定大小的数组作为深度缓存。
		/// </summary>
		private void CreateBuffer()
		{
			mColorBuffer = new Bitmap((int)mSize.x, (int)mSize.y);
			mGraphicsframe = Graphics.FromImage(mColorBuffer);
			mDepthBuffer = new float[(int)mSize.x, (int)mSize.y];
		}

		/// <summary>
		/// 设置颜色缓存的某个位置的颜色
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="color"></param>
		public void SetPointColor(int posX, int posY, Color color, float depth = 0.0f)
		{
			if(posX > 799)
			{
				int a = 10;
			}
			posX = posX < 0 ? 0 : posX;
			posX = posX >= mSize.x ? (int)mSize.x - 1 : posX;
			posY = posY < 0 ? 0 : posY;
			posY = posY >= mSize.y ? (int)mSize.y - 1 : posY;
			if (mDepthTest == true)
			{
				float tmpdepth = mDepthBuffer[posX, posY];
				if (depth < tmpdepth)
					return;
			}
			mColorBuffer.SetPixel(posX, posY, color.TransFormToSystemColor());
		}

		/// <summary>
		/// 清除FrameBuffer中的颜色缓存和深度缓存
		/// </summary>
		/// <param name="flag">清除选项，不同位标识不同清除内容</param>
		/// <param name="color">要清除的颜色</param>
		public void ClearBuffer()
		{
			Array.Clear(mDepthBuffer, 0, mDepthBuffer.Length);
			mGraphicsframe.Clear(System.Drawing.Color.Black);
		}
	}
}
