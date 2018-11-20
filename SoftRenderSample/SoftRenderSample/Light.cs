using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class Light
	{
		public Vector4 LightPos;
		public Color LightColor;
		public Color ambientColor = new Color(150, 150, 150);
		public float VLight;

		public Light(Vector4 pos, Color color)
		{
			this.LightPos = pos;
			this.LightColor = color;
			this.VLight = 1f;
		}

		/// <summary>
		/// 计算光向量和法线向量之间的余弦值
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="normal"></param>
		/// <returns></returns>
		public float ComputeNDotL(Vector4 pos, Vector4 normal)
		{
			var lightDirection = this.LightPos ;
			normal.Normalize();
			LightPos.Normalize();
			lightDirection.Normalize();
			return Math.Max(0,Vector4.Dot(normal, lightDirection));
		}
		
		/// <summary>
		/// 计算光的颜色值
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public Color LightColorV(float t)
		{
			return ambientColor + this.LightColor * t;
		}
	}
}
