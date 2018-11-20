using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	//矢量
	struct Vector4
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public Vector4(float x, float y, float z, float w) : this()
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		public Vector4(Vector4 vector)
		{
			this.X = vector.X;
			this.Y = vector.Y;
			this.Z = vector.Z;
			this.W = vector.W;
		}

		/// <summary>
		/// 矢量的加法
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 operator +(Vector4 a,Vector4 b)
		{
			return new Vector4(a.X+b.X,a.Y+b.Y,a.Z+b.Z,1);
		}

		/// <summary>
		/// 矢量的减法
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 operator -(Vector4 a,Vector4 b)
		{
			return new Vector4(a.X-b.X,a.Y-b.Y,a.Z-b.Z,1);
		}

		/// <summary>
		/// 矢量的值 乘法
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 operator *(Vector4 a, float b)
		{
		   return new Vector4(a.X*b,a.Y*b,a.Z*b,1);
		}

		/// <summary>
		/// 矢量与值得 除法
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 operator /(Vector4 a, float b)
		{
			if (b != 0)
			{
				return new Vector4(a.X / b,a.Y/b,a.Z/b,1);
			}
			return a;
		}

		/// <summary>
		/// 点乘
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static float Dot(Vector4 a, Vector4 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}

		/// <summary>
		/// 叉乘
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 Cross(Vector4 a, Vector4 b)
		{
			float m1, m2, m3;
			m1 = a.Y * b.Z - a.Z * b.Y;
			m2 = a.Z * b.X - a.X * b.Z;
			m3 = a.X * b.Y - a.Y * b.X;
			return new Vector4(m1, m2, m3, 1f);
		}

		/// <summary>
		/// 归一化
		/// </summary>
		public void Normalize()
		{
			float length = (float)Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
			if (length != 0.0f)
			{
				float inv = 1.0f / length;
				this.X *= inv;
				this.Y *= inv;
				this.Z *= inv;
			}
		}
	}
}
