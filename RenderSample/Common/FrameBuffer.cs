using System;
using System.Drawing;
using System.Drawing.Imaging;

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

		public BitmapData mBmData;
		private Rectangle mRect;

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


		public PixelFormat PixelFormat
		{
			get { return PixelFormat.Format24bppRgb; }
		}
		/// <summary>
		/// 构造一个指定大小的缓存区
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public FrameBuffer(int width, int height)
		{
			mSize = new Vector2(width, height);
			mColorBuffer = new Bitmap((int)mSize.x, (int)mSize.y, PixelFormat.Format24bppRgb);
			mGraphicsframe = Graphics.FromImage(mColorBuffer);
			mDepthBuffer = new float[(int)mSize.x, (int)mSize.y];
			mRect = new Rectangle(0, 0, (int)mSize.x, (int)mSize.y);
		}

		/// <summary>
		/// 根据深度值判断是否需要
		/// </summary>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		public bool TestDepth(int posX, int posY, float depth)
		{
			posX = posX < 0 ? 0 : posX;
			posX = posX >= mSize.x ? (int)mSize.x - 1 : posX;
			posY = posY < 0 ? 0 : posY;
			posY = posY >= mSize.y ? (int)mSize.y - 1 : posY;
			float tmpdepth = mDepthBuffer[posX, posY];
			return tmpdepth < depth;
		}


		public void Putpixel(int x, int y, Color color, float depth)
		{
			if (x < 0 || x >= mSize.x || y < 0 || y >= mSize.y)
				return;

			unsafe
			{
				byte* ptr = (byte*)(this.mBmData.Scan0);
				byte* row = ptr + (y * this.mBmData.Stride);
				row[x * 3] = (byte)(color.r * 255);
				row[x * 3 + 1] = (byte)(color.g * 255);
				row[x * 3 + 2] = (byte)(color.b * 255);
			}
		}

		public void LockBuffer()
		{
			mBmData = mColorBuffer.LockBits(mRect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
		}

		public void UnLockBuffer()
		{
			if (mBmData != null)
				mColorBuffer.UnlockBits(mBmData);
		}

		public void ClearBuffer()
		{
			Array.Clear(mDepthBuffer, 0, mDepthBuffer.Length);
			if (mBmData == null)
				return;

			unsafe
			{
				byte* ptr = (byte*)(mBmData.Scan0);
				for (int i = 0; i < mBmData.Height; i++)
				{
					for (int j = 0; j < mBmData.Width; j++)
					{

						*ptr = 0;
						*(ptr + 1) = 0;
						*(ptr + 2) = 0;
						ptr += 3;
					}
					ptr += mBmData.Stride - mBmData.Width * 3;
				}
			}
		}
	}
}
