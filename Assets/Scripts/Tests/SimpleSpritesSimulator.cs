using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vadersb.utils;
using vadersb.utils.unity;

public class SimpleSpritesSimulator : MonoBehaviour
{
	public Camera m_Camera;
	
	public float m_X_Min = -3;
	public float m_X_Max = 3;

	public float m_Y_Min = -3;
	public float m_Y_Max = 3;

	public float m_SpeedMin = 0.1f;
	public float m_SpeedMax = 1.0f;

	public int m_SpritesCount = 1000;

	public SpriteBatcher m_Batcher;

	public bool m_DebugUseFixedTimeDelta = true;
	
	private class SpriteData
	{
		public Vector2 m_Coords;
		public Color m_Color;

		public bool m_HorLeft;
		public bool m_VerUp;

		public float m_SpeedMult;

		public int m_SpriteIndex;

		public float m_Angle;

		public float m_Scale;
	}

	private List<SpriteData> m_Sprites;

	// Start is called before the first frame update
	void Start()
	{

		WeightedRandomizer<Color> m_Randomizer = new WeightedRandomizer<Color>();

		m_Randomizer.AddValue(Color.black, 1.0f);
		m_Randomizer.AddValue(Color.blue, 1.0f);
		m_Randomizer.AddValue(Color.green, 1.0f);
		m_Randomizer.AddValue(Color.red, 1.0f);
		m_Randomizer.AddValue(Color.white, 50.0f);
		
		float minX = m_X_Min;
		float maxX = m_X_Max;
		float minY = m_Y_Min;
		float maxY = m_Y_Max;
		
		//getting camera bounds
		if (m_Camera)
		{
			float halfSize = m_Camera.orthographicSize;
			float ratio = m_Camera.aspect;
			
			minY = -halfSize;
			maxY = halfSize;

			minX = -halfSize * ratio;
			maxX = halfSize * ratio;
		}
		
		
		//generate sprites
		m_Sprites = new List<SpriteData>(m_SpritesCount);

		for (int i = 0; i < m_SpritesCount; i++)
		{
			var newSprite = new SpriteData();

			newSprite.m_Coords.x = MathHelpers.Random_Float(minX, maxX);
			newSprite.m_Coords.y = MathHelpers.Random_Float(minY, maxY);
			newSprite.m_Color = m_Randomizer.GetRandomValue();
			newSprite.m_HorLeft = MathHelpers.Random_CheckChance(0.5f);
			newSprite.m_VerUp = MathHelpers.Random_CheckChance(0.5f);
			newSprite.m_SpeedMult = MathHelpers.Random_Factor();
			newSprite.m_SpeedMult = newSprite.m_SpeedMult * newSprite.m_SpeedMult * newSprite.m_SpeedMult * newSprite.m_SpeedMult * newSprite.m_SpeedMult * newSprite.m_SpeedMult;
			newSprite.m_SpriteIndex = MathHelpers.Random_Int(0, 1);
			newSprite.m_Angle = MathHelpers.Random_Angle();

			newSprite.m_Scale = Interpolation.Linear(0.5f, 2.0f, (Mathf.Pow(MathHelpers.Random_Factor(), 3.0f)));

			m_Sprites.Add(newSprite);



		}

	}


	// Update is called once per frame
	void Update()
	{
		
		float minX = m_X_Min;
		float maxX = m_X_Max;
		float minY = m_Y_Min;
		float maxY = m_Y_Max;

		float deltaTime = Time.deltaTime;
		
		//getting camera bounds
		if (m_Camera)
		{
			float halfSize = m_Camera.orthographicSize;
			float ratio = m_Camera.aspect;

			minY = -halfSize;
			maxY = halfSize;

			minX = -halfSize * ratio;
			maxX = halfSize * ratio;
		}
		
		//updating the sprites
		for (int i = 0; i < m_SpritesCount; i++)
		{
			float curSpeed = Interpolation.Linear(m_SpeedMin, m_SpeedMax, m_Sprites[i].m_SpeedMult) * deltaTime;

			var curSprite = m_Sprites[i];
			
			
			//hor
			if (curSprite.m_HorLeft == true)
			{
				curSprite.m_Coords.x -= curSpeed;
				if (curSprite.m_Coords.x <= minX)
				{
					curSprite.m_HorLeft = false;
				}
			}
			else
			{
				curSprite.m_Coords.x += curSpeed;
				if (curSprite.m_Coords.x >= maxX)
				{
					curSprite.m_HorLeft = true;
				}
			}
			
			//ver
			if (curSprite.m_VerUp == true)
			{
				curSprite.m_Coords.y += curSpeed;
				if (curSprite.m_Coords.y >= maxY)
				{
					curSprite.m_VerUp = false;
				}
			}
			else
			{
				curSprite.m_Coords.y -= curSpeed;
				if (curSprite.m_Coords.y <= minY)
				{
					curSprite.m_VerUp = true;
				}
			}
			
			//rotation
			curSprite.m_Angle += 0.2f * deltaTime;
		}
		
		//rendering the sprites
		if (m_Batcher == null) return;

		for (int i = 0; i < m_SpritesCount; i++)
		{
			var curSprite = m_Sprites[i];
			m_Batcher.DrawSprite(curSprite.m_SpriteIndex, curSprite.m_Coords, curSprite.m_Angle, curSprite.m_Scale, curSprite.m_Color);
		}
		
		m_Batcher.CompleteMesh();
	}
}