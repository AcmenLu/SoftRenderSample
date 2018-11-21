using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class Material
	{
		private float mAmbientStregth;
		private Color mDiffuse;

		/// <summary>
		/// 环境光系数
		/// </summary>
		public float AmbientStregth
		{
			get { return mAmbientStregth; }
			set { mAmbientStregth = value; }
		}

		/// <summary>
		/// 光颜色
		/// </summary>
		public Color Diffuse
		{
			get { return mDiffuse; }
			set { mDiffuse = value; }
		}

		/// <summary>
		/// 指定一个环境光系数和一个漫反射光来构造材质
		/// </summary>
		/// <param name="ambient"></param>
		/// <param name="color"></param>
		public Material(float ambient, Color color)
		{
			mAmbientStregth = ambient;
			mDiffuse = color;
		}
	}
}
