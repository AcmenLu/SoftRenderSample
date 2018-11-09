using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Vector2
	{
		private float mX;
		private float mY;

		public float x
		{
			get { return mX; }
			set { mX = value; }
		}

		public float y
		{
			get { return mY; }
			set { mY = value; }
		}

		/// <summary>
		/// 无参构造，设置默认的x和y为0
		/// </summary>
		public Vector2()
		{
			mX = mY = 0.0f;
		}

		/// <summary>
		/// 有参数构造，x和y的值为参数传入
		/// </summary>
		/// <param name="fX"></param>
		/// <param name="fY"></param>
		public Vector2(float fX, float fY)
		{
			mX = fX;
			mY = fY;
		}

		/// <summary>
		/// 复制构造
		/// </summary>
		/// <param name="vec2"></param>
		public Vector2(Vector2 vec2)
		{
			mX = vec2.x;
			mY = vec2.y;
		}
	}
}
