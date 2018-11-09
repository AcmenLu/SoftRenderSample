using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Matrix4X4
	{
		private float[,] mfValues;

		public float this[int i, int j]
		{
			get { return mfValues[i, j]; }
			set { mfValues[i, j] = value; }
		}

		/// <summary>
		/// 矩阵的无参数构造
		/// </summary>
		public Matrix4X4()
		{
			mfValues = new float[4, 4];
			this.Identity();
		}

		/// <summary>
		/// 举证的参数构造
		/// </summary>
		/// <param name="a1"></param>
		/// <param name="a2"></param>
		/// <param name="a3"></param>
		/// <param name="a4"></param>
		/// <param name="b1"></param>
		/// <param name="b2"></param>
		/// <param name="b3"></param>
		/// <param name="b4"></param>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <param name="c3"></param>
		/// <param name="c4"></param>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <param name="d3"></param>
		/// <param name="d4"></param>
		public Matrix4X4(float a1, float a2, float a3, float a4,
			float b1, float b2, float b3, float b4,
			float c1, float c2, float c3, float c4,
			float d1, float d2, float d3, float d4)
		{
			mfValues = new float[4, 4];
			mfValues[0, 0] = a1; mfValues[0, 1] = a2; mfValues[0, 2] = a3; mfValues[0, 3] = a4;
			mfValues[1, 0] = b1; mfValues[1, 1] = b2; mfValues[1, 2] = b3; mfValues[1, 3] = b4;
			mfValues[2, 0] = c1; mfValues[2, 1] = c2; mfValues[2, 2] = c3; mfValues[2, 3] = c4;
			mfValues[3, 0] = d1; mfValues[3, 1] = d2; mfValues[3, 2] = d3; mfValues[3, 3] = d4;
		}

		/// <summary>
		/// 复制构造
		/// </summary>
		/// <param name="mat4"></param>
		public Matrix4X4(Matrix4X4 mat4)
		{
			for (int i = 0; i < 4; i ++)
			{
				for (int j = 0; j < 4; j++)
					mfValues[i, j] = mat4[i, j];
			}
		}

		/// <summary>
		/// 单位化矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4X4 Identity()
		{
			mfValues[0, 0] = 1.0f; mfValues[0, 1] = 0.0f; mfValues[0, 2] = 0.0f; mfValues[0, 3] = 0.0f;
			mfValues[1, 0] = 0.0f; mfValues[1, 1] = 1.0f; mfValues[1, 2] = 0.0f; mfValues[1, 3] = 0.0f;
			mfValues[2, 0] = 0.0f; mfValues[2, 1] = 0.0f; mfValues[2, 2] = 1.0f; mfValues[2, 3] = 0.0f;
			mfValues[3, 0] = 0.0f; mfValues[3, 1] = 0.0f; mfValues[3, 2] = 0.0f; mfValues[3, 3] = 1.0f;
			return this;
		}

		/// <summary>
		/// 归0化矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4X4 Zero()
		{
			mfValues[0, 0] = 0.0f; mfValues[0, 1] = 0.0f; mfValues[0, 2] = 0.0f; mfValues[0, 3] = 0.0f;
			mfValues[1, 0] = 0.0f; mfValues[1, 1] = 0.0f; mfValues[1, 2] = 0.0f; mfValues[1, 3] = 0.0f;
			mfValues[2, 0] = 0.0f; mfValues[2, 1] = 0.0f; mfValues[2, 2] = 0.0f; mfValues[2, 3] = 0.0f;
			mfValues[3, 0] = 0.0f; mfValues[3, 1] = 0.0f; mfValues[3, 2] = 0.0f; mfValues[3, 3] = 0.0f;
			return this;
		}

		/// <summary>
		/// 矩阵转置
		/// </summary>
		/// <returns></returns>
		public Matrix4X4 Transpose()
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = i; j < 4; j++)
				{
					float temp = mfValues[i, j];
					mfValues[i, j] = mfValues[j, i];
					mfValues[j, i] = temp;
				}
			}

			return this;
		}

		/// <summary>
		/// 两个4*4矩阵相乘
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static Matrix4X4 operator *(Matrix4X4 lhs, Matrix4X4 rhs)
		{
			Matrix4X4 mat4 = new Matrix4X4();
			mat4.Zero();
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						mat4[i, j] += lhs[i, k] * rhs[k, j];
					}
				}
			}
			return mat4;
		}

		/// <summary>
		/// 获取平移矩阵
		/// </summary>
		/// <param name="fX"></param>
		/// <param name="fY"></param>
		/// <param name="fZ"></param>
		/// <returns></returns>
		public static Matrix4X4 Translate(float fX, float fY, float fZ)
		{
			return new Matrix4X4(1.0f, 0.0f, 0.0f, 0.0f,
								0.0f, 1.0f, 0.0f, 0.0f,
								0.0f, 0.0f, 1.0f, 0.0f,
								fX, fY, fZ, 1.0f);
		}

		/// <summary>
		/// 获取平移矩阵
		/// </summary>
		/// <param name="vec3"></param>
		/// <returns></returns>
		public static Matrix4X4 Translate(Vector3D vec3)
		{
			return Translate(vec3.x, vec3.y, vec3.z);
		}

		/// <summary>
		/// 获取缩放矩阵
		/// </summary>
		/// <param name="fX"></param>
		/// <param name="fY"></param>
		/// <param name="fZ"></param>
		/// <returns></returns>
		public static Matrix4X4 Scale(float fX, float fY, float fZ)
		{
			return new Matrix4X4(fX, 0.0f, 0.0f, 0.0f,
								 0.0f, fY, 0.0f, 0.0f,
								 0.0f, 0.0f, fZ, 0.0f,
								 0.0f, 0.0f, 0.0f, 1.0f);
		}

		/// <summary>
		/// 缩放矩阵
		/// </summary>
		/// <param name="vec3"></param>
		/// <returns></returns>
		public static Matrix4X4 Scale(Vector3D vec3)
		{
			return Scale(vec3.x, vec3.y, vec3.z);
		}

		/// <summary>
		/// 绕X轴旋转的矩阵
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public static Matrix4X4 RotateX(float r)
		{
			Matrix4X4 mat4 = new Matrix4X4();
			mat4.Identity();
			mat4[1, 1] = (float)(System.Math.Cos(r));
			mat4[1, 2] = (float)(System.Math.Sin(r));

			mat4[2, 1] = (float)(-System.Math.Sin(r));
			mat4[2, 2] = (float)(System.Math.Cos(r));
			return mat4;
		}

		/// <summary>
		/// 绕Y轴旋转
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public static Matrix4X4 RotateY(float r)
		{
			Matrix4X4 mat4 = new Matrix4X4();
			mat4.Identity();
			mat4[0, 0] = (float)(System.Math.Cos(r));
			mat4[0, 2] = (float)(-System.Math.Sin(r));

			mat4[2, 0] = (float)(System.Math.Sin(r));
			mat4[2, 2] = (float)(System.Math.Cos(r));
			return mat4;
		}

		/// <summary>
		/// 绕Z轴旋转
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		public static Matrix4X4 RotateZ(float r)
		{
			Matrix4X4 mat4 = new Matrix4X4();
			mat4.Identity();
			mat4[0, 0] = (float)(System.Math.Cos(r));
			mat4[0, 1] = (float)(System.Math.Sin(r));
			mat4[1, 0] = (float)(-System.Math.Sin(r));
			mat4[1, 1] = (float)(System.Math.Cos(r));
			return mat4;
		}

		/// <summary>
		/// 投影矩阵
		/// </summary>
		/// <param name="fov">视角大小</param>
		/// <param name="aspect">宽高比</param>
		/// <param name="zn">近截面距离</param>
		/// <param name="zf">远截面距离</param>
		/// <returns></returns>
		public static Matrix4X4 Projection(float fov, float aspect, float zn, float zf)
		{
			Matrix4X4 mat4 = new Matrix4X4();
			mat4.Zero();
			mat4[0, 0] = (float)(1 / (System.Math.Tan(fov * 0.5f) * aspect));
			mat4[1, 1] = (float)(1 / System.Math.Tan(fov * 0.5f));
			mat4[2, 2] = zf / (zf - zn);
			mat4[2, 3] = 1f;
			mat4[3, 2] = (zn * zf) / (zn - zf);
			return mat4;
		}
	}
}
