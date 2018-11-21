using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftRenderSample
{
	public partial class SoftRenderSample : Form
	{
		private Device mDevice;
		private Bitmap mBmp;
		private Rectangle mRect;
		private Graphics mGraphics;
		private Scene mScene;
		private PixelFormat mPixelFormat;
		private BitmapData mData;

		private Matrix4x4 mViewMat;
		private Matrix4x4 mProjectionMat;
		private Mesh mCube;
		private bool mIsMouseLeftDown = false;
		private Vector2 mMouseLeftPos = new Vector2();


		public SoftRenderSample()
		{
			InitializeComponent();
			InitSettings();
			Init();
			this.CenterToScreen();
		}

		/// <summary>
		/// 初始化winfor设置
		/// </summary>
		private void InitSettings()
		{
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		/// <summary>
		/// 初始化自定义变量
		/// </summary>
		private void Init()
		{
			mBmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Format24bppRgb);
			mDevice = new Device(mBmp);
			mScene = new Scene();
			mViewMat = mScene.Camera.GetLookAt();
			mProjectionMat = mScene.Camera.GetProject((float)System.Math.PI * 0.3f, (float)ClientSize.Width / (float)ClientSize.Height, 1f, 100.0f);
			this.mRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
			this.mPixelFormat = mBmp.PixelFormat;
			AddCubeToScene(mScene);
		}

		/// <summary>
		/// 绘制事件
		/// </summary>
		/// <param name="pe"></param>
		protected override void OnPaint(PaintEventArgs pe)
		{
			mData = this.mBmp.LockBits(mRect, ImageLockMode.ReadWrite, this.mPixelFormat);
			this.mDevice.Clear(mData);
			mDevice.Render(mScene, mData, mViewMat, mProjectionMat);
			this.mBmp.UnlockBits(mData);
			mGraphics = pe.Graphics;
			mGraphics.DrawImage(this.mBmp, new Rectangle(0, 0, mBmp.Width, mBmp.Height));
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="g"></param>
		private void OnLoad(object sender,EventArgs g)
		{
			this.MouseWheel += new MouseEventHandler(OnMouseWheel);
			this.MouseMove += new MouseEventHandler(OnMouseMove);
			this.MouseDown += new MouseEventHandler(OnMouseDown);
			this.MouseUp += new MouseEventHandler(OnMouseUp);
		}

		/// <summary>
		/// 键盘事件
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.NumPad0:
				case Keys.Q:
					mDevice.renderMode = RenderMode.WIREFRAME;
					break;
				case Keys.NumPad1:
				case Keys.W:
					mDevice.renderMode = RenderMode.VERTEXCOLOR;
					break;
				case Keys.NumPad2:
				case Keys.E:
					mDevice.renderMode = RenderMode.TEXTURED;
					break;
				case Keys.NumPad3:
				case Keys.R:
					mDevice.renderMode = RenderMode.CUBETEXTURED;
					break;
				case Keys.F1:
					Light light = new Light(new Vector4(5, 5, -5, 1), new Color(200, 255, 255));
					mScene.AddLight(light);
					mScene.IsUseLight = true;
					break;
				case Keys.F2:
					mScene.DelLight();
					mScene.IsUseLight = false;
					break;
				case Keys.Escape:
					Close();
					break;
			}

			if (keyData != Keys.Escape)
				this.Invalidate();

			return true;
		}

		/// <summary>
		/// 鼠标滚动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta == 0)
				return;

			mScene.Camera.MoveForward(e.Delta / (float)900);
			mViewMat = mScene.Camera.GetLookAt();
			this.Invalidate();
		}

		/// <summary>
		/// 鼠标移动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (mIsMouseLeftDown == false)
				return;

			if (e.Button == MouseButtons.Left)
			{
				float x = e.X;
				float y = e.Y;
				float dx = mMouseLeftPos.X - x;
				float dy = mMouseLeftPos.Y - y;
				mCube.Transform = mCube.Transform * Matrix4x4.RotateY(dx / 5f);
				mCube.Transform = mCube.Transform * Matrix4x4.RotateX(dy / 5f);
				mMouseLeftPos.X = x;
				mMouseLeftPos.Y = y;
				this.Invalidate();
			}
		}

		/// <summary>
		/// 鼠标按下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				mIsMouseLeftDown = true;
				mMouseLeftPos.X = e.X;
				mMouseLeftPos.Y = e.Y;
			}
		}

		/// <summary>
		/// 鼠标抬起
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				mIsMouseLeftDown = false;
				mMouseLeftPos.X = 0f;
				mMouseLeftPos.Y = 0f;
			}
		}

		/// <summary>
		/// 添加一个立方体到场景中
		/// </summary>
		/// <param name="scene"></param>
		private void AddCubeToScene(Scene scene)
		{
			if (scene == null)
				return;

			mCube = new Mesh("Cube");
			mCube.Vertices = new Vertex[24] {
				new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, -1, -1, 1), new Vector2(0, 0), new Color(0, 0, 0)),
				new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, -1, -1, 1), new Vector2(1, 0), new Color(0, 0, 0)),
				new Vertex(new Vector4(-1, -1, -1, 1), new Vector4(-1, -1, -1, 1), new Vector2(0, 0), new Color(0, 0, 0)),

				new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, -1, -1, 1), new Vector2(1, 0), new Color(255, 0, 0)),
				new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, -1, -1, 1),  new Vector2(0, 0), new Color(255, 0, 0)),
				new Vertex(new Vector4(1, -1, -1, 1), new Vector4(1, -1, -1, 1), new Vector2(1, 0), new Color(255, 0, 0)),

				new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 1, -1, 1), new Vector2(1, 0), new Color(255, 255, 0)),
				new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 1, -1, 1), new Vector2(0, 1), new Color(255, 255, 0)),
				new Vertex(new Vector4(1, 1, -1, 1), new Vector4(1, 1, -1, 1), new Vector2(1, 1), new Color(255, 255, 0)),

				new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 1, -1, 1), new Vector2(0, 0), new Color(0, 255, 0)),
				new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 1, -1, 1), new Vector2(1, 1), new Color(0, 255, 0)),
				new Vertex(new Vector4(-1, 1, -1, 1), new Vector4(-1, 1, -1, 1), new Vector2(0, 1), new Color(0, 255, 0)),

				new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, -1, 1, 1), new Vector2(0, 1), new Color(0, 0, 255)),
				new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, -1, 1, 1), new Vector2(0, 0), new Color(0, 0, 255)),
				new Vertex(new Vector4(-1, -1, 1, 1), new Vector4(-1, -1, 1, 1), new Vector2(0, 0), new Color(0, 0, 255)),

				new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, -1, 1, 1), new Vector2(1, 1), new Color(255, 0, 255)),
				new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, -1, 1, 1), new Vector2(1, 0), new Color(255, 0, 255)),
				new Vertex(new Vector4(1, -1, 1, 1), new Vector4(1, -1, 1, 1), new Vector2(1, 0), new Color(255, 0, 255)),

				new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), new Vector2(1, 1), new Color(255, 255, 255)),
				new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), new Vector2(1, 1), new Color(255, 255, 255)),
				new Vertex(new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1), new Vector2(1, 1), new Color(255, 255, 255)),

				new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 1, 1, 1), new Vector2(0, 1), new Color(0, 255, 255)),
				new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 1, 1, 1), new Vector2(0, 1), new Color(0, 255, 255)),
				new Vertex(new Vector4(-1, 1, 1, 1), new Vector4(-1, 1, 1, 1), new Vector2(0, 1), new Color(0, 255, 255)),

			};

			mCube.Faces = new Face[] {
				// 正面
				new Face(2, 5, 8, FaceTypes.NEAR),
				new Face(2, 8, 11, FaceTypes.NEAR),
				// 右面
				new Face(4, 16, 7, FaceTypes.RIGHT),
				new Face(16, 19, 7, FaceTypes.RIGHT),
				// 左面
				new Face(13, 1, 10, FaceTypes.LEFT),
				new Face(13, 10, 22, FaceTypes.LEFT),
				// 背面
				new Face(17, 14, 23, FaceTypes.FAR),
				new Face(17, 23, 20, FaceTypes.FAR),
				// 上面
				new Face(9, 6, 18, FaceTypes.TOP),
				new Face(9, 18, 21, FaceTypes.TOP),
				// 下面
				new Face(12, 15, 3, FaceTypes.BUTTOM),
				new Face(12, 3, 0, FaceTypes.BUTTOM)
			};
			mCube.InitTextureMap();
			scene.AddMesh(mCube);
		}

	}
}
