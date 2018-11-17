using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	/// <summary>
	/// 公共数学库
	/// </summary>
	public class MathUntil
	{
		public static float PIDEV2 = (float)Math.PI / 2;

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
				return a;
			else if (t >= 1)
				return b;
			else
				return b * t + (1 - t) * a;
		}

	}
}
