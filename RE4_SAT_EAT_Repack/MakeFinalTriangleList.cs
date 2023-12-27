using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_Repack.Structures;
using RE4_SAT_EAT_Repack.Vector;

namespace RE4_SAT_EAT_Repack
{
    public static class MakeFinalTriangleList
    {
        public static FinalStructure GetFinalStructure(IntermediaryStructure intermediaryStructure)
        {
            FinalStructure finalStructure = new FinalStructure();

            List<FinalTriangle> finalTriangles = new List<FinalTriangle>();
            List<IntermediaryTriangle> intermediaryTriangles = new List<IntermediaryTriangle>();
            intermediaryTriangles.AddRange(intermediaryStructure.FaceFloor);
            intermediaryTriangles.AddRange(intermediaryStructure.FaceSlope);
            intermediaryTriangles.AddRange(intermediaryStructure.FaceWall);

            Dictionary<EdgeConnection, List<int>> EdgeConnectionDic = new Dictionary<EdgeConnection, List<int>>();

            for (int i = 0; i < intermediaryTriangles.Count; i++)
            {
                EdgeConnection AB = new EdgeConnection(intermediaryTriangles[i].IndexPositionA, intermediaryTriangles[i].IndexPositionB);
                EdgeConnection AC = new EdgeConnection(intermediaryTriangles[i].IndexPositionA, intermediaryTriangles[i].IndexPositionC);
                EdgeConnection BC = new EdgeConnection(intermediaryTriangles[i].IndexPositionB, intermediaryTriangles[i].IndexPositionC);

                if (EdgeConnectionDic.ContainsKey(AB))
                {
                    EdgeConnectionDic[AB].Add(i);
                }
                else 
                {
                    EdgeConnectionDic.Add(AB, new List<int>() { i });
                }

                if (EdgeConnectionDic.ContainsKey(AC))
                {
                    EdgeConnectionDic[AC].Add(i);
                }
                else
                {
                    EdgeConnectionDic.Add(AC, new List<int>() { i });
                }

                if (EdgeConnectionDic.ContainsKey(BC))
                {
                    EdgeConnectionDic[BC].Add(i);
                }
                else
                {
                    EdgeConnectionDic.Add(BC, new List<int>() { i });
                }
            }

            for (int i = 0; i < intermediaryTriangles.Count; i++)
            {
                var item = intermediaryTriangles[i];

                FinalTriangle final = new FinalTriangle();
                final.Key = item.Key;
                final.Type = item.Type;
                final.IndexPositionA = (ushort)item.IndexPositionA;
                final.IndexPositionB = (ushort)item.IndexPositionB;
                final.IndexPositionC = (ushort)item.IndexPositionC;
                final.IndexNormal = (ushort)item.IndexNormal;
                final.IndexEdgeA = (ushort)item.IndexEdgeA;
                final.IndexEdgeB = (ushort)item.IndexEdgeB;
                final.IndexEdgeC = (ushort)item.IndexEdgeC;
                final.BackupPositionA = item.BackupPositionA;
                final.BackupPositionB = item.BackupPositionB;
                final.BackupPositionC = item.BackupPositionC;
                final.BackupNormal = item.BackupNormal;
                final.BackupEdgeA = item.BackupEdgeA;
                final.BackupEdgeB = item.BackupEdgeB;
                final.BackupEdgeC = item.BackupEdgeC;
                final.SharedFace = YellowByte.none;

                EdgeConnection AB = new EdgeConnection(item.IndexPositionA, item.IndexPositionB);
                EdgeConnection AC = new EdgeConnection(item.IndexPositionA, item.IndexPositionC);
                EdgeConnection BC = new EdgeConnection(item.IndexPositionB, item.IndexPositionC);

                if (CheckFace(ref EdgeConnectionDic, ref intermediaryTriangles, ref intermediaryStructure.Positions, i, AB)) //AB
                {
                    final.SharedFace |= YellowByte.face1_shared;
                }

                if (CheckFace(ref EdgeConnectionDic, ref intermediaryTriangles, ref intermediaryStructure.Positions, i, BC)) //BC
                {
                    final.SharedFace |= YellowByte.face2_shared;
                }

                if (CheckFace(ref EdgeConnectionDic, ref intermediaryTriangles, ref intermediaryStructure.Positions, i, AC)) //AC
                {
                    final.SharedFace |= YellowByte.face3_shared;
                }

                finalTriangles.Add(final);
            }


            finalStructure.FaceWallCount = (ushort)intermediaryStructure.FaceWall.Count;
            finalStructure.FaceSlopeCount = (ushort)intermediaryStructure.FaceSlope.Count;
            finalStructure.FaceFloorCount = (ushort)intermediaryStructure.FaceFloor.Count;
            finalStructure.FacesCount = (ushort)intermediaryTriangles.Count;
            finalStructure.FinalTriangles = finalTriangles.ToArray();
            finalStructure.Positions = intermediaryStructure.Positions;
            finalStructure.Normals = intermediaryStructure.Normals;
            finalStructure.Edges = intermediaryStructure.Edges;
            return finalStructure;
        }


        private static bool CheckFace(
            ref Dictionary<EdgeConnection, List<int>> EdgeConnectionDic,
            ref List<IntermediaryTriangle> intermediaryTriangles,
            ref Vector3[] Positions,
            int i,
            EdgeConnection Ppair)
        {
            bool res = false;

            if (EdgeConnectionDic[Ppair].Count >= 2)
            {
                foreach (var triID in EdgeConnectionDic[Ppair])
                {
                    if (triID != i)
                    {
                        Vector3 vA3 = Positions[Ppair.p1];
                        Vector3 vB3 = Positions[Ppair.p2];
                        Vector3 vC3 = GetThirdVector(Ppair, intermediaryTriangles[i]);
                        Vector3 vF3 = GetThirdVector(Ppair, intermediaryTriangles[triID]);

                        var shapes = ToDeriveConvexShape(vA3, vB3, vC3, vF3, intermediaryTriangles[i].BackupNormal, intermediaryTriangles[triID].BackupNormal);
                        bool isColliding = CheckCollision(shapes.A, shapes.B);

                        res |= isColliding;

                        if (intermediaryTriangles[i].BackupNormal == intermediaryTriangles[triID].BackupNormal)
                        {
                            res = true;
                        }
                    }
                }

            }

            return res;
        }

        private static Vector3 GetThirdVector(EdgeConnection Ppair, IntermediaryTriangle tri) 
        {
            if (!(tri.IndexPositionA == Ppair.p1 || tri.IndexPositionA == Ppair.p2))
            {
                return tri.BackupPositionA;
            }
            else if (!(tri.IndexPositionB == Ppair.p1 || tri.IndexPositionB == Ppair.p2))
            {
                return tri.BackupPositionB;
            }
            else if (!(tri.IndexPositionC == Ppair.p1 || tri.IndexPositionC == Ppair.p2))
            {
                return tri.BackupPositionC;
            }

            return new Vector3(0,0,0);
        }


        private static(ConvexShape A, ConvexShape B) ToDeriveConvexShape(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, Vector3 Normal1, Vector3 Normal2)
        {
            float extrudeDistanceMax = GreaterDistance(P1, P2, P3, P4);
            Vector3 tri1_ExtrusionVectorMax = MultVector3Float(Normal1, extrudeDistanceMax);
            Vector3 tri2_ExtrusionVectorMax = MultVector3Float(Normal2, extrudeDistanceMax);

            Vector3 tri1P4 = PlusVector3(P1, tri1_ExtrusionVectorMax);
            Vector3 tri1P5 = PlusVector3(P2, tri1_ExtrusionVectorMax);
            Vector3 tri1P6 = PlusVector3(P3, tri1_ExtrusionVectorMax);

            Vector3 tri2P4 = PlusVector3(P1, tri2_ExtrusionVectorMax);
            Vector3 tri2P5 = PlusVector3(P2, tri2_ExtrusionVectorMax);
            Vector3 tri2P6 = PlusVector3(P4, tri2_ExtrusionVectorMax);

            ConvexShape shape1 = new ConvexShape(new List<Vector3>() { P1, P2, P3, tri1P4, tri1P5, tri1P6 });
            ConvexShape shape2 = new ConvexShape(new List<Vector3>() { P1, P2, P4, tri2P4, tri2P5, tri2P6 });

            return (shape1, shape2);
        }


        private static float GreaterDistance(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4)
        {
            float distance = Distance(P1, P3);
            float distance2 = Distance(P1, P4);
            float distance3 = Distance(P2, P3);
            float distance4 = Distance(P2, P4);

            if (distance2 > distance)
            {
                distance = distance2;
            }
            if (distance3 > distance)
            {
                distance = distance3;
            }
            if (distance4 > distance)
            {
                distance = distance4;
            }

            return distance;
        }

        private static float Distance(Vector3 P1, Vector3 P2)
        {
            float X = P2.X - P1.X;
            float Y = P2.Y - P1.Y;
            float Z = P2.Z - P1.Z;
            return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        private static Vector3 MultVector3Float(Vector3 v, float f)
        {
            return new Vector3(v.X * f, v.Y * f, v.Z * f);
        }

        private static Vector3 PlusVector3(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        private static bool CheckCollision(ConvexShape shape1, ConvexShape shape2)
        {
            List<Vector3> axes = new List<Vector3>();

            // Adiciona os eixos normais das faces do primeiro objeto
            axes.AddRange(shape1.GetFaceNormals());

            // Adiciona os eixos normais das faces do segundo objeto
            axes.AddRange(shape2.GetFaceNormals());

            foreach (Vector3 axis in axes)
            {
                if (Separated(shape1.Vertices, shape2.Vertices, axis))
                {
                    return false; // Os objetos não estão colidindo ao longo deste eixo
                }
            }

            foreach (var axes1 in shape1.GetFaceNormals())
            {
                foreach (var axes2 in shape2.GetFaceNormals())
                {
                    if (Separated(shape1.Vertices, shape2.Vertices, axes1.Cross(axes2)))
                    {
                        return false; // Os objetos não estão colidindo ao longo deste eixo
                    }
                }
            }


            return true; // Os objetos estão colidindo em todos os eixos
        }

        //https://gamedev.stackexchange.com/questions/44500/how-many-and-which-axes-to-use-for-3d-obb-collision-with-sat/
        private static bool Separated(Vector3[] vertsA, Vector3[] vertsB, Vector3 axis)
        {
            // Handles the cross product = {0,0,0} case
            if (axis == new Vector3(0, 0, 0))
                return false;

            var aMin = float.MaxValue;
            var aMax = float.MinValue;
            var bMin = float.MaxValue;
            var bMax = float.MinValue;

            // Define two intervals, a and b. Calculate their min and max values
            for (var i = 0; i < 6; i++)
            {
                var aDist = vertsA[i].Dot(axis);
                aMin = aDist < aMin ? aDist : aMin;
                aMax = aDist > aMax ? aDist : aMax;
                var bDist = vertsB[i].Dot(axis);
                bMin = bDist < bMin ? bDist : bMin;
                bMax = bDist > bMax ? bDist : bMax;
            }

            // One-dimensional intersection test between a and b
            var longSpan = Math.Max(aMax, bMax) - Math.Min(aMin, bMin);
            var sumSpan = aMax - aMin + bMax - bMin;
            return longSpan >= sumSpan; // > to treat touching as intersection
        }


    }

    public class ConvexShape
    {
        List<Vector3> vertices;

        public Vector3[] Vertices { get { return vertices.ToArray(); } }

        public ConvexShape(List<Vector3> vertices)
        {
            this.vertices = vertices;
        }

        public List<Vector3> GetFaceNormals()
        {
            List<Vector3> faceNormals = new List<Vector3>();
            faceNormals.Add(GetNormal(vertices[0], vertices[1], vertices[2]));
            faceNormals.Add(GetNormal(vertices[3], vertices[4], vertices[5]));
            faceNormals.Add(GetNormal(vertices[0], vertices[1], vertices[3]));
            faceNormals.Add(GetNormal(vertices[1], vertices[2], vertices[4]));
            faceNormals.Add(GetNormal(vertices[0], vertices[2], vertices[5]));
            return faceNormals;
        }

        private Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 vectorAB = p2.Subtract(p1);
            Vector3 vectorAC = p3.Subtract(p1);
            return vectorAB.Cross(vectorAC).Normalized();
        }

    }

    public static class Vector3Addons
    {

        public static Vector3 Subtract(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 Cross(this Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        public static float Dot(this Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3 Normalized(this Vector3 a)
        {
            float length = (float)Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
            return new Vector3(a.X / length, a.Y / length, a.Z / length);
        }
    }


    public class EdgeConnection
    {
        public int p1 = -1;
        public int p2 = -1;

        public EdgeConnection(int p1, int p2) 
        {
        this.p1 = p1;
        this.p2 = p2;
        }

        public override bool Equals(object obj)
        {
            return obj is EdgeConnection e && ((e.p1 == p1 && e.p2 == p2) || (e.p1 == p2 && e.p2 == p1));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return p1 * p2;
            }
        }

        public override string ToString()
        {
            return p1 + " | " + p2;
        }
    }

}
