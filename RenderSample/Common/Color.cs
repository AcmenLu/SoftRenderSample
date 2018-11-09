using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Color
	{
		public static Color White = new Color(1.0f, 1.0f, 1.0f);
		public static Color Black = new Color(0.0f, 0.0f, 0.0f);
		public static Color Red = new Color(1.0f, 0.0f, 0.0f);
		public static Color Green = new Color(0.0f, 1.0f, 0.0f);
		public static Color Blue = new Color(0.0f, 0.0f, 1.0f);

		private float mR;
		private float mG;
		private float mB;

		public float r
		{
			get { return mR; }
			set { mR = MathUntil.Range(value, 0.0f, 1.0f); }
		}

		public float g
		{
			get { return mG; }
			set { mG = MathUntil.Range(value, 0.0f, 1.0f); }
		}

		public float b
		{
			get { return mB; }
			set { mB = MathUntil.Range(value, 0.0f, 1.0f); }
		}

		/// <summary>
		/// 无参构造函数，默认设置颜色为（0,0,0）
		/// </summary>
		public Color()
		{
			mR = mG = mB = 0.0f;
		}

		/// <summary>
		/// 构造函数，输入分别为rgb三色
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		public Color(float r, float g, float b)
		{
			mR = MathUntil.Range(r, 0.0f, 1.0f);
			mG = MathUntil.Range(g, 0.0f, 1.0f);
			mB = MathUntil.Range(b, 0.0f, 1.0f);
		}

		/// <summary>
		/// 复制构造函数
		/// </summary>
		/// <param name="color"></param>
		public Color(Color color)
		{
			mR = color.r;
			mG = color.g;
			mB = color.b;
		}

		/// <summary>
		/// 转换为系统的颜色
		/// </summary>
		/// <returns></returns>
		public System.Drawing.Color TransFormToSystemColor()
		{
			float r = mR * 255;
			float g = mG * 255;
			float b = mB * 255;
			return System.Drawing.Color.FromArgb((int)r, (int)g, (int)b);
		}

		public static Color operator *(Color a, Color b)
		{
			Color c = new Color();
			c.r = a.r * b.r;
			c.g = a.g * b.g;
			c.b = a.b * b.b;
			return c;
		}

		/// <summary>
		/// 数字和颜色相乘
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Color operator *(float a, Color b)
		{
			Color c = new Color();
			c.r = a * b.r;
			c.g = a * b.g;
			c.b = a * b.b;
			return c;
		}

		/// <summary>
		/// 颜色和数学相乘
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Color operator *(Color a, float b)
		{
			Color c = new Color();
			c.r = a.r * b;
			c.g = a.g * b;
			c.b = a.b * b;
			return c;
		}

		/// <summary>
		/// 颜色相加
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Color operator +(Color a, Color b)
		{
			Color c = new Color();
			c.r = a.r + b.r;
			c.g = a.g + b.g;
			c.b = a.b + b.b;
			return c;
		}

		/// <summary>
		/// 将系统颜色转换为自定义的颜色。
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color ConvertSystemColor(System.Drawing.Color color)
		{
			float r = color.R / (float)255.0f;
			float g = color.G / (float)255.0f;
			float b = color.B / (float)255.0f;
			return new Color(r, g, b);
		}
	}
}
