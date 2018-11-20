using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class MathUntily
	{
		/// <summary>
		/// 数值
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="x2"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		public static float Lerp(float x1, float x2, float k)
		{
			return x1 + (x2 - x1) * k;
		}

		/// <summary>
		/// 坐标线性插值
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		public static Vector4 Lerp(Vector4 v1, Vector4 v2, float k)
		{
			return new Vector4(
				v1.X+(v2.X-v1.X)*k,
				v1.Y+(v2.Y-v1.Y)*k,
				v1.Z+(v2.Z-v1.Z)*k,
				v1.W+(v2.W-v1.W)*k);
		}


		/// <summary>
		/// 颜色线性插值
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		public static Color Lerp(Color c1,Color c2,float k)
		{
			byte r = (byte)(c1.X + (c2.X - c1.X) * k);
			byte g = (byte)(c1.Y + (c2.Y - c1.Y) * k);
			byte b = (byte)(c1.Z + (c2.Z - c1.Z) * k);
			return new Color(r,g,b);
		}
	}
}
