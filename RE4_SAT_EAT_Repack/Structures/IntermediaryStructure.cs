using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_REPACK.Vector;

namespace RE4_SAT_EAT_REPACK.Structures
{
    public class IntermediaryStructure
    {
        public List<IntermediaryTriangle> FaceFloor;
        public List<IntermediaryTriangle> FaceWall;
        public List<IntermediaryTriangle> FaceSlope;

        public Vector3[] Positions;
        public Vector3[] Normals;
        public Vector3[] Edges;

        public IntermediaryStructure() 
        {
            FaceFloor = new List<IntermediaryTriangle>();
            FaceWall = new List<IntermediaryTriangle>();
            FaceSlope = new List<IntermediaryTriangle>();
        }
    }

    public class IntermediaryTriangle 
    {
        public FaceType Type;

        public Status3Key Key;

        public int IndexPositionA;
        public int IndexPositionB;
        public int IndexPositionC;

        public int IndexNormal;

        public int IndexEdgeA;
        public int IndexEdgeB;
        public int IndexEdgeC;

        //---
        public Vector3 BackupPositionA;
        public Vector3 BackupPositionB;
        public Vector3 BackupPositionC;

        public Vector3 BackupNormal;

        public Vector3 BackupEdgeA;
        public Vector3 BackupEdgeB;
        public Vector3 BackupEdgeC;

        public IntermediaryTriangle() { }
    }

    public enum FaceType 
    {
        Null,
        Floor,
        Slope,
        Wall
    }

}
