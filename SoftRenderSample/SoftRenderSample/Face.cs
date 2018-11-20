using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	public enum FancType
	{
		LEFT = 0,
		RIGHT,
		// y
		TOP,
		BUTTOM,
		// z
		NEAR,
		FAR,
		NONE,
	}

	class Face
	{
		public int A;
		public int B;
		public int C;
		public FancType FaceType;

		public Face(int a,int b,int c)
		{
			this.A = a;
			this.B = b;
			this.C = c;
			FaceType = FancType.NONE;
		}

		public Face(int a, int b, int c, FancType face)
		{
			this.A = a;
			this.B = b;
			this.C = c;
			this.FaceType = face;
		}
	}
}
