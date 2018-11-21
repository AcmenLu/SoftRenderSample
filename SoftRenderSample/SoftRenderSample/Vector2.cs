using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	public class Vector2
	{
		private float mX;
		private float mY;

		public float X
		{
			get { return mX; }
			set { mX = value; }
		}

		public float Y
		{
			get { return mY; }
			set { mY = value; }
		}

		public float U
		{
			get { return mX; }
			set { mX = value; }
		}

		public float V
		{
			get { return mY; }
			set { mY = value; }
		}

		public Vector2()
		{
			mX = mX = 0f;
		}

		public Vector2(float x, float y)
		{
			mX = x;
			mY = y;
		}
	}
}
