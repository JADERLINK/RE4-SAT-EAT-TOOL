using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kahshiu
{
	//https://code.tutsplus.com/collision-detection-using-the-separating-axis-theorem--gamedev-169t
	class SeparatingAxisTheorem
	{
		public static bool Check(
			((float x, float z) min, (float x, float z) max) square,
			((float x, float z) p1, (float x, float z) p2, (float x, float z) p3) triangle
			)
		{
			var s1 = square.min;
			var s2 = (square.min.x, square.max.z);
			var s3 = (square.max.x, square.min.z);
			var s4 = square.max;
			var half = ((Math.Abs(square.max.x - square.min.x) / 2), (Math.Abs(square.max.z - square.min.z) / 2));
			var s0 = ((square.min.x + half.Item1), (square.min.z + half.Item2));

			SimplePolygon p1 = new SimplePolygon(s0, new List<(float x, float y)>() { s1, s2, s3, s4 });

			var tri0x = (triangle.p1.x + triangle.p2.x + triangle.p3.x) / 3;
			var tri0z = (triangle.p1.z + triangle.p2.z + triangle.p3.z) / 3;

			SimplePolygon p2 = new SimplePolygon((tri0x, tri0z), new List<(float x, float y)>() { triangle.p1, triangle.p2, triangle.p3 });


			SeparatingAxisTheorem sat = new SeparatingAxisTheorem(p1, p2);

			return sat.CheckIfItCollided();
		}

		private SimplePolygon hex;
		private SimplePolygon tri;

		public SeparatingAxisTheorem(SimplePolygon hex, SimplePolygon tri)
		{
			this.hex = hex;
			this.tri = tri;
		}

		public bool CheckIfItCollided()
		{
			//prepare the normals
			List<Vector2d> normals_hex = hex.getNorm();
			List<Vector2d> normals_tri = tri.getNorm();

			List<Vector2d> vecs_hex = hex.prepareVector();
			List<Vector2d> vecs_tri = tri.prepareVector();
			bool isSeparated = false;

			//use hexagon's normals to evaluate
			for (int i = 0; i < normals_hex.Count; i++)
			{
				var result_box1 = getMinMax(vecs_hex, normals_hex[i]);
				var result_box2 = getMinMax(vecs_tri, normals_hex[i]);

				isSeparated = result_box1.max_proj < result_box2.min_proj || result_box2.max_proj < result_box1.min_proj;
				if (isSeparated) break;
			}
			//use triangle's normals to evaluate
			if (!isSeparated)
			{
				for (int j = 1; j < normals_tri.Count; j++)
				{
					var result_P1 = getMinMax(vecs_hex, normals_tri[j]);
					var result_P2 = getMinMax(vecs_tri, normals_tri[j]);

					isSeparated = result_P1.max_proj < result_P2.min_proj || result_P2.max_proj < result_P1.min_proj;
					if (isSeparated) break;
				}
			}

			return !isSeparated;
		}

		/**
		 * Calculates the min-max projections 
		 * @param	vecs_box	vectors to box coordinate
		 * @param	axis		axis currently evaluating
		 * @return	object array of [min, min_index, max, max_index]
		 */
		private (float min_proj, float max_proj, int min_index, int max_index) getMinMax(List<Vector2d> vecs_box, Vector2d axis)
		{
			float min_proj_box = vecs_box[1].dotProduct(axis); int min_dot_box = 1;
			float max_proj_box = vecs_box[1].dotProduct(axis); int max_dot_box = 1;

			for (int j = 2; j < vecs_box.Count; j++)
			{
				float curr_proj = vecs_box[j].dotProduct(axis);
				//select the maximum projection on axis to corresponding box corners
				if (min_proj_box > curr_proj)
				{
					min_proj_box = curr_proj;
					min_dot_box = j;
				}
				//select the minimum projection on axis to corresponding box corners
				if (curr_proj > max_proj_box)
				{
					max_proj_box = curr_proj;
					max_dot_box = j;
				}
			}

			return (
					min_proj: min_proj_box,
					max_proj: max_proj_box,
					min_index: min_dot_box,
					max_index: max_dot_box
					);
		}
	}
}
