using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Vertex
	{
		private Vector3D	mPosition;
		private Vector3D mNormal;
		private Vector2	mTexcoord;
		private Color	mColor;
		private Color mLightColor;

		public Vector3D Position
		{
			get { return mPosition; }
			set { mPosition = value; }
		}

		public Vector3D Normal
		{
			get { return mNormal; }
			set { mNormal = value; }
		}

		public Vector2 TexCoord
		{
			get { return mTexcoord; }
			set { mTexcoord = value; }
		}

		public Color Color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		public Color LightColor
		{
			get { return mLightColor; }
			set { mLightColor = value; }
		}

		public Vertex()
		{
			mPosition = new Vector3D();
			mNormal = new Vector3D();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
			mLightColor = new Color(1.0f, 1.0f, 1.0f);
		}

		public Vertex(Vector3D pos)
		{
			mPosition = new Vector3D(pos);
			mNormal = new Vector3D();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
			mLightColor = new Color(1.0f, 1.0f, 1.0f);
		}

		public Vertex(Vector3D pos, Vector3D normal, Vector2 texcoord, Color color, Color lightColor)
		{
			mPosition = pos;
			mNormal = normal;
			mTexcoord = texcoord;
			mColor = color;
			mLightColor = lightColor;
		}

		public Vertex(Vertex v)
		{
			mPosition = v.Position;
			mNormal = v.Normal;
			mTexcoord = v.TexCoord;
			mColor = v.Color;
			mLightColor = v.LightColor;
		}

		/// <summary>
		/// 插值生成新顶点(在屏幕空间，Z值是不需要进行差值的，只需要对x和y进行差值)
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static void LerpVertexInScreen(ref Vertex v, Vertex v1, Vertex v2, float t)
		{
			v.TexCoord.x = MathUntil.Lerp(v1.TexCoord.x, v2.TexCoord.x, t);
			v.TexCoord.y = MathUntil.Lerp(v1.TexCoord.y, v2.TexCoord.y, t);
			v.Color = Color.Lerp(v1.Color, v2.Color, t);
			v.LightColor = Color.Lerp(v1.LightColor, v2.LightColor, t);
		}

	}
}
