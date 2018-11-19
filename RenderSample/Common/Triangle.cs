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
			//逆时针
			Vector3D v1 = mVertex1.Position - mVertex0.Position;
			Vector3D v2 = mVertex0.Position - mVertex2.Position;
			Vector3D normal = Vector3D.Cross(v1, v2);
			//return mVertex0.Normal;
			return normal.Normalize();
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
			if (mMaterial == null || renderer.LightList.Count() <= 0)
				return;

			// 目前只接受一个光源。
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
			vertex.ZView = vertex.Position.z;
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
		private void StandardView(ref Vertex v)
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
			// 思路：1. 计算出三角形的每一条线所穿过的面
			// 2. 计算三角形所穿过的面
			// 3. 根据三角形穿过的其中一个面将三角形切割为一个或两个三角形（一个面最多将一个三角形切割为两个可见的小三角形）
			// 4. 将上面切割的三角形重复1-3的过程，直到所有三角形在包围盒之内。

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
			else if ((p1.AreaCode & p2.AreaCode) != 0 && (p2.AreaCode & p3.AreaCode) != 0 && (p3.AreaCode & p1.AreaCode) != 0)
			{
				return;
			}

			// 一部分在视野之内
			List<FaceType> faces = new List<FaceType>();
			TriPunctureFaces(p1, p2, p3, ref faces);
			if (faces.Count() == 0)
				return;
			Vertex v0 = new Vertex(p1);
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

				v0.Position.y = 1;
				float dt = (p1.Position.y - 1.0f) / (p1.Position.y - p3.Position.y);
				v0.Position.x = MathUntil.Lerp(p1.Position.x, p3.Position.x, dt);
				v0.Position.z = MathUntil.Lerp(p1.Position.z, p3.Position.z, dt);
				Vertex.LerpColor(ref v0, p1, p3, dt);

				if (p2.Position.y < 1)
				{
					v1 = new Vertex(p1);
					v1.Position.y = 1;
					dt = (p1.Position.y - 1.0f) / (p1.Position.y - p2.Position.y);
					v1.Position.x = MathUntil.Lerp(p1.Position.x, p2.Position.x, dt);
					v1.Position.z = MathUntil.Lerp(p1.Position.z, p2.Position.z, dt);
					Vertex.LerpColor(ref v1, p1, p2, dt);
				}
				else if (p2.Position.y > 1)
				{
					v2 = new Vertex(p2);
					v2.Position.y = 1;
					dt = (p2.Position.y - 1.0f) / (p2.Position.y - p3.Position.y);
					v2.Position.x = MathUntil.Lerp(p2.Position.x, p3.Position.x, dt);
					v2.Position.z = MathUntil.Lerp(p2.Position.z, p3.Position.z, dt);
					Vertex.LerpColor(ref v2, p2, p3, dt);
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
				v0.Position.x = MathUntil.Lerp(p1.Position.x, p3.Position.x, dt);
				v0.Position.z = MathUntil.Lerp(p1.Position.z, p3.Position.z, dt);
				Vertex.LerpColor(ref v0, p1, p3, dt);

				if (p2.Position.y > -1)
				{
					v1 = new Vertex(p2);
					v1.Position.y = -1;
					dt = (p2.Position.y + 1.0f) / (p2.Position.y - p3.Position.y);
					v1.Position.x = MathUntil.Lerp(p2.Position.x, p3.Position.x, dt);
					v1.Position.z = MathUntil.Lerp(p2.Position.z, p3.Position.z, dt);
					Vertex.LerpColor(ref v1, p2, p3, dt);
				}
				else if (p2.Position.y < -1)
				{
					v2 = new Vertex(p1);
					v2.Position.y = -1;
					dt = (p1.Position.y + 1.0f) / (p1.Position.y - p2.Position.y);
					v2.Position.x = MathUntil.Lerp(p1.Position.x, p2.Position.x, dt);
					v2.Position.z = MathUntil.Lerp(p1.Position.z, p2.Position.z, dt);
					Vertex.LerpColor(ref v2, p1, p2, dt);
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
				v0.Position.y = MathUntil.Lerp(p1.Position.y, p3.Position.y, dt);
				v0.Position.z = MathUntil.Lerp(p1.Position.z, p3.Position.z, dt);
				Vertex.LerpColor(ref v0, p1, p3, dt);

				if (p2.Position.x > -1)
				{
					v1 = new Vertex(p2);
					v1.Position.x = -1;
					dt = (p2.Position.x + 1.0f) / (p2.Position.x - p3.Position.x);
					v1.Position.y = MathUntil.Lerp(p2.Position.y, p3.Position.y, dt);
					v1.Position.z = MathUntil.Lerp(p2.Position.z, p3.Position.z, dt);
					Vertex.LerpColor(ref v1, p2, p3, dt);
				}
				else if (p2.Position.x < -1)
				{
					v2 = new Vertex(p1);
					v2.Position.x = -1;
					dt = (p1.Position.x + 1.0f) / (p1.Position.x - p2.Position.x);
					v2.Position.y = MathUntil.Lerp(p1.Position.y, p2.Position.y, dt);
					v2.Position.z = MathUntil.Lerp(p1.Position.z, p2.Position.z, dt);
					Vertex.LerpColor(ref v2, p1, p2, dt);
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
			else if (faceType == FaceType.RIGHT)
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
				v0.Position.y = MathUntil.Lerp(p1.Position.y, p3.Position.y, dt);
				v0.Position.z = MathUntil.Lerp(p1.Position.z, p3.Position.z, dt);
				Vertex.LerpColor(ref v0, p1, p3, dt);

				if (p2.Position.x < 1)
				{
					v1 = new Vertex(p1);
					v1.Position.x = 1;
					dt = (p1.Position.x - 1.0f) / (p1.Position.x - p2.Position.x);
					v1.Position.y = MathUntil.Lerp(p1.Position.y, p2.Position.y, dt);
					v1.Position.z = MathUntil.Lerp(p1.Position.z, p2.Position.z, dt);
					Vertex.LerpColor(ref v1, p1, p2, dt);
				}
				else if (p2.Position.x > 1)
				{
					v2 = new Vertex(p2);
					v2.Position.x = 1;
					dt = (p2.Position.x - 1.0f) / (p2.Position.x - p3.Position.x);
					v2.Position.y = MathUntil.Lerp(p2.Position.y, p3.Position.y, dt);
					v2.Position.z = MathUntil.Lerp(p3.Position.z, p3.Position.z, dt);
					Vertex.LerpColor(ref v2, p2, p3, dt);
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
				v0.Position.y = MathUntil.Lerp(p1.Position.y, p3.Position.y, dt);
				v0.Position.x = MathUntil.Lerp(p1.Position.x, p3.Position.x, dt);
				Vertex.LerpColor(ref v0, p1, p3, dt);

				if (p2.Position.z < 1)
				{
					v1 = new Vertex(p2);
					v1.Position.z = 1;
					dt = (p1.Position.z - 1.0f) / (p1.Position.z - p2.Position.z);
					v1.Position.y = MathUntil.Lerp(p1.Position.y, p2.Position.y, dt);
					v1.Position.x = MathUntil.Lerp(p1.Position.x, p2.Position.x, dt);
					Vertex.LerpColor(ref v1, p1, p2, dt);
				}
				else if (p2.Position.z > 1)
				{
					v2 = new Vertex(p1);
					v2.Position.z = 1;
					dt = (p2.Position.z - 1.0f) / (p2.Position.z - p3.Position.z);
					v2.Position.y = MathUntil.Lerp(p2.Position.y, p3.Position.y, dt);
					v2.Position.x = MathUntil.Lerp(p3.Position.x, p3.Position.x, dt);
					Vertex.LerpColor(ref v2, p2, p3, dt);
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
				v0.Position.y = MathUntil.Lerp(p1.Position.y, p3.Position.y, dt);
				v0.Position.x = MathUntil.Lerp(p1.Position.x, p3.Position.x, dt);
				Vertex.LerpColor(ref v0, p1, p3, dt);

				if (p2.Position.z > 0)
				{
					v1 = new Vertex(p2);
					v1.Position.z = 0;
					dt = p2.Position.z / (p2.Position.z - p3.Position.z);
					v1.Position.y = MathUntil.Lerp(p2.Position.y, p3.Position.y, dt);
					v1.Position.x = MathUntil.Lerp(p2.Position.x, p3.Position.x, dt);
					Vertex.LerpColor(ref v1, p2, p3, dt);
				}
				else if (p2.Position.z < 0)
				{
					v2 = new Vertex(p1);
					v2.Position.z = 0;
					dt = p1.Position.z / (p1.Position.z - p2.Position.z);
					v2.Position.y = MathUntil.Lerp(p1.Position.y, p2.Position.y, dt);
					v2.Position.x = MathUntil.Lerp(p1.Position.x, p2.Position.x, dt);
					Vertex.LerpColor(ref v2, p1, p2, dt);
				}
				Triangle triang1 = null;
				if (v2 != null)
					triang1 = new Triangle(v2, v0, p1);
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
			StandardView(ref p1);
			StandardView(ref p2);
			StandardView(ref p3);

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
				TriangleRasterization(renderer, ref p1, ref p2, ref p3);
			}
		}

		/// <summary>
		/// 绘制直线，使用bresenham算法画出两点之间的连线
		/// </summary>
		/// <param name="Position1"></param>
		/// <param name="Position2"></param>
		public void DrawLine(Renderer renderer, Vertex p1, Vertex p2)
		{
			float dx = p2.Position.x - p1.Position.x;
			float dy = p2.Position.y - p1.Position.y;

			float steps;
			if (Math.Abs(dx) > Math.Abs(dy))
				steps = Math.Abs(dx);
			else
				steps = Math.Abs(dy);

			float deltaX = (float)dx / (float)steps;
			float deltaY = (float)dy / (float)steps;

			float x = p1.Position.x;
			float y = p1.Position.y;
			Color mTmpVColor = new Color();
			for (int k = 0; k < steps; ++k)
			{
				x += deltaX;
				y += deltaY;
				if (x < 0 || x >= renderer.Size.x || y < 0 || y >= renderer.Size.y)
					continue;

				float t = k / (float)steps;
				Color.Lerp(p1.Color, p2.Color, t, ref mTmpVColor);
				float depth = MathUntil.Lerp(1f / p1.ZView, 1f / p2.ZView, t);
				depth = 1f / depth;
				renderer.FrameBuffer.Putpixel((int)(x + 0.5), (int)(y + 0.5), mTmpVColor, depth);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		public void TriangleRasterization(Renderer renderer, ref Vertex p1, ref Vertex p2, ref Vertex p3)
		{
			// 排序
			if (p1.Position.y > p2.Position.y)
				SwapVertex(ref p1, ref p2);
			if (p1.Position.y > p3.Position.y)
				SwapVertex(ref p1, ref p3);
			if (p2.Position.y > p3.Position.y)
				SwapVertex(ref p2, ref p3);

			if(Math.Abs(p1.Position.y - p2.Position.y) <= 0.001f)
			{
				DrawTopTriangle(renderer, p1, p2, p3);
			}
			else if(Math.Abs(p2.Position.y - p3.Position.y) <= 0.001f)
			{
				DrawBottomTriangle(renderer, p1, p2, p3);
			}
			else
			{
				Vertex ver = new Vertex();
				float t = (p1.Position.y - p2.Position.y) / (p1.Position.y - p3.Position.y);
				ver.Position.y = p2.Position.y;
				ver.Position.x = MathUntil.Lerp(p1.Position.x, p3.Position.x, t);
				ver.Position.z = MathUntil.Lerp(p1.Position.z, p3.Position.z, t);
				Vertex.LerpColor(ref ver, p1, p3, t);
				DrawBottomTriangle(renderer, p1, ver, p2);
				DrawTopTriangle(renderer, ver, p2, p3);
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
				newMiddle.Position.z = MathUntil.Lerp(top.Position.z, bottom.Position.z, t);

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
			Vertex new1 = new Vertex(p1);
			Vertex new2 = new Vertex(p2);
			int miny = (int)p1.Position.y;
			int maxy = (int)Math.Ceiling(p3.Position.y);
			for (int y = miny; y <= maxy ; y += 1)
			{
				if (y < 0 || y > renderer.Size.y)
					continue;

				float t = (float)(y - miny) / (float)(maxy - miny);
				float xl = (p3.Position.x - p1.Position.x) * t + p1.Position.x;
				float xr = (p3.Position.x - p2.Position.x) * t + p2.Position.x;

				new1.Position.x = xl;
				new1.Position.y = y;
				Vertex.LerpColor(ref new1, p1, p3, t);
				new2.Position.x = xr;
				new2.Position.y = y;
				Vertex.LerpColor(ref new2, p2, p3, t);
				//扫描线填充
				if (new1.Position.x < new2.Position.x)
					ScanlineFill(renderer, new1, new2, y);
				else
					ScanlineFill(renderer, new2, new1, y);
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
			Vertex new1 = new Vertex();
			Vertex new2 = new Vertex();
			int miny = (int)p1.Position.y;
			int maxy = (int)Math.Ceiling(p3.Position.y);
			for (int y = miny; y <= maxy; y += 1)
			{
				if (y < 0 || y > renderer.Size.y)
					continue;

				float t = (float)(y - miny) / (float)(maxy - miny);
				float xLeft = (p2.Position.x - p1.Position.x) * t + p1.Position.x;
				float xRight = (p3.Position.x - p1.Position.x) * t + p1.Position.x;

				new1.Position.x = xLeft;
				new1.Position.y = y;
				Vertex.LerpColor(ref new1, p1, p2, t);

				new2.Position.x = xRight;
				new2.Position.y = y;
				Vertex.LerpColor(ref new2, p1, p3, t);
				//扫描行进行填充
				if (new1.Position.x < new2.Position.x)
					ScanlineFill(renderer, new1, new2, y);
				else
					ScanlineFill(renderer, new2, new1, y);
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
			int minx = (int)left.Position.x;
			int maxx = (int)Math.Ceiling(right.Position.x);

			for (int x = minx; x <= maxx; x += 1)
			{
				if (x < 0 || x > renderer.Size.x)
					continue;

				float t = 0;
				if (right.Position.x - left.Position.x > 0)
					t = (x - left.Position.x) / (right.Position.x - left.Position.x);

				//深度测试
				float depth = 0f;
				if (left.ZView != 0 && right.ZView != 0)
					depth = MathUntil.Lerp(1f / left.ZView, 1f / right.ZView, t);
				depth = depth == 0f ? 0 : 1f / depth;

				if (renderer.FrameBuffer.TestDepth(x, yIndex, depth) == false)
					continue;

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
						// 纹理矫正
						float lerpFactor = t;
						if (right.ZView - left.ZView != 0)
							lerpFactor = (depth - left.ZView) / (right.ZView - left.ZView);
						float u = MathUntil.Lerp(left.TexCoord.x, right.TexCoord.x, t);
						float v = MathUntil.Lerp(left.TexCoord.y, right.TexCoord.y, t);
						mTexture.GetPixelColor(u, v, ref vColor);
					}
					else
					{
						vColor = Color.Red;
					}
				}
				vColor = vColor * lightColor;
				//renderer.FrameBuffer.SetPointColor(x, yIndex, vColor, depth);
				renderer.FrameBuffer.Putpixel(x, yIndex, vColor, depth);
			}
		}


	}
}
