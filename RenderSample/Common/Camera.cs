using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Camera
	{
		private Vector3D	mPosition;
		private Vector3D	mLookAtPos;
		private Vector3D	mUp;

		/// <summary>
		/// mPosition的get/set
		/// </summary>
		public Vector3D Position
		{
			get { return mPosition; }
			set { mPosition = value; }
		}

		/// <summary>
		/// mLookAtPos的get/set
		/// </summary>
		public Vector3D LookAt
		{
			get { return mLookAtPos; }
			set { mLookAtPos = value; }
		}

		/// <summary>
		/// mUp的get/set
		/// </summary>
		public Vector3D Up
		{
			get { return mUp; }
			set { mUp = value; }
		}

		/// <summary>
		/// 无参数构造，默认给定一个位置信息
		/// </summary>
		public Camera()
		{
			mPosition = new Vector3D(0, 0, 1, 1);
			mLookAtPos = new Vector3D(0, 0, 0, 1);
			mUp = new Vector3D(0, 1, 0, 0);
		}

		/// <summary>
		/// 带有摄像机相关信息的构造
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="lookAt"></param>
		/// <param name="up"></param>
		public Camera(Vector3D pos, Vector3D lookAt, Vector3D up)
		{
			mPosition = pos;
			mLookAtPos = lookAt;
			mUp = up;
		}

		/// <summary>
		/// 获取当前摄像机的摄像机矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4X4 GetViewMat()
		{
			Vector3D dir = mLookAtPos - mPosition;
			Vector3D right = Vector3D.Cross(mUp, dir);
			right.Normalize();
			Matrix4X4 trans = new Matrix4X4(1.0f, 0.0f, 0.0f, 0.0f,
										0.0f, 1.0f, 0.0f, 0.0f,
										0.0f, 0.0f, 1.0f, 0.0f,
										-mPosition.x, -mPosition.y, -mPosition.z, 1.0f);

			Matrix4X4 rotate = new Matrix4X4(right.x, mUp.x, dir.x, 0.0f,
										right.y, mUp.y, dir.y, 0.0f,
										right.z, mUp.z, dir.z, 0.0f,
										0.0f, 0.0f, 0.0f, 1.0f);
			return trans * rotate;
		}
	}
}
