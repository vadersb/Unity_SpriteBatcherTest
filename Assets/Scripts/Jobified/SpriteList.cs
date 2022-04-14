using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace vadersb.utils.unity.jobs
{
	[Serializable]
	public class SpriteList
	{
		[SerializeField]
		private List<Sprite> m_Sprites;

		private NativeArray<SpriteData> m_SpriteDataArray;

		private Texture m_Texture = null;
		
		public void Init()
		{
			Debug.Assert(m_Sprites != null);
			
			//0. texture
			if (m_Sprites.Count == 0)
			{
				m_Texture = null;
			}
			else
			{
				m_Texture = m_Sprites[0].texture;
			}
			
			//1. sprites list validation
			foreach (var sprite in m_Sprites)
			{
				if (sprite.packed == false)
				{
					Debug.LogError("Sprite " + sprite.name + " is not packed!");
				}

				if (sprite.texture != m_Texture)
				{
					Debug.LogError("Sprite texture is " + sprite.texture.name + " which differs from the first sprite texture: ");

					if (m_Texture != null)
					{
						Debug.LogError(m_Texture.name + ". All sprites should have the same texture (belong to the same atlas)!");
					}
				}

				if (sprite.vertices.Length != 4)
				{
					Debug.LogError("Sprite " + sprite.name + " is tightly packed! Only rectangle packing is supported for sprite batching!");
				}
			}
			
			//2. sprite data array init
			if (m_SpriteDataArray.IsCreated)
			{
				m_SpriteDataArray.Dispose();
			}
			
			m_SpriteDataArray = new NativeArray<SpriteData>(m_Sprites.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);

			for (int i = 0; i < m_Sprites.Count; i++)
			{
				m_SpriteDataArray[i] = new SpriteData(m_Sprites[i]);
			}
		}


		public void Dispose()
		{
			m_SpriteDataArray.Dispose();
		}
		
		public int SpritesCount
		{
			get
			{
				if (m_Sprites == null)
				{
					return 0;
				}
				else
				{
					return m_Sprites.Count;
				}
			}
		}

		public NativeArray<SpriteData> SpriteDataArray => m_SpriteDataArray;

		public Texture Texture => m_Texture;


	}
	
}