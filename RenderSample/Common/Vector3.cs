using System;


namespace SampleCommon
{
	public class Vector3
	{
		private float mX;
		private float mY;
		private float mZ;

		public float x
		{
			get { return mX; }
			set { mX = value; }
		}

		public float y
		{
			get { return mY; }
			set { mY = value; }
		}

		public float z
		{
			get { return mZ; }
			set { mZ = value; }
		}

		/// <summary>
		/// 求向量的长度
		/// </summary>
		/// <returns></returns>
		public float Length
		{
			get
			{
				float sq = mX * mX + mY * mY + mZ * mZ;
				return (float)System.Math.Sqrt(sq);
			}
		}

		/// <summary>
		/// 三维向量的无参构造，默认值为0
		/// </summary>
		public Vector3()
		{
			mX = mY = mZ = 0;
		}

		/// <summary>
		/// 带位置信息的构造函数
		/// </summary>
		/// <param name="fX"></param>
		/// <param name="fY"></param>
		/// <param name="fZ"></param>
		public Vector3(float fX, float fY, float fZ)
		{
			mX = fX;
			mY = fY;
			mZ = fZ;
		}

		/// <summary>
		/// 复制构造
		/// </summary>
		/// <param name="vec3"></param>
		public Vector3(Vector3 vec3)
		{
			mX = vec3.x;
			mY = vec3.y;
			mZ = vec3.z;
		}

		/// <summary>
		/// 求向量的模
		/// </summary>
		/// <returns></returns>
		public Vector3 Normalize()
		{
			float length = Length;
			if (length > 0)
			{
				float s = 1 / length;
				mX *= s;
				mY *= s;
				mZ *= s;
			}
			return this;
		}

		/// <summary>
		/// 两个向量相加
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		/// <summary>
		/// 两个向量相减
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		/// <summary>
		/// 向量和一个数字相乘
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="fValue"></param>
		/// <returns></returns>
		public static Vector3 operator *(Vector3 lhs, float fValue)
		{
			return new Vector3(lhs.x * fValue, lhs.y * fValue, lhs.z * fValue);
		}

		/// <summary>
		/// 向量和矩阵相乘
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3 operator *(Vector3 lhs, Matrix4X4 rhs)
		{
			Vector3 v = new Vector3();
			v.x = lhs.x * rhs[0, 0] + lhs.y * rhs[1, 0] + lhs.z * rhs[2, 0] + rhs[3, 0];
			v.y = lhs.x * rhs[0, 1] + lhs.y * rhs[1, 1] + lhs.z * rhs[2, 1] + rhs[3, 1];
			v.z = lhs.x * rhs[0, 2] + lhs.y * rhs[1, 2] + lhs.z * rhs[2, 2] + rhs[3, 2];
			return v;
		}

		/// <summary>
		/// 向量和一个数字相除
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="fValue"></param>
		/// <returns></returns>
		public static Vector3 operator /(Vector3 lhs, float fValue)
		{
			return new Vector3(lhs.x / fValue, lhs.y / fValue, lhs.z / fValue);
		}

		/// <summary>
		/// 两个向量的点积
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static float Dot(Vector3 lhs, Vector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		/// <summary>
		/// 两个向量的叉乘
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
		{
			float x = lhs.y * rhs.z - lhs.z * rhs.y;
			float y = lhs.z * rhs.x - lhs.x * rhs.z;
			float z = lhs.x * rhs.y - lhs.y * rhs.x;
			return new Vector3(x, y, z);
		}
	}
}
