using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kahshiu
{
	/**
	* ...
	* @author Shiu
	*/
	public class Vector2d
	{
		private float _x, _y;

		public float getx()
		{
			return _x;
		}

		public void setx(float value)
		{
			_x = value;
		}

		public float gety()
		{
			return _y;
		}

		public void sety(float value)
		{
			_y = value;
		}

		public float getmagnitude()
		{
			return Pythagoras(_x, _y);
		}

		private float Pythagoras(float xDist, float yDist)
		{
			return (float)(Math.Sqrt(xDist * xDist + yDist * yDist));
		}

		public void setmagnitude(float value)
		{
			float curr_angle = getangle();
			_x = (float)(value * Math.Cos(curr_angle));
			_y = (float)(value * Math.Sin(curr_angle));
		}

		public float getangle()
		{
			return (float)(Math.Atan2(_y, _x));
		}

		public void setangle(float value)
		{
			float current_magnitude = getmagnitude();
			_x = (float)(current_magnitude * Math.Cos(value));
			_y = (float)(current_magnitude * Math.Sin(value));
		}

		public Vector2d getnormR()
		{
			return new Vector2d(-1 * this._y, this._x);
		}

		public Vector2d getnormL()
		{
			return new Vector2d(this._y, -1 * this._x);
		}

		public Vector2d getunitVector()
		{
			return new Vector2d(_x / getmagnitude(), _y / getmagnitude());
		}

		/**** Static functions ****/
		/**
		 * Performs operation A+B
		 * @param	A	Vector2d to add
		 * @param	B	Vector2d
		 * @return	A+B
		 */
		public static Vector2d add(Vector2d A, Vector2d B)
		{
			return new Vector2d(A.getx() + B.getx(), A.getx() + B.gety());
		}

		/**
		 * Performs operation A-B
		 * @param	A	Vector2d to minus
		 * @param	B	Vector2d
		 * @return	A-B
		 */
		public static Vector2d minus(Vector2d A, Vector2d B)
		{
			return new Vector2d(A.getx() - B.getx(), A.getx() - B.gety());
		}

		public static Vector2d rotate(Vector2d A, float angle)
		{
			Vector2d B = A.clone();
			B.rotate(angle);
			return B;
		}

		/**
		 * Calculate the angle to rotate from vector A to B
		 * @param	A	Vector2d to start rotating
		 * @param	B	Vector2d to end
		 * @return	angle from A to B
		 */
		public static float angleBetween(Vector2d A, Vector2d B)
		{
			Vector2d A_unitVector = A.getunitVector();
			Vector2d B_unitVector = B.getunitVector();
			return (float)(Math.Acos(A.dotProduct(B)));
		}

		/**
		 * Interpolate input vector to value
		 * @param	A	Vector2d to interpolate
		 * @param	value	interpolation value between 0 and 1
		 * @return	new Vector2d interpolated
		 */
		public static Vector2d interpolate(Vector2d A, float value)
		{
			return new Vector2d(A.getx() * value, A.gety() * value);
		}

		/**** Class functions ****/
		/**
		 * Constructor of Vector2d
		 * @param	x	horizontal length of vector
		 * @param	y	vertical length of vector
		 */
		public Vector2d(float x, float y)
		{
			this._x = x;
			this._y = y;
		}

		/**
		 * Create a copy of current vector
		 * @return Copied vector
		 */
		public Vector2d clone()
		{
			return new Vector2d(this._x, this._y);
		}


		/**
		 * Scale current vector
		 * @param	factor
		 * Original is 1, half is 0.5;
		 */
		public void scale(float factor)
		{
			_x *= factor;
			_y *= factor;
		}

		/**
		 * Invert current vector
		 * @param	type
			* input parameters: "x" or "y" or "xy"
			* "x"	invert x only
			* "y"	invert y only
			* "xy"	invert both
		 * 
		 */
		public void invert(string type)
		{
			if (type[0] == 'x') this._x *= -1;
			if (type[0] == 'y' || type[1] == 'y') this._y *= -1;
		}

		/**
		 * Add current vector by B, self+B
		 * @param	B	to minus B
		 */
		public void add(Vector2d B)
		{
			this._x += B.getx();
			this._y += B.gety();
		}

		/**
		 * Minus current vector by B, self-B
		 * @param	B	to minus B
		 */
		public void minus(Vector2d B)
		{
			this._x -= B.getx();
			this._y -= B.gety();
		}

		/**
		 * Rotate current vector by angle
		 * @param	value	angle in radians
		 */
		public void rotate(float value)
		{
			_x = (float)(_x * Math.Cos(value) - _y * Math.Sin(value));
			_y = (float)(_x * Math.Sin(value) + _y * Math.Cos(value));
		}

		/**
		 * Calculate the dot product between current vector and B
		 * @param	B	Input vector
		 * @return	dot product, a scalar value
		 */
		public float dotProduct(Vector2d B)
		{
			return _x * B.getx() + _y * B.gety();
		}

		/**
		 * Calculate the perpendicular product between current vector and B
		 * @param	B	Input vector
		 * @return	perpendicular product, a scalar value
		 */
		public float perpProduct(Vector2d B)
		{
			return _y * B.getx() + _x * -B.gety();
		}

		/**
		 * Calculate cross product of current vector and input, self x B
		 * @param	B	Input vector
		 * @return
		 */
		public float crossProduct(Vector2d B)
		{
			return _x * B.gety() - _y * B.getx();
		}

		/**
		 * Calculate whether current vector is equivalent to input
		 * @param	B
		 * @return
		 */
		public bool equivalent(Vector2d B)
		{
			float diff = (float)(Math.Pow(4, -10));
			return (_x - B.getx() < diff && _y - B.gety() < diff);
		}

	}
}
