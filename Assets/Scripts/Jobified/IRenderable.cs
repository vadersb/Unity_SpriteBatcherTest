using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace vadersb.utils.unity.jobs
{
	public interface IRenderable
	{
		//visibility
		bool IsVisible();
	
		//sprite index
		int GetSpriteIndex();
		
		//transform
		float2 GetPosition();
		float2 GetScale();
		float GetRotationAngle();
		
		//color
		Color GetColor();
	}	
}