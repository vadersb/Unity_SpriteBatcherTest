using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace vadersb.utils
{
	public static class Interpolation
	{
		//=======
		// Linear
		//=======

		public static float Linear(float p1, float p2, float factor)
		{
			return p2 * factor + p1 * (1.0f - factor);
		}

		public static Vector2 Linear(Vector2 p1, Vector2 p2, float factor)
		{
			return new Vector2(Linear(p1.x, p2.x, factor),
				Linear(p1.y, p2.y, factor));
		}

		public static Vector3 Linear(Vector3 p1, Vector3 p2, float factor)
		{
			return new Vector3(Linear(p1.x, p2.x, factor),
				Linear(p1.y, p2.y, factor),
				Linear(p1.z, p2.z, factor));
		}


		//==========
		// Quadratic
		//==========

		public static float Quadratic(float p1, float p2, float p3, float factor)
		{
			float antiFactor = 1.0f - factor;
			return p3 * factor * factor + p2 * 2.0f * factor * antiFactor + p1 * antiFactor * antiFactor;
		}

		public static Vector2 Quadratic(Vector2 p1, Vector2 p2, Vector2 p3, float factor)
		{
			return new Vector2(Quadratic(p1.x, p2.x, p3.x, factor),
				Quadratic(p1.y, p2.y, p3.y, factor));
		}

		public static Vector3 Quadratic(Vector3 p1, Vector3 p2, Vector3 p3, float factor)
		{
			return new Vector3(Quadratic(p1.x, p2.x, p3.x, factor),
				Quadratic(p1.y, p2.y, p3.y, factor),
				Quadratic(p1.z, p2.z, p3.z, factor));
		}


		//======
		// Cubic
		//======

		public static float Cubic(float p1, float p2, float p3, float p4, float factor)
		{
			float antiFactor = 1.0f - factor;
			return p4 * (factor * factor * factor) + p3 * 3.0f * factor * factor * antiFactor + p2 * 3.0f * factor * antiFactor * antiFactor + p1 * (antiFactor * antiFactor * antiFactor);
		}

		public static Vector2 Cubic(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float factor)
		{
			return new Vector2(Cubic(p1.x, p2.x, p3.x, p4.x, factor),
				Cubic(p1.y, p2.y, p3.y, p4.y, factor));
		}

		public static Vector3 Cubic(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float factor)
		{
			return new Vector3(Cubic(p1.x, p2.x, p3.x, p4.x, factor),
				Cubic(p1.y, p2.y, p3.y, p4.y, factor),
				Cubic(p1.z, p2.z, p3.z, p4.z, factor));
		}


		//=============
		// Cubic smooth
		//=============

		public static float CubicSmooth(float prevPoint, float startPoint, float endPoint, float nextPoint, float factor)
		{
			//return p[1] + 0.5 * x*(p[2] - p[0] + x*(2.0*p[0] - 5.0*p[1] + 4.0*p[2] - p[3] + x*(3.0*(p[1] - p[2]) + p[3] - p[0])));

			//p0 - a_PrevPoint
			//p1 - a_StartPoint
			//p2 - a_EndPoint
			//p3 - a_NextPoint
			//x - a_Factor

			return startPoint + 0.5f * factor * (endPoint - prevPoint + factor * (2.0f * prevPoint - 5.0f * startPoint + 4.0f * endPoint - nextPoint + factor * (3.0f * (startPoint - endPoint) + nextPoint - prevPoint)));
		}

		public static Vector2 CubicSmooth(Vector2 prevPoint, Vector2 startPoint, Vector2 endPoint, Vector2 nextPoint, float factor)
		{
			return new Vector2(CubicSmooth(prevPoint.x, startPoint.x, endPoint.x, nextPoint.x, factor),
				CubicSmooth(prevPoint.y, startPoint.y, endPoint.y, nextPoint.y, factor));
		}
		
		public static Vector3 CubicSmooth(Vector3 prevPoint, Vector3 startPoint, Vector3 endPoint, Vector3 nextPoint, float factor)
		{
			return new Vector3(CubicSmooth(prevPoint.x, startPoint.x, endPoint.x, nextPoint.x, factor),
				CubicSmooth(prevPoint.y, startPoint.y, endPoint.y, nextPoint.y, factor),
				CubicSmooth(prevPoint.z, startPoint.z, endPoint.z, nextPoint.z, factor));
		}
	}
}