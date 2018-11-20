﻿using System.Collections.Generic;
using System.Drawing;
using SoftRenderSample;

namespace SoftRenderSample
{
	class Clip
	{
		private Device mDevice;
		private List<Vertex> mOutputList;

		public Device Device
		{
			get { return mDevice; }
		}
	
		public List<Vertex> OutputList
		{
			get { return this.mOutputList; }
		}

		public Clip(Device device)
		{
			this.mOutputList = new List<Vertex>();
			this.mDevice = device;
		}

		/// <summary>
		/// 求交点, 返回的pos是clip空间下坐标
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="face"></param>
		/// <param name="wMin"></param>
		/// <param name="wMax"></param>
		/// <returns></returns>
		Vertex Intersect(Vertex v1, Vertex v2, int face, Vector4 wMin, Vector4 wMax)
		{
			Vertex vertex = new Vertex();
			float k1 = 0, k2 = 0, k3 = 0, k4 = 0, k5 = 0, k6 = 0;
			Vector4 p1 = v1.ClipPosition;
			Vector4 p2 = v2.ClipPosition;
			
			if (p1.X != p2.X)
				{ k1 = (wMin.X - p1.X) / (p2.X - p1.X); k2 = (wMax.X - p1.X) / (p2.X - p1.X); }
			else
				{ k1 = k2 = 1; }
			if (p1.Y != p2.Y)
				{ k3 = (wMin.Y - p1.Y) / (p2.Y - p1.Y); k4 = (wMax.Y - p1.Y) / (p2.Y - p1.Y); }
			else
				{ k3 = k4 = 1; }
			if (p1.Z != p2.Z)
				{ k5 = (wMin.Z - p1.Z) / (p2.Z - p1.Z); k6 = (wMax.Z - p1.Z) / (p2.Z - p1.Z); }
			else
				{ k5 = k6 = 1; }

			Vector4 clipPos = new Vector4();
			Vector4 pos = new Vector4();
			Color col = new Color(0, 0, 0);
			Vector4 normal = new Vector4();
			Vector4 uv = new Vector4();
			float colk = 0.0f;
			switch (face)
			{
				case 0:
					clipPos.X = wMin.X;
					clipPos.Y = p1.Y + (p2.Y - p1.Y) * k1;
					clipPos.Z = p1.Z + (p2.Z - p1.Z) * k1;
					clipPos.W = p1.W + (p2.W - p1.W) * k1;
					col = MathUntily.Lerp(v1.color, v2.color, k1);
					normal = MathUntily.Lerp(v1.Normal, v2.Normal, k1);
					pos = MathUntily.Lerp(v1.Position, v2.Position, k1);
					uv = MathUntily.Lerp(v1.UV, v2.UV,k1);
					colk = MathUntily.Lerp(v1.ColK,v2.ColK,k1);
					break;
				case 1:
					clipPos.X = wMax.X;
					clipPos.Y = p1.Y + (p2.Y - p1.Y) * k2;
					clipPos.Z = p1.Z + (p2.Z - p1.Z) * k2;
					clipPos.W = p1.W + (p2.W - p1.W) * k2;
					col = MathUntily.Lerp(v1.color, v2.color, k2);
					normal = MathUntily.Lerp(v1.Normal, v2.Normal, k2);
					pos = MathUntily.Lerp(v1.Position, v2.Position, k2);
					uv = MathUntily.Lerp(v1.UV, v2.UV, k2);
					colk = MathUntily.Lerp(v1.ColK, v2.ColK, k2);
					break;
				case 2:
					clipPos.Y = wMin.Y;
					clipPos.X = p1.X + (p2.X - p1.X) * k3;
					clipPos.Z = p1.Z + (p2.Z - p1.Z) * k3;
					clipPos.W = p1.W + (p2.W - p1.W) * k3;
					col = MathUntily.Lerp(v1.color, v2.color, k3);
					normal = MathUntily.Lerp(v1.Normal, v2.Normal, k3);
					pos = MathUntily.Lerp(v1.Position, v2.Position, k3);
					uv = MathUntily.Lerp(v1.UV, v2.UV, k3);
					colk = MathUntily.Lerp(v1.ColK, v2.ColK, k3);
					break;
				case 3:
					clipPos.Y = wMax.Y;
					clipPos.X = p1.X + (p2.X - p1.X) * k4;
					clipPos.Z = p1.Z + (p2.Z - p1.Z) * k4;
					clipPos.W = p1.W + (p2.W - p1.W) * k4;
					col = MathUntily.Lerp(v1.color, v2.color, k4);
					normal = MathUntily.Lerp(v1.Normal, v2.Normal, k4);
					pos = MathUntily.Lerp(v1.Position, v2.Position, k4);
					uv = MathUntily.Lerp(v1.UV, v2.UV, k4);
					colk = MathUntily.Lerp(v1.ColK, v2.ColK, k4);
					break;
				case 4:
					clipPos.Z = wMin.Z;
					clipPos.X = p1.X + (p2.X - p1.X) * k5;
					clipPos.Y = p1.Y + (p2.Y - p1.Y) * k5;
					clipPos.W = p1.W + (p2.W - p1.W) * k5;
					col = MathUntily.Lerp(v1.color, v2.color, k5);
					normal = MathUntily.Lerp(v1.Normal, v2.Normal, k5);
					pos = MathUntily.Lerp(v1.Position, v2.Position, k5);
					uv = MathUntily.Lerp(v1.UV, v2.UV, k5);
					colk = MathUntily.Lerp(v1.ColK, v2.ColK, k5);
					break;
				case 5:
					clipPos.Z = wMax.Z;
					clipPos.X = p1.X + (p2.X - p1.X) * k6;
					clipPos.Y = p1.Y + (p2.Y - p1.Y) * k6;
					clipPos.W = p1.W + (p2.W - p1.W) * k6;
					col = MathUntily.Lerp(v1.color, v2.color, k6);
					normal = MathUntily.Lerp(v1.Normal, v2.Normal, k6);
					pos = MathUntily.Lerp(v1.Position, v2.Position, k6);
					uv = MathUntily.Lerp(v1.UV, v2.UV, k6);
					colk = MathUntily.Lerp(v1.ColK, v2.ColK, k6);
					break;
			}

			vertex.Position = pos;
			vertex.ClipPosition = clipPos;
			vertex.ScreenPosition = this.mDevice.ViewPort(clipPos);
			vertex.Normal = normal;
			vertex.UV = uv;
			vertex.color = col;
			vertex.ColK = colk;
			return vertex;
		}

		/// <summary>
		/// 判断是否在多边形内
		/// </summary>
		/// <param name="p"></param>
		/// <param name="face"></param>
		/// <param name="wMin"></param>
		/// <param name="wMax"></param>
		/// <returns></returns>
		bool Inside(Vector4 p, int face , Vector4 wMin, Vector4 wMax)
		{
			bool mark = true;
			switch (face)
			{
				case 0:
					if (p.X < wMin.X)
						mark = false;
					break;
				case 1:
					if (p.X > wMax.X)
						mark = false;
					break;
				case 2:
					if (p.Y < wMin.Y)
						mark = false;
					break;
				case 3:
					if (p.Y > wMax.Y)
						mark = false;
					break;
				case 4:
					if (p.Z < wMin.Z)
						mark = false;
					break;
				case 5:
					if (p.Z > wMax.Z)
						mark = false;
					break;
			}
			if (p.W < 0)
			{
				mark = false;
			}
			return mark;
		}

		/// <summary>
		/// 多边形剪裁，需要clip空间下的坐标（除以w分量）
		/// </summary>
		/// <param name="face"></param>
		/// <param name="wMin"></param>
		/// <param name="wMax"></param>
		/// <param name="vertexList"></param>
		public void HodgmanPolygonClip(int face, Vector4 wMin, Vector4 wMax, Vertex[] vertexList)
		{
			Vertex s = vertexList[vertexList.Length - 1];
			for (int i = 0; i < vertexList.Length; i++)
			{
				Vertex p = vertexList[i];

				if (Inside(p.ClipPosition, face, wMin, wMax))
				{
					if (Inside(s.ClipPosition, face, wMin, wMax))
					{
						this.mOutputList.Add(p);
					}
					else
					{
						this.mOutputList.Add(Intersect(s, p, face, wMin, wMax));
						this.mOutputList.Add(vertexList[i]);
					}
				}
				else if (Inside(s.ClipPosition, face, wMin, wMax))
				{
					this.mOutputList.Add(Intersect(s, p, face, wMin, wMax));
				}
				s = vertexList[i];
			}
		}
	}
}
