using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_REPACK.Structures;
using RE4_SAT_EAT_REPACK.Vector;

namespace RE4_SAT_EAT_REPACK
{
    public static class MakeIntermediary
    {
        public static IntermediaryStructure GetIntermediaryStructure(StartStructure startStructure) 
        {
            IntermediaryStructure intermediary = new IntermediaryStructure();
            List<Vector3> SetOfEdges = new List<Vector3>();

            foreach (var item in startStructure.SetOfTriangles)
            {
                IntermediaryTriangle triangle = new IntermediaryTriangle();
                triangle.Key = item.Key;
                triangle.IndexNormal = item.IndexNormal;
                triangle.IndexPositionA = item.IndexPositionA;
                triangle.IndexPositionB = item.IndexPositionB;
                triangle.IndexPositionC = item.IndexPositionC;

                triangle.BackupNormal = startStructure.SetOfNormals[item.IndexNormal];
                triangle.BackupPositionA = startStructure.SetOfPositions[item.IndexPositionA];
                triangle.BackupPositionB = startStructure.SetOfPositions[item.IndexPositionB];
                triangle.BackupPositionC = startStructure.SetOfPositions[item.IndexPositionC];

                //----------
                //Edge

                // eA = vB - vA
                // eB = vC - vB
                // eC = vA - vC

                Vector3 eA = Minus(triangle.BackupPositionB, triangle.BackupPositionA);
                Vector3 eB = Minus(triangle.BackupPositionC, triangle.BackupPositionB);
                Vector3 eC = Minus(triangle.BackupPositionA, triangle.BackupPositionC);

                triangle.BackupEdgeA = eA;
                triangle.BackupEdgeB = eB;
                triangle.BackupEdgeC = eC;

                if (!SetOfEdges.Contains(eA))
                {
                    SetOfEdges.Add(eA);
                }

                if (!SetOfEdges.Contains(eB))
                {
                    SetOfEdges.Add(eB);
                }

                if (!SetOfEdges.Contains(eC))
                {
                    SetOfEdges.Add(eC);
                }

                triangle.IndexEdgeA = SetOfEdges.IndexOf(eA);
                triangle.IndexEdgeB = SetOfEdges.IndexOf(eB);
                triangle.IndexEdgeC = SetOfEdges.IndexOf(eC);

                //--------------------
                //define o Type
                // e coloca na lista certa

                //Floor = chão
                if (triangle.BackupPositionA.Y == triangle.BackupPositionB.Y && triangle.BackupPositionA.Y == triangle.BackupPositionC.Y
                    && triangle.BackupPositionB.Y == triangle.BackupPositionC.Y)
                {
                    if (triangle.BackupNormal.Y > 0.0f) // 1.0 virado para cima
                    {
                        triangle.Type = FaceType.Floor;
                        intermediary.FaceFloor.Add(triangle);
                    }
                    else //-1.0 virado para baixo
                    {
                        triangle.Type = FaceType.Wall;
                        intermediary.FaceWall.Add(triangle); // se a face estiver de cabeça para baixo é considerada parede.
                    }
                }
                else if (triangle.BackupNormal.Y >= 0.9845f) // no limite 0.9845
                {
                    triangle.Type = FaceType.Floor;
                    intermediary.FaceFloor.Add(triangle);
                }

                //Wall = paredes
                else if (triangle.BackupNormal.Y <= 0.0f)
                {
                    triangle.Type = FaceType.Wall;
                    intermediary.FaceWall.Add(triangle);
                }
                else if (triangle.BackupNormal.Y <= 0.5f) // valor arbitrario, meio testado
                {
                    triangle.Type = FaceType.Wall;
                    intermediary.FaceWall.Add(triangle);
                }

                //Slope = Diagonais
                else
                {
                    triangle.Type = FaceType.Slope;
                    intermediary.FaceSlope.Add(triangle);
                }

            }

            intermediary.Edges = SetOfEdges.ToArray();
            intermediary.Positions = startStructure.SetOfPositions.ToArray();
            intermediary.Normals = startStructure.SetOfNormals.ToArray();
            return intermediary;
        }

        private static Vector3 Minus(Vector3 v1, Vector3 v2) 
        {
          return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }


    }
}
