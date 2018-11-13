using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Material
	{
		// 自发光颜色
		public Color mEmissive;
		// 环境光系数。
		public float mKA;
		// 漫反射颜色
		public Color mDiffuse;

		// TODO:增加镜面高光的支持。

		public Color Emissive
		{
			get { return mEmissive; }
			set { mEmissive = value; }
		}

		public float KA
		{
			get { return mKA; }
			set { mKA = KA; }
		}

		public Color Diffuse
		{
			get { return mDiffuse; }
			set { mDiffuse = value; }
		}

		public Material(Color emissive, float ka, Color diffuse)
		{
			mEmissive = emissive;
			mKA = ka;
			mDiffuse = diffuse;
		}
	}
}
