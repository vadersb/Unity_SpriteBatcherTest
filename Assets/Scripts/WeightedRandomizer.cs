using System.Collections.Generic;
using UnityEngine;

namespace vadersb.utils
{
	public class WeightedRandomizer<T>
	{
		private List<T> m_Values;
		private List<float> m_Weights;
		private float m_TotalWeight;
		private int m_Count;

		public WeightedRandomizer(int sizeToReserve = 10)
		{
			Reset(sizeToReserve);
		}

		public WeightedRandomizer(WeightedRandomizer<T> anotherRandomizer)
		{
			SetupByCopy(anotherRandomizer);
		}

		public void SetupByCopy(WeightedRandomizer<T> anotherRandomizer)
		{
			if (anotherRandomizer == null)
			{
				Debug.LogError("another Randomizer is null!");
				Reset();
				return;
			}
			
			m_Values = new List<T>(anotherRandomizer.m_Count);
			m_Weights = new List<float>(anotherRandomizer.m_Count);
			m_TotalWeight = anotherRandomizer.m_TotalWeight;
			m_Count = anotherRandomizer.m_Count;

			for (int i = 0; i < m_Count; i++)
			{
				m_Values[i] = anotherRandomizer.m_Values[i];
				m_Weights[i] = anotherRandomizer.m_Weights[i];
			}
		}

		public void Reset(int sizeToReserve = 10)
		{
			m_Values = new List<T>(sizeToReserve);
			m_Weights = new List<float>(sizeToReserve);
			m_TotalWeight = 0.0f;
			m_Count = 0;
		}

		public T GetValue(int index)
		{
			if (m_Count == 0)
			{
				Debug.LogError("WeightedRandomizer is empty!");
				return default(T);
			}

			if (index < 0 || index >= m_Count)
			{
				Debug.Log("Invalid index: " + index + ". count: " + m_Count);
				return default(T);
			}

			return m_Values[index];
		}

		public float GetWeight(int index)
		{
			if (m_Count == 0)
			{
				Debug.LogError("WeightedRandomizer is empty!");
				return 0.0f;
			}

			if (index < 0 || index >= m_Count)
			{
				Debug.Log("Invalid index: " + index + ". count: " + m_Count);
				return 0.0f;
			}

			return m_Weights[index];
		}

		public void AddValue(T valueToAdd, float weight, bool strict = false)
		{
			if (weight <= 0.0f)
			{
				Debug.LogError("Can't add value to randomizer. Invalid weight: " + weight);
				return;
			}

			if (strict == true)
			{
				if (m_Values.Contains(valueToAdd) == true)
				{
					Debug.Log("Can't add value to randomizer. Same value is already added.");
					return;
				}
			}

			m_Values.Add(valueToAdd);
			m_Weights.Add(weight);
			m_TotalWeight += weight;
			m_Count++;
		}

		public int Count
		{
			get { return m_Count; }
		}

		public T GetRandomValue()
		{
			if (m_Count <= 0)
			{
				Debug.LogError("randomizer is empty!");
				return default(T);
			}

			if (m_Count == 1)
			{
				return m_Values[0];
			}

			float randomWeight = MathHelpers.Random_Factor_Looped() * m_TotalWeight;

			float curAccumulatedWeight = 0.0f;

			for (int i = 0; i < m_Count; i++)
			{
				curAccumulatedWeight += m_Weights[i];

				if (curAccumulatedWeight > randomWeight)
				{
					return m_Values[i];
				}
			}

			return m_Values[m_Count - 1];
		}
	}
}
