using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class Scene
	{
		//private Mesh mMesh;
		private Light mLight;
		private RenderTexture textureMap;
		private RenderTexture[] textureMaps;
		private Camera mCamera;

		private List<Mesh> mMeshs;

		public bool IsUseLight = false;

		public Light Lights
		{
			get { return mLight; }
		}

		public List<Mesh> Meshs
		{
			get { return mMeshs; }
		}

		public Camera Camera
		{
			get { return mCamera; }
		}
	
		public Scene()
		{
			InitCarmera();
			InitTextureMap();
		}

		/// <summary>
		/// 为当前场景初始化一个摄像机
		/// </summary>
		public void InitCarmera()
		{
			mCamera = new Camera();
			mCamera.Position = new Vector4(0,0,-5, 1);
			mCamera.Target = new Vector4(0, 0, 0, 1);
			mCamera.Up = new Vector4(0, 1, 0, 1);
			mCamera.Pitch = 0f;
			mCamera.Yaw = 0f;
		}

		/// <summary>
		/// 加载所用到的贴图
		/// </summary>
		public void InitTextureMap()
		{
			textureMap = new RenderTexture(@"env1.bmp");
			textureMaps = new RenderTexture[6];
			for(int i = 0; i < 6; i ++)
				textureMaps[i] = new RenderTexture(@"env" + i.ToString() + ".bmp");
		}

		/// <summary>
		/// 根据模型的面的方向返回贴图
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public RenderTexture GetTextureByFace(FaceTypes types)
		{
			if (types == FaceTypes.NONE)
			{
				return textureMap;
			}
			else
			{
				int index = (int)types;
				if (textureMaps.Length == 6 && index >= 0 && index < 6)
					return textureMaps[index];
				else
					return textureMap;
			}
		}
		/// <summary>
		/// 添加一个光
		/// </summary>
		public void AddLight(Light light)
		{
			mLight = light;
		}

		/// <summary>
		/// 删除光
		/// </summary>
		/// <param name="light"></param>
		public void delLight()
		{
			mLight = null;
		}

		/// <summary>
		/// 增加一个渲染的模型
		/// </summary>
		/// <param name="msh"></param>
		public void AddMesh(Mesh msh)
		{
			if (mMeshs == null)
				mMeshs = new List<Mesh>();

			mMeshs.Add(msh);
		}
	}
}
