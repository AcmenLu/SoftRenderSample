using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class MathUntil
	{
		/// <summary>
		/// 将一个值限定在指定范围内
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static float Range(float value, float min, float max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		/// <summary>
		/// 计算两个数之间的插值
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float Lerp(float a, float b, float t)
		{
			if (t <= 0)
			{
				return a;
			}
			else if (t >= 1)
			{
				return b;
			}
			else
			{
				return b * t + (1 - t) * a;
			}
		}

		/// <summary>
		/// 计算两个颜色之间的插值
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static Color Lerp(Color a, Color b, float t)
		{
			if (t <= 0)
				return a;
			else if (t >= 1)
				return b;
			else
				return t * b + (1 - t) * a;
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
			v.Color = MathUntil.Lerp(v1.Color, v2.Color, t);
		}
	}
}
