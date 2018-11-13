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
		private float		mPitch = 0;
		private float		mYaw = 0;

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
			if (distance > 0 && dir.Length < 2.4f)
				return;

			if (distance < 0 && dir.Length > 40)
				return;
	
			if (mTarget.IsEqual(mPosition + (dir.Normalize() * distance)))
				return;

			mPosition = mPosition + (dir.Normalize() * distance);
			mPosition.w = w;
		}

		/// <summary>
		/// 向右移动摄像机
		/// </summary>
		/// <param name="distance"></param>
		public void MoveRight(float distance)
		{
			Vector3D dir = (mTarget - mPosition);
			Vector3D right = Vector3D.Cross(mUp, dir);
			float w = mPosition.w;
			mPosition = mPosition + (right.Normalize() * distance);
			mPosition.w = w;
		}
		
		/// <summary>
		/// 上下左右移动摄像机
		/// </summary>
		/// <param name="pr"></param>
		/// <param name="yr"></param>
		public void MovePitchAndYaw(float pr, float yr)
		{
			mPitch += pr;
			mPitch = mPitch > MathUntil.PIDEV2 - 0.3f ? MathUntil.PIDEV2 - 0.3f : mPitch;
			mPitch = mPitch < -MathUntil.PIDEV2 + 0.3f ? -MathUntil.PIDEV2 + 0.3f : mPitch;
			mYaw += yr;
			Vector3D dir = (mTarget - mPosition);
			float length = dir.Length;

			float x = (float)(-Math.Sin(mYaw) * Math.Cos(mPitch));
			float y = (float)Math.Sin(mPitch);
			float z = (float)(-Math.Cos(mPitch) * Math.Cos(mYaw));

			mPosition.x = mTarget.x - x * length;
			mPosition.y = mTarget.y - y * length;
			mPosition.z = mTarget.z- z * length;
		}

	}
}