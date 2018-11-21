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
		private Matrix4x4 mMatrix;
		private BitmapData mData;
		
		public int oldX = 0;
		public int oldY = 0;
		public int RotionX = 0;
		public int RotionY = 0;
		public bool isMouseDown = false;


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
			mMatrix = mDevice.GetMvpMatrix(mScene.Camera, RotionX, RotionY);
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
			mDevice.Render(mScene, mData, mMatrix);
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
					this.Invalidate();
					break;
				case Keys.NumPad1:
				case Keys.W:
					mDevice.renderMode = RenderMode.VERTEXCOLOR;
					mData = this.mBmp.LockBits(mRect, ImageLockMode.ReadWrite, this.mPixelFormat);
					this.Invalidate();
					this.mBmp.UnlockBits(mData);
					break;
				case Keys.NumPad2:
				case Keys.E:
					mDevice.renderMode = RenderMode.TEXTURED;
					this.Invalidate();
					break;
				case Keys.NumPad3:
				case Keys.R:
					mDevice.renderMode = RenderMode.CUBETEXTURED;
					this.Invalidate();
					break;
				case Keys.F1:
					Light light = new Light(new Vector4(5, 5, -5, 1), new Color(200, 255, 255));
					mScene.AddLight(light);
					mScene.IsUseLight = true;
					this.Invalidate();
					break;
				case Keys.F2:
					mScene.delLight();
					mScene.IsUseLight = false;
					this.Invalidate();
					break;
				case Keys.Escape:
					Close();
					break;
			}

			return true;
		}

		/// <summary>
		/// 鼠标滚动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseWheel(object sender, MouseEventArgs e)
		{
			float oriX = this.mScene.Camera.Position.X;
			float oriY = this.mScene.Camera.Position.Y;
			float oriZ = this.mScene.Camera.Position.Z;
			float oriz = this.mDevice.ScaleV;

			if (e.Delta < 0)
			{
				if (oriz <= 0.6f)
					mDevice.ScaleV = 0.6f;
				else
					mDevice.ScaleV = oriz - 0.1f;
				mMatrix = mDevice.GetMvpMatrix(mScene.Camera, new Matrix4x4(1));
				this.Invalidate();
			}
			else
			{
				mDevice.ScaleV = oriz + 0.1f;
				mMatrix = mDevice.GetMvpMatrix(mScene.Camera, new Matrix4x4(1));
				this.Invalidate();
			}
		}

		/// <summary>
		/// 鼠标移动
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (isMouseDown == false)
				return;

			int currentX = 0, currentY = 0;
			if (e.Button == MouseButtons.Left)
			{
				currentX = e.X;
				currentY = e.Y;

				RotionY = (oldX - currentX) / 5 ;
				RotionX = (oldY - currentY) / 5 ;
				mScene.Camera.Pitch = RotionX;
				mScene.Camera.Yaw = RotionY;
				Matrix4x4 M = Matrix4x4.RotateX(mScene.Camera.Pitch) * Matrix4x4.RotateY(mScene.Camera.Yaw);
				mMatrix = mDevice.GetMvpMatrix(mScene.Camera, M);
				oldX = currentX;
				oldY = currentY;
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
				isMouseDown = true;
				oldX = e.X;
				oldY = e.Y;
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
				isMouseDown = false;
				oldX = 0;
				oldY = 0;
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

			Mesh cube = new Mesh("Cube", 8, 12);
			cube.Vertices = new Vertex[24] {
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

			cube.MakeFace();
			scene.AddMesh(cube);
		}

	}
}
