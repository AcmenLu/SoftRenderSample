using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Material
	{
		// 环境光系数。
		public float mAmbientStregth;
		// 漫反射
		public Color mDiffuse;

		public float AmbientStregth
		{
			get { return mAmbientStregth; }
			set { mAmbientStregth = value; }
		}

		public Color Diffuse
		{
			get { return mDiffuse; }
			set { mDiffuse = value; }
		}

		public Material(float ambient, Color color)
		{
			mAmbientStregth = ambient;
			mDiffuse = color;
		}
	}
}
