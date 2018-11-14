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

		public List<Triangle> Triangles
		{
			get { return mTriangles; }
			set { mTriangles = value; }
		}

		public Matrix4X4 Transform
		{
			get { return mTransform; }
			set { mTransform = value; }
		}

		public Material Material
		{
			get { return mMaterial; }
			set { mMaterial = value; }
		}

		public RenderTexture Texture
		{
			get { return mTexture; }
			set { mTexture = value; }
		}

		public RenderTexture[] CubeTexture
		{
			get { return mCubeTextures; }
			set { mCubeTextures = value; }
		}

		private Color mTmpVColor = new Color(); // 为了减少new，使用临时的私有变量
		
		/// <summary>
		/// 构造
		/// </summary>
		public Mesh()
		{
			mTriangles = new List<Triangle>();
			mTransform = new Matrix4X4();
		}

		/// <summary>
		/// 构造
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