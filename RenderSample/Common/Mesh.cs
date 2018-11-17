using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleCommon
{
	public class Mesh
	{
		private List<Triangle> mTriangles;
		private Matrix4X4 mTransform;
		private Material mMaterial;
		private RenderTexture mTexture;
		private RenderTexture[] mCubeTextures;

		/// <summary>
		/// 模型的三角形列表
		/// </summary>
		public List<Triangle> Triangles
		{
			get { return mTriangles; }
			set { mTriangles = value; }
		}

		/// <summary>
		/// 模型的Transform
		/// </summary>
		public Matrix4X4 Transform
		{
			get { return mTransform; }
			set { mTransform = value; }
		}

		/// <summary>
		/// 模型上面的材质
		/// </summary>
		public Material Material
		{
			get { return mMaterial; }
			set { mMaterial = value; }
		}

		/// <summary>
		/// 贴图
		/// </summary>
		public RenderTexture Texture
		{
			get { return mTexture; }
			set { mTexture = value; }
		}

		/// <summary>
		/// 立方体贴图的六张图
		/// </summary>
		public RenderTexture[] CubeTexture
		{
			get { return mCubeTextures; }
			set { mCubeTextures = value; }
		}

		/// <summary>
		/// 默认构造，构造一个没有任何三角形的空mesh
		/// </summary>
		public Mesh()
		{
			mTriangles = new List<Triangle>();
			mTransform = new Matrix4X4();
		}

		/// <summary>
		/// 用三角形列表构造
		/// </summary>
		public Mesh(List<Triangle> triangles)
		{
			mTriangles = triangles;
			mTransform = new Matrix4X4();
		}

		/// <summary>
		/// 渲染事件
		/// </summary>
		/// <param name="renderer"></param>
		public void OnRender(Renderer renderer)
		{
			if (mTriangles.Count() <= 0)
				return;

			Vector3D normal = new Vector3D();
			Triangle triangle = null;
			for (int i = 0; i < mTriangles.Count(); i++)
			{
				triangle = mTriangles[i];
				normal = triangle.GetNormal().Normalize();
				if (renderer.RenderMode == RenderMode.CUBETEXTURED && mCubeTextures != null && mCubeTextures.Length == 6)
				{
					// 前面
					if (normal.IsEqual(0, 0, -1))
						mTexture = mCubeTextures[0];
					// 后面
					else if (normal.IsEqual(0, 0, 1))
						mTexture = mCubeTextures[1];
					// 左面
					else if (normal.IsEqual(-1, 0, 0))
						mTexture = mCubeTextures[2];
					// 右面
					else if (normal.IsEqual(1, 0, 0))
						mTexture = mCubeTextures[3];
					//上面
					else if (normal.IsEqual(0, 1, 0))
						mTexture = mCubeTextures[4];
					// 下面
					else if (normal.IsEqual(0, -1, 0))
						mTexture = mCubeTextures[5];
				}

				mTriangles[i].Draw(renderer, mTransform, mTexture, mMaterial);
			}
		}
	}
}