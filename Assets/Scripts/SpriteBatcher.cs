using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


namespace vadersb.utils.unity
{
	public class SpriteBatcher : MonoBehaviour
	{
		private const string ColorProperty = "_RendererColor";
		private const string TextureProperty = "_MainTex";

		private const int DefaultCapacity = 4096;

		[SerializeField]
		private SpriteAtlas m_SpriteAtlas = null;

		[SerializeField]
		private Color m_Color;


		//material property block
		private MaterialPropertyBlock m_MaterialPropertyBlock;


		//sprite data
		private Sprite[] m_Sprites;
		private Vector2[][] m_Sprites_Vertices;
		private Vector2[][] m_Sprites_UVs;
		private ushort[][] m_Sprites_Triangles;

		//dynamic mesh
		private Mesh m_Mesh;

		private int m_Dynamic_IndexOffset;
		private List<Vector3> m_Dynamic_Vertices;
		private List<Vector2> m_Dynamic_UVs;
		private List<Color> m_Dynamic_Colors;
		private List<int> m_Dynamic_Triangles;


		private void Awake()
		{
			RefreshSpritesList();
			RefreshMaterialPropertyBlock();
			ApplyMaterialPropertyBlock();

			//creating a mesh
			m_Mesh = new Mesh();
			m_Mesh.name = "SpriteBatcher dynamic mesh";
			m_Mesh.MarkDynamic();

			//mesh filter
			var meshFilter = GetComponent<MeshFilter>();

			Debug.Assert(meshFilter != null);

			meshFilter.mesh = m_Mesh;

			//creating dynamic buffers
			m_Dynamic_IndexOffset = 0;
			m_Dynamic_Vertices = new List<Vector3>(DefaultCapacity);
			m_Dynamic_UVs = new List<Vector2>(DefaultCapacity);
			m_Dynamic_Colors = new List<Color>(DefaultCapacity);
			m_Dynamic_Triangles = new List<int>(DefaultCapacity);
		}


		private void OnValidate()
		{
			RefreshSpritesList();

			if (m_MaterialPropertyBlock != null)
			{
				RefreshMaterialPropertyBlock();
				ApplyMaterialPropertyBlock();
			}
		}

		//----------------------------------------------------------------------
		//Settings
		public Color Color
		{
			get => m_Color;
			set
			{
				m_Color = value;
				if (m_MaterialPropertyBlock != null)
				{
					// ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
					m_MaterialPropertyBlock.SetColor(ColorProperty, m_Color);
					ApplyMaterialPropertyBlock();
				}
			}
		}

		//----------------------------------------------------------------------
		//Finalizing the mesh
		public void CompleteMesh()
		{
			m_Mesh.Clear();

			m_Mesh.SetVertices(m_Dynamic_Vertices);
			m_Mesh.SetColors(m_Dynamic_Colors);
			m_Mesh.SetUVs(0, m_Dynamic_UVs);
			m_Mesh.SetTriangles(m_Dynamic_Triangles, 0, false);

			m_Dynamic_Vertices.Clear();
			m_Dynamic_Colors.Clear();
			m_Dynamic_UVs.Clear();
			m_Dynamic_Triangles.Clear();
			m_Dynamic_IndexOffset = 0;
		}

		public bool IsComplete => m_Dynamic_Vertices.Count == 0;

		//----------------------------------------------------------------------
		//Getting sprite index
		public int GetSpriteIndex(Sprite sprite)
		{
			int nameSearchResult = GetSpriteIndex(sprite.name);

			if (nameSearchResult != -1)
			{
				return nameSearchResult;
			}

			for (int i = 0; i < m_SpriteAtlas.spriteCount; i++)
			{
				if (m_Sprites[i] == sprite)
				{
					return i;
				}
			}

			#if DEBUG
			Debug.LogError("Failed to find sprite: " + sprite.name + " in atlas: " + m_SpriteAtlas.name);
			#endif

			return -1;
		}

		public int GetSpriteIndex(string spriteName)
		{
			spriteName += "(Clone)";

			for (int i = 0; i < m_SpriteAtlas.spriteCount; i++)
			{
				if (m_Sprites[i].name == spriteName)
				{
					return i;
				}
			}

			#if DEBUG
			Debug.LogError("Failed to find sprite: " + spriteName + " in atlas: " + m_SpriteAtlas.name);
			#endif

			return -1;
		}

		//----------------------------------------------------------------------
		//Drawing Sprites
		public void DrawSprite(int spriteIndex, Vector2 position, Color color)
		{
			//-----
			//index test
			if (spriteIndex < 0 || spriteIndex >= m_Sprites_Vertices.Length)
			{
#if DEBUG
				Debug.LogError("invalid spriteIndex: " + spriteIndex + "");
#endif
				return;
			}

			//-----
			//Adding sprite geometry
			var vertices = m_Sprites_Vertices[spriteIndex];
			var uvs = m_Sprites_UVs[spriteIndex];
			var triangles = m_Sprites_Triangles[spriteIndex];

			int count = vertices.Length;

			for (int i = 0; i < count; i++)
			{
				//vertex position
				var curVertex = vertices[i];

				curVertex += position; //translation

				m_Dynamic_Vertices.Add(new Vector3(curVertex.x, curVertex.y, 0.0f));

				//uv
				var curUV = uvs[i];

				m_Dynamic_UVs.Add(curUV);

				//color
				m_Dynamic_Colors.Add(color);
			}


			count = triangles.Length;

			for (int i = 0; i < count; i++)
			{
				//index
				int curIndex = triangles[i];

				m_Dynamic_Triangles.Add(curIndex + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += vertices.Length;
		}

		public void DrawSprite(int spriteIndex, Vector2 position, float rotationAngle, Color color)
		{
			//-----
			//index test
			if (spriteIndex < 0 || spriteIndex >= m_Sprites_Vertices.Length)
			{
#if DEBUG
				Debug.LogError("invalid spriteIndex: " + spriteIndex + "");
#endif
				return;
			}


			//-----
			//Adding sprite geometry
			var vertices = m_Sprites_Vertices[spriteIndex];
			var uvs = m_Sprites_UVs[spriteIndex];
			var triangles = m_Sprites_Triangles[spriteIndex];

			int count = vertices.Length;

			for (int i = 0; i < count; i++)
			{
				//vertex position
				var curVertex = vertices[i];

				curVertex = VectorTools.Rotate(curVertex, rotationAngle); //rotation

				curVertex += position; //translation

				m_Dynamic_Vertices.Add(new Vector3(curVertex.x, curVertex.y, 0.0f));

				//uv
				var curUV = uvs[i];

				m_Dynamic_UVs.Add(curUV);

				//color
				m_Dynamic_Colors.Add(color);
			}


			count = triangles.Length;

			for (int i = 0; i < count; i++)
			{
				//index
				int curIndex = triangles[i];

				m_Dynamic_Triangles.Add(curIndex + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += vertices.Length;
		}

		public void DrawSprite(int spriteIndex, Vector2 position, float rotationAngle, Vector2 scale, Color color)
		{
			//-----
			//index test
			if (spriteIndex < 0 || spriteIndex >= m_Sprites_Vertices.Length)
			{
#if DEBUG
				Debug.LogError("invalid spriteIndex: " + spriteIndex + "");
#endif
				return;
			}


			//-----
			//Adding sprite geometry
			var vertices = m_Sprites_Vertices[spriteIndex];
			var uvs = m_Sprites_UVs[spriteIndex];
			var triangles = m_Sprites_Triangles[spriteIndex];

			int count = vertices.Length;

			for (int i = 0; i < count; i++)
			{
				//vertex position
				var curVertex = vertices[i];

				curVertex = VectorTools.Rotate(curVertex, rotationAngle); //rotation

				curVertex *= scale; //scale

				curVertex += position; //translation

				m_Dynamic_Vertices.Add(new Vector3(curVertex.x, curVertex.y, 0.0f));

				//uv
				var curUV = uvs[i];

				m_Dynamic_UVs.Add(curUV);

				//color
				m_Dynamic_Colors.Add(color);
			}


			count = triangles.Length;

			for (int i = 0; i < count; i++)
			{
				//index
				int curIndex = triangles[i];

				m_Dynamic_Triangles.Add(curIndex + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += vertices.Length;
		}

		public void DrawSprite(int spriteIndex, Vector2 position, float rotationAngle, float scale, Color color)
		{
			//-----
			//index test
			if (spriteIndex < 0 || spriteIndex >= m_Sprites_Vertices.Length)
			{
#if DEBUG
				Debug.LogError("invalid spriteIndex: " + spriteIndex + "");
#endif
				return;
			}


			//-----
			//Adding sprite geometry
			var vertices = m_Sprites_Vertices[spriteIndex];
			var uvs = m_Sprites_UVs[spriteIndex];
			var triangles = m_Sprites_Triangles[spriteIndex];

			int count = vertices.Length;

			for (int i = 0; i < count; i++)
			{
				//vertex position
				var curVertex = vertices[i];

				curVertex = VectorTools.Rotate(curVertex, rotationAngle); //rotation

				curVertex *= scale; //scale

				curVertex += position; //translation

				m_Dynamic_Vertices.Add(new Vector3(curVertex.x, curVertex.y, 0.0f));

				//uv
				var curUV = uvs[i];

				m_Dynamic_UVs.Add(curUV);

				//color
				m_Dynamic_Colors.Add(color);
			}


			count = triangles.Length;

			for (int i = 0; i < count; i++)
			{
				//index
				int curIndex = triangles[i];

				m_Dynamic_Triangles.Add(curIndex + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += vertices.Length;
		}

		public void DrawSpriteQuad(SpriteQuad spriteQuad)
		{
			//-----
			//index test
			if (spriteQuad == null)
			{
#if DEBUG
				Debug.LogError("spriteQuad is null!");
#endif
				return;
			}

			//-----
			//Adding sprite geometry

			for (int i = 0; i < SpriteQuad.VerticesCount; i++)
			{
				//vertex position
				m_Dynamic_Vertices.Add(spriteQuad.m_Vertices[i]);

				//uv
				m_Dynamic_UVs.Add(spriteQuad.m_UVs[i]);

				//color
				m_Dynamic_Colors.Add(spriteQuad.m_Colors[i]);
			}


			for (int i = 0; i < SpriteQuad.IndicesCount; i++)
			{
				//index
				m_Dynamic_Triangles.Add(spriteQuad.m_Triangles[i] + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += SpriteQuad.VerticesCount;
		}

		public void DrawMesh(Vector2[] vertices, Vector2[] uvs, Color[] colors, int[] triangles)
		{
			var verticesCount = vertices.Length;

			if (verticesCount == 0)
			{
				return;
			}

			Debug.Assert(uvs.Length == verticesCount);
			Debug.Assert(colors.Length == verticesCount);

			var indicesCount = triangles.Length;

			if (indicesCount == 0)
			{
				return;
			}

			//-----
			//Adding sprite geometry

			for (int i = 0; i < verticesCount; i++)
			{
				//vertex position
				m_Dynamic_Vertices.Add(vertices[i]);

				//uv
				m_Dynamic_UVs.Add(uvs[i]);

				//color
				m_Dynamic_Colors.Add(colors[i]);
			}
			
			for (int i = 0; i < indicesCount; i++)
			{
				//index
				m_Dynamic_Triangles.Add(triangles[i] + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += verticesCount;
		}

		public void DrawMesh(Vector2[] vertices, Vector2[] uvs, Color color, int[] triangles)
		{
			var verticesCount = vertices.Length;

			if (verticesCount == 0)
			{
				return;
			}

			Debug.Assert(uvs.Length == verticesCount);

			var indicesCount = triangles.Length;

			if (indicesCount == 0)
			{
				return;
			}

			//-----
			//Adding sprite geometry

			for (int i = 0; i < verticesCount; i++)
			{
				//vertex position
				m_Dynamic_Vertices.Add(vertices[i]);

				//uv
				m_Dynamic_UVs.Add(uvs[i]);

				//color
				m_Dynamic_Colors.Add(color);
			}
			
			for (int i = 0; i < indicesCount; i++)
			{
				//index
				m_Dynamic_Triangles.Add(triangles[i] + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += verticesCount;
		}

		public void DrawMesh(List<Vector2> vertices, List<Vector2> uvs, List<Color> colors, List<int> triangles)
		{
			if (vertices == null) return;

			var verticesCount = vertices.Count;

			if (verticesCount == 0)
			{
				return;
			}

			Debug.Assert(uvs.Count == verticesCount);
			Debug.Assert(colors.Count == verticesCount);

			var indicesCount = triangles.Count;

			if (indicesCount == 0)
			{
				return;
			}

			//-----
			//Adding sprite geometry

			for (int i = 0; i < verticesCount; i++)
			{
				//vertex position
				m_Dynamic_Vertices.Add(vertices[i]);

				//uv
				m_Dynamic_UVs.Add(uvs[i]);

				//color
				m_Dynamic_Colors.Add(colors[i]);
			}
			
			for (int i = 0; i < indicesCount; i++)
			{
				//index
				m_Dynamic_Triangles.Add(triangles[i] + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += verticesCount;
		}

		public void DrawMesh(List<Vector2> vertices, List<Vector2> uvs, Color color, List<int> triangles)
		{
			if (vertices == null) return;

			var verticesCount = vertices.Count;

			if (verticesCount == 0)
			{
				return;
			}

			Debug.Assert(uvs.Count == verticesCount);

			var indicesCount = triangles.Count;

			if (indicesCount == 0)
			{
				return;
			}

			//-----
			//Adding sprite geometry

			for (int i = 0; i < verticesCount; i++)
			{
				//vertex position
				m_Dynamic_Vertices.Add(vertices[i]);

				//uv
				m_Dynamic_UVs.Add(uvs[i]);

				//color
				m_Dynamic_Colors.Add(color);
			}
			
			for (int i = 0; i < indicesCount; i++)
			{
				//index
				m_Dynamic_Triangles.Add(triangles[i] + m_Dynamic_IndexOffset);
			}

			m_Dynamic_IndexOffset += verticesCount;
		}

		//----------------------------------------------------------------------
		//Helper functions
		private void RefreshSpritesList()
		{
			m_Sprites = null;

			if (m_SpriteAtlas == null) return;

			m_Sprites = new Sprite[m_SpriteAtlas.spriteCount];

			m_SpriteAtlas.GetSprites(m_Sprites);

			//sprites vertices
			m_Sprites_Vertices = new Vector2[m_Sprites.Length][];

			for (int i = 0; i < m_Sprites.Length; i++)
			{
				m_Sprites_Vertices[i] = m_Sprites[i].vertices;
			}

			//sprites uvs
			m_Sprites_UVs = new Vector2[m_Sprites.Length][];

			for (int i = 0; i < m_Sprites.Length; i++)
			{
				m_Sprites_UVs[i] = m_Sprites[i].uv;
			}

			//sprites triangles
			m_Sprites_Triangles = new ushort[m_Sprites.Length][];

			for (int i = 0; i < m_Sprites.Length; i++)
			{
				m_Sprites_Triangles[i] = m_Sprites[i].triangles;
			}
		}


		private void RefreshMaterialPropertyBlock()
		{
			m_MaterialPropertyBlock = new MaterialPropertyBlock();

			Texture2D texture = null;

			if (m_Sprites.Length > 0)
			{
				texture = m_Sprites[0].texture;
			}

			// ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
			m_MaterialPropertyBlock.SetTexture(TextureProperty, texture);
			// ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
			m_MaterialPropertyBlock.SetColor(ColorProperty, m_Color);
		}


		private void ApplyMaterialPropertyBlock()
		{
			var myRenderer = GetComponent<MeshRenderer>();

			if (myRenderer != null)
			{
				myRenderer.SetPropertyBlock(m_MaterialPropertyBlock, 0);
			}
		}
	}
}