using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftRenderSample
{
	class ScanLine
	{
		public Color userColor;
		public Device device;


		public ScanLine(Device device)
		{
			this.device = device;
			userColor = new Color(255, 255, 255);
		}
		
		/// <summary>
		/// 扫描三角形
		/// </summary>
		/// <param name="triangle"></param>
		/// <param name="scene"></param>
		/// <param name="ort"></param>
		public void ProcessScanLine(Triangle triangle, Scene scene, Triangle ort, FancType types)
		{
			var P1 = triangle.vertices[0].ScreenPosition;
			var P2 = triangle.vertices[1].ScreenPosition;
			var P3 = triangle.vertices[2].ScreenPosition;
			Vertex V1 = triangle.vertices[0];
			Vertex V2 = triangle.vertices[1];
			Vertex V3 = triangle.vertices[2];
			// 交换
			if (P1.Y > P2.Y)
			{
				var temp = P2;
				P2 = P1;
				P1 = temp;
				var tempv = V2;
				V2 = V1;
				V1 = tempv;
			}
			if (P2.Y > P3.Y)
			{
				var temp = P2;
				P2 = P3;
				P3 = temp;
				var tempv = V2;
				V2 = V3;
				V3 = tempv;
			}
			if (P1.Y > P2.Y)
			{

				var temp = P2;
				P2 = P1;
				P1 = temp;
				var tempv = V2;
				V2 = V1;
				V1 = tempv;
			}

			//计算斜率
			float dp1dp2=0, dp1dp3=0;
			if (P2.Y - P1.Y > 0)
				dp1dp2 = (P2.X - P1.X) / (P2.Y - P1.Y);
			
			if (P3.Y - P1.Y > 0)
				dp1dp3 = (P3.X - P1.X) / (P3.Y - P1.Y);

			 if(dp1dp2 == 0)
			{
				if (P1.X > P2.X)
				{
					var temp = P1;
					P1 = P2;
					P2 = temp;
					var tempv = V1;
					V1 = V2;
					V2 = tempv;
				}
				for (var y = (int)P1.Y; y < (int)P3.Y; y++)
					ScanLineValue(triangle, (int)y, V1, V3, V2, V3, scene, ort, types);
			}
			
			var temp1 = P1;var temp2 = P2;
			var temp3 = P3;

			if (dp1dp2 > dp1dp3)
			{
				for (var y = (int)P1.Y;y<=(int)P3.Y;y++)
				{
					if (y<P2.Y)
						ScanLineValue(triangle, y, V1, V3, V1, V2, scene,ort, types);
					else
						ScanLineValue(triangle, y, V1, V3, V2, V3, scene,ort, types);
				}
			}
			else
			{
				for (var y = (int)P1.Y; y<=(int)P3.Y;y++)
				{
					if (y < P2.Y)
						ScanLineValue(triangle, y, V1, V2, V1, V3, scene,ort, types);
					else
						ScanLineValue(triangle, y, V2, V3, V1, V3, scene,ort, types);
				}
			}
		}

		public void ScanLineValue(Triangle triangle, int y, Vertex v1, Vertex v2, Vertex v3, Vertex v4, Scene scene,Triangle ort, FancType types)
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

			float colk1 = MathUntily.Lerp(v1.ColK, v2.ColK, r1);
			float colk2 = MathUntily.Lerp(v3.ColK, v4.ColK, r2);
		   
			Color c3 = new Color();

			Vector4 pos1 = MathUntily.Lerp(v1.Position, v2.Position,r1);
			Vector4 pos2 = MathUntily.Lerp(v3.Position, v4.Position, r2);

			Vector4 nor1 = MathUntily.Lerp(v1.Normal, v2.Normal, r1);
			Vector4 nor2 = MathUntily.Lerp(v3.Normal, v4.Normal, r2);
			Vector4 pos3 = new Vector4();
			Vector4 nor3 = new Vector4();

			//开始计算线性方程的系数
			ort.PreCallWeight();
			
			for (int x = dx1; x < dx2; x++)
			{
				float r3 = Clamp((float)(x - dx1) / (dx2 - dx1));
				pos3 = MathUntily.Lerp(pos1, pos2, r3);
				float z = MathUntily.Lerp(z1, z2, r3);
		
				ort.CallWeight(new Vector4(x, y, z, 0));
				nor3 = ort.GetNormal();

				Light light = scene.Lights;
				float d = 0;
				if (scene.IsUseLight == false || light == null)
				{
					c3 = MathUntily.Lerp(c1, c2, r3);
				}
				else
				{
					d = light.ComputeNDotL(nor3, nor3);
					c3 = MathUntily.Lerp(c1, c2, r3) * light.LightColorV(d);
				}

				userColor = c3;
				if(device.renderMode == RenderMode.TEXTURED || device.renderMode == RenderMode.CUBETEXTURED)
				{
					ort.CallWeight(new Vector4(x, y, 0, 0));
					Vector4 uv = ort.GetUV();
					FancType typ = types;
					if (device.renderMode == RenderMode.TEXTURED)
						typ = FancType.NONE;

					RenderTexture texture = scene.GetTextureByFace(typ);
					if (scene.IsUseLight == false || light == null)
						userColor = texture.GetPixelColor(uv.X, uv.Y);
					else
						userColor = texture.GetPixelColor(uv.X, uv.Y) * light.LightColorV(d);
				}

				this.device.DrawPoint(new Vector4(x, y, z, 0), userColor);
			}
		}
		
		/// <summary>
		/// 计算范围值
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
