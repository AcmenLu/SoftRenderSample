using System;
using System.Drawing;
using System.IO;

namespace SampleCommon
{
	public class RenderTexture
	{
		private Bitmap mTexture;
		private int mWidth;
		private int mHeight;

		public Bitmap Texture
		{
			get { return mTexture; }
		}


		/// <summary>
		/// 构造函数，从本地文件中读取一个文件
		/// </summary>
		/// <param name="filePath"></param>
		public RenderTexture(string filePath)
		{
			try
			{
				Image img = Image.FromFile(filePath);
				mWidth = img.Width;
				mHeight = img.Height;
				mTexture = new Bitmap(img, mWidth, mHeight);
			}
			catch
			{
				mWidth = 256;
				mHeight = 256;
				mTexture = new Bitmap(mWidth, mHeight);
				FillTextureWithRed();
			}
		}

		/// <summary>
		/// 设置用红色填充一张图片
		/// </summary>
		private void FillTextureWithRed()
		{
			for (int i = 0; i < mWidth; i ++)
			{
				for(int j = 0; j < mHeight; j ++)
				{
					mTexture.SetPixel(i, j, System.Drawing.Color.Red);
				}
			}
		}

		/// <summary>
		/// 获取图片某个位置的颜色。
		/// </summary>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		/// <returns></returns>
		public Color GetPixelColor(int posX, int posY)
		{
			posX = posX > 0 ? posX : 0;
			posX = posX >= mWidth ? mWidth - 1 : posX;

			posY = posY > 0 ? posY : 0;
			posY = posY >= mHeight ? mHeight  - 1: posY;
			System.Drawing.Color color = mTexture.GetPixel(posX, posY);
			return Color.ConvertSystemColor(color);
		}

		/// <summary>
		/// 用0···1之间的值获取到一个位置的颜色
		/// </summary>
		/// <param name="factorx"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		public Color GetPixelColor(float factorx, float factory)
		{
			int posX = (int)Math.Round(factorx * mWidth, 0);
			int posY = (int)Math.Round(factory * mHeight, 0);
			return GetPixelColor(posX, posY);
		}
	}
}
