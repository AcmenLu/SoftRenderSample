using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	// rgb 0-255
	public struct Color
	{
		private byte mR;
		private byte mG;
		private byte mB;

		public byte X
		{
			get { return mR; }
			set { mR = value; }
		}

		public byte Y
		{
			get { return mG; }
			set { mG = value; }
		}

		public byte Z
		{
			get { return mB; }
			set { mB = value; }
		}

		public byte R
		{
			get { return mR; }
			set { mR = value; }
		}

		public byte G
		{
			get { return mG; }
			set { mG = value; }
		}

		public byte B
		{
			get { return mB; }
			set { mB = value; }
		}

		/// <summary>
		/// 用rgb构造一个颜色
		/// </summary>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		public Color(byte red, byte green, byte blue): this()
		{
			this.mR = red;
			this.mG = green;
			this.mB = blue;
		}

		/// <summary>
		/// 颜色之间相乘
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static Color operator *(Color c1, Color c2)
		{
			float r = (c1.X / 255f) * (c2.X / 255f);
			float g = (c1.Y / 255f) * (c2.Y / 255f);
			float b = (c1.Z / 255f) * (c2.Z / 255f);
			return new Color((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		/// <summary>
		/// 颜色乘数值
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static Color operator *(Color c1, float t)
		{
			byte r = (byte)Math.Min((c1.X * t), 255);
			byte g = (byte)Math.Min((c1.Y * t), 255);
			byte b = (byte)Math.Min((c1.Z * t), 255);
			return new Color(r, g, b);
		}

		/// <summary>
		/// 颜色相加
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static Color operator +(Color c1, Color c2)
		{
			byte r = (byte)Math.Min(c1.X + c2.X, 255);
			byte g = (byte)Math.Min(c1.Y + c2.Y, 255);
			byte b = (byte)Math.Min(c1.Z + c2.Z, 255);
			return new Color(r, g, b);
		}
	}
}
