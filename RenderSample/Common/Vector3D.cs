using System;


namespace SampleCommon
{
	public class Vector3D
	{
		private float mX;
		private float mY;
		private float mZ;
		private float mW;

		/// <summary>
		/// x坐标的值
		/// </summary>
		public float x
		{
			get { return mX; }
			set { mX = value; }
		}

		/// <summary>
		/// y坐标的值
		/// </summary>
		public float y
		{
			get { return mY; }
			set { mY = value; }
		}

		/// <summary>
		/// z坐标的值
		/// </summary>
		public float z
		{
			get { return mZ; }
			set { mZ = value; }
		}

		/// <summary>
		/// 齐次坐标的w值
		/// </summary>
		public float w
		{
			get { return mW; }
			set { mW = value; }
		}

		/// <summary>
		/// 求向量的长度
		/// </summary>
		/// <returns></returns>
		public float Length
		{
			get { return (float)Math.Sqrt(mX * mX + mY * mY + mZ * mZ); }
		}

		/// <summary>
		/// 三维向量的无参构造，默认值为0
		/// </summary>
		public Vector3D()
		{
			mX = mY = mZ = mW = 0.0f;
		}

		/// <summary>
		/// 传入x,y,z初始化
		/// </summary>
		/// <param name="fX"></param>
		/// <param name="fY"></param>
		/// <param name="fZ"></param>
		public Vector3D(float fX, float fY, float fZ)
		{
			mX = fX;
			mY = fY;
			mZ = fZ;
			mW = 0.0f;
		}

		/// <summary>
		/// 传入x,y,z,w来初始化
		/// </summary>
		/// <param name="fX"></param>
		/// <param name="fY"></param>
		/// <param name="fZ"></param>
		/// <param name="fW"></param>
		public Vector3D(float fX, float fY, float fZ, float fW)
		{
			mX = fX;
			mY = fY;
			mZ = fZ;
			mW = fW;
		}

		/// <summary>
		/// 复制构造
		/// </summary>
		/// <param name="vec3"></param>
		public Vector3D(Vector3D vec3)
		{
			mX = vec3.x;
			mY = vec3.y;
			mZ = vec3.z;
			mW = vec3.w;
		}

		/// <summary>
		/// 归一化向量
		/// </summary>
		/// <returns></returns>
		public Vector3D Normalize()
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
		/// 判断两个位置是否相等
		/// </summary>
		/// <param name="vec3"></param>
		/// <returns></returns>
		public bool IsEqual(Vector3D vec3)
		{
			return mX == vec3.x && mY == vec3.y && mZ == vec3.z;
		}

		/// <summary>
		/// 判断向量相等
		/// </summary>
		/// <param name="fx"></param>
		/// <param name="fy"></param>
		/// <param name="fz"></param>
		/// <returns></returns>
		public bool IsEqual(float fx, float fy, float fz)
		{
			return mX == fx && mY == fy && mZ == fz;
		}

		/// <summary>
		/// 两个向量相加
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3D operator +(Vector3D lhs, Vector3D rhs)
		{
			return new Vector3D(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, 0);
		}

		/// <summary>
		/// 两个向量相减
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3D operator -(Vector3D lhs, Vector3D rhs)
		{
			return new Vector3D(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, 0);
		}

		/// <summary>
		/// 向量和矩阵相乘
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3D operator *(Vector3D lhs, Matrix4X4 rhs)
		{
			Vector3D v = new Vector3D();
			v.x = lhs.x * rhs[0, 0] + lhs.y * rhs[1, 0] + lhs.z * rhs[2, 0] + lhs.w * rhs[3, 0];
			v.y = lhs.x * rhs[0, 1] + lhs.y * rhs[1, 1] + lhs.z * rhs[2, 1] + lhs.w * rhs[3, 1];
			v.z = lhs.x * rhs[0, 2] + lhs.y * rhs[1, 2] + lhs.z * rhs[2, 2] + lhs.w * rhs[3, 2];
			v.w = lhs.x * rhs[0, 3] + lhs.y * rhs[1, 3] + lhs.z * rhs[2, 3] + lhs.w * rhs[3, 3];
			return v;
		}

		/// <summary>
		/// 向量和一个数相乘
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public static Vector3D operator *(Vector3D lhs, float len)
		{
			Vector3D v = new Vector3D();
			v.x = lhs.x * len;
			v.y = lhs.y * len;
			v.z = lhs.z * len;
			v.w = lhs.w;
			return v;
		}

		/// <summary>
		/// 两个向量的点积
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static float Dot(Vector3D lhs, Vector3D rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		/// <summary>
		/// 两个向量的叉乘
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Vector3D Cross(Vector3D lhs, Vector3D rhs)
		{
			float x = lhs.y * rhs.z - lhs.z * rhs.y;
			float y = lhs.z * rhs.x - lhs.x * rhs.z;
			float z = lhs.x * rhs.y - lhs.y * rhs.x;
			return new Vector3D(x, y, z, lhs.w);
		}
	}
}
