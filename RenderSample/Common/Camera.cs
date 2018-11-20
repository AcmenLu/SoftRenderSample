using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	/// <summary>
	/// 摄像机类，用来定义摄像机的相关操作
	/// </summary>
	public class Camera
	{
		private Vector3D	mPosition;
		private Vector3D	mTarget;
		private Vector3D	mUp;

		private float mFov;
		private float mNear;
		private float mFar;
		private float mAspect;

		private Matrix4X4 mPerspective;
		private Matrix4X4 mViewMatrix;

		private float mTheta = 0; // 用来限制摄像机绕X轴旋转的角度


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
		/// 透视投影矩阵
		/// </summary>
		public Matrix4X4 PerspectiveMatrix
		{
			get { return mPerspective; }
		}

		/// <summary>
		/// 无参数构造，默认给定一个位置信息
		/// </summary>
		public Camera()
		{
			mPosition = new Vector3D(0, 0, 1, 1);
			mTarget = new Vector3D(0, 0, 0, 1);
			mUp = new Vector3D(0, 1, 0, 0);
			mAspect = 1;
			mNear = 1;
			mFar = 50;
			mFov = (float)Math.PI / 3.0f;
			InitPerspectiveMat();
		}

		/// <summary>
		/// 带有摄像机相关信息的构造
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="lookAt"></param>
		/// <param name="up"></param>
		public Camera(Vector3D eye, Vector3D at, Vector3D up, float aspect, float near, float far, float fov)
		{
			mPosition = eye;
			mTarget = at;
			mUp = up;
			mAspect = aspect;
			mNear = near;
			mFar = far;
			mFov = fov;
			InitPerspectiveMat();
		}

		/// <summary>
		/// 初始化透视投影矩阵
		/// </summary>
		public void InitPerspectiveMat()
		{
			if (mPerspective == null)
				mPerspective = new Matrix4X4();
			mPerspective = Matrix4X4.BuildPerspectiveFovLH(mFov, mAspect, mNear, mFar, ref mPerspective);
		}

		/// <summary>
		/// 获取当前摄像机的摄像机矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4X4 GetViewMat()
		{
			if (mViewMatrix == null)
				mViewMatrix = new Matrix4X4();
			Matrix4X4.BuildLookAtLH(mPosition, mTarget, mUp, ref mViewMatrix);
			return mViewMatrix;
		}

		/// <summary>
		/// 前后移动摄像机
		/// </summary>
		/// <param name="distance">移动的距离</param>
		public void MoveForward(float distance)
		{
			Vector3D dir = (mTarget - mPosition);
			float w = mPosition.w;
			if (distance > 0 && dir.Length < 1.5f)
				return;

			if (distance < 0 && dir.Length > 30)
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