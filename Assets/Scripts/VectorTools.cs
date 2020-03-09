using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vadersb.utils
{
	public static class VectorTools
	{
		public static readonly Vector2 ZeroAngleVector = new Vector2(1.0f, 0.0f);


		public static Vector3 ToVector3(this Vector2 src)
		{
			return new Vector3(src.x, src.y, 0.0f);
		}

		///////
		//setup
		///////
		public static Vector2 Setup_ByCoords(float x, float y)
		{
			return new Vector2(x, y);
		}

		public static Vector2 Setup_ByFromTo(Vector2 from, Vector2 to)
		{
			return to - from;
		}

		public static Vector2 Setup_ByInvertedCopy(Vector2 src)
		{
			return new Vector2(-src.x, -src.y);
		}

		public static Vector2 Setup_ByAngle(float angle)
		{
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}

		public static Vector2 Setup_ByAngleMagnitude(float angle, float magnitude)
		{
			var result = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			result *= magnitude;
			return result;
		}
		
		//============
		//modification
		//============
		public static Vector2 Invert(Vector2 src)
		{
			return new Vector2(-src.x, -src.y);
		}

		/// <summary>
		///   <para>Rotates point by supplied angle in radians, around the origin point [0;0]</para>
		/// </summary>
		/// <param name="angle">angle in radians. positive angle == CCW rotation in Unity! and CW rotation in Pixel Coords!</param>
		public static Vector2 Rotate(Vector2 src, float angle)
		{
			float curSin = Mathf.Sin(angle);
			float curCos = Mathf.Cos(angle);

			return new Vector2((src.x * curCos) - (src.y * curSin), (src.x * curSin) + (src.y * curCos));
		}

		public static Vector2 RotateAroundCenter(Vector2 src, Vector2 center, float angle)
		{
			var tmp = src - center;
			var curSin = Mathf.Sin(angle); 
			var curCos = Mathf.Cos(angle);
			return new Vector2(center.x + ((tmp.x * curCos) - (tmp.y * curSin)), center.y + ((tmp.x * curSin) + (tmp.y * curCos)));
		}

		public static Vector2 ScaleAgainstCenter(Vector2 src, Vector2 center, float scaleFactor)
		{
			return new Vector2(center.x + ((src.x - center.x) * scaleFactor), center.y + ((src.y - center.y) * scaleFactor));
		}

		public static Vector2 ScaleXAgainstCenter(Vector2 src, float centerX, float scaleFactor)
		{
			src.x = centerX + ((src.x - centerX) * scaleFactor);
			return src;
		}
		
		public static Vector2 ScaleYAgainstCenter(Vector2 src, float centerY, float scaleFactor)
		{
			src.y = centerY + ((src.y - centerY) * scaleFactor);
			return src;
		}

		public static Vector2 Mirror(Vector2 src, Vector2 center)
		{
			src.x = center.x + (center.x - src.x);
			src.y = center.y + (center.y - src.y);
			return src;
		}

		public static Vector2 MirrorX(Vector2 src, float centerX)
		{
			src.x = centerX + (centerX - src.x);
			return src;
		}
		
		public static Vector2 MirrorY(Vector2 src, float centerY)
		{
			src.y = centerY + (centerY - src.y);
			return src;
		}		
		
		//============
		//clamp
		//============


		public static Vector2 Clamp_MagnitudeRange(Vector2 src, float minMagnitude, float maxMagnitude)
		{
			float magnitude = src.magnitude;

			var result = src;
			
			if (magnitude < minMagnitude)
			{
				result /= magnitude;
				result *= minMagnitude;
			}
			else if (magnitude > maxMagnitude)
			{
				result /= magnitude;
				result *= maxMagnitude;
			}

			return result;
		}
		
		//============
		//calculations
		//============

		public static bool IsZero(this Vector2 src)
		{
			return src == Vector2.zero;
		}


		public static bool IsZero(this Vector3 src)
		{
			return Mathf.Approximately(src.x, 0.0f) == true &&
			       Mathf.Approximately(src.y, 0.0f) == true &&
			       Mathf.Approximately(src.z, 0.0f) == true;
		}

		/// <summary>
		///   <para>-90 degrees perpendicular in pixels coords (Y goes down)</para>
		///   <para>+90 degrees perpendicular in Unity  coords (Y goes up)</para>
		/// </summary>
		public static Vector2 GetNormal(this Vector2 src)
		{
			float tx = src.x;
			src.x = src.y;
			src.y = -tx;
			src.Normalize();
			return src;
		}
		
		public static float GetDistanceTo(this Vector2 from, Vector2 to)
		{
			return Vector2.Distance(from, to);
		}

		public static float GetDistanceSquareTo(this Vector2 from, Vector2 to)
		{
			float dx = to.x - from.x;
			float dy = to.y - from.y;

			return dx * dx + dy * dy;
		}

		public static float GetAngleAgainstVector(this Vector2 src, Vector2 anotherVector)
		{
			src.Normalize();
			anotherVector.Normalize();

			return Mathf.Acos(Vector2.Dot(src, anotherVector));
		}
		
		public static float GetAngleAgainstVector_Signed(this Vector2 src, Vector2 anotherVector)
		{
			src.Normalize();
			anotherVector.Normalize();

			float result = Mathf.Acos(Vector2.Dot(src, anotherVector));

			var n = anotherVector.GetNormal();

			if (Vector2.Dot(n, src) > 0.0f)
			{
				result = -result;
			}

			return result;
		}

		public static float GetAngleCommon(this Vector2 src)
		{
			src.Normalize();

			float result = Mathf.Acos(Vector2.Dot(src, ZeroAngleVector));

			if (src.y < 0.0f)
			{
				result = MathHelpers.Pi_Double - result;
			}

			return result;
		}
		
		//======
		//paths length
		//======


		public static float GetPathLength_CubicInterpolation(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, int stepsCount = 32)
		{
			Debug.Assert(stepsCount > 0);

			if (stepsCount < 1)
			{
				return 0.01f;
			}

			float curFactor = 0.0f;
			float curStep = 1.0f / ((float)stepsCount);

			int i;

			float result = 0.0f;

			Vector2 curPoint = Interpolation.Cubic(p1, p2, p3, p4, curFactor);

			for (i = 0; i < stepsCount; i++)
			{
				curFactor += curStep;

				Vector2 nextPoint = Interpolation.Cubic(p1, p2, p3, p4, curFactor + curStep);

				result += curPoint.GetDistanceTo(nextPoint);

				curPoint = nextPoint;
			}

			return result;
		}

		//======
		//random
		//======
		public static Vector2 Random_InRect(Rect bounds)
		{
			Vector2 result = new Vector2(MathHelpers.Random_Float(bounds.xMin, bounds.xMax),
			MathHelpers.Random_Float(bounds.yMin, bounds.yMax));

			return result;
		}

		public static Vector2 Random_InCircle(Vector2 center, float radius, float radiusMin = 0.0f)
		{
			Vector2 result = center;

			Vector2 move = Setup_ByAngleMagnitude(MathHelpers.Random_Angle_Looped(), Interpolation.Linear(radiusMin, radius, MathHelpers.Random_Factor()));

			result += move;

			return result;
		}

		public static Vector2 Random_OnSegment(Vector2 segmentFrom, Vector2 segmentTo)
		{
			return Interpolation.Linear(segmentFrom, segmentTo, MathHelpers.Random_Factor());
		}


		public static Vector2 Random_Deviation(Vector2 deviation, Vector2 center = default)
		{
			Vector2 result = center;
			result.x += MathHelpers.Random_Float(-deviation.x, deviation.x);
			result.y += MathHelpers.Random_Float(-deviation.y, deviation.y);
			return result;
		}

	}

	public static class VectorDynamics
	{
		public static void Velocity_ApplyFriction(this Vector2 target, float frictionFactor, float timeDelta)
		{
			frictionFactor *= timeDelta;

			if (frictionFactor > target.magnitude)
			{
				target.Set(0.0f, 0.0f);
				return;
			}

			Vector2 frictionVector = VectorTools.Invert(target);

			frictionVector.Normalize();
			frictionVector *= frictionFactor;

			target += frictionVector;
		}


		public static void Velocity_ApplyImpulse(this ref Vector2 target, Vector2 impulse)
		{
			target += impulse;
		}


		public static void Velocity_ApplyAcceleration(this ref Vector2 target, Vector2 acceleration, float timeDelta)
		{
			target += acceleration * timeDelta;
		}


		public static void Position_ApplyVelocity(this ref Vector2 target, Vector2 velocity, float timeDelta)
		{
			target += velocity * timeDelta;
		}


		public static float CalculateCollisionVelocity(Vector2 velocityA, Vector2 velocityB, Vector2 normalFromA)
		{
			float result = 0.0f;

			//a contribution
			result += Vector2.Dot(velocityA, normalFromA);

			//b contribution
			result += Vector2.Dot(velocityB, -normalFromA);

			return result;
		}

	}
}
