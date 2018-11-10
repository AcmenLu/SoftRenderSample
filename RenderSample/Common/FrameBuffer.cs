﻿using System;
using System.Drawing;

namespace SampleCommon
{
	enum CLEARFLAG
	{
		DEPTHBUFFER = 1,
		COLORBUFFER = 2,
	}

	public class FrameBuffer
	{
		private Bitmap mColorBuffer;
		private float[,] mDepthBuffer;
		private Graphics mGraphicsframe;
		private Vector2 mSize;
		private bool mDepthTest;

		public bool EnableDepthTest
		{
			get { return mDepthTest; }
			set { mDepthTest = value; }
		}

		public Bitmap ColorBuffer
		{
			get { return mColorBuffer; }
		}

		public FrameBuffer(float width, float height)
		{
			mSize = new Vector2(width, height);
			CreateBuffer();
		}

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
			if (posX < 0 || posX >= mSize.x || posY < 0 || posY >= mSize.y)
				return;
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