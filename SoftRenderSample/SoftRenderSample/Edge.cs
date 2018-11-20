using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class Edge: ICloneable
	{
		public int ymax; //边上端点的y坐标

		public float x; //表示当前扫描线与边的交点的x坐标  初值 ET中的值  为边的下端点的x坐标
		public float deltx; //边的斜率的倒数
		public Edge nextEdge;

		public Vertex v1;
		public Vertex v2;

		public Object Clone()
		{
			Edge e = new Edge();
			e.deltx = this.deltx;
			e.x = this.x;
			e.ymax = this.ymax;
			e.nextEdge = this.nextEdge;
			e.v1 = this.v1;
			e.v2 = this.v2;
			return e;
		}
	}
}
