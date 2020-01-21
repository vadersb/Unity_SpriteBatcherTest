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
		public readonly Vector2[] m_UVs_Original;
		public readonly Vector2[] m_UVs;
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
			m_UVs_Original = new Vector2[VerticesCount];
			m_UVs = new Vector2[VerticesCount];
			m_Colors = new Color[VerticesCount];
			m_Triangles = new ushort[IndicesCount];
			m_Width = m_Height = 0;
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
			m_UVs_Original[Index_TopLeft] = new Vector2(0.0f, 1.0f);
			m_UVs_Original[Index_TopRight] = new Vector2(1.0f, 1.0f);
			m_UVs_Original[Index_BottomLeft] = new Vector2(0.0f, 0.0f);
			m_UVs_Original[Index_BottomRight] = new Vector2(1.0f, 0.0f);

			UVs_Reset();

			//colors
			Colors_Reset();

			//triangles
			m_Triangles[0] = 0;
			m_Triangles[1] = 1;
			m_Triangles[2] = 2;
			m_Triangles[4] = 2;
			m_Triangles[5] = 1;
			m_Triangles[6] = 3;
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
		public void UVs_Reset()
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UVs[i] = m_UVs_Original[i];
			}
		}

		public void UVs_Move(Vector2 delta)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UVs[i] += delta;
			}
		}

		public void UVs_MoveX(float delta)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UVs[i].x += delta;
			}
		}

		public void UVs_MoveY(float delta)
		{
			for (int i = 0; i < VerticesCount; i++)
			{
				m_UVs[i].y += delta;
			}
		}

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
				Debug.LogError("Unsupported sprite packing mode: " + sprite.packingMode);
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
				m_UVs_Original[i] = spriteUVs[i];
			}

			Recalculate_WidthHeight();
			Vertices_Reset();
			UVs_Reset();

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


		//------------------------------------------------------------
		//Helpers
		public void Recalculate_WidthHeight()
		{
			m_Width = m_Vertices[0].GetDistanceTo(m_Vertices[1]);
			m_Height = m_Vertices[0].GetDistanceTo(m_Vertices[2]);
		}
	}
}