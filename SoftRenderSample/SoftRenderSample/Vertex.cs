using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class Vertex
	{
		public Vector4 Position;
		//光照 法线
		public Vector4 Normal;
		//材质 uv
		public Vector4 UV;
		//颜色
		public Color color;

		//进行扫描线算法的时候用到的 
		public Vector4 ScreenPosition;
		public Vector4 ClipPosition;
		public float ColK;

		public Vertex()
		{ }

		public Vertex(Vector4 position,Vector4 normal,Vector4 uv,Color c)
		{
			this.Position = position;
			this.Normal = normal;
			this.UV = uv;
			this.color = c;
		}

	}
}
