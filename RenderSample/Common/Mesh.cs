using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleCommon
{
	public class Mesh
	{
		private List<Vertex> mVertices;
		private Matrix4X4 mTransform;
		private Material mMaterial;
		private RenderTexture mTexture;

		public List<Vertex> Vertices
		{
			get { return mVertices; }
			set { mVertices = value; }
		}

		public Matrix4X4 Transform
		{
			get { return mTransform; }
			set { mTransform = new Matrix4X4(value); }
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

		/// <summary>
		/// 构造
		/// </summary>
		public Mesh()
		{
			mVertices = new List<Vertex>();
			mTransform = new Matrix4X4();
		}

		/// <summary>
		/// 构造
		/// </summary>
		public Mesh(List<Vertex> vertices)
		{
			mVertices = vertices;
			mTransform = new Matrix4X4();
		}

		/// <summary>
		/// 渲染事件
		/// </summary>
		/// <param name="renderer"></param>
		public void OnRender(Renderer renderer)
		{
			if (mVertices.Count() <= 0)
				return;

			for (int i = 0; i + 2 < mVertices.Count(); i += 3)
				DrawTriangle(renderer, mVertices[i], mVertices[i + 1], mVertices[i + 2]);

			//for (int i = 0; i < mVertices.Count(); i++)
			//	DrawPosition(renderer, mVertices[i]);
		}

		/// <summary>
		/// 对一个定点进行mvp变换。
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="vertex"></param>
		public void VertexTransform(Renderer renderer, ref Vertex vertex)
		{
			Matrix4X4 mat = renderer.Camera.GetViewMat().Identity();
			mTransform = Matrix4X4.Translate(new Vector3(0.0f, 0.0f, 10.0f));
			vertex.Position = vertex.Position * mTransform * mat * renderer.Projection;
		}

		/// <summary>
		/// 绘制一个三角形
		/// </summary>
		public void DrawTriangle(Renderer renderer, Vertex v1, Vertex v2, Vertex v3)
		{
			Vertex p1 = new Vertex(v1);
			Vertex p2 = new Vertex(v2);
			Vertex p3 = new Vertex(v3);
			VertexTransform(renderer, ref p1);
			VertexTransform(renderer, ref p2);
			VertexTransform(renderer, ref p3);
			if (renderer.RenderMode == RenderMode.Wireframe)
			{
				DrawLine(renderer, p1, v2);
				DrawLine(renderer, v2, v3);
				DrawLine(renderer, v3, v1);
			}
			else
			{
				TriangleRasterization(renderer, v1, v2, v3);
			}
		}

		/// <summary>
		/// 绘制直线，使用bresenham算法画出两点之间的连线
		/// </summary>
		/// <param name="Position1"></param>
		/// <param name="Position2"></param>
		public void DrawLine(Renderer renderer, Vertex p1, Vertex p2)
		{
			int x = (int)Math.Round(p1.Position.x, 0);
			int y = (int)Math.Round(p1.Position.y, 0);
			int dx = (int)Math.Round(p2.Position.x - p1.Position.x, 0);
			int dy = (int)Math.Round(p2.Position.y - p1.Position.y, 0);
			int stepx = dx > 0 ? 1 : -1;
			int stepy = dy > 0 ? 1 : -1;
			dx = Math.Abs(dx);
			dy = Math.Abs(dy);

			int dx2 = 2 * dx;
			int dy2 = 2 * dy;

			// 斜率小于1
			if (dx > dy)
			{
				int error = dy2 - dx;
				for (int i = 0; i <= dx; i++)
				{
					renderer.FrameBuffer.SetPointColor(x, y, Color.Red);
					if (error >= 0)
					{
						error -= dx2;
						y += stepy;
					}
					error += dy2;
					x += stepx;
				}
			}
			else
			{
				int error = dx2 - dy;
				for (int i = 0; i <= dy; i++)
				{
					renderer.FrameBuffer.SetPointColor(x, y, Color.Red);
					if (error >= 0)
					{
						error -= dy2;
						x += stepx;
					}
					error += dx2;
					y += stepy;
				}
			}
		}

		/// <summary>
		/// 绘制一个点
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="p"></param>
		public void DrawPosition(Renderer renderer, Vertex p)
		{
			//VertexTransform(renderer, ref p);
			renderer.FrameBuffer.SetPointColor((int)p.Position.x, (int)p.Position.y, Color.Red);
		}

		/// <summary>
		/// 光栅化三角形
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		public void TriangleRasterization(Renderer renderer, Vertex p1, Vertex p2, Vertex p3)
		{
			//平顶三角形：就是在计算机中显示的上面两个顶点的Y坐标相同。
			//平底三角形：就是在计算机中显示的时候下面两个顶点的Y坐标相同。
			//右边为主三角形：这种三角形三个点的Y坐标都不相同，但是右边的一条边是最长的斜边
			//左边为主的三角形：这种三角形的三个点的Y坐标不相同，但是左边的一条边是最长的斜边。
			if (p1.Position.y == p2.Position.y)
			{
				if (p1.Position.y < p3.Position.y)
				{
					DrawTopTriangle(renderer, p1, p2, p3);
				}
				else
				{
					DrawBottomTriangle(renderer, p3, p1, p2);
				}
			}
			else if (p1.Position.y == p3.Position.y)
			{
				if (p1.Position.y < p2.Position.y)
				{
					DrawTopTriangle(renderer, p1, p3, p2);
				}
				else
				{
					DrawBottomTriangle(renderer, p2, p1, p3);
				}
			}
			else if (p2.Position.y == p3.Position.y)
			{
				if (p2.Position.y < p1.Position.y)
				{
					DrawTopTriangle(renderer, p2, p3, p1);
				}
				else
				{
					DrawBottomTriangle(renderer, p1, p2, p3);
				}
			}
			else
			{
				// 对非平顶和平底三角形进行分割,分割后是一个平顶三角形和一个平底三角形
				Vertex top; // Y值最大的顶点
				Vertex bottom; // Y值最小的顶点
				Vertex middle; // Y值在中间的顶点
				if (p1.Position.y > p2.Position.y && p2.Position.y > p3.Position.y)
				{
					top = p3;
					middle = p2;
					bottom = p1;
				}
				else if (p3.Position.y > p2.Position.y && p2.Position.y > p1.Position.y)
				{
					top = p1;
					middle = p2;
					bottom = p3;
				}
				else if (p2.Position.y > p1.Position.y && p1.Position.y > p3.Position.y)
				{
					top = p3;
					middle = p1;
					bottom = p2;
				}
				else if (p3.Position.y > p1.Position.y && p1.Position.y > p2.Position.y)
				{
					top = p2;
					middle = p1;
					bottom = p3;
				}
				else if (p1.Position.y > p3.Position.y && p3.Position.y > p2.Position.y)
				{
					top = p2;
					middle = p3;
					bottom = p1;
				}
				else if (p2.Position.y > p3.Position.y && p3.Position.y > p1.Position.y)
				{
					top = p1;
					middle = p3;
					bottom = p2;
				}
				else
				{
					// 三点共线则不需要填充
					return;
				}
				//插值求中间点x
				float middlex = (middle.Position.y - top.Position.y) * (bottom.Position.x - top.Position.x) / (bottom.Position.y - top.Position.y) + top.Position.x;
				Vertex newMiddle = new Vertex();
				newMiddle.Position.x = middlex;
				newMiddle.Position.y = middle.Position.y;

				float dy = middle.Position.y - top.Position.y;
				float t = dy / (bottom.Position.y - top.Position.y);
				MathUntil.LerpVertexInScreen(ref newMiddle, top, bottom, t);

				DrawBottomTriangle(renderer, top, newMiddle, middle);
				DrawTopTriangle(renderer, newMiddle, middle, bottom);
			}
		}

		/// <summary>
		/// 绘制平顶三角形
		/// </summary>
		/// <param name="renderer">全局渲染器</param>
		/// <param name="p1">平顶三角形的顶点</param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		private void DrawTopTriangle(Renderer renderer, Vertex p1, Vertex p2, Vertex p3)
		{
			for (float y = p1.Position.y; y <= p3.Position.y; y += 0.5f)
			{
				int yIndex = (int)(System.Math.Round(y, MidpointRounding.AwayFromZero));
				if (yIndex >= 0 && yIndex < renderer.Size.x)
				{
					float xl = (y - p1.Position.y) * (p3.Position.x - p1.Position.x) / (p3.Position.y - p1.Position.y) + p1.Position.x;
					float xr = (y - p2.Position.y) * (p3.Position.x - p2.Position.x) / (p3.Position.y - p2.Position.y) + p2.Position.x;

					float dy = y - p1.Position.y;
					float t = dy / (p3.Position.y - p1.Position.y);
					//插值生成左右顶点
					Vertex new1 = new Vertex();
					new1.Position.x = xl;
					new1.Position.y = y;
					MathUntil.LerpVertexInScreen(ref new1, p1, p3, t);
					//
					Vertex new2 = new Vertex();
					new2.Position.x = xr;
					new2.Position.y = y;
					MathUntil.LerpVertexInScreen(ref new2, p2, p3, t);
					//扫描线填充
					if (new1.Position.x < new2.Position.x)
						ScanlineFill(renderer, new1, new2, yIndex);
					else
						ScanlineFill(renderer, new2, new1, yIndex);
				}
			}
		}

		/// <summary>
		/// 绘制平底三角形
		/// </summary>
		/// <param name="renderer">全局渲染器</param>
		/// <param name="p1">平底三角形的上顶点</param>
		/// <param name="p2">平底三角形的下面第一个顶点</param>
		/// <param name="p3">平底三角形的下面第二个顶点</param>
		private void DrawBottomTriangle(Renderer renderer, Vertex p1, Vertex p2, Vertex p3)
		{
			for (float y = p1.Position.y; y <= p2.Position.y; y += 0.5f)
			{
				int yIndex = (int)Math.Round(y, 0);
				if (yIndex >= 0 && yIndex < renderer.Size.y)
				{
					float xLeft = (y - p1.Position.y) * (p2.Position.x - p1.Position.x) / (p2.Position.y - p1.Position.y) + p1.Position.x;
					float xRight = (y - p1.Position.y) * (p3.Position.x - p1.Position.x) / (p3.Position.y - p1.Position.y) + p1.Position.x;

					float dy = y - p1.Position.y;
					float t = dy / (p2.Position.y - p1.Position.y);
					//插值生成左右顶点
					Vertex new1 = new Vertex();
					new1.Position.x = xLeft;
					new1.Position.y = y;
					MathUntil.LerpVertexInScreen(ref new1, p1, p2, t);

					Vertex new2 = new Vertex();
					new2.Position.x = xRight;
					new2.Position.y = y;
					MathUntil.LerpVertexInScreen(ref new2, p1, p3, t);

					//扫描行进行填充
					if (new1.Position.x < new2.Position.x)
						ScanlineFill(renderer, new1, new2, yIndex);
					else
						ScanlineFill(renderer, new2, new1, yIndex);
				}
			}
		}

		/// <summary>
		/// 扫描填充三角形
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="yIndex"></param>
		private void ScanlineFill(Renderer renderer, Vertex left, Vertex right, int yIndex)
		{
			float dx = right.Position.x - left.Position.x;
			float step = dx == 0 ? 1 : 1 / dx;

			for (float x = left.Position.x; x <= right.Position.x; x += 0.5f)
			{
				int xIndex = (int)Math.Round(x, 0);
				if (xIndex >= 0 && xIndex < renderer.Size.y)
				{
					Color vcolor = new Color();

					//根据渲染模式选择使用顶点色还是使用图片的颜色。
					if (renderer.RenderMode == RenderMode.VertexColor)
					{
						float t = 0;
						if (right.Position.x - left.Position.x > 0)
							t = (x - left.Position.x) / (right.Position.x - left.Position.x);

						vcolor = MathUntil.Lerp(left.Color, right.Color, t);
					}
					else
					{
						if (mTexture != null)
						{
							float lerpFactor = 0;
							if (dx != 0)
								lerpFactor = (x - left.Position.x) / dx;

							float u = MathUntil.Lerp(left.TexCoord.x, right.TexCoord.x, lerpFactor);
							float v = MathUntil.Lerp(left.TexCoord.y, right.TexCoord.y, lerpFactor);
							vcolor = mTexture.GetPixelColor(u, v);
						}
					}

					// 根据环境中是否有光将光照颜色加入到顶点上。
					if (renderer.LightList.Count() > 0)
					{
						Color lightColor = renderer.LightList[0].Color;
						vcolor = vcolor * lightColor;
					}
					renderer.FrameBuffer.SetPointColor(xIndex, yIndex, vcolor);
				}
			}
		}

	}
}