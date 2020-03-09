using UnityEngine;
using vadersb.utils;

namespace vadersb.utils.unity
{
	public class SpriteQuad
	{
		public const int VerticesCount = 4;
		public const int IndicesCount = 6;

		public const int Index_TopLeft = 0;
		public const int Index_TopRight = 1;
		public const int Index_BottomLeft = 2;
		public const int Index_BottomRight = 3;

		//INDICES:
		//0 1 2
		//2 1 3

		//VERTICES
		//0---1
		//|  /|
		//| / |
		//|/  |
		//2---3

		//UVS
		//[0;1]   [1;1]
		//             
		//             
		//[0;0]   [1;0]
		public readonly Vector2[] m_Vertices_Original;
		public readonly Vector2[] m_Vertices;
		public readonly Vector2[] m_UV1_Original;
		public readonly Vector2[] m_UV1;

		public readonly Vector2[] m_UV2;
		public readonly Vector2[] m_UV3;
		public readonly Vector2[] m_UV4;

		public readonly Color[] m_Colors;
		public readonly ushort[] m_Triangles;

		private float m_Width;
		private float m_Height;

		public float Width => m_Width;
		public float Height => m_Height;

		public SpriteQuad()
		{
			m_Vertices_Original = new Vector2[VerticesCount];
			m_Vertices = new Vector2[VerticesCount];
			m_UV1_Original = new Vector2[VerticesCount];
			m_UV1 = new Vector2[VerticesCount];
			m_UV2 = new Vector2[VerticesCount];
			m_UV3 = new Vector2[VerticesCount];
			m_UV4 = new Vector2[VerticesCount];
			m_Colors = new Color[VerticesCount];
			m_Triangles = new ushort[IndicesCount];
			m_Width = m_Height = 0;

			//triangles
			m_Triangles[0] = 0;
			m_Triangles[1] = 1;
			m_Triangles[2] = 2;
			m_Triangles[3] = 2;
			m_Triangles[4] = 1;
			m_Triangles[5] = 3;
		}


		public void Reset()
		{
			//vertices
			m_Vertices_Original[Index_TopLeft] = new Vector2(-0.5f, 0.5f);
			m_Vertices_Original[Index_TopRight] = new Vector2(0.5f, 0.5f);
			m_Vertices_Original[Index_BottomLeft] = new Vector2(-0.5f, -0.5f);
			m_Vertices_Original[Index_BottomRight] = new Vector2(0.5f, -0.5f);

			Recalculate_WidthHeight();
			Vertices_Reset();

			//uvs
			m_UV1_Original[Index_TopLeft] = new Vector2(0.0f, 1.0f);
			m_UV1_Original[Index_TopRight] = new Vector2(1.0f, 1.0f);
			m_UV1_Original[Index_BottomLeft] = new Vector2(0.0f, 0.0f);
			m_UV1_Original[Index_BottomRight] = new Vector2(1.0f, 0.0f);

			UV1_Reset();

			//uv2
			UV2_Reset();

			//uv3
			UV3_Reset();

			//uv4
			UV4_Reset();

			//colors
			Colors_Reset();

			//triangles
			m_Triangles[0] = 0;
			m_Triangles[1] = 1;
			m_Triangles[2] = 2;
			m_Triangles[3] = 2;
			m_Triangles[4] = 1;
			m_Triangles[5] = 3;
		}

		//------------------------------------------------------------
		//Vertices
		public void Vertices_Reset()
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i] = m_Vertices_Original[i];
			}
		}

		public void Vertices_Move(Vector2 delta)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i] += delta;
			}
		}

		public void Vertices_MoveX(float deltaX)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i].x += deltaX;
			}
		}

		public void Vertices_MoveY(float deltaY)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i].y += deltaY;
			}
		}

		/// <summary>
		///   <para>Rotates vertices by supplied angle in radians</para>
		/// </summary>
		/// <param name="angle">angle in radians. positive angle == CCW rotation in Unity!</param>
		public void Vertices_Rotate(float angle)
		{
			//angle = -angle; //cw rotation is negative in Unity

			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i] = VectorTools.Rotate(m_Vertices[i], angle);
			}
		}

		public void Vertices_Scale(Vector2 scale)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i] *= scale;
			}
		}

		public void Vertices_Scale(float scale)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i] *= scale;
			}
		}

		public void Vertices_ScaleX(float scaleX)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i].x *= scaleX;
			}
		}

		public void Vertices_ScaleY(float scaleY)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices[i].y *= scaleY;
			}
		}

		/// <summary>
		///   <para>Setup quad as a line from one point to another with width == SpriteQuad.Width</para>
		/// </summary>
		/// <param name="from">a point between vertices 2 and 3</param>
		/// <param name="to">a point between vertices 0 and 1</param>
		public void Vertices_FromTo(Vector2 from, Vector2 to)
		{
			Vector2 fromTo = to - from;

			Vector2 normal = fromTo.GetNormal();
			normal *= (m_Width * 0.5f);

			//from: vertices 2 and 3
			m_Vertices[Index_BottomLeft] = from - normal;
			m_Vertices[Index_BottomRight] = from + normal;

			//to: vertices 0 and 1
			m_Vertices[Index_TopLeft] = to - normal;
			m_Vertices[Index_TopRight] = to + normal;
		}

		/// <summary>
		///   <para>Setup quad as a line from one point to another with width == SpriteQuad.Width</para>
		/// </summary>
		/// <param name="from">a point between vertices 2 and 3</param>
		/// <param name="to">a point between vertices 0 and 1</param>
		/// <param name="fromWidth">from side width</param>
		/// <param name="toWidth">to side width</param>
		public void Vertices_FromTo(Vector2 from, Vector2 to, float fromWidth, float toWidth)
		{
			Vector2 fromTo = to - from;

			Vector2 normal = fromTo.GetNormal();
			Vector2 normalFrom = normal *= (fromWidth * 0.5f);
			Vector2 normalTo = normal *= (toWidth * 0.5f);

			//from: vertices 2 and 3
			m_Vertices[Index_BottomLeft] = from - normalFrom;
			m_Vertices[Index_BottomRight] = from + normalFrom;

			//to: vertices 0 and 1
			m_Vertices[Index_TopLeft] = to - normalTo;
			m_Vertices[Index_TopRight] = to + normalTo;
		}

		//------------------------------------------------------------
		//UVs
		public void UV1_Reset()
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV1[i] = m_UV1_Original[i];
			}
		}

		public void UV1_Move(Vector2 delta)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV1[i] += delta;
			}
		}

		public void UV1_MoveX(float delta)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV1[i].x += delta;
			}
		}

		public void UV1_MoveY(float delta)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV1[i].y += delta;
			}
		}


		public void UV1_SetUniform(Vector2 value)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV1[i] = value;
			}
		}

		public void UV1_SetFromAnotherOriginal(SpriteQuad anotherQuad)
		{
			Debug.Assert(anotherQuad != null);

			m_UV1[Index_TopLeft] = anotherQuad.m_UV1_Original[Index_TopLeft];
			m_UV1[Index_TopRight] = anotherQuad.m_UV1_Original[Index_TopRight];
			m_UV1[Index_BottomLeft] = anotherQuad.m_UV1_Original[Index_BottomLeft];
			m_UV1[Index_BottomRight] = anotherQuad.m_UV1_Original[Index_BottomRight];
		}

		//------------------------------------------------------------
		//UV2
		public void UV2_Reset()
		{
			m_UV2[Index_TopLeft] = new Vector2(0.0f, 1.0f);
			m_UV2[Index_TopRight] = new Vector2(1.0f, 1.0f);
			m_UV2[Index_BottomLeft] = new Vector2(0.0f, 0.0f);
			m_UV2[Index_BottomRight] = new Vector2(1.0f, 0.0f);
		}

		public void UV2_SetFromOriginal()
		{
			m_UV2[Index_TopLeft] = m_UV1_Original[Index_TopLeft];
			m_UV2[Index_TopRight] = m_UV1_Original[Index_TopRight];
			m_UV2[Index_BottomLeft] = m_UV1_Original[Index_BottomLeft];
			m_UV2[Index_BottomRight] = m_UV1_Original[Index_BottomRight];
		}

		public void UV2_SetFromAnotherOriginal(SpriteQuad anotherQuad)
		{
			Debug.Assert(anotherQuad != null);

			m_UV2[Index_TopLeft] = anotherQuad.m_UV1_Original[Index_TopLeft];
			m_UV2[Index_TopRight] = anotherQuad.m_UV1_Original[Index_TopRight];
			m_UV2[Index_BottomLeft] = anotherQuad.m_UV1_Original[Index_BottomLeft];
			m_UV2[Index_BottomRight] = anotherQuad.m_UV1_Original[Index_BottomRight];
		}

		public void UV2_SetUniform(Vector2 value)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV2[i] = value;
			}
		}

		//------------------------------------------------------------
		//UV3
		public void UV3_Reset()
		{
			m_UV3[Index_TopLeft] = new Vector2(0.0f, 1.0f);
			m_UV3[Index_TopRight] = new Vector2(1.0f, 1.0f);
			m_UV3[Index_BottomLeft] = new Vector2(0.0f, 0.0f);
			m_UV3[Index_BottomRight] = new Vector2(1.0f, 0.0f);
		}

		public void UV3_SetFromOriginal()
		{
			m_UV3[Index_TopLeft] = m_UV1_Original[Index_TopLeft];
			m_UV3[Index_TopRight] = m_UV1_Original[Index_TopRight];
			m_UV3[Index_BottomLeft] = m_UV1_Original[Index_BottomLeft];
			m_UV3[Index_BottomRight] = m_UV1_Original[Index_BottomRight];
		}

		public void UV3_SetFromAnotherOriginal(SpriteQuad anotherQuad)
		{
			Debug.Assert(anotherQuad != null);

			m_UV3[Index_TopLeft] = anotherQuad.m_UV1_Original[Index_TopLeft];
			m_UV3[Index_TopRight] = anotherQuad.m_UV1_Original[Index_TopRight];
			m_UV3[Index_BottomLeft] = anotherQuad.m_UV1_Original[Index_BottomLeft];
			m_UV3[Index_BottomRight] = anotherQuad.m_UV1_Original[Index_BottomRight];
		}
		
		public void UV3_SetUniform(Vector2 value)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV3[i] = value;
			}
		}

		//------------------------------------------------------------
		//UV4
		public void UV4_Reset()
		{
			m_UV4[Index_TopLeft] = new Vector2(0.0f, 1.0f);
			m_UV4[Index_TopRight] = new Vector2(1.0f, 1.0f);
			m_UV4[Index_BottomLeft] = new Vector2(0.0f, 0.0f);
			m_UV4[Index_BottomRight] = new Vector2(1.0f, 0.0f);
		}

		public void UV4_SetFromOriginal()
		{
			m_UV4[Index_TopLeft] = m_UV1_Original[Index_TopLeft];
			m_UV4[Index_TopRight] = m_UV1_Original[Index_TopRight];
			m_UV4[Index_BottomLeft] = m_UV1_Original[Index_BottomLeft];
			m_UV4[Index_BottomRight] = m_UV1_Original[Index_BottomRight];
		}

		public void UV4_SetFromAnotherOriginal(SpriteQuad anotherQuad)
		{
			Debug.Assert(anotherQuad != null);

			m_UV4[Index_TopLeft] = anotherQuad.m_UV1_Original[Index_TopLeft];
			m_UV4[Index_TopRight] = anotherQuad.m_UV1_Original[Index_TopRight];
			m_UV4[Index_BottomLeft] = anotherQuad.m_UV1_Original[Index_BottomLeft];
			m_UV4[Index_BottomRight] = anotherQuad.m_UV1_Original[Index_BottomRight];
		}
		
		public void UV4_SetUniform(Vector2 value)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UV4[i] = value;
			}
		}

		//------------------------------------------------------------
		//Colors
		public void Colors_Reset()
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Colors[i] = Color.white;
			}
		}

		public void Colors_Reset(Color color)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Colors[i] = color;
			}
		}

		public void Colors_SetAlpha(float alpha)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Colors[i].a = alpha;
			}
		}


		public void Colors_SetAlphaFromTo(float fromAlpha, float toAlpha)
		{
			m_Colors[Index_BottomLeft].a = fromAlpha;
			m_Colors[Index_BottomRight].a = fromAlpha;
			m_Colors[Index_TopLeft].a = toAlpha;
			m_Colors[Index_TopRight].a = toAlpha;
		}

		//------------------------------------------------------------
		//Setup
		public void SetupFromSprite(Sprite sprite)
		{
			if (sprite == null)
			{
				Debug.LogError("sprite is null!");
				Reset();
				return;
			}

			if (sprite.packingMode != SpritePackingMode.Rectangle)
			{
				Debug.LogError("Unsupported sprite packing mode: " + sprite.packingMode + " in sprite: " + sprite.name);
				Reset();
				return;
			}

			//-----
			//vertices
			var spriteVertices = sprite.vertices;

			Debug.Assert(spriteVertices.Length == VerticesCount);

			//uvs
			var spriteUVs = sprite.uv;

			Debug.Assert(spriteUVs.Length == VerticesCount);


			//copying vertices and uvs
			for (int i = 0; i < VerticesCount; i++)
			{
				m_Vertices_Original[i] = spriteVertices[i];
				m_UV1_Original[i] = spriteUVs[i];
			}

			Recalculate_WidthHeight();
			Vertices_Reset();
			UV1_Reset();
			UV2_Reset();
			UV3_Reset();
			UV4_Reset();

			//-----
			//triangles
			var spriteTriangles = sprite.triangles;

			Debug.Assert(spriteTriangles.Length == IndicesCount);

			//copying triangles
			for (int i = 0; i < IndicesCount; i++)
			{
#if DEBUG
				if (m_Triangles[i] != spriteTriangles[i])
				{
					Debug.LogWarning("Unexpected index data in sprite!");
				}
#endif
				m_Triangles[i] = spriteTriangles[i];
			}

			//-----
			//colors reset
			Colors_Reset();
		}

		public void SetupFromSpriteBatcher(SpriteBatcher spriteBatcher, string spriteName)
		{
			var curSprite = spriteBatcher.GetSprite(spriteName);

			if (curSprite != null)
			{
				SetupFromSprite(curSprite);
			}
			else
			{
				Debug.LogError("Failed to find sprite " + spriteName + " in SpriteBatcher!");
			}
		}


		//------------------------------------------------------------
		//Helpers
		public void Recalculate_WidthHeight()
		{
			m_Width = m_Vertices_Original[0].GetDistanceTo(m_Vertices_Original[1]);
			m_Height = m_Vertices_Original[0].GetDistanceTo(m_Vertices_Original[2]);
		}
	}
}