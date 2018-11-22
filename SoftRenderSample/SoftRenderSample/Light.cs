
namespace SoftRenderSample
{
	class Light
	{
		private Vector4 mPosition;
		private Color mColor;

		/// <summary>
		/// 位置
		/// </summary>
		public Vector4 Position
		{
			get { return mPosition; }
			set { mPosition = value; }
		}

		/// <summary>
		/// 颜色
		/// </summary>
		public Color Color
		{
			get { return mColor; }
			set { mColor = value;}
		}

		public Light(Vector4 pos, Color color)
		{
			mPosition = pos;
			mColor = color;
		}
	}
}
