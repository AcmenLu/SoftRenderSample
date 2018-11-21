using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class Mesh
	{
		public string Name;
		public Vertex[] Vertices;
		public Triangle[] Triangles;
		public Face[] faces;

		public Mesh(string name,int verticesCount)
		{
			Vertices = new Vertex[verticesCount];
			Name = name;
		}

		public Mesh(string name, int verticesCount, int triangesCount)
		{
			Name = name;
			InitTriangle(triangesCount);
		}

		public void InitTriangle(int c)
		{
			Triangles = new Triangle[c];
		}

		public void MakeFace()
		{
			faces = new Face[] {
				// 正面
				new Face(2, 5, 8, FaceTypes.NEAR),
				new Face(2, 8, 11, FaceTypes.NEAR),
				// 右面
				new Face(4, 16, 7, FaceTypes.RIGHT),
				new Face(16, 19, 7, FaceTypes.RIGHT),
				// 左面
				new Face(13, 1, 10, FaceTypes.LEFT),
				new Face(13, 10, 22, FaceTypes.LEFT),
				// 背面
				new Face(17, 14, 23, FaceTypes.FAR),
				new Face(17, 23, 20, FaceTypes.FAR),
				// 上面
				new Face(9, 6, 18, FaceTypes.TOP),
				new Face(9, 18, 21, FaceTypes.TOP),
				// 下面
				new Face(12, 15, 3, FaceTypes.BUTTOM),
				new Face(12, 3, 0, FaceTypes.BUTTOM)
			};
		}
	}
}
