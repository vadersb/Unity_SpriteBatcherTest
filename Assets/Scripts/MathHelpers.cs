using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace vadersb.utils
{
	public static class MathHelpers
	{
		//-----
		//pi consts
		public const float Pi = Mathf.PI;

		public const float Pi_Double = Pi * 2.0f;
		public const float Pi_Half = Pi * 0.50f;
		public const float Pi_OneAndHalf = Pi * 1.50f;
		public const float Pi_OneFourth = Pi / 4.0f;
		public const float Pi_OneSixth = Pi / 6.0f;

		public const float ConvertAngle_ToRadians = Pi / 180.0f;
		public const float ConvertAngle_ToDegrees = 180.0f / Pi;
		
		
		//================
		//random functions
		//================
		
		#region Random Functions
		
		/// <summary>
		///   <para>Returns random float in range [0.0; 1.0]</para>
		/// </summary>
		public static float Random_Factor()
		{
			return Random.Range(0.0f, 1.0f);
		}

		/// <summary>
		///   <para>Returns random float in range [0.0; 1.0)</para>
		/// </summary>
		public static float Random_Factor_Looped()
		{
			var result = Random.Range(0.0f, 1.0f);

			if (result >= 1.0f)
			{
				result -= 1.0f;
			}

			return result;
		}

		/// <summary>
		///   <para>Returns random float in specified range [min; max]</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public static float Random_Float(float min, float max)
		{
			return Random.Range(min, max);
		}

		/// <summary>
		///   <para>Returns random float in specified range [center - radius; center + radius]</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		public static float Random_Float_FromCenter(float center, float radius)
		{
			return Random.Range(center - radius, center + radius);
		}
		
		/// <summary>
		///   <para>Returns random int in specified range [min; max]</para>
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public static int Random_Int(int min, int max)
		{
			return Random.Range(min, max + 1);
		}

		/// <summary>
		///   <para>Returns random int in specified range [center - radius; center + radius]</para>
		/// </summary>
		/// <param name="center"></param>
		/// <param name="radius"></param>
		public static int Random_Int_FromCenter(int center, int radius)
		{
			return Random.Range(center - radius, center + radius + 1);
		}

		/// <summary>
		///   <para>Returns random angle in range [0; 2Pi]</para>
		/// </summary>
		/// <returns>
		///   <para>Float value of the random angle.</para>
		/// </returns>
		public static float Random_Angle()
		{
			return Random_Factor() * Pi_Double;
		}

		/// <summary>
		///   <para>Returns random angle in range [0; 2Pi)</para>
		/// </summary>
		/// <returns>
		///   <para>Float value of the random angle.</para>
		/// </returns>
		public static float Random_Angle_Looped()
		{
			return Random_Factor_Looped() * Pi_Double;
		}

		/// <summary>
		///   <para>Checks if a probable event happened.</para>
		/// </summary>
		/// <param name="probability">Probability of the event. Expected range is in [0.0; 1.0] range.</param>
		/// <returns>
		///   <para>Boolean value: is event happened or not.</para>
		///   <para>Always returns false if probability is less or equal 0</para>
		///   <para>Always returns true  if probability is more or equal 1</para>
		/// </returns>
		public static bool Random_CheckChance(float probability)
		{
			if (probability >= 1.0f) return true;
			if (probability <= 0.0f) return false;

			return probability >= Random.Range(0.0f, 1.0f);
		}
		
		//Random Functions
		#endregion 
		
		//=====
		//clamp
		//=====


		public static float Clamp_InRangeLooped(float valueToClamp, float range)
		{
			while (valueToClamp < 0.0f)
			{
				valueToClamp += range;
			}

			while (valueToClamp >= range)
			{
				valueToClamp -= range;
			}

			return valueToClamp;
		}
		
		/// <summary>
		///   <para>Clamps float number into [0.0; 1.0] range</para>
		/// </summary>
		/// <param name="factor"></param>
		public static float Clamp_Factor(float factor)
		{
			if (factor < 0.0f) return 0.0f;
			if (factor > 1.0f) return 1.0f;
			return factor;
		}

		/// <summary>
		///   <para>Clamps float number into [0.0; 1.0) range</para>
		/// </summary>
		/// <param name="factor"></param>
		public static float Clamp_Factor_Looped(float factor)
		{
			while (factor < 0.0f)
			{
				factor += 1.0f;
			}

			while (factor >= 1.0f)
			{
				factor -= 1.0f;
			}
			
			return factor;
		}
		
		/// <summary>
		///   <para>Clamps float number into [0.0; 2Pi] range</para>
		/// </summary>
		/// <param name="angle"></param>
		public static float Clamp_Angle(float angle)
		{
			while (angle < 0.0f)
			{
				angle += Pi_Double;
			}

			while (angle > Pi_Double)
			{
				angle -= Pi_Double;
			}

			return angle;
		}

		/// <summary>
		///   <para>Clamps float number into [0.0; 2Pi) range</para>
		/// </summary>
		/// <param name="angle"></param>
		public static float Clamp_Angle_Looped(float angle)
		{
			while (angle < 0.0f)
			{
				angle += Pi_Double;
			}

			while (angle >= Pi_Double)
			{
				angle -= Pi_Double;
			}

			return angle;
		}
		
		
		//======
		//factor
		//======

		/// <summary>
		///   <para>Returns a factor of a value inside specified range.</para>
		/// </summary>
		/// <param name="rangeFrom">range start</param>
		/// <param name="rangeTo">range end</param>
		/// <param name="curValue">value on the range</param>
		public static float Factor_FromRange(float rangeFrom, float rangeTo, float curValue)
		{
			if (Mathf.Approximately(rangeFrom, rangeTo))
			{
				return 1.0f;
			}

			return (curValue - rangeFrom) / (rangeTo - rangeFrom);
		}

		/// <summary>
		///   <para>Returns a sub-factor of a value inside specified sub-factor range.</para>
		/// </summary>
		/// <param name="subFrom">range start</param>
		/// <param name="subTo">range end</param>
		/// <param name="factor">value on the range</param>
		public static float Factor_SubFactor(float factor, float subFrom, float subTo)
		{
			float length = subTo - subFrom;

			if (length <= 0.0f)
			{
				#if DEBUG
				Debug.LogWarning("subfactor length <= 0.0f. subFrom: " + subFrom + " subTo: " + subTo);
				#endif
				return 1.0f;
			}

			return (factor - subFrom) / length;
		}

		/// <summary>
		///   <para>Takes a factor going 0.0->1.0 and makes it 0.0->1.0->0.0</para>
		/// </summary>
		/// <param name="factor">souce factor</param>
		/// <param name="autoClamp">auto clamp source factor in looped way</param>
		public static float Factor_ToTwoWay(float factor, bool autoClamp = true)
		{
			if (autoClamp == true)
			{
				while (factor < 0.0f) factor += 1.0f;
				while (factor > 1.0f) factor -= 1.0f;
			}
			
			if (factor < 0.5f)
			{
				return factor * 2.0f;
			}
			else
			{
				return (1.0f - factor) * 2.0f;
			}
		}


		public static float Factor_Repeated(float factor, int repetitionsCount, bool isPingPong)
		{
			if (repetitionsCount < 2)
			{
				#if DEBUG
				Debug.LogError("repetitionsCount: " + repetitionsCount);
				#endif
				return factor;
			}
			
			float repetitionsCountFloat = repetitionsCount;
			float curStep = (int)(repetitionsCountFloat * factor);

			if (factor >= 1.0f)
			{
				if (isPingPong == true)
				{
					if (repetitionsCount % 2 == 0)
					{
						return 0.0f;
					}
				}
				return 1.0f;
			}


			float factorStep = 1.0f / repetitionsCountFloat;

			float result = (factor - (curStep * factorStep)) / factorStep;

			if (isPingPong == true)
			{
				if (((int)(curStep)) % 2 == 1)
				{
					result = 1.0f - result;
				}
			}

			return result;
			
		}

		//========
		//sequence
		//========
		/// <summary>
		///   <para>Returns an element index in a sequence [0; stepsCount - 1] based on current time and full time. Clamps if current time is out of bounds.</para>
		/// </summary>
		/// <param name="curTime"></param>
		/// <param name="fullTime"></param>
		/// <param name="stepsCount"></param>
		public static int Sequence_Timed_Clamped(float curTime, float fullTime, int stepsCount)
		{
			Debug.Assert(fullTime > 0.0f);
			Debug.Assert(stepsCount > 0);

			int result = (int)(((float)(stepsCount)) * (curTime / fullTime));

			result = Mathf.Clamp(result, 0, stepsCount - 1);

			return result;
		}
		
		/// <summary>
		///   <para>Returns an element index in a sequence [0; stepsCount - 1] based on current time and full time. Loops if current time is out of bounds.</para>
		/// </summary>
		/// <param name="curTime"></param>
		/// <param name="fullTime"></param>
		/// <param name="stepsCount"></param>
		public static int Sequence_Timed_Looped(float curTime, float fullTime, int stepsCount)
		{
			Debug.Assert(fullTime > 0.0f);
			Debug.Assert(stepsCount > 0);

			int result = (int)(((float)(stepsCount)) * (curTime / fullTime));

			while (result >= stepsCount)
			{
				result -= stepsCount;
			}

			while (result < 0)
			{
				result += stepsCount;
			}

			return result;
		}
		
		/// <summary>
		///   <para>Returns an element index in a sequence [0; stepsCount - 1] based on factor. Clamps if factor is out of bounds.</para>
		/// </summary>
		/// <param name="curFactor"></param>
		/// <param name="stepsCount"></param>
		public static int Sequence_FromFactor_Clamped(float curFactor, int stepsCount)
		{
			Debug.Assert(stepsCount > 0);

			int result = (int)(((float)(stepsCount)) * curFactor);

			result = Mathf.Clamp(result, 0, stepsCount - 1);

			return result;
		}
		
		/// <summary>
		///   <para>Returns an element index in a sequence [0; stepsCount - 1] based on factor. Loops if factor is out of bounds.</para>
		/// </summary>
		/// <param name="curFactor"></param>
		/// <param name="stepsCount"></param>
		public static int Sequence_FromFactor_Looped(float curFactor, int stepsCount)
		{
			Debug.Assert(stepsCount > 0);

			int result = (int)(((float)(stepsCount)) * curFactor);

			while (result >= stepsCount)
			{
				result -= stepsCount;
			}

			while (result < 0)
			{
				result += stepsCount;
			}

			return result;
		}
		
		
		//====
		//swap
		//====
		/// <summary>
		///   <para>Swaps two values passed by reference</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static void Swap<T>(ref T a, ref T b)
		{
			var tmp = a;
			a = b;
			b = tmp;
		}

		
		//====
		//misc
		//====
		public static int DigitsInInteger(int number)
		{
			// In case of negative numbers
			number = Mathf.Abs(number);

			int digits = 1;

			while (number >= 10)
			{
				digits++;
				number /= 10;
			}

			return digits;
		}
		
	}
}