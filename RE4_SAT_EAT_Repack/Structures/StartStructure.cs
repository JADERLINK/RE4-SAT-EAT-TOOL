using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_Repack.Vector;

namespace RE4_SAT_EAT_Repack.Structures
{

    public class StartStructure // versão do codigo modificado, em relação as versões usado nos modelos 3d;
    {

        // lista pricipal onde sera atribuida as faces do modelo 3d;
        public List<StartFace> SetOfFaces { get; set; }

        // lista final de com o conjunto de triagulos;
        public List<StartTriangle> SetOfTriangles { get; set; }

        //lista final de Positions;
        public List<Vector3> SetOfPositions { get; set; }

        //lista final de Normals;
        public List<Vector3> SetOfNormals { get; set; }

        public StartStructure()
        {
            SetOfFaces = new List<StartFace>();
        }

        public void CompressAllFaces()
        {
            SetOfTriangles = new List<StartTriangle>();
            SetOfPositions = new List<Vector3>();
            SetOfNormals = new List<Vector3>();

            foreach (var item in SetOfFaces)
            {
                StartTriangle triangle = new StartTriangle();
                triangle.Key = item.Key;

                if (!SetOfNormals.Contains(item.Normal))
                {
                    SetOfNormals.Add(item.Normal);
                }
                int indexnormal = SetOfNormals.IndexOf(item.Normal);
                triangle.IndexNormal = indexnormal;

                //-----
                if (!SetOfPositions.Contains(item.PositionA))
                {
                    SetOfPositions.Add(item.PositionA);
                }

                if (!SetOfPositions.Contains(item.PositionB))
                {
                    SetOfPositions.Add(item.PositionB);
                }

                if (!SetOfPositions.Contains(item.PositionC))
                {
                    SetOfPositions.Add(item.PositionC);
                }

                int indexA = SetOfPositions.IndexOf(item.PositionA);
                int indexB = SetOfPositions.IndexOf(item.PositionB);
                int indexC = SetOfPositions.IndexOf(item.PositionC);

                triangle.IndexPositionA = indexA;
                triangle.IndexPositionB = indexB;
                triangle.IndexPositionC = indexC;

                SetOfTriangles.Add(triangle);
            }

        }

    }

    /// <summary>
    /// Reprenta um triangulo
    /// </summary>
    public class StartTriangle
    {
        public Status3Key Key;

        public int IndexPositionA;
        public int IndexPositionB;
        public int IndexPositionC;

        public int IndexNormal;

        public StartTriangle() { }
    }

    public class StartFace
    {
        public Status3Key Key;

        public Vector3 PositionA;
        public Vector3 PositionB;
        public Vector3 PositionC;

        public Vector3 Normal;

        public StartFace() { }
    }


    //representa um vertex, classe de adaptação do OBJ
    public class StartVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public bool Equals(StartVertex obj)
        {
            return obj.Position == Position
                && obj.Normal == Normal;
        }

        public override bool Equals(object obj)
        {
            return obj is StartVertex o
                && o.Position == Position
                && o.Normal == Normal;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() + Normal.GetHashCode()).GetHashCode();
            }
        }

    }



}
