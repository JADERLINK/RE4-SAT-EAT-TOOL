using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_SAT_EAT_REPACK.Vector
{
    /// <summary>
    /// coordenadas 2D
    /// </summary>
    public class Vector2 : IEquatable<Vector2>
    {
        public float X { get; private set; }
        public float Z { get; private set; }

        private int hashCode;

        public Vector2(float x, float z)
        {
            X = x;
            Z = z;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + X.GetHashCode();
                hashCode = hashCode * 23 + Z.GetHashCode();
            }
        }

        public (float x, float z) GetXZ()
        {
            return (X, Z);
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector2 lhs, Vector2 rhs) => !(lhs == rhs);

        public bool Equals(Vector2 obj)
        {
            return obj.X == X && obj.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 o && o.X == X && o.Z == Z;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
