using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

using SampleCommon;
using System.Collections.Generic;

namespace RenderSample
{
	public partial class RenderSample : Form
	{
		private Renderer mRenderer;
		private Graphics mGraphics;
		private Mesh mCube;
		private bool mIsMouseLeftDown;
		private bool mIsMouseRightDown;
		private Vector2 mMouseLeftPos;
		private Vector2 mMouseRightpos;

		public RenderSample()
		{
			InitializeComponent();
			mIsMouseLeftDown = false;
			mIsMouseRightDown = false;
			mMouseLeftPos = new Vector2(0, 0);
			mMouseRightpos = new Vector2(0, 0);
			this.MouseWheel += new MouseEventHandler(OnMouseWheel);
			this.MouseDown += new MouseEventHandler(OnMouseDown);
			this.MouseUp += new MouseEventHandler(OnMouseUp);
			this.MouseMove += new MouseEventHandler(OnMouseMove);
			Width = 800;
			Height = 600;
			OnInit();
		}


		/// <summary>
		/// 初始化事件
		/// </summary>
		public void OnInit()
		{
			mGraphics = CreateGraphics();
			mRenderer = new Renderer(Width, Height, (float)System.Math.PI / 4f, 1f, 500f);
			mRenderer.BindGraphics(mGraphics);
			mRenderer.Camera = new Camera(new Vector3D(0, 0, -5, 1), new Vector3D(0, 0, 0, 1), new Vector3D(0, 1, 0, 0));
			mRenderer.EnableDepthTest = true;
			System.Timers.Timer mainTimer = new System.Timers.Timer(1000 / 60f);
			mainTimer.Elapsed += new ElapsedEventHandler(OnIdle);
			mainTimer.AutoReset = true;
			mainTimer.Enabled = true;
			mainTimer.Start();
			CreateCube();
		}

		/// <summary>
		/// 键盘事件
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (mRenderer == null)
				return false;

			switch (keyData)
			{
				case Keys.NumPad0:
					mRenderer.RenderMode = RenderMode.WIREFRAME;
					break;
				case Keys.NumPad1:
					mRenderer.RenderMode = RenderMode.VERTEXCOLOR;
					break;
				case Keys.NumPad2:
					mRenderer.RenderMode = RenderMode.TEXTURED;
					break;
				case Keys.NumPad3:
					mRenderer.RenderMode = RenderMode.CUBETEXTURED;
					break;
				case Keys.NumPad4:
					mRenderer.CullMode = CullMode.CULL_NONE;
					break;
				case Keys.NumPad5:
					mRenderer.CullMode = CullMode.CULL_CAMERA;
					break;
				case Keys.NumPad6:
					mRenderer.CullMode = CullMode.CULL_CVV;
					break;
				case Keys.NumPad7:
					mRenderer.CullMode = CullMode.CULL_ALL;
					break;
				case Keys.F1:
					AddLightToScene();
					break;
				case Keys.F2:
					ClearLightFromScene();
					break;
				case Keys.F3:
					AddTexture("env3.bmp");
					break;
				case Keys.F4:
					string[] names = { "env0.bmp", "env1.bmp", "env2.bmp", "env3.bmp", "env4.bmp", "env5.bmp" };
					AddCubeTexture(names);
					break;
				case Keys.Escape:
					Close();
					break;
				case Keys.W:
					mRenderer.Camera.MoveTheta(0.1f);
					break;
				case Keys.S:
					mRenderer.Camera.MoveTheta(-0.1f);
					break;
				case Keys.A:
					mRenderer.Camera.MovePhi(0.1f);
					break;
				case Keys.D:
					mRenderer.Camera.MovePhi(-0.1f);
					break;
				case Keys.Q:
					mRenderer.Camera.MoveForward(0.2f);
					break;
				case Keys.E:
					mRenderer.Camera.MoveForward(-0.2f);
					break;
				case Keys.Up:
					mCube.Transform = Matrix4X4.RotateX(0.2f) * mCube.Transform;
					break;
				case Keys.Down:
					mCube.Transform = Matrix4X4.RotateX(-0.2f) * mCube.Transform;
					break;
				case Keys.Left:
					mCube.Transform = Matrix4X4.RotateY(0.2f) * mCube.Transform;
					break;
				case Keys.Right:
					mCube.Transform = Matrix4X4.RotateY(-0.2f) * mCube.Transform;
					break;
			}

			return true;
		}

		/// <summary>
		/// 鼠标事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Delta == 0)
				return;

			mRenderer.Camera.MoveForward(e.Delta / (float)800);
		}

		/// <summary>
		/// 鼠标按下事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				mIsMouseLeftDown = true;
				mMouseLeftPos.x = e.X;
				mMouseLeftPos.y = e.Y;
			}
			else if(e.Button == MouseButtons.Right)
			{
				mIsMouseRightDown = true;
				mMouseRightpos.x = e.X;
				mMouseRightpos.y = e.Y;
			}
		}

		/// <summary>
		/// 鼠标抬起事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				mIsMouseLeftDown = false;
				mMouseLeftPos.x = 0;
				mMouseLeftPos.y = 0;
			}
			else
			{
				mIsMouseRightDown = false;
				mMouseRightpos.x = 0;
				mMouseRightpos.y = 0;
			}
		}

		/// <summary>
		/// 鼠标移动事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (mIsMouseLeftDown == false && mIsMouseRightDown == false)
				return;

			float x = e.X;
			float y = e.Y;
			if (mIsMouseLeftDown == true)
			{
				float dx = mMouseLeftPos.x - x;
				float dy = mMouseLeftPos.y - y;
				mCube.Transform = mCube.Transform * Matrix4X4.RotateY(dx / (float)300);
				mCube.Transform = mCube.Transform * Matrix4X4.RotateX(dy / (float)300);
				mMouseLeftPos.x = x;
				mMouseLeftPos.y = y;
			}
			else if (mIsMouseRightDown == true)
			{
				float dx = mMouseRightpos.x - x;
				float dy = mMouseRightpos.y - y;
				mRenderer.Camera.MovePhi(dx / (float)300);
				mRenderer.Camera.MoveTheta(dy / (float)300);
				mMouseRightpos.x = x;
				mMouseRightpos.y = y;
			}
		}

		/// <summary>
		/// 每帧运行的函数：MainLoop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnIdle(object sender, EventArgs e)
		{
			if (mRenderer == null)
				return;

			mRenderer.OnRender();
		}

		/// <summary>
		/// 三角形的测试函数
		/// </summary>
		public void TriangleTest()
		{
			List<Vertex> vertices = new List<Vertex>();
			Vertex v1 = new Vertex(new Vector3D(400.0f, 50.0f, 1.0f));
			v1.Color = new SampleCommon.Color(1.0f, 0.0f, 0.0f);
			vertices.Add(v1);
	
			Vertex v2 = new Vertex(new Vector3D(200.0f, 450.0f, 0.0f));
			v2.Color = new SampleCommon.Color(0.0f, 1.0f, 0.0f);
			vertices.Add(v2);

			Vertex v3 = new Vertex(new Vector3D(600.0f, 450.0f, 0.0f));
			v3.Color = new SampleCommon.Color(0.0f, 0.0f, 1.0f);
			vertices.Add(v3);
			Mesh msh = new Mesh(vertices);
			if (mRenderer != null)
			{
				mRenderer.ClearRenderObjects();
				mRenderer.AddRenderObject(msh);
			}
		}

		/// <summary>
		/// 矩形的测试函数
		/// </summary>
		public void RectangularTest()
		{
			List<Vertex> vertices = new List<Vertex>();
			Vertex v1 = new Vertex(new Vector3D(200.0f, 50.0f, 1.0f));
			v1.Color = new SampleCommon.Color(1.0f, 0.0f, 0.0f);
			vertices.Add(v1);

			Vertex v2 = new Vertex(new Vector3D(600.0f, 50.0f, 1.0f));
			v2.Color = new SampleCommon.Color(0.0f, 1.0f, 0.0f);
			vertices.Add(v2);

			Vertex v3 = new Vertex(new Vector3D(200.0f, 500.0f, 1.0f));
			v3.Color = new SampleCommon.Color(0.0f, 0.0f, 1.0f);
			vertices.Add(v3);

			Vertex v4 = new Vertex(new Vector3D(600.0f, 50.0f, 1.0f));
			v4.Color = new SampleCommon.Color(1.0f, 0.0f, 0.0f);
			vertices.Add(v4);

			Vertex v5 = new Vertex(new Vector3D(200.0f, 500.0f, 1.0f));
			v5.Color = new SampleCommon.Color(0.0f, 1.0f, 0.0f);
			vertices.Add(v5);

			Vertex v6 = new Vertex(new Vector3D(600.0f, 500.0f, 1.0f));
			v6.Color = new SampleCommon.Color(0.0f, 0.0f, 1.0f);
			vertices.Add(v6);

			Mesh msh = new Mesh(vertices);
			if (mRenderer != null)
			{
				mRenderer.ClearRenderObjects();
				mRenderer.AddRenderObject(msh);
			}
		}

		/// <summary>
		/// 简单光照测试函数
		/// </summary>
		public void LightTest()
		{
			List<Vertex> vertices = new List<Vertex>();
			Vertex v1 = new Vertex(new Vector3D(400.0f, 50.0f, 1.0f));
			v1.Color = new SampleCommon.Color(1.0f, 0.0f, 0.0f);
			vertices.Add(v1);

			Vertex v2 = new Vertex(new Vector3D(200.0f, 450.0f, 0.0f));
			v2.Color = new SampleCommon.Color(0.0f, 1.0f, 0.0f);
			vertices.Add(v2);

			Vertex v3 = new Vertex(new Vector3D(600.0f, 450.0f, 0.0f));
			v3.Color = new SampleCommon.Color(0.0f, 0.0f, 1.0f);
			vertices.Add(v3);
			Mesh msh = new Mesh(vertices);
			if (mRenderer != null)
			{
				Light light = new Light(new Vector3D(0.0f, 1.0f, 0.0f), new SampleCommon.Color(0.2f, 0.2f, 0.2f));
				mRenderer.AddLight(light);
				mRenderer.ClearRenderObjects();
				mRenderer.AddRenderObject(msh);
			}
		}

		/// <summary>
		/// 渲染图片的测试函数
		/// </summary>
		public void TextureTest()
		{
			List<Vertex> vertices = new List<Vertex>();
			Vertex v1 = new Vertex(new Vector3D(200.0f, 50.0f, 1.0f));
			v1.Color = new SampleCommon.Color(1.0f, 0.0f, 0.0f);
			v1.TexCoord = new Vector2(0.0f, 0.0f);
			vertices.Add(v1);

			Vertex v2 = new Vertex(new Vector3D(600.0f, 50.0f, 1.0f));
			v2.Color = new SampleCommon.Color(0.0f, 1.0f, 0.0f);
			v2.TexCoord = new Vector2(1.0f, 0.0f);
			vertices.Add(v2);

			Vertex v3 = new Vertex(new Vector3D(200.0f, 500.0f, 1.0f));
			v3.Color = new SampleCommon.Color(0.0f, 0.0f, 1.0f);
			v3.TexCoord = new Vector2(0.0f, 1.0f);
			vertices.Add(v3);

			Vertex v4 = new Vertex(new Vector3D(600.0f, 50.0f, 1.0f));
			v4.Color = new SampleCommon.Color(1.0f, 0.0f, 0.0f);
			v4.TexCoord = new Vector2(1.0f, 0.0f);
			vertices.Add(v4);

			Vertex v5 = new Vertex(new Vector3D(200.0f, 500.0f, 1.0f));
			v5.Color = new SampleCommon.Color(0.0f, 1.0f, 0.0f);
			v5.TexCoord = new Vector2(0.0f, 1.0f);
			vertices.Add(v5);

			Vertex v6 = new Vertex(new Vector3D(600.0f, 500.0f, 1.0f));
			v6.Color = new SampleCommon.Color(0.0f, 0.0f, 1.0f);
			v6.TexCoord = new Vector2(1.0f, 1.0f);
			vertices.Add(v6);

			Mesh msh = new Mesh(vertices);
			RenderTexture texture = new RenderTexture("env3.bmp");
			msh.Texture = texture;
			if (mRenderer != null)
			{
				mRenderer.ClearRenderObjects();
				mRenderer.AddRenderObject(msh);
			}
		}

		/// <summary>
		/// 创建一个立方体
		/// </summary>
		public void CreateCube()
		{
			// 顶点位置
			Vector3D[] points =
			{
				new Vector3D(-1, 1, -1),
				new Vector3D(-1, -1, -1),
				new Vector3D(1, -1, -1),
				new Vector3D(1, 1, -1),

				new Vector3D(-1, 1, 1),
				new Vector3D(-1, -1, 1),
				new Vector3D(1, -1, 1),
				new Vector3D(1, 1, 1)
			};
			// 顶点索引
			int[] indices =
			{
				// 前面
				0,1,2, 0,2,3,
				// 后面
				7,6,5, 7,5,4,
				// 左面
				0,4,5, 0,5,1,
				// 右面
				2,6,7, 2,7,3,
				// 上面
				3,7,4, 3,4,0,
				// 下面
				1,5,6, 1,6,2
			};
			// UV坐标
			Vector2[] texcoords ={
				// 前面
				new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
				new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
				// 后面
				new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
				new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
				// 左面
				new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
				new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
				// 右面
				new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
				new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
				// 上面
				new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
				new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0),
				// 下面
				new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
				new Vector2(0, 0),new Vector2(1, 1),new Vector2(1, 0)
			};
			//法线
			Vector3D[] norlmas = {
				// 前面
				new Vector3D(0, 0, -1), new Vector3D(0, 0, -1), new Vector3D(0, 0, -1),
				new Vector3D(0, 0, -1), new Vector3D(0, 0, -1), new Vector3D(0, 0, -1),
				// 后面
				new Vector3D(0, 0, 1), new Vector3D(0, 0, 1), new Vector3D(0, 0, 1),
				new Vector3D(0, 0, 1), new Vector3D(0, 0, 1), new Vector3D(0, 0, 1),
				// 左面
				new Vector3D(-1, 0, 0), new Vector3D(-1, 0, 0), new Vector3D(-1, 0, 0),
				new Vector3D(-1, 0, 0), new Vector3D(-1, 0, 0), new Vector3D(-1, 0, 0),
				// 右面
				new Vector3D(1, 0, 0), new Vector3D(1, 0, 0), new Vector3D(1, 0, 0),
				new Vector3D(1, 0, 0), new Vector3D(1, 0, 0), new Vector3D(1, 0, 0),
				// 上面
				new Vector3D(0, 1, 0), new Vector3D(0, 1, 0), new Vector3D(0, 1, 0),
				new Vector3D(0, 1, 0 ), new Vector3D(0, 1, 0), new Vector3D(0, 1, 0),
				// 下面
				new Vector3D(0, -1, 0), new Vector3D(0, -1, 0), new Vector3D(0, -1, 0),
				new Vector3D(0, -1, 0), new Vector3D(0, -1, 0), new Vector3D(0, -1, 0),
			};

			List<Vertex> vertices = new List<Vertex>();
			for(int i = 0; i < 36; i ++)
			{
				Vertex vertex = new Vertex();
				Vector3D point = points[indices[i]];
				vertex.Position = point;
				vertex.Position.w = 1;
				vertex.TexCoord = texcoords[i];
				vertex.Normal = norlmas[i];
				vertex.Color = new SampleCommon.Color((point.x + 1) / 2, (point.y + 1) / 2, (point.z + 1) / 2);
				vertices.Add(vertex);
			}
			mCube = new Mesh(vertices);
			mRenderer.AddRenderObject(mCube);
		}

		/// <summary>
		/// 给场景中增加光照
		/// </summary>
		public void AddLightToScene()
		{
			Light light = new Light(new Vector3D(2, 0, 0), new SampleCommon.Color(0.0f, 0.9f, 0.2f));
			mRenderer.AddLight(light);
			Material mat = new Material(0.9f, new SampleCommon.Color(0.8f, 0.8f, 0.8f));
			mCube.Material = mat;
		}

		/// <summary>
		/// 清除场景中所有光照
		/// </summary>
		public void ClearLightFromScene()
		{
			mCube.Material = null;
			mRenderer.ClearLight();
		}

		/// <summary>
		/// 添加一个图片到模型上
		/// </summary>
		/// <param name="filename"></param>
		public void AddTexture(string filename)
		{
			if (mCube.Texture == null)
			{
				RenderTexture texture = new RenderTexture(filename);
				mCube.Texture = texture;
			}
			else
			{
				mCube.Texture = null;
			}
		}

		/// <summary>
		/// 添加立方体贴图
		/// </summary>
		/// <param name="names"></param>
		public void AddCubeTexture(string[] names)
		{
			if (names.Length != 6)
				return;

			RenderTexture[] textures = new RenderTexture[names.Length];
			for (int i = 0; i < names.Length; i ++)
			{
				textures[i] = new RenderTexture(names[i]);
			}
			mCube.CubeTexture = textures;
		}
	}
}
