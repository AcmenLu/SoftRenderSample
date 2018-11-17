using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleCommon
{
	public class Vertex
	{
		private Vector3D	mPosition;
		private Vector3D	mNormal;
		private Vector2		mTexcoord;
		private Color		mColor;
		private Color		mLightColor;
		private byte		mAreaCode;
		private float		mZView;

		/// <summary>
		/// 顶点位置
		/// </summary>
		public Vector3D Position
		{
			get { return mPosition; }
			set { mPosition = value; }
		}

		/// <summary>
		/// 顶点法线
		/// </summary>
		public Vector3D Normal
		{
			get { return mNormal; }
			set { mNormal = value; }
		}

		/// <summary>
		/// uv坐标
		/// </summary>
		public Vector2 TexCoord
		{
			get { return mTexcoord; }
			set { mTexcoord = value; }
		}

		/// <summary>
		/// 顶点色
		/// </summary>
		public Color Color
		{
			get { return mColor; }
			set { mColor = value; }
		}

		/// <summary>
		/// 顶点光的颜色
		/// </summary>
		public Color LightColor
		{
			get { return mLightColor; }
			set { mLightColor = value; }
		}

		/// <summary>
		/// 裁剪使用的顶点编码
		/// </summary>
		public byte AreaCode
		{
			get { return mAreaCode; }
			set { mAreaCode = value; }
		}

		/// <summary>
		/// 纹理矫正
		/// </summary>
		public float ZView
		{
			get { return mZView; }
			set { mZView = value; }
		}

		/// <summary>
		/// 默认构造
		/// </summary>
		public Vertex()
		{
			mPosition = new Vector3D();
			mNormal = new Vector3D();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
			mLightColor = new Color(1.0f, 1.0f, 1.0f);
		}

		/// <summary>
		/// 使用位置构造一个顶点
		/// </summary>
		/// <param name="pos"></param>
		public Vertex(Vector3D pos)
		{
			mPosition = new Vector3D(pos);
			mNormal = new Vector3D();
			mTexcoord = new Vector2();
			mColor = new Color(1.0f, 1.0f, 1.0f);
			mLightColor = new Color(1.0f, 1.0f, 1.0f);
			mAreaCode = (byte)FaceType.NONE;
		}

		/// <summary>
		/// 复制构造
		/// </summary>
		/// <param name="v"></param>
		public Vertex(Vertex v)
		{
			mPosition = new Vector3D(v.Position);
			mNormal = new Vector3D(v.Normal);
			mTexcoord = new Vector2(v.TexCoord);
			mColor = v.Color;
			mLightColor = v.LightColor;
			mAreaCode = v.AreaCode;
		}

		/// <summary>
		/// 计算区域码
		/// </summary>
		public void CallAreaCode()
		{
			mAreaCode = (byte)FaceType.NONE;
			if (mPosition.x < -mPosition.w) mAreaCode |= (byte)FaceType.LEFT;
			if (mPosition.x > mPosition.w) mAreaCode |= (byte)FaceType.RIGHT;
			if (mPosition.y < -mPosition.w) mAreaCode |= (byte)FaceType.BUTTOM;
			if (mPosition.y > mPosition.w) mAreaCode |= (byte)FaceType.TOP;
			if (mPosition.z < 0) mAreaCode |= (byte)FaceType.NEAR;
			if (mPosition.z > mPosition.w) mAreaCode |= (byte)FaceType.FAR;
		}

		/// <summary>
		/// 插值生成新顶点(在屏幕空间，Z值是不需要进行差值的，只需要对x和y进行差值)
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		public static void LerpColor(ref Vertex v, Vertex v1, Vertex v2, float t)
		{
			v.TexCoord.x = MathUntil.Lerp(v1.TexCoord.x, v2.TexCoord.x, t);
			v.TexCoord.y = MathUntil.Lerp(v1.TexCoord.y, v2.TexCoord.y, t);
			v.Color = Color.Lerp(v1.Color, v2.Color, t);
			v.LightColor = Color.Lerp(v1.LightColor, v2.LightColor, t);
		}
	}
}
