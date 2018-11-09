using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Vertex
	{
		private Vector3	mPosition;
		private Vector3 mNormal;
		private Vector2	mTexcoord;
		private Color	mColor;

		public Vector3 Position
		{
			get { return mPosition; }
			set { mPosition = value; }
		}

		public Vector3 Normal
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
			mPosition = new Vector3();
			mNormal = new Vector3();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
		}

		public Vertex(Vector3 pos)
		{
			mPosition = new Vector3(pos);
			mNormal = new Vector3();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
		}

		public Vertex(Vector3 pos, Vector3 normal, Vector2 texcoord, Color color)
		{
			mPosition = new Vector3(pos);
			mNormal = new Vector3(normal);
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
