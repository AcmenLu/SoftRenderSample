using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public enum FaceType
	{
		NONE = 0,
		// x
		LEFT = 1,
		RIGHT = 2,
		// y
		TOP = 4,
		BUTTOM = 8,
		// z
		NEAR = 16,
		FAR = 32,
	}

	/// <summary>
	/// 三角形类
	/// </summary>
	public class Triangle
	{
		private Vertex mVertex0;
		private Vertex mVertex1;
		private Vertex mVertex2;
		private Material mMaterial;
		private RenderTexture mTexture;
		private List<Triangle> mRenderLst = new List<Triangle>();

		public Vertex Vertex0
		{
			get { return mVertex0; }
		}

		public Vertex Vertex1
		{
			get { return mVertex1; }
		}

		public Vertex Vertex2
		{
			get { return mVertex2; }
		}

		public Triangle(Vertex v1, Vertex v2, Vertex v3)
		{
			mVertex0 = v1;
			mVertex1 = v2;
			mVertex2 = v3;
		}

		/// <summary>
		/// 返回三角形面的法线
		/// </summary>
		/// <returns></returns>
		public Vector3D GetNormal()
		{
			return mVertex0.Normal;
		}

		/// <summary>
		/// 根据顶点信息将光照信息填充到顶点
		/// </summary>
		/// <param name="vertex"></param>
		/// <returns></returns>
		public void UseLight(Renderer renderer, Matrix4X4 transform, ref Vertex vertex)
		{
			// 光照计算：vertex color = ambient（环境光颜色） + diffuse（漫反射颜色） + specular（镜面光颜色） + emission(自发光)
			// 如果模型没有材质或者环境中没有光源，则不受光照影响
			if (mMaterial == null)
				return;

			if (renderer.LightList.Count() <= 0)
				return;

			Light light = renderer.LightList[0];
			// 环境光
			Color ambient = light.Color * mMaterial.AmbientStregth;
			// 漫反射
			Vector3D normal = vertex.Normal * transform;
			Vector3D lightdir = (light.Position - vertex.Position).Normalize();
			float diff = Math.Max(Vector3D.Dot(normal.Normalize(), lightdir), 0);
			Color diffuse = mMaterial.Diffuse * diff;

			vertex.LightColor = ambient + diffuse;
		}

		/// <summary>
		/// 对一个定点进行mvp变换。
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="vertex"></param>
		public void TransformToView(Renderer renderer, Matrix4X4 transform, ref Vertex vertex)
		{
			vertex.Position = vertex.Position * transform * renderer.Camera.GetViewMat();
		}

		/// <summary>
		/// 对一个定点进行mvp变换。
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="vertex"></param>
		public void TransformToProjection(Renderer renderer, ref Vertex vertex)
		{
			vertex.Position = vertex.Position * renderer.Camera.PerspectiveMatrix;
		}

		/// <summary>
		/// 规范视椎体
		/// </summary>
		/// <param name="v"></param>
		private void ConvertToView(ref Vertex v)
		{
			if (v.Position.w != 0)
			{
				v.Position.x *= 1 / v.Position.w;
				v.Position.y *= 1 / v.Position.w;
				v.Position.z *= 1 / v.Position.w;
				v.Position.w = 1;
			}
		}

		/// <summary>
		/// 转换到屏幕坐标系
		/// </summary>
		/// <param name="v"></param>
		private void TransformToScreen(Renderer renderer, ref Vertex v)
		{
			v.Position = v.Position * renderer.ViewPort;
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
		/// 计算线段穿过的面
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="faces"></param>
		public void LinePunctureFaces(Vertex p1, Vertex p2, ref List<FaceType> faces)
		{
			//如果一个端点的某位为0而另一个端点的相同位为1
			//则该线段穿过相应的裁剪边界
			int a = (int)p1.AreaCode ^ (int)p2.AreaCode;
			byte p = 1;
			while (a != 0)
			{
				if ((a & 1) == 1)
				{
					if (faces.Exists(x=>x == (FaceType)p) == false)
						faces.Add((FaceType)p);
				}
				p *= 2;
				a >>= 1;
			}
		}
	
		/// <summary>
		/// 计算三角形穿过的面
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <param name="faces"></param>
		private void TriPunctureFaces(Vertex p1, Vertex p2, Vertex p3, ref List<FaceType> faces)
		{
			LinePunctureFaces(p1, p2, ref faces);
			LinePunctureFaces(p2, p3, ref faces);
			LinePunctureFaces(p3, p1, ref faces);
		}

		/// <summary>
		/// 交换两个顶点
		/// </summary>
		/// <param name="v0"></param>
		/// <param name="v1"></param>
		private void SwapVertex(ref Vertex v0, ref Vertex v1)
		{
			Vertex temp = v0;
			v0 = v1;
			v1 = temp;
		}

		/// <summary>
		/// 将三角形分成一些小的三角形
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <param name="faceType"></param>
		/// <param name="triangles"></param>
		private void VertexClip(Vertex point1, Vertex point2, Vertex point3, ref List<Triangle> triangles)
		{
			if (point1 == null || point2 == null || point3 == null)
				return;

			Vertex p1 = new Vertex(point1);
			Vertex p2 = new Vertex(point2);
			Vertex p3 = new Vertex(point3);
			p1.CallAreaCode();
			p2.CallAreaCode();
			p3.CallAreaCode();

			// 全部在视野之内
			if (p1.AreaCode == (byte)FaceType.NONE && p2.AreaCode == (byte)FaceType.NONE && p3.AreaCode == (byte)FaceType.NONE)
			{
				triangles.Add(new Triangle(p1, p2, p3));
				return;
			}
			// 显然不可见
			else if ((p1.AreaCode & p2.AreaCode) != 0 & (p2.AreaCode & p3.AreaCode) != 0 & (p3.AreaCode & p1.AreaCode) != 0)
			{
				return;
			}

			// 一部分在视野之内
			List<FaceType> faces = new List<FaceType>();
			TriPunctureFaces(p1, p2, p3, ref faces);
			if (faces.Count() == 0)
				return;
			Vertex v0 = new Vertex();
			Vertex v1 = null;
			Vertex v2 = null;
			FaceType faceType = faces[0];
			if (faceType == FaceType.TOP)
			{
				if (p1.Position.y < p2.Position.y)
				{
					SwapVertex(ref p1, ref p2);
				}
				if (p1.Position.y < p3.Position.y)
				{
					SwapVertex(ref p1, ref p3);
				}
				if (p2.Position.y < p3.Position.y)
				{
					SwapVertex(ref p2, ref p3);
				}

				if (p3.Position.y > 1)
					return;

				if (p1.Position.y <= 1)
				{
					triangles.Add(new Triangle(p1, p2, p3));
					return;
				}

				 // y0 -- y2的交点
				v0.Position.y = 1;
				float dt = (p1.Position.y - 1.0f) / (p1.Position.y - p3.Position.y);
				v0.Position.x = (float)(p1.Position.x - (p1.Position.x - p3.Position.x) * dt);
				v0.Position.z = (float)(p1.Position.z - (p1.Position.z - p3.Position.z) * dt);
				v0.Position.w = 1;
				v0.Normal = p1.Normal;
				v0.LightColor = Color.Lerp(p1.LightColor, p3.LightColor, dt);
				v0.Color = Color.Lerp(p1.Color, p3.Color, dt);
				v0.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p3.TexCoord.x, dt);
				v0.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p3.TexCoord.y, dt);

				if (p2.Position.y < 1)
				{
					v1 = new Vertex(); // y0 -- y1的交点
					v1.Position.y = 1;
					dt = (p1.Position.y - 1.0f) / (p1.Position.y - p2.Position.y);
					v1.Position.x = (float)(p1.Position.x - (p1.Position.x - p2.Position.x) * dt);
					v1.Position.z = (float)(p1.Position.z - (p1.Position.z - p2.Position.z) * dt);
					v1.Position.w = 1;
					v1.Normal = p1.Normal;
					v1.LightColor = Color.Lerp(p1.LightColor, p2.LightColor, dt);
					v1.Color = Color.Lerp(p1.Color, p2.Color, dt);
					v1.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p2.TexCoord.x, dt);
					v1.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p2.TexCoord.y, dt);
				}
				else if (p2.Position.y > 1)
				{
					v2 = new Vertex(); // y1 -- y2的交点
					v2.Position.y = 1;
					dt = (p2.Position.y - 1.0f) / (p2.Position.y - p3.Position.y);
					v2.Position.x = (float)(p2.Position.x - (p2.Position.x - p3.Position.x) * dt);
					v2.Position.z = (float)(p2.Position.z - (p2.Position.z - p3.Position.z) * dt);
					v2.Position.w = 1;
					v2.Normal = p2.Normal;
					v2.LightColor = Color.Lerp(p2.LightColor, p3.LightColor, dt);
					v2.Color = Color.Lerp(p2.Color, p3.Color, dt);
					v2.TexCoord.x = MathUntil.Lerp(p2.TexCoord.x, p3.TexCoord.x, dt);
					v2.TexCoord.y = MathUntil.Lerp(p2.TexCoord.y, p3.TexCoord.y, dt);
				}

				Triangle triang1 = null;
				if (v2 == null)
					triang1 = new Triangle(v0, p2, p3);
				else
					triang1 = new Triangle(v0, v2, p3);

				faces.Clear();
				TriPunctureFaces(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref faces);
				if (faces.Count() > 0)
					VertexClip(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref triangles);
				else
					triangles.Add(triang1);

				if (v1 != null)
				{
					Vertex t1 = new Vertex(v0);
					Vertex t2 = new Vertex(p2);
					faces.Clear();
					TriPunctureFaces(t1, v1, t2, ref faces);
					if (faces.Count() > 0)
						VertexClip(t1, v1, t2, ref triangles);
					else
						triangles.Add(new Triangle(t1, v1, t2));
				}
			}
			else if (faceType == FaceType.BUTTOM)
			{
				if (p1.Position.y < p2.Position.y)
				{
					SwapVertex(ref p1, ref p2);
				}
				if (p1.Position.y < p3.Position.y)
				{
					SwapVertex(ref p1, ref p3);
				}
				if (p2.Position.y < p3.Position.y)
				{
					SwapVertex(ref p2, ref p3);
				}

				if (p1.Position.y < -1)
					return;

				if (p3.Position.y >= -1)
				{
					triangles.Add(new Triangle(p1, p2, p3));
					return;
				}

				v0.Position.y = -1;
				float dt = (p1.Position.y + 1.0f) / (p1.Position.y - p3.Position.y);
				v0.Position.x = (float)(p1.Position.x - (p1.Position.x - p3.Position.x) * dt);
				v0.Position.z = (float)(p1.Position.z - (p1.Position.z - p3.Position.z) * dt);
				v0.Position.w = 1;
				v0.Normal = p1.Normal;
				v0.LightColor = Color.Lerp(p1.LightColor, p3.LightColor, dt);
				v0.Color = Color.Lerp(p1.Color, p3.Color, dt);
				v0.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p3.TexCoord.x, dt);
				v0.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p3.TexCoord.y, dt);

				if (p2.Position.y > -1)
				{
					v1 = new Vertex(); // y1 -- y2的交点
					v1.Position.y = -1;
					dt = (p2.Position.y + 1.0f) / (p2.Position.y - p3.Position.y);
					v1.Position.x = (float)(p2.Position.x - (p2.Position.x - p3.Position.x) * dt);
					v1.Position.z = (float)(p2.Position.z - (p2.Position.z - p3.Position.z) * dt);
					v1.Position.w = 1;
					v1.Normal = p1.Normal;
					v1.LightColor = Color.Lerp(p2.LightColor, p3.LightColor, dt);
					v1.Color = Color.Lerp(p2.Color, p3.Color, dt);
					v1.TexCoord.x = MathUntil.Lerp(p2.TexCoord.x, p3.TexCoord.x, dt);
					v1.TexCoord.y = MathUntil.Lerp(p2.TexCoord.y, p3.TexCoord.y, dt);
				}
				else if (p2.Position.y < -1)
				{
					v2 = new Vertex(); // y0 -- y1的交点
					v2.Position.y = -1;
					dt = (p1.Position.y + 1.0f) / (p1.Position.y - p2.Position.y);
					v2.Position.x = (float)(p1.Position.x - (p1.Position.x - p2.Position.x) * dt);
					v2.Position.z = (float)(p1.Position.z - (p1.Position.z - p2.Position.z) * dt);
					v2.Position.w = 1;
					v2.Normal = p1.Normal;
					v2.LightColor = Color.Lerp(p1.LightColor, p2.LightColor, dt);
					v2.Color = Color.Lerp(p1.Color, p2.Color, dt);
					v2.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p2.TexCoord.x, dt);
					v2.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p2.TexCoord.y, dt);
				}

				Triangle triang1 = null;
				if (v2 == null)
					triang1 = new Triangle(p1, p2, v0);
				else
					triang1 = new Triangle(p1, v2, v0);

				faces.Clear();
				TriPunctureFaces(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref faces);
				if (faces.Count() > 0)
					VertexClip(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref triangles);
				else
					triangles.Add(triang1);

				if (v1 != null)
				{
					Vertex t1 = new Vertex(v0);
					Vertex t2 = new Vertex(p2);
					faces.Clear();
					TriPunctureFaces(t2, v1, t1, ref faces);
					if (faces.Count() > 0)
						VertexClip(t2, v1, t1, ref triangles);
					else
						triangles.Add(new Triangle(t2, v1, t1));
				}
			}
			else if (faceType == FaceType.LEFT)
			{
				if (p1.Position.x < p2.Position.x)
				{
					SwapVertex(ref p1, ref p2);
				}
				if (p1.Position.x < p3.Position.x)
				{
					SwapVertex(ref p1, ref p3);
				}
				if (p2.Position.x < p3.Position.x)
				{
					SwapVertex(ref p2, ref p3);
				}
				if (p1.Position.x < -1)
					return;

				if (p3.Position.x >= -1)
				{
					triangles.Add(new Triangle(p1, p2, p3));
					return;
				}

				v0.Position.x = -1;
				float dt = (p1.Position.x + 1.0f) / (p1.Position.x - p3.Position.x);
				v0.Position.y = (float)(p1.Position.y - (p1.Position.y - p3.Position.y) * dt);
				v0.Position.z = (float)(p1.Position.z - (p1.Position.z - p3.Position.z) * dt);
				v0.Position.w = 1;
				v0.Normal = p1.Normal;
				v0.LightColor = Color.Lerp(p1.LightColor, p3.LightColor, dt);
				v0.Color = Color.Lerp(p1.Color, p3.Color, dt);
				v0.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p3.TexCoord.x, dt);
				v0.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p3.TexCoord.y, dt);

				if (p2.Position.x > -1)
				{
					v1 = new Vertex(); // y1 -- y2的交点
					v1.Position.x = -1;
					dt = (p2.Position.x + 1.0f) / (p2.Position.x - p3.Position.x);
					v1.Position.y = (float)(p2.Position.y - (p2.Position.y - p3.Position.y) * dt);
					v1.Position.z = (float)(p2.Position.z - (p2.Position.z - p3.Position.z) * dt);
					v1.Position.w = 1;
					v1.Normal = p2.Normal;
					v1.LightColor = Color.Lerp(p2.LightColor, p3.LightColor, dt);
					v1.Color = Color.Lerp(p2.Color, p3.Color, dt);
					v1.TexCoord.x = MathUntil.Lerp(p2.TexCoord.x, p3.TexCoord.x, dt);
					v1.TexCoord.y = MathUntil.Lerp(p2.TexCoord.y, p3.TexCoord.y, dt);
				}
				else if (p2.Position.x < -1)
				{
					v2 = new Vertex(); // y0 -- y1的交点
					v2.Position.x = -1;
					dt = (p1.Position.x + 1.0f) / (p1.Position.x - p2.Position.x);
					v2.Position.y = (float)(p1.Position.y - (p1.Position.y - p2.Position.y) * dt);
					v2.Position.z = (float)(p1.Position.z - (p1.Position.z - p2.Position.z) * dt);
					v2.Position.w = 1;
					v2.Normal = p1.Normal;
					v2.LightColor = Color.Lerp(p1.LightColor, p2.LightColor, dt);
					v2.Color = Color.Lerp(p1.Color, p2.Color, dt);
					v2.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p2.TexCoord.x, dt);
					v2.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p2.TexCoord.y, dt);
				}

				Triangle triang1 = null;
				if (v2 != null)
					triang1 = new Triangle(v0, p1, v2);
				else
					triang1 = new Triangle(v0, p1, p2);

				faces.Clear();
				TriPunctureFaces(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref faces);
				if (faces.Count() > 0)
					VertexClip(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref triangles);
				else
					triangles.Add(triang1);

				if (v1 != null)
				{
					Vertex t1 = new Vertex(v0);
					Vertex t2 = new Vertex(p2);
					TriPunctureFaces(t1, t2, v1, ref faces);
					if (faces.Count() > 0)
						VertexClip(t1, t2, v1, ref triangles);
					else
						triangles.Add(new Triangle(t1, t2, v1));
				}
			}
			if (faceType == FaceType.RIGHT)
			{
				if (p1.Position.x < p2.Position.x)
				{
					SwapVertex(ref p1, ref p2);
				}
				if (p1.Position.x < p3.Position.x)
				{
					SwapVertex(ref p1, ref p3);
				}
				if (p2.Position.x < p3.Position.x)
				{
					SwapVertex(ref p2, ref p3);
				}

				if (p3.Position.x > 1)
					return;

				if (p1.Position.x <= 1)
				{
					triangles.Add(new Triangle(p1, p2, p3));
					return;
				}

				v0.Position.x = 1;
				float dt = (p1.Position.x - 1.0f) / (p1.Position.x - p3.Position.x);
				v0.Position.y = (float)(p1.Position.y - (p1.Position.y - p3.Position.y) * dt);
				v0.Position.z = (float)(p1.Position.z - (p1.Position.z - p3.Position.z) * dt);
				v0.Position.w = 1;
				v0.Normal = p1.Normal;
				v0.LightColor = Color.Lerp(p1.LightColor, p3.LightColor, dt);
				v0.Color = Color.Lerp(p1.Color, p3.Color, dt);
				v0.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p3.TexCoord.x, dt);
				v0.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p3.TexCoord.y, dt);

				if (p2.Position.x < 1)
				{
					v1 = new Vertex(); // y0 -- y1的交点
					v1.Position.x = 1;
					dt = (p1.Position.x - 1.0f) / (p1.Position.x - p2.Position.x);
					v1.Position.y = (float)(p1.Position.y - (p1.Position.y - p2.Position.y) * dt);
					v1.Position.z = (float)(p1.Position.z - (p1.Position.z - p2.Position.z) * dt);
					v1.Position.w = 1;
					v1.Normal = p1.Normal;
					v1.LightColor = Color.Lerp(p1.LightColor, p2.LightColor, dt);
					v1.Color = Color.Lerp(p1.Color, p2.Color, dt);
					v1.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p2.TexCoord.x, dt);
					v1.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p2.TexCoord.y, dt);
				}
				else if (p2.Position.x > 1)
				{
					v2 = new Vertex(); // y1 -- y2的交点
					v2.Position.x = 1;
					dt = (p2.Position.x - 1.0f) / (p2.Position.x - p3.Position.x);
					v2.Position.y = (float)(p2.Position.y - (p2.Position.y - p3.Position.y) * dt);
					v2.Position.z = (float)(p3.Position.z - (p2.Position.z - p3.Position.z) * dt);
					v2.Position.w = 1;
					v2.Normal = p1.Normal;
					v2.LightColor = Color.Lerp(p2.LightColor, p3.LightColor, dt);
					v2.Color = Color.Lerp(p2.Color, p3.Color, dt);
					v2.TexCoord.x = MathUntil.Lerp(p2.TexCoord.x, p3.TexCoord.x, dt);
					v2.TexCoord.y = MathUntil.Lerp(p2.TexCoord.y, p3.TexCoord.y, dt);
				}

				Triangle triang1 = null;
				if (v2 != null)
					triang1 = new Triangle(v0, v2, p3);
				else
					triang1 = new Triangle(v0, p2, p3);

				faces.Clear();
				TriPunctureFaces(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref faces);
				if (faces.Count() > 0)
					VertexClip(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref triangles);
				else
					triangles.Add(triang1);

				if (v1 != null)
				{
					Vertex t1 = new Vertex(v0);
					Vertex t2 = new Vertex(p2);
					TriPunctureFaces(t1, v1, t2, ref faces);
					if (faces.Count() > 0)
						VertexClip(t1, v1, t2, ref triangles);
					else
						triangles.Add(new Triangle(t1, v1, t2));
				}
			}
			else if (faceType == FaceType.FAR)
			{
				if (p1.Position.z < p2.Position.z)
				{
					SwapVertex(ref p1, ref p2);
				}
				if (p1.Position.z < p3.Position.z)
				{
					SwapVertex(ref p1, ref p3);
				}
				if (p2.Position.z < p3.Position.z)
				{
					SwapVertex(ref p2, ref p3);
				}

				if (p3.Position.z > 1)
					return;

				if (p1.Position.x <= 1)
				{
					triangles.Add(new Triangle(p1, p2, p3));
					return;
				}

				v0.Position.z = 1;
				float dt = (p1.Position.z - 1.0f) / (p1.Position.z - p3.Position.z);
				v0.Position.y = (float)(p1.Position.y - (p1.Position.y - p3.Position.y) * dt);
				v0.Position.x = (float)(p1.Position.x - (p1.Position.x - p3.Position.x) * dt);
				v0.Position.w = 1;
				v0.Normal = p1.Normal;
				v0.LightColor = Color.Lerp(p1.LightColor, p3.LightColor, dt);
				v0.Color = Color.Lerp(p1.Color, p3.Color, dt);
				v0.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p3.TexCoord.x, dt);
				v0.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p3.TexCoord.y, dt);

				if (p2.Position.z < 1)
				{
					v1 = new Vertex(); // y0 -- y1的交点
					v1.Position.z = 1;
					dt = (p1.Position.z - 1.0f) / (p1.Position.z - p2.Position.z);
					v1.Position.y = (float)(p1.Position.y - (p1.Position.y - p2.Position.y) * dt);
					v1.Position.x = (float)(p1.Position.x - (p1.Position.x - p2.Position.x) * dt);
					v1.Position.w = 1;
					v1.Normal = p1.Normal;
					v1.LightColor = Color.Lerp(p1.LightColor, p2.LightColor, dt);
					v1.Color = Color.Lerp(p1.Color, p2.Color, dt);
					v1.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p2.TexCoord.x, dt);
					v1.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p2.TexCoord.y, dt);
				}
				else if (p2.Position.z > 1)
				{
					v2 = new Vertex(); // y1 -- y2的交点
					v2.Position.z = 1;
					dt = (p2.Position.z - 1.0f) / (p2.Position.z - p3.Position.z);
					v2.Position.y = (float)(p2.Position.y - (p2.Position.y - p3.Position.y) * dt);
					v2.Position.x = (float)(p3.Position.x - (p2.Position.x - p3.Position.x) * dt);
					v2.Position.w = 1;
					v2.Normal = p1.Normal;
					v2.LightColor = Color.Lerp(p2.LightColor, p3.LightColor, dt);
					v2.Color = Color.Lerp(p2.Color, p3.Color, dt);
					v2.TexCoord.x = MathUntil.Lerp(p2.TexCoord.x, p3.TexCoord.x, dt);
					v2.TexCoord.y = MathUntil.Lerp(p2.TexCoord.y, p3.TexCoord.y, dt);
				}
				Triangle triang1 = null;
				if (v2 != null)
					triang1 = new Triangle(v0, v2, p3);
				else
					triang1 = new Triangle(v0, p2, p3);

				faces.Clear();
				TriPunctureFaces(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref faces);
				if (faces.Count() > 0)
					VertexClip(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref triangles);
				else
					triangles.Add(triang1);

				if (v1 != null)
				{
					Vertex t1 = new Vertex(v0);
					Vertex t2 = new Vertex(p2);
					TriPunctureFaces(t1, v1, t2, ref faces);
					if (faces.Count() > 0)
						VertexClip(t1, v1, t2, ref triangles);
					else
						triangles.Add(new Triangle(t1, v1, t2));
				}
			}
			else if (faceType == FaceType.NEAR)
			{
				if (p1.Position.z < p2.Position.z)
				{
					SwapVertex(ref p1, ref p2);
				}
				if (p1.Position.z < p3.Position.z)
				{
					SwapVertex(ref p1, ref p3);
				}
				if (p2.Position.z < p3.Position.z)
				{
					SwapVertex(ref p2, ref p3);
				}

				if (p1.Position.z < 0)
					return;

				if (p3.Position.z >= 0)
				{
					triangles.Add(new Triangle(p1, p2, p3));
					return;
				}

				v0.Position.z = 0;
				float dt = p1.Position.z / (p1.Position.z - p3.Position.z);
				v0.Position.y = (float)(p1.Position.y - (p1.Position.y - p3.Position.y) * dt);
				v0.Position.x = (float)(p1.Position.x - (p1.Position.x - p3.Position.x) * dt);
				v0.Position.w = 1;
				v0.Normal = p1.Normal;
				v0.LightColor = Color.Lerp(p1.LightColor, p3.LightColor, dt);
				v0.Color = Color.Lerp(p1.Color, p3.Color, dt);
				v0.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p3.TexCoord.x, dt);
				v0.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p3.TexCoord.y, dt);

				if (p2.Position.z > 0)
				{
					v1 = new Vertex(); // y1 -- y2的交点
					v1.Position.z = 0;
					dt = p2.Position.z / (p2.Position.z - p3.Position.z);
					v1.Position.y = (float)(p2.Position.y - (p2.Position.y - p3.Position.y) * dt);
					v1.Position.x = (float)(p2.Position.x - (p2.Position.x - p3.Position.x) * dt);
					v1.Position.w = 1;
					v1.Normal = p2.Normal;
					v1.LightColor = Color.Lerp(p2.LightColor, p3.LightColor, dt);
					v1.Color = Color.Lerp(p2.Color, p3.Color, dt);
					v1.TexCoord.x = MathUntil.Lerp(p2.TexCoord.x, p3.TexCoord.x, dt);
					v1.TexCoord.y = MathUntil.Lerp(p2.TexCoord.y, p3.TexCoord.y, dt);
				}
				else if (p2.Position.z < 0)
				{
					v2 = new Vertex(); // y0 -- y1的交点
					v2.Position.z = -1;
					dt = p1.Position.z / (p1.Position.z - p2.Position.z);
					v2.Position.y = (float)(p1.Position.y - (p1.Position.y - p2.Position.y) * dt);
					v2.Position.x = (float)(p1.Position.x - (p1.Position.x - p2.Position.x) * dt);
					v2.Position.w = 1;
					v2.Normal = p1.Normal;
					v2.LightColor = Color.Lerp(p1.LightColor, p2.LightColor, dt);
					v2.Color = Color.Lerp(p1.Color, p2.Color, dt);
					v2.TexCoord.x = MathUntil.Lerp(p1.TexCoord.x, p2.TexCoord.x, dt);
					v2.TexCoord.y = MathUntil.Lerp(p1.TexCoord.y, p2.TexCoord.y, dt);
				}
				Triangle triang1 = null;
				if (v2 != null)
					triang1 = new Triangle(v0, p1, v2);
				else
					triang1 = new Triangle(v0, p1, p2);

				faces.Clear();
				TriPunctureFaces(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref faces);
				if (faces.Count() > 0)
					VertexClip(triang1.Vertex0, triang1.Vertex1, triang1.Vertex2, ref triangles);
				else
					triangles.Add(triang1);

				if (v1 != null)
				{
					Vertex t1 = new Vertex(v0);
					Vertex t2 = new Vertex(p2);
					TriPunctureFaces(t1, t2, v1, ref faces);
					if (faces.Count() > 0)
						VertexClip(t1, t2, v1, ref triangles);
					else
						triangles.Add(new Triangle(t1, t2, v1));
				}
			}
		}

		/// <summary>
		/// 根据材质、图片等信息，绘制三角形面
		/// </summary>
		public void Draw(Renderer renderer, Matrix4X4 transform, RenderTexture texture, Material material)
		{
			if (mVertex0 == null || mVertex1 == null || mVertex2 == null)
				return;

			mTexture = texture;
			mMaterial = material;
			Vertex p1 = new Vertex(mVertex0);
			Vertex p2 = new Vertex(mVertex1);
			Vertex p3 = new Vertex(mVertex2);

			//计算顶点光照颜色
			if (renderer.LightList.Count() > 0)
			{
				UseLight(renderer, transform, ref p1);
				UseLight(renderer, transform, ref p2);
				UseLight(renderer, transform, ref p3);
			}

			// 因为要进行摄像机背面剔除，因此将mvp 分开为mv和p
			TransformToView(renderer, transform, ref p1);
			TransformToView(renderer, transform, ref p2);
			TransformToView(renderer, transform, ref p3);
			// 摄像机背面剔除
			if (CameraBackCulling(renderer, p1, p2, p3) == false)
				return;

			// 透视投影
			TransformToProjection(renderer, ref p1);
			TransformToProjection(renderer, ref p2);
			TransformToProjection(renderer, ref p3);

			// 规范视椎体
			ConvertToView(ref p1);
			ConvertToView(ref p2);
			ConvertToView(ref p3);

			mRenderLst.Clear();
			if ((renderer.CullMode & CullMode.CULL_CVV) != 0)
				VertexClip(p1, p2, p3, ref mRenderLst);
			else
				mRenderLst.Add(new Triangle(p1, p2, p3));

			for (int i = 0; i < mRenderLst.Count(); i++)
				DrawTriangle(renderer, mRenderLst[i]);
		}

		/// <summary>
		/// 绘制指定的一个三角形面
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="triangle"></param>
		public void DrawTriangle(Renderer renderer, Triangle triangle)
		{
			Vertex p1 = triangle.Vertex0;
			Vertex p2 = triangle.Vertex1;
			Vertex p3 = triangle.Vertex2;

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

			Color mTmpVColor = new Color();
			// 斜率小于1
			if (dx > dy)
			{
				int error = dy2 - dx;
				for (int i = 0; i <= dx; i++)
				{
					float t = dx == 0 ? 0 : i / (float)dx;
					Color.Lerp(p1.Color, p2.Color, t, ref mTmpVColor);
					float depth = MathUntil.Lerp(p1.Position.z, p2.Position.z, t);
					renderer.FrameBuffer.SetPointColor(x, y, mTmpVColor, depth);
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
					Color.Lerp(p1.Color, p2.Color, t, ref mTmpVColor);
					float depth = MathUntil.Lerp(p1.Position.z, p2.Position.z, t);
					renderer.FrameBuffer.SetPointColor(x, y, mTmpVColor, depth);
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
			// 平顶三角形：就是在计算机中显示的上面两个顶点的Y坐标相同。
			// 平底三角形：就是在计算机中显示的时候下面两个顶点的Y坐标相同。
			// 右边为主三角形：这种三角形三个点的Y坐标都不相同，但是右边的一条边是最长的斜边
			// 左边为主的三角形：这种三角形的三个点的Y坐标不相同，但是左边的一条边是最长的斜边。
			if (p1.Position.y == p2.Position.y)
			{
				if (p1.Position.y < p3.Position.y)
					DrawTopTriangle(renderer, p1, p2, p3);
				else
					DrawBottomTriangle(renderer, p3, p1, p2);
			}
			else if (p1.Position.y == p3.Position.y)
			{
				if (p1.Position.y < p2.Position.y)
					DrawTopTriangle(renderer, p1, p3, p2);
				else
					DrawBottomTriangle(renderer, p2, p1, p3);
			}
			else if (p2.Position.y == p3.Position.y)
			{
				if (p2.Position.y < p1.Position.y)
					DrawTopTriangle(renderer, p2, p3, p1);
				else
					DrawBottomTriangle(renderer, p1, p2, p3);
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
			for (float y = p1.Position.y; y <= p3.Position.y; y += 1f)
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
			for (float y = p1.Position.y; y <= p2.Position.y; y += 1)
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

			for (float x = left.Position.x; x <= right.Position.x; x += 1)
			{
				int xIndex = (int)Math.Round(x, 0);
				if (xIndex >= 0 && xIndex < renderer.Size.x)
				{
					float t = 0;
					if (right.Position.x - left.Position.x > 0)
						t = (x - left.Position.x) / (right.Position.x - left.Position.x);

					Color vColor = new Color();
					Color lightColor = Color.Lerp(left.LightColor, right.LightColor, t);
					//根据渲染模式选择使用顶点色还是使用图片的颜色。
					if (renderer.RenderMode == RenderMode.VERTEXCOLOR)
					{
						Color.Lerp(left.Color, right.Color, t, ref vColor);
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
							mTexture.GetPixelColor(u, v, ref vColor);
						}
						else
						{
							vColor = Color.Red;
						}
					}
					vColor = vColor * lightColor;
					float depth = MathUntil.Lerp(left.Position.z, right.Position.z, t);
					renderer.FrameBuffer.SetPointColor(xIndex, yIndex, vColor, depth);
				}
			}
		}

	}
}
