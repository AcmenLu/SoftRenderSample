using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SoftRenderSample
{
	public enum RenderMode
	{
		/// <summary>
		/// 线框模式
		/// </summary>
		WIREFRAME = 0,
		/// <summary>
		/// 顶点色
		/// </summary>
		VERTEXCOLOR,
		/// <summary>
		/// 单张纹理
		/// </summary>
		TEXTURED,
		/// <summary>
		/// 多张纹理
		/// </summary>
		CUBETEXTURED,
	}

	class Device
	{
		// 渲染缓冲相关
		private Bitmap mBmp;
		private BitmapData mBmData;
		private int mHeight;
		private int mWidth;
		private readonly float[] mDepthBuffer;

		private Camera mCamera;
		private Matrix4x4 mModel = new Matrix4x4(1);
		private Matrix4x4 mRotation = new Matrix4x4(1);
		private Matrix4x4 mView;
		// 裁剪
		private Clip mHodgmanclip;
		// 扫描线
		private ScanLine mScanline;

		// 裁剪区域定义
		public Vector4 clipmin = new Vector4(-1, -1, -1, 1);
		public Vector4 clipmax = new Vector4(1, 1, 1, 1);

		public RenderMode renderMode = RenderMode.WIREFRAME;
		public Matrix4x4 Mat = new Matrix4x4(1);
		public float ScaleV = 1.0f;

		public int Height
		{
			get { return this.mHeight; }
		}

		public int Width
		{
			get { return this.mWidth; }
		}

		public Device(Bitmap bmp)
		{
			this.mBmp = bmp;
			this.mHeight = bmp.Height;
			this.mWidth = bmp.Width;
			this.mDepthBuffer = new float[bmp.Width * bmp.Height];
			mScanline = new ScanLine(this);
		}

		/// <summary>
		/// 清除后台缓存
		/// </summary>
		/// <param name="data"></param>
		public void Clear(BitmapData data)
		{
			for (int index = 0; index < mDepthBuffer.Length; index++)
			{
				mDepthBuffer[index] = float.MaxValue;
			}
			unsafe
			{
				byte* ptr = (byte*)(data.Scan0);
				for (int i = 0; i < data.Height; i++)
				{
					for (int j = 0; j < data.Width; j++)
					{

						*ptr = 0;
						*(ptr + 1) = 0;
						*(ptr + 2) = 0;
						ptr += 3;
					}
					ptr += data.Stride - data.Width * 3;
				}
			}
		}

		/// <summary>
		/// 绘画某个位置的像素
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="color"></param>
		public void Putpixel(int x, int y, float z, Color color)
		{
			int index = (x + y * Width);
			if (mDepthBuffer[index] < z)
				return;
			mDepthBuffer[index] = z;
			unsafe
			{
				byte* ptr = (byte*)(this.mBmData.Scan0);
				byte* row = ptr + (y * this.mBmData.Stride);
				row[x * 3] = color.B;
				row[x * 3 + 1] = color.G;
				row[x * 3 + 2] = color.R;
			}
		}

		/// <summary>
		/// 透视投影.
		/// 过程由两步组成：(1)乘以投影矩阵 (2)透视除法
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="mvp"></param>
		/// <returns></returns>
		public Vector4 Projection(Vector4 vector, Matrix4x4 mvp)
		{
			Vector4 point = mvp.LeftApply(vector);
			Vector4 viewpoint = ProjectionDev(point);
			return viewpoint;
		}

		/// <summary>
		/// 变换到齐次坐标
		/// </summary>
		/// <param name="x"></param>
		/// <param name="M"></param>
		/// <returns></returns>
		public Vector4 ToHomogeneous(Vector4 x, Matrix4x4 M)
		{
			Vector4 val = M.LeftApply(x);
			float rh = 1.0f / val.W;
			val.X = val.X * rh;
			val.Y = val.Y * rh;
			val.Z = val.Z * rh;
			return val;
		}

		/// <summary>
		/// 透视除法
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		private Vector4 ProjectionDev(Vector4 x)
		{
			Vector4 val = new Vector4();
			float rhw = 1.0f / x.W;
			val.X = (1.0f + x.X * rhw) * Width * 0.5f;
			val.Y = (1.0f - x.Y * rhw) * Height * 0.5f;
			val.Z = x.Z * rhw;
			val.W = 1.0f;
			return val;
		}

		/// <summary>
		/// 画一个点
		/// </summary>
		/// <param name="point"></param>
		/// <param name="c"></param>
		public void DrawPoint(Vector4 point, Color c)
		{
			if (point.X >= 0 && point.Y >= 0 && point.X <= Width && point.Y <= Height)
			{
				if (point.X == Width)
					point.X = point.X - 1;
				if (point.Y == Height)
					point.Y = point.Y - 1;

				Putpixel((int)point.X, (int)point.Y, point.Z, c);
			}
		}

		/// <summary>
		/// 画一条直线
		/// </summary>
		/// <param name="point0"></param>
		/// <param name="point1"></param>
		/// <param name="scene"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		public void DrawLine(Vector4 point0, Vector4 point1, Scene scene, Vertex v1, Vertex v2)
		{
			int x0 = (int)point0.X;
			int y0 = (int)point0.Y;
			int x1 = (int)point1.X;
			int y1 = (int)point1.Y;

			int dx = x1 - x0;
			int dy = y1 - y0;
			int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));
			if (steps == 0)
				return;

			float offsetX = (float)dx / (float)steps;
			float offsetY = (float)dy / (float)steps;

			float x = x0;
			float y = y0;
			Light light = scene.Lights;
			float dot1 = 0;
			float dot2 = 0;
			if (scene.IsUseLight != false || light != null)
			{
				dot1 = light.ComputeNormalDot(v1.Position, v1.Normal);
				dot2 = light.ComputeNormalDot(v2.Position, v2.Normal);
			}
			float dot = 0;
			Color vColor = new Color(128, 128, 128);
			for (int i = 1; i <= steps; i++)
			{
				float dt = (float)(i) / (float)steps;
				if (!scene.IsUseLight || light == null)
				{
					vColor = MathUntily.Lerp(v1.color, v2.color, dt);
				}
				else
				{
					dot = MathUntily.Lerp(dot1, dot2, dt);
					vColor = MathUntily.Lerp(v1.color, v2.color, dt) * light.LightColorV(dot);
				}

				float z = MathUntily.Lerp(point0.Z, point1.Z, dt);
				if (float.IsNaN(z))
					return;

				DrawPoint(new Vector4((int)x, (int)y, z, 0), vColor);
				x += offsetX;
				y += offsetY;
			}
		}

		/// <summary>
		/// 设备渲染事件
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="bmp"></param>
		/// <param name="viewMatrix"></param>
		public void Render(Scene scene, BitmapData bmp, Matrix4x4 viewMatrix)
		{
			this.mBmData = bmp;
			foreach (Mesh msh in scene.Meshs)
			{
				foreach (var faces in msh.faces)
				{
					Vertex verA = msh.Vertices[faces.A];
					Vertex verB = msh.Vertices[faces.B];
					Vertex verC = msh.Vertices[faces.C];

					Vertex verA2 = new Vertex();
					Vertex verB2 = new Vertex();
					Vertex verC2 = new Vertex();

					// 转换到齐次坐标
					verA.ClipPosition = this.ToHomogeneous(verA.Position, viewMatrix);
					verB.ClipPosition = this.ToHomogeneous(verB.Position, viewMatrix);
					verC.ClipPosition = this.ToHomogeneous(verC.Position, viewMatrix);

					// 光照和顶点法线之间的夹角的余弦值
					float cosa = 0;
					float cosb = 0;
					float cosc = 0;
					if (scene.IsUseLight || scene.Lights != null)
					{
						cosa = scene.Lights.ComputeNormalDot(this.mModel.LeftApply(verA.Position), this.mModel.LeftApply(verA.Normal));
						cosb = scene.Lights.ComputeNormalDot(this.mModel.LeftApply(verB.Position), this.mModel.LeftApply(verB.Normal));
						cosc = scene.Lights.ComputeNormalDot(this.mModel.LeftApply(verC.Position), this.mModel.LeftApply(verC.Normal));
					}

					verA2.color = verA.color;
					verB2.color = verB.color;
					verC2.color = verC.color;

					//对应屏幕坐标 左上角
					verA.ScreenPosition = this.ViewPort(verA.ClipPosition);
					verB.ScreenPosition = this.ViewPort(verB.ClipPosition);
					verC.ScreenPosition = this.ViewPort(verC.ClipPosition);

					verA2.ClipPosition = verA.ClipPosition;
					verA2.ScreenPosition = verA.ScreenPosition;
					verA2.Position = this.mModel.LeftApply(verA.Position);
					verA2.UV = verA.UV;
					verA2.ColK = cosa;

					verB2.ClipPosition = verB.ClipPosition;
					verB2.ScreenPosition = verB.ScreenPosition;
					verB2.Position = this.mModel.LeftApply(verB.Position);
					verB2.UV = verB.UV;
					verB2.ColK = cosb;

					verC2.ClipPosition = verC.ClipPosition;
					verC2.ScreenPosition = verC.ScreenPosition;
					verC2.Position = this.mModel.LeftApply(verC.Position);
					verC2.UV = verC.UV;
					verC2.ColK = cosc;

					if (scene.IsUseLight)
					{
						verA2.Normal = verA.Normal;
						verB2.Normal = verB.Normal;
						verC2.Normal = verC.Normal;
					}
					else
					{
						verA2.Normal = this.mModel.LeftApply(verA.Normal);
						verB2.Normal = this.mModel.LeftApply(verB.Normal);
						verC2.Normal = this.mModel.LeftApply(verC.Normal);
					}

					List<Vertex> list = new List<Vertex>();
					list.Add(verA2);
					list.Add(verB2);
					list.Add(verC2);
					Triangle triang1 = new Triangle(verA2, verB2, verC2);
					//进行裁剪
					List<Vertex> triangleVertex = new List<Vertex>();
					//放在构造函数中初始化引起list 集合累加

					for (FaceTypes face = FaceTypes.LEFT; face <= FaceTypes.FAR; face++)
					{
						if (list.Count == 0) break;
						mHodgmanclip = new Clip(this);
						mHodgmanclip.HodgmanPolygonClip(face, clipmin, clipmax, list.ToArray());
						list = mHodgmanclip.OutputList;
					}

					List<Triangle> tringleList = GetDrawTriangleList(list);
					if (renderMode == RenderMode.WIREFRAME)
					{
						for (int i = 0; i < tringleList.Count; i++)
						{
							if (!IsInBack(tringleList[i]))
							{
								DrawLine(this.ViewPort(tringleList[i].vertices[0].ClipPosition), this.ViewPort(tringleList[i].vertices[1].ClipPosition), scene, tringleList[i].vertices[0], tringleList[i].vertices[1]);
								DrawLine(this.ViewPort(tringleList[i].vertices[1].ClipPosition), this.ViewPort(tringleList[i].vertices[2].ClipPosition), scene, tringleList[i].vertices[1], tringleList[i].vertices[2]);
								DrawLine(this.ViewPort(tringleList[i].vertices[2].ClipPosition), this.ViewPort(tringleList[i].vertices[0].ClipPosition), scene, tringleList[i].vertices[2], tringleList[i].vertices[0]);
							}
						}
					}
					else
					{
						for (int i = 0; i < tringleList.Count; i++)
						{
							if (!this.IsInBack(tringleList[i]))
								this.mScanline.ProcessScanLine(tringleList[i], scene, triang1, faces.FaceType);
						}
					}
				}
			}
		}

		/// <summary>
		/// 获取要画的三角形列表
		/// </summary>
		/// <param name="vertex"></param>
		/// <returns></returns>
		public List<Triangle> GetDrawTriangleList(List<Vertex> vertex)
		{
			List<Triangle> t = new List<Triangle>();
			for (int i = 0; i < vertex.Count - 2; i++)
			{
				t.Add(new Triangle(vertex[0], vertex[i + 1], vertex[i + 2]));
			}
			return t;
		}

		/// <summary>
		/// 转换为屏幕坐标显示
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public Vector4 ViewPort(Vector4 x)
		{
			Vector4 val = new Vector4();
			val.X = (1.0f + x.X) * Width * 0.5f;
			val.Y = (1.0f - x.Y) * Height * 0.5f;
			val.Z = x.Z;
			val.W = 1.0f;
			return val;
		}

		/// <summary>
		/// 判断三角形的法向量z值是否大于0, 如果大于0，则在背面
		/// </summary>
		/// <param name="tri"></param>
		/// <returns></returns>
		public bool IsInBack(Triangle tri)
		{
			Vector4 v1 = tri.vertices[0].ScreenPosition;
			Vector4 v2 = tri.vertices[1].ScreenPosition;
			Vector4 v3 = tri.vertices[2].ScreenPosition;
			v1.Z = v2.Z = v3.Z = 0;
			Vector4 v1v2 = v2 - v1;
			Vector4 v1v3 = v3 - v1;
			return Vector4.Cross(v1v2, v1v3).Z > 0;
		}

		/// <summary>
		/// 获取某一点的颜色
		/// </summary>
		/// <param name="u"></param>
		/// <param name="v"></param>
		/// <param name="texture"></param>
		/// <returns></returns>
		public Color GetPixelColor(float u, float v, TextureMap texture)
		{
			int x = Math.Abs((int)((1f - u) * texture.GetWidth()) % texture.GetWidth());
			int y = Math.Abs((int)((1f - v) * texture.GetHeight()) % texture.GetHeight());

			byte red = 0;
			byte green = 0;
			byte blue = 0;
			unsafe
			{
				byte* ptr = (byte*)(texture.data.Scan0);
				byte* row = ptr + (y * texture.data.Stride);

				red = row[x * 3 + 2];
				green = row[x * 3 + 1];
				blue = row[x * 3];
			}

			return new Color(red, green, blue);
		}

		/// <summary>
		/// 获取mvp
		/// </summary>
		/// <param name="mCamera"></param>
		/// <param name="Rox"></param>
		/// <param name="Roy"></param>
		/// <returns></returns>
		public Matrix4x4 GetMvpMatrix(Camera camera, int Rox, int Roy)
		{
			this.mCamera = camera;
			Matrix4x4 translate = new Matrix4x4(1);
			translate.TranslateM(0, 0, 0);
			Matrix4x4 scale = new Matrix4x4(1);
			scale.ScaleM(ScaleV, ScaleV, ScaleV);
			mRotation *= Matrix4x4.RotateY(Roy) * Matrix4x4.RotateX(Rox);
			mModel = scale * mRotation * translate;

			Matrix4x4 view = this.mCamera.GetLookAt();
			Matrix4x4 projection = this.mCamera.GetProject((float)System.Math.PI * 0.3f, (float)mWidth / (float)mHeight, 1f, 100.0f);
			return mModel * view * projection;
		}

		/// <summary>
		/// 世界矩阵
		/// </summary>
		/// <returns></returns>
		public Matrix4x4 GetWorldMatrix()
		{
			Matrix4x4 translate = new Matrix4x4(1);
			translate.TranslateM(0, 0, 0);
			Matrix4x4 scale = new Matrix4x4(1);
			scale *= scale.ScaleM(ScaleV, ScaleV, ScaleV);
			mModel = scale * mRotation * translate;
			return mModel;
		}

		/// <summary>
		/// 获取mvp举证
		/// </summary>
		/// <param name="mCamera"></param>
		/// <param name="M"></param>
		/// <returns></returns>
		public Matrix4x4 GetMvpMatrix(Camera mCamera, Matrix4x4 M)
		{
			this.mCamera = mCamera;
			Mat *= M;
			mView = Mat * this.mCamera.GetLookAt();
			Matrix4x4 projection = this.mCamera.GetProject((float)System.Math.PI * 0.3f, (float)mWidth / (float)mHeight, 1f, 100.0f);
			return GetWorldMatrix() * mView * projection;
		}
	}
}
