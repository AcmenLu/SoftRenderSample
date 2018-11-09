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
		private Vector3D mDirection;
		private Color mColor;

		public Vector3D Direction
		{
			get { return mDirection; }
			set { mDirection = value; }
		}

		public Color Color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		public Light(Vector3D direction, Color color)
		{
			mDirection = direction;
			mColor = color;
		}
	}
}
