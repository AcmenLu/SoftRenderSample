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
		private RenderTexture[] mCubeTextures;

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

		public RenderTexture[] CubeTexture
		{
			get { return mCubeTextures; }
			set { mCubeTextures = value; }
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

			// 每三个顶点组成一个三角形进行渲染。
			for (int i = 0; i + 2 < mVertices.Count(); i += 3)
				DrawTriangle(renderer, mVertices[i], mVertices[i + 1], mVertices[i + 2]);
		}

		/// <summary>
		/// 根据顶点信息将光照信息填充到顶点
		/// </summary>
		/// <param name="vertex"></param>
		/// <returns></returns>
		public void UseLight(Renderer renderer, ref Vertex vertex)
		{
			// 光照计算：vertex color = ambient（环境光颜色） + diffuse（漫反射颜色） + specular（镜面光颜色） + emission(自发光)
			// 如果模型没有材质或者环境中没有光源，则不受光照影响
			if (mMaterial == null)
				return;

			if (renderer.LightList.Count() <= 0)
				return;

			// 暂时只支持第一个光源， TODO：支持多光源
			Light light = renderer.LightList[0];
			// 1. 将顶点和法线转换到世界空间中来
			Vector3D position = vertex.Position * mTransform;
			Vector3D normal = vertex.Normal * mTransform;
			Vector3D dir = light.Position - vertex.Position;
			dir.Normalize();

			float diffuse = Vector3D.Dot(normal, dir);
			if (diffuse < 0)
				diffuse = 0;

			Color diffuseColor = mMaterial.Diffuse * diffuse * light.Color; // 漫反射
			Color emission = mMaterial.Emissive;
			Color ambient = emission * mMaterial.KA;

			vertex.LightColor = ambient + emission + diffuseColor;
		}

		/// <summary>
		/// 对一个定点进行mvp变换。
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="vertex"></param>
		public void TransformToView(Renderer renderer, ref Vertex vertex)
		{
			vertex.Position = vertex.Position * mTransform * renderer.Camera.GetViewMat();
		}

		/// <summary>
		/// 对一个定点进行mvp变换。
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="vertex"></param>
		public void TransformToProjection(Renderer renderer, ref Vertex vertex)
		{
			vertex.Position = vertex.Position * renderer.Projection;
		}

		/// <summary>
		/// 转换到屏幕坐标系
		/// </summary>
		/// <param name="v"></param>
		private void TransformToScreen(Renderer renderer, ref Vertex v)
		{
			if (v.Position.w != 0)
			{
				//先进行透视除法，转到cvv
				v.Position.x *= 1 / v.Position.w;
				v.Position.y *= 1 / v.Position.w;
				v.Position.z *= 1 / v.Position.w;
				v.Position.w = 1;
				//cvv到屏幕坐标
				v.Position.x = (v.Position.x + 1) * 0.5f * renderer.Size.x;
				v.Position.y = (1 - v.Position.y) * 0.5f * renderer.Size.y;
			}
		}

		/// <summary>
		/// 摄像机空间的裁剪
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <returns></returns>
		private bool CameraBackCulling(Renderer renderer, Vertex p1, Vertex p2, Vertex p3)
		{
			if ((renderer.CullMode & CullMode.CULL_CAMERA) == 0)
				return true;

			// 裁剪原理：计算出这个面的法线， 然后判断法线和摄像机朝向的夹角，如果夹角是[0, 90)则需要被裁掉
			Vector3D v1 = p2.Position - p1.Position;
			Vector3D v2 = p3.Position - p2.Position;
			Vector3D normal = Vector3D.Cross(v1, v2);
			Vector3D viewDir = p1.Position - new Vector3D(0, 0, 0);
			if (Vector3D.Dot(normal, viewDir) > 0)
				return true;

			return false;
		}

		/// <summary>
		/// 检查是否裁剪这个顶点,简单的cvv裁剪,在透视除法之前
		/// </summary>
		/// <returns>是否通关剪裁</returns>
		private bool VertexClip(Renderer renderer, Vertex v)
		{
			if ((renderer.CullMode & CullMode.CULL_CVV) == 0)
				return true;

			// 经过齐次变换后x取值在[-1, 1], y取值在[-1, 1],z取值在[0, 1]
			// 齐次变换为 x = x / w, y = y / w, z = z / w
			// 所以-1 <= x / w <= 1, -1 <= y / w <= 1, 0 <= z / w <= 1
			float x = v.Position.x / v.Position.w;
			float y = v.Position.y / v.Position.w;
			float z = v.Position.z / v.Position.w;
			if (-1 <= x && x <= 1 && -1 <= y && y <= 1 && 0 <= z && z <= 1)
				return true;

			return false;
		}

		/// <summary>
		/// 绘制一个三角形
		/// </summary>
		public void DrawTriangle(Renderer renderer, Vertex v1, Vertex v2, Vertex v3)
		{
			Vertex p1 = new Vertex(v1);
			Vertex p2 = new Vertex(v2);
			Vertex p3 = new Vertex(v3);

			if(renderer.RenderMode == RenderMode.CUBETEXTURED && mCubeTextures != null)
			{
				if (mCubeTextures.Length != 6)
					return;
				// 前面
				if(p1.Normal.IsEqual(new Vector3D(0, 0, -1)))
					mTexture = mCubeTextures[0];
				// 后面
				else if (p1.Normal.IsEqual(new Vector3D(0, 0, 1)))
					mTexture = mCubeTextures[1];
				// 左面
				else if (p1.Normal.IsEqual(new Vector3D(-1, 0, 0)))
					mTexture = mCubeTextures[2];
				// 右面
				else if (p1.Normal.IsEqual(new Vector3D(1, 0, 0)))
					mTexture = mCubeTextures[3];
				//上面
				else if (p1.Normal.IsEqual(new Vector3D(0, 1, 0)))
					mTexture = mCubeTextures[4];
				// 下面
				else if (p1.Normal.IsEqual(new Vector3D(0, -1, 0)))
					mTexture = mCubeTextures[5];
			}

			//计算顶点光照颜色
			if (renderer.LightList.Count() > 0)
			{
				UseLight(renderer, ref p1);
				UseLight(renderer, ref p2);
				UseLight(renderer, ref p3);
			}

			// 因为要进行摄像机背面剔除，因此将mvp 分开为mv和p
			TransformToView(renderer, ref p1);
			TransformToView(renderer, ref p2);
			TransformToView(renderer, ref p3);
			// 摄像机背面剔除
			if (CameraBackCulling(renderer, p1, p2, p3) == false)
				return;

			// 透视投影
			TransformToProjection(renderer, ref p1);
			TransformToProjection(renderer, ref p2);
			TransformToProjection(renderer, ref p3);

			// 齐次裁剪(范围裁剪)
			if (VertexClip(renderer, p1) == false || VertexClip(renderer, p2) == false || VertexClip(renderer, p3) == false)
				return;

			TransformToScreen(renderer, ref p1);
			TransformToScreen(renderer, ref p2);
			TransformToScreen(renderer, ref p3);

			if (renderer.RenderMode == RenderMode.WIREFRAME)
			{
				DrawLine(renderer, p1, p2);
				DrawLine(renderer, p2, p3);
				DrawLine(renderer, p3, p1);
			}
			else
			{
				TriangleRasterization(renderer, p1, p2, p3);
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
					float t = dx == 0 ? 0 : i / (float)dx;
					Color vcolor = Color.Lerp(p1.Color, p2.Color, t);
					float depth = MathUntil.Lerp(p1.Position.z, p2.Position.z, t);
					renderer.FrameBuffer.SetPointColor(x, y, vcolor, depth);
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
					float t = dy == 0 ? 0 : i / (float)dy;
					Color vcolor = Color.Lerp(p1.Color, p2.Color, t);
					float depth = MathUntil.Lerp(p1.Position.z, p2.Position.z, t);
					renderer.FrameBuffer.SetPointColor(x, y, vcolor, depth);
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
				Vertex.LerpColor(ref newMiddle, top, bottom, t);

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

					Vertex new1 = new Vertex();
					new1.Position.x = xl;
					new1.Position.y = y;
					Vertex.LerpColor(ref new1, p1, p3, t);
					//
					Vertex new2 = new Vertex();
					new2.Position.x = xr;
					new2.Position.y = y;
					Vertex.LerpColor(ref new2, p2, p3, t);
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
					Vertex new1 = new Vertex();
					new1.Position.x = xLeft;
					new1.Position.y = y;
					Vertex.LerpColor(ref new1, p1, p2, t);

					Vertex new2 = new Vertex();
					new2.Position.x = xRight;
					new2.Position.y = y;
					Vertex.LerpColor(ref new2, p1, p3, t);

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

					float t = 0;
					if (right.Position.x - left.Position.x > 0)
						t = (x - left.Position.x) / (right.Position.x - left.Position.x);

					Color lightColor = Color.Lerp(left.LightColor, right.LightColor, t);
					//根据渲染模式选择使用顶点色还是使用图片的颜色。
					if (renderer.RenderMode == RenderMode.VERTEXCOLOR)
					{
						vcolor = Color.Lerp(left.Color, right.Color, t);
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
						else
						{
							vcolor = Color.Red;
						}
					}
					vcolor = vcolor * lightColor;
					float depth = MathUntil.Lerp(left.Position.z, right.Position.z, t);
					renderer.FrameBuffer.SetPointColor(xIndex, yIndex, vcolor, depth);
				}
			}
		}

	}
}