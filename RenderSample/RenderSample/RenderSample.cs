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

		public RenderSample()
		{
			InitializeComponent();
			Width = 800;
			Height = 600;

			mGraphics = CreateGraphics();
			mRenderer = new Renderer(Width, Height, (float)System.Math.PI / 4f, 1, 500);
			mRenderer.BindGraphics(mGraphics);
			System.Timers.Timer mainTimer = new System.Timers.Timer(1000 / 60f);
			mainTimer.Elapsed += new ElapsedEventHandler(OnIdle);
			mainTimer.AutoReset = true;
			mainTimer.Enabled = true;
			mainTimer.Start();
			CreateCube();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (mRenderer == null)
				return false;

			switch (keyData)
			{
				case Keys.NumPad0:
					mRenderer.RenderMode = RenderMode.Wireframe;
					break;
				case Keys.NumPad1:
					mRenderer.RenderMode = RenderMode.VertexColor;
					break;
				case Keys.NumPad2:
					mRenderer.RenderMode = RenderMode.Textured;
					break;
				case Keys.A:
					AddLightToScene();
					break;
				case Keys.B:
					ClearLightFromScene();
					break;
				case Keys.Escape:
					Close();
					break;
			}

			return true;
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
		/// </summary>1
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
				new Vector3D(-1, 0, 0), new Vector3D( -1, 0, 0), new Vector3D(-1, 0, 0),
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
			//顶点色
			SampleCommon.Color[] vertColors = {
				// 前面
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(0, 0, 1), new SampleCommon.Color(1, 0, 0),
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(1, 0, 0), new SampleCommon.Color(0, 0, 1),
				// 后面
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(0, 0, 1), new SampleCommon.Color(1, 0, 0),
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(1, 0, 0), new SampleCommon.Color(0, 0, 1),
				// 左面
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(0, 0, 1), new SampleCommon.Color(1, 0, 0),
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(1, 0, 0), new SampleCommon.Color(0, 0, 1),
				// 右面
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(0, 0, 1), new SampleCommon.Color(1, 0, 0),
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(1, 0, 0), new SampleCommon.Color(0, 0, 1),
				// 上面
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(0, 0, 1), new SampleCommon.Color(1, 0, 0),
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(1, 0, 0), new SampleCommon.Color(0, 0, 1),
				// 下面
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(0, 0, 1), new SampleCommon.Color(1, 0, 0),
				new SampleCommon.Color(0, 1, 0), new SampleCommon.Color(1, 0, 0), new SampleCommon.Color(0, 0, 1),
			 };
			List<Vertex> vertices = new List<Vertex>();
			for(int i = 0; i < 36; i ++)
			{
				Vertex vertex = new Vertex();
				vertex.Position = points[indices[i]];
				vertex.Position.w = 1;
				vertex.TexCoord = texcoords[i];
				vertex.Normal = norlmas[i];
				vertex.Color = vertColors[i];
				vertices.Add(vertex);
			}
			mCube = new Mesh(vertices);
			RenderTexture texture = new RenderTexture("env3.bmp");
			mCube.Texture = texture;
			mRenderer.AddRenderObject(mCube);
		}

		/// <summary>
		/// 给场景中增加光照
		/// </summary>
		public void AddLightToScene()
		{
			Light light = new Light(new Vector3D(0.0f, 1.0f, 0.0f), new SampleCommon.Color(0.2f, 0.2f, 0.2f));
			mRenderer.AddLight(light);
		}

		/// <summary>
		/// 清除场景中所有光照
		/// </summary>
		public void ClearLightFromScene()
		{
			mRenderer.ClearLight();
		}
	}
}
