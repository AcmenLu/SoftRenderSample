﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	/// <summary>
	/// 只支持方向光，其他的暂时不实现
	/// </summary>

	public class Light
	{
		private Vector3D mPosition;
		private Color mColor;

		public Vector3D Position
		{
			get { return mPosition; }
			set { mPosition = value; }
		}

		public Color Color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		/// <summary>
		/// 指定光的方向和颜色来模拟一个方向光。
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="color"></param>
		public Light(Vector3D direction, Color color)
		{
			mPosition = direction;
			mColor = color;
		}
	}
}
