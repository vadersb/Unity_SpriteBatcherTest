using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace vadersb.utils.unity.jobs
{
	public struct SpriteData
	{
		//vertices
		public float2 v0 { get; }
		public float2 v1 { get; }
		public float2 v2 { get; }
		public float2 v3 { get; }
		
		//texture coords
		public Vector2 t0 { get; }
		public Vector2 t1 { get; }
		public Vector2 t2 { get; }
		public Vector2 t3 { get; }

		public SpriteData(Sprite sprite)
		{
			//validation
			Debug.Assert(sprite != null);
			Debug.Assert(sprite.vertices.Length == 4);
			
			//vertices
			v0 = sprite.vertices[0];
			v1 = sprite.vertices[1];
			v2 = sprite.vertices[2];
			v3 = sprite.vertices[3];
			
			//texture coords
			t0 = sprite.uv[0];
			t1 = sprite.uv[1];
			t2 = sprite.uv[2];
			t3 = sprite.uv[3];
		}
	}
}