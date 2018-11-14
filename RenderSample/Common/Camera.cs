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
		private Vector3D	mTarget;
		private Vector3D	mUp;
		private float		mTheta = 0; // 用来限制摄像机绕X轴旋转的角度

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
		public Vector3D Target
		{
			get { return mTarget; }
			set { mTarget = value; }
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
			mTarget = new Vector3D(0, 0, 0, 1);
			mUp = new Vector3D(0, 1, 0, 0);
		}

		/// <summary>
		/// 带有摄像机相关信息的构造
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="lookAt"></param>
		/// <param name="up"></param>
		public Camera(Vector3D pos, Vector3D target, Vector3D up)
		{
			mPosition = pos;
			mTarget = target;
			mUp = up;
		}

		/// <summary>
		/// 获取当前摄像机的摄像机矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4X4 GetViewMat()
		{
			Vector3D dir = mTarget - mPosition;
			Vector3D right = Vector3D.Cross(mUp, dir);
			right.Normalize();
			Matrix4X4 trans = new Matrix4X4(1, 0, 0, 0,
											0, 1, 0, 0,
											0, 0, 1, 0,
											-mPosition.x, -mPosition.y, -mPosition.z, 1);

			Matrix4X4 rotate = new Matrix4X4(right.x, mUp.x, dir.x, 0,
											right.y, mUp.y, dir.y, 0,
											right.z, mUp.z, dir.z, 0,
											0, 0, 0, 1);
			return trans * rotate;
		}

		/// <summary>
		/// 前后移动摄像机
		/// </summary>
		/// <param name="distance">移动的距离</param>
		public void MoveForward(float distance)
		{
			Vector3D dir = (mTarget - mPosition);
			float w = mPosition.w;
			if (distance > 0 && dir.Length < 0.4f)
				return;

			if (distance < 0 && dir.Length > 40)
				return;
	
			if (mTarget.IsEqual(mPosition + (dir.Normalize() * distance)))
				return;

			mPosition = mPosition + (dir.Normalize() * distance);
			mPosition.w = w;
		}

		/// <summary>
		/// 绕X轴旋转
		/// </summary>
		/// <param name="r"></param>
		public void MoveTheta(float r)
		{
			if (mTheta + r > MathUntil.PIDEV2 - 0.3f)
				return;
			if (mTheta +r < -MathUntil.PIDEV2 + 0.3f)
				return;

			mTheta += r;
			mPosition = mPosition * Matrix4X4.RotateX(r);
		}

		/// <summary>
		/// 绕y轴旋转
		/// </summary>
		/// <param name="r"></param>
		public void MovePhi(float r)
		{
			mPosition = mPosition * Matrix4X4.RotateY(r);
		}

	}
}