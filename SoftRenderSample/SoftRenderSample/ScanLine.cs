﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class ScanLine
	{
		private Color mUserColor;
		private Device mDevice;

		public ScanLine(Device device)
		{
			this.mDevice = device;
			mUserColor = new Color(255, 255, 255);
		}

		/// <summary>
		/// 扫描三角形
		/// </summary>
		/// <param name="triangle"></param>
		/// <param name="scene"></param>
		/// <param name="ort"></param>
		public void ProcessScanLine(Triangle triangle, Scene scene, Triangle ort, FaceTypes types)
		{
			Vector4 P1 = triangle.vertices[0].ScreenPosition;
			Vector4 P2 = triangle.vertices[1].ScreenPosition;
			Vector4 P3 = triangle.vertices[2].ScreenPosition;

			Vertex V1 = triangle.vertices[0];
			Vertex V2 = triangle.vertices[1];
			Vertex V3 = triangle.vertices[2];

			// 根据y从小到大排序
			if (P1.Y > P2.Y)
			{
				Vector4.SwapVector4(ref P1, ref P2);
				Vertex.SwapVertex(ref V1, ref V2);
			}

			if (P2.Y > P3.Y)
			{
				Vector4.SwapVector4(ref P2, ref P3);
				Vertex.SwapVertex(ref V2, ref V3);
			}

			if (P1.Y > P2.Y)
			{
				Vector4.SwapVector4(ref P1, ref P2);
				Vertex.SwapVertex(ref V1, ref V2);
			}

			//计算斜率
			float dp1dp2 = 0, dp1dp3 = 0;
			if (P2.Y - P1.Y > 0)
				dp1dp2 = (P2.X - P1.X) / (P2.Y - P1.Y);

			if (P3.Y - P1.Y > 0)
				dp1dp3 = (P3.X - P1.X) / (P3.Y - P1.Y);

			if (dp1dp2 == 0)
			{
				if (P1.X > P2.X)
				{
					Vector4.SwapVector4(ref P1, ref P2);
					Vertex.SwapVertex(ref V1, ref V2);
				}
				for (var y = (int)P1.Y; y < (int)P3.Y; y++)
					ScanLineX(triangle, (int)y, V1, V3, V2, V3, scene, ort, types);
			}

			Vector4 temp1 = P1;
			Vector4 temp2 = P2;
			Vector4 temp3 = P3;

			if (dp1dp2 > dp1dp3)
			{
				for (int y = (int)P1.Y; y <= (int)P3.Y; y++)
				{
					if (y < P2.Y)
						ScanLineX(triangle, y, V1, V3, V1, V2, scene, ort, types);
					else
						ScanLineX(triangle, y, V1, V3, V2, V3, scene, ort, types);
				}
			}
			else
			{
				for (int y = (int)P1.Y; y <= (int)P3.Y; y++)
				{
					if (y < P2.Y)
						ScanLineX(triangle, y, V1, V2, V1, V3, scene, ort, types);
					else
						ScanLineX(triangle, y, V2, V3, V1, V3, scene, ort, types);
				}
			}
		}

		/// <summary>
		/// 根据给定的两条线扫描X轴
		/// </summary>
		/// <param name="triangle"></param>
		/// <param name="y"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="v3"></param>
		/// <param name="v4"></param>
		/// <param name="scene"></param>
		/// <param name="ort"></param>
		/// <param name="types"></param>
		public void ScanLineX(Triangle triangle, int y, Vertex v1, Vertex v2, Vertex v3, Vertex v4, Scene scene, Triangle ort, FaceTypes types)
		{
			Vector4 screen11 = v1.ScreenPosition;
			Vector4 screen12 = v2.ScreenPosition;
			Vector4 screen21 = v3.ScreenPosition;
			Vector4 screen22 = v4.ScreenPosition;

			var r1 = screen11.Y != screen12.Y ? (y - screen11.Y) / (screen12.Y - screen11.Y) : 0.5f;
			var r2 = screen21.Y != screen22.Y ? (y - screen21.Y) / (screen22.Y - screen21.Y) : 0.5f;
			r1 = Clamp(r1);
			r2 = Clamp(r2);

			int dx1 = (int)MathUntily.Lerp(screen11.X, screen12.X, r1);
			int dx2 = (int)MathUntily.Lerp(screen21.X, screen22.X, r2);

			float z1 = MathUntily.Lerp(screen11.Z, screen12.Z, r1);
			float z2 = MathUntily.Lerp(screen21.Z, screen22.Z, r2);

			Color c1 = MathUntily.Lerp(v1.color, v2.color, r1);
			Color c2 = MathUntily.Lerp(v3.color, v4.color, r2);
			Color c3 = new Color();

			float colk1 = MathUntily.Lerp(v1.ColK, v2.ColK, r1);
			float colk2 = MathUntily.Lerp(v3.ColK, v4.ColK, r2);

			Vector4 pos1 = MathUntily.Lerp(v1.Position, v2.Position, r1);
			Vector4 pos2 = MathUntily.Lerp(v3.Position, v4.Position, r2);
			Vector4 pos3 = new Vector4();

			Vector4 nor1 = MathUntily.Lerp(v1.Normal, v2.Normal, r1);
			Vector4 nor2 = MathUntily.Lerp(v3.Normal, v4.Normal, r2);
			Vector4 nor3 = new Vector4();

			// 计算线性方程的系数
			ort.PreCallLerp();

			Vector4 tmppos = new Vector4();
			for (int x = dx1; x < dx2; x++)
			{
				float r3 = Clamp((float)(x - dx1) / (dx2 - dx1));
				pos3 = MathUntily.Lerp(pos1, pos2, r3);
				float z = MathUntily.Lerp(z1, z2, r3);

				tmppos.X = x;
				tmppos.Y = y;
				tmppos.Z = z;
				tmppos.W = 0;

				ort.CallLerp(tmppos);
				nor3 = ort.GetNormal();

				Light light = scene.Lights;
				float d = 0;
				if (scene.IsUseLight == false || light == null)
				{
					c3 = MathUntily.Lerp(c1, c2, r3);
				}
				else
				{
					d = light.ComputeNormalDot(nor3, nor3);
					c3 = MathUntily.Lerp(c1, c2, r3) * light.LightColorV(d);
				}

				mUserColor = c3;
				if (mDevice.renderMode == RenderMode.TEXTURED || mDevice.renderMode == RenderMode.CUBETEXTURED)
				{
					ort.CallLerp(new Vector4(x, y, 0, 0));
					Vector4 uv = ort.GetUV();
					FaceTypes typ = types;
					if (mDevice.renderMode == RenderMode.TEXTURED)
						typ = FaceTypes.NONE;

					RenderTexture texture = scene.GetTextureByFace(typ);
					if (scene.IsUseLight == false || light == null)
						mUserColor = texture.GetPixelColor(uv.X, uv.Y);
					else
						mUserColor = texture.GetPixelColor(uv.X, uv.Y) * light.LightColorV(d);
				}

				this.mDevice.DrawPoint(tmppos, mUserColor);
			}
		}

		/// <summary>
		/// [0, 1]的范围值
		/// </summary>
		/// <param name="g"></param>
		/// <returns></returns>
		public float Clamp(float g)
		{
			if (g.CompareTo(0) < 0)
				return 0;
			else if (g.CompareTo(1) > 0)
				return 1;
			return g;
		}
	}
}
