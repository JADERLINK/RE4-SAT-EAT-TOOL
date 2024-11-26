using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kahshiu
{
	public class SimplePolygon
	{
		private List<(float x, float y)> dots;

		public SimplePolygon((float x, float y) center, List<(float x, float y)> points)
		{
			dots = new List<(float x, float y)>();
			dots.Add(center);
			dots.AddRange(points);
		}

		public List<Vector2d> getNorm()
		{
			List<Vector2d> normals = new List<Vector2d>();
			for (int i = 1; i < dots.Count - 1; i++)
			{
				Vector2d currentNormal = new Vector2d(
			dots[i + 1].x - dots[i].x,
			dots[i + 1].y - dots[i].y
			   ).getnormL();
				normals.Add(currentNormal);
			}

			normals.Add(
				new Vector2d(
					dots[1].x - dots[dots.Count - 1].x,
					dots[1].y - dots[dots.Count - 1].y
				).getnormL());

			return normals;
		}

		public List<Vector2d> prepareVector()
		{
			List<Vector2d> vecs_box = new List<Vector2d>();

			for (int i = 0; i < dots.Count; i++)
			{
				var corner_box = (dots[i].x, dots[i].y);
				vecs_box.Add(new Vector2d(corner_box.x, corner_box.y));
			}

			return vecs_box;
		}

	}
}
