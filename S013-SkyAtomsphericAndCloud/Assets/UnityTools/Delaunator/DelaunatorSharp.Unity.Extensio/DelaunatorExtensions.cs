using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunatorSharp.Unity.Extensions
{

	public static class DelaunatorExtensions
	{
		public static IPoint[] ToPoints(this IEnumerable<Vector2> vertices)
		{
			return vertices.Select((Vector2 vertex) => new Point(vertex.x, vertex.y)).OfType<IPoint>().ToArray();
		}

		public static IPoint[] ToPoints(this Transform[] vertices)
		{
			return vertices.Select((Transform x) => x.transform.position).OfType<Vector2>().ToPoints();
		}

		public static Vector2[] ToVectors2(this IEnumerable<IPoint> points)
		{
			return points.Select((IPoint point) => point.ToVector2()).ToArray();
		}

		public static Vector3[] ToVectors3(this IEnumerable<IPoint> points)
		{
			return points.Select((IPoint point) => point.ToVector3()).ToArray();
		}

		public static Vector2 ToVector2(this IPoint point)
		{
			return new Vector2((float) point.X, (float) point.Y);
		}

		public static Vector3 ToVector3(this IPoint point)
		{
			return new Vector3((float) point.X, (float) point.Y);
		}
	}
}