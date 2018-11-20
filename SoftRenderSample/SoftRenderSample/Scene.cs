using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class Scene
	{
		private Mesh mMesh;
		private Light mLight;
		private RenderTexture textureMap;
		private RenderTexture[] textureMaps;
		private Camera mCamera;

		public bool IsUseLight = false;

		public Light Lights
		{
			get { return mLight; }
		}

		public Mesh SubMesh
		{
			get { return mMesh; }
		}

		public Camera Camera
		{
			get { return mCamera; }
		}
	
		public Scene()
		{
			InitCarmera();
			InitMesh();
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
		/// 旋转摄像机
		/// </summary>
		/// <param name="position"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void CameraRotate(Vector4 position, float x, float y)
		{
			this.mCamera.Position = (Matrix4x4.RotateX(x) * Matrix4x4.RotateY(y)).LeftApply(position);
		}

		/// <summary>
		/// 改变相机的位置
		/// </summary>
		/// <param name="pos"></param>
		public void UpdateCameraPos(Vector4 pos)
		{
			this.mCamera.Position = pos;
		}

		/// <summary>
		/// 改变相机的上下角度
		/// </summary>
		/// <param name="pitch"></param>
		public void UpdateCameraPitch(float pitch)
		{
			this.mCamera.Pitch = pitch;
		}

		/// <summary>
		/// //改变相机的左右角度
		/// </summary>
		/// <param name="yaw"></param>
		public void UpdateCamerYaw(float yaw)
		{
			this.mCamera.Yaw = yaw;
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
		public RenderTexture GetTextureByFace(FancType types)
		{
			if (types == FancType.NONE)
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
		/// 初始化一个正方体
		/// </summary>
		public void InitMesh()
		{
			mMesh = new Mesh("Cube", 8, 12);
			mMesh.Vertices = new Vertex[24] {
				new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, -1, -1, 1), new Vector4(0, 0, 0, 0), new Color(0, 0, 0)),
				new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, -1, -1, 1), new Vector4(1, 0, 0, 0), new Color(0, 0, 0)),
				new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, -1, -1, 1), new Vector4(0, 0, 0, 0), new Color(0, 0, 0)),

				new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, -1, -1, 1), new Vector4(1, 0, 0, 0), new Color(255, 0, 0)),
				new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, -1, -1, 1),  new Vector4(0, 0, 0, 0), new Color(255, 0, 0)),
				new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, -1, -1, 1), new Vector4(1, 0, 0, 0), new Color(255, 0, 0)),

				new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 1, -1, 1), new Vector4(1, 0, 0, 0), new Color(255, 255, 0)),
				new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 1, -1, 1), new Vector4(0, 1, 0, 0), new Color(255, 255, 0)),
				new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 1, -1, 1), new Vector4(1, 1, 0, 0), new Color(255, 255, 0)),

				new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 1, -1, 1), new Vector4(0, 0, 0, 0), new Color(0, 255, 0)),
				new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 1, -1, 1), new Vector4(1, 1, 0, 0), new Color(0, 255, 0)),
				new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 1, -1, 1), new Vector4(0, 1, 0, 0), new Color(0, 255, 0)),

				new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, -1, 1, 1), new Vector4(0, 1, 0, 0), new Color(0, 0, 255)),
				new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, -1, 1, 1), new Vector4(0, 0, 0, 0), new Color(0, 0, 255)),
				new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, -1, 1, 1), new Vector4(0, 0, 0, 0), new Color(0, 0, 255)),

				new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, -1, 1, 1), new Vector4(1, 1, 0, 0), new Color(255, 0, 255)),
				new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, -1, 1, 1), new Vector4(1, 0, 0, 0), new Color(255, 0, 255)),
				new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, -1, 1, 1), new Vector4(1, 0, 0, 0), new Color(255, 0, 255)),

				new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), new Vector4(1, 1, 0, 0), new Color(255, 255, 255)),
				new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), new Vector4(1, 1, 0, 0), new Color(255, 255, 255)),
				new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), new Vector4(1, 1, 0, 0), new Color(255, 255, 255)),

				new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 1, 1, 1), new Vector4(0, 1, 0, 0), new Color(0, 255, 255)),
				new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 1, 1, 1), new Vector4(0, 1, 0, 0), new Color(0, 255, 255)),
				new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 1, 1, 1), new Vector4(0, 1, 0, 0), new Color(0, 255, 255)),

			};

			mMesh.MakeFace();
		}
	}
}
