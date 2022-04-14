using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace vadersb.utils.unity.jobs
{
	public struct ItemsGroupInfo
	{
		public int Count { get; }
		
		public ItemsGroupInfo(int count)
		{
			Count = count;
		}
	}
}