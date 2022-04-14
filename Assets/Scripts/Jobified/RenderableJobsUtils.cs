using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace vadersb.utils.unity.jobs
{
	public static class RenderableJobsUtils
	{
		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct VertexData
		{
			public Vector3 m_Position;
			public Color32 m_Color;
			public Vector2 m_TextureCoords;
		
			public VertexData(Vector3 position, Color32 color, Vector2 textureCoords)
			{
				m_Position = position;
				m_Color = color;
				m_TextureCoords = textureCoords;
			}
		}

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct QuadVertexData
		{
			private VertexData m_V0;
			private VertexData m_V1;
			private VertexData m_V2;
			private VertexData m_V3;


			public QuadVertexData(VertexData v0, VertexData v1, VertexData v2, VertexData v3)
			{
				m_V0 = v0;
				m_V1 = v1;
				m_V2 = v2;
				m_V3 = v3;
			}
		}

		[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct QuadIndexData
		{
			private uint m_Index0;
			private uint m_Index1;
			private uint m_Index2;

			private uint m_Index3;
			private uint m_Index4;
			private uint m_Index5;


			public QuadIndexData(uint i0, uint i1, uint i2, uint i3, uint i4, uint i5)
			{
				m_Index0 = i0;
				m_Index1 = i1;
				m_Index2 = i2;

				m_Index3 = i3;
				m_Index4 = i4;
				m_Index5 = i5;
			}
		}
		
		private static readonly VertexAttributeDescriptor[] s_VertexLayout = new[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
			new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2)
		};

		public static VertexAttributeDescriptor[] VertexLayout => s_VertexLayout;


		public static float2 CalculatePosition(float2 localPosition, float2 worldPosition, float2 scale, float rotationAngle)
		{
			//scale
			float2 result = localPosition * scale;
			
			//rotation
			float curSin = math.sin(rotationAngle);
			float curCos = math.cos(rotationAngle);

			float rotatedX = result.x * curCos - result.y * curSin;
			float rotatedY = result.x * curSin + result.y * curCos;

			result = new float2(rotatedX, rotatedY);
			
			//translation
			result += worldPosition;

			//finally
			return result;
		}
		
		
		public static float2 CalculatePosition(float2 localPosition, float2 worldPosition, float2 scale, float rotationAngleSin, float rotationAngleCos)
		{
			//scale
			float2 result = localPosition * scale;
			
			//rotation
			float rotatedX = result.x * rotationAngleCos - result.y * rotationAngleSin;
			float rotatedY = result.x * rotationAngleSin + result.y * rotationAngleCos;

			result = new float2(rotatedX, rotatedY);
			
			//translation
			result += worldPosition;

			//finally
			return result;
		}
	}	
}

