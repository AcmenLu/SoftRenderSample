using System;

namespace SoftRenderSample
{
	//相机类
	class Camera
	{
		/// <summary>
		/// 相机位置
		/// </summary>
		public Vector4 Position;
		//目标
		public Vector4 Target;
		//观察方向
		public Vector4 Up;
		public float Pitch;
		public float Yaw;

		/// <summary>
		/// 创建一个观察矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4x4 LookAt()
		{
			Matrix4x4 view = new Matrix4x4(1);
			Vector4 xaxis, yaxis, zaxis;

			//法向量 z
			zaxis = Target - Position;
			zaxis.Normalize();
			xaxis = Vector4.Cross(Up, zaxis);
			xaxis.Normalize();
			yaxis = Vector4.Cross(zaxis, xaxis);
			yaxis.Normalize();

			view.matrix[0, 0] = xaxis.X;
			view.matrix[1, 0] = xaxis.Y;
			view.matrix[2, 0] = xaxis.Z;
			view.matrix[3, 0] = -Vector4.Dot(xaxis, Position);

			view.matrix[0, 1] = yaxis.X;
			view.matrix[1, 1] = yaxis.Y;
			view.matrix[2, 1] = yaxis.Z;
			view.matrix[3, 1] = -Vector4.Dot(yaxis, Position);

			view.matrix[0, 2] = zaxis.X;
			view.matrix[1, 2] = zaxis.Y;
			view.matrix[2, 2] = zaxis.Z;
			view.matrix[3, 2] = -Vector4.Dot(zaxis, Position);

			view.matrix[0, 3] = view.matrix[1, 3] = view.matrix[2, 3] = 0.0f;
			view.matrix[3, 3] = 1.0f;

			return view;
		}

		//投影矩阵
		/// <summary>
		/// 投影矩阵
		/// </summary>
		/// <param name="fov">y方向的视角</param>
		/// <param name="aspect">纵横比</param>
		/// <param name="zn">近裁剪 平面到原点的距离</param>
		/// <param name="zf">远裁剪 平面到原点的距离</param>
		/// <returns></returns>
		public Matrix4x4 Project(float fov,float aspect,float zn,float zf)
		{
			Matrix4x4 project = new Matrix4x4(1);
			project.SetZero();
			project.matrix[0, 0] = 1 / ((float)Math.Tan(fov * 0.5f) * aspect);
			project.matrix[1, 1] = 1 / (float)Math.Tan(fov*0.5f);
			project.matrix[2, 2] = (zf+zn) / (zf - zn);
			project.matrix[2, 3] = 1.0f;
			project.matrix[3, 2] = 2*(zn*zf) / (zn-zf);

			return project;
		}
	}
}
