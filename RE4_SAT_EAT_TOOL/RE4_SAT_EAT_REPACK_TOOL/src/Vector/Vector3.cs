using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_SAT_EAT_REPACK.Vector
{
    /// <summary>
    /// Representação de um Vector3, usado para Position/Normal
    /// </summary>
    public class Vector3 : IEquatable<Vector3>
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        private int hashCode;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + X.GetHashCode();
                hashCode = hashCode * 23 + Y.GetHashCode();
                hashCode = hashCode * 23 + Z.GetHashCode();
            }
        }


        public static bool operator ==(Vector3 lhs, Vector3 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector3 lhs, Vector3 rhs) => !(lhs == rhs);

        public bool Equals(Vector3 obj)
        {
            return obj.X == X && obj.Y == Y && obj.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 o && o.X == X && o.Y == Y && o.Z == Z;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
