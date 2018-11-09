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

		public Vertex()
		{
			mPosition = new Vector3D();
			mNormal = new Vector3D();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
		}

		public Vertex(Vector3D pos)
		{
			mPosition = new Vector3D(pos);
			mNormal = new Vector3D();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
		}

		public Vertex(Vector3D pos, Vector3D normal, Vector2 texcoord, Color color)
		{
			mPosition = new Vector3D(pos);
			mNormal = new Vector3D(normal);
			mTexcoord = new Vector2(texcoord);
			mColor = new Color(color);
		}

		public Vertex(Vertex v)
		{
			mPosition = v.Position;
			mNormal = v.Normal;
			mTexcoord = v.TexCoord;
			mColor = v.Color;
		}
	}
}
