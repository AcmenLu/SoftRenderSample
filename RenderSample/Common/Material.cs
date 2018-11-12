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
		public float mKA;
		// 漫反射颜色
		public Color mDiffuse;
		// 镜面光
		public Color mSpecular;

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

		public Color Specular
		{
			get { return mSpecular; }
			set { mSpecular = value; }
		}

		public Material(Color emissive, float ka, Color diffuse, Color specular)
		{
			mEmissive = emissive;
			mKA = ka;
			mDiffuse = diffuse;
			mSpecular = specular;
		}
	}
}
