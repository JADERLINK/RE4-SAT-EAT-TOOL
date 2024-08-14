using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_REPACK.Vector;

namespace RE4_SAT_EAT_REPACK.Structures
{
    public class FinalStructure
    {
        public FinalTriangle[] FinalTriangles;

        public Vector3[] Positions;
        public Vector3[] Normals;
        public Vector3[] Edges;

        // counts
        public ushort FacesCount;
        public ushort FaceFloorCount;
        public ushort FaceSlopeCount;
        public ushort FaceWallCount;
    }

    public class FinalTriangle
    {
        public YellowByte SharedFace;

        public FaceType Type;

        public Status3Key Key;

        public ushort IndexPositionA;
        public ushort IndexPositionB;
        public ushort IndexPositionC;

        public ushort IndexNormal;

        public ushort IndexEdgeA;
        public ushort IndexEdgeB;
        public ushort IndexEdgeC;

        //---
        public Vector3 BackupPositionA;
        public Vector3 BackupPositionB;
        public Vector3 BackupPositionC;

        public Vector3 BackupNormal;

        public Vector3 BackupEdgeA;
        public Vector3 BackupEdgeB;
        public Vector3 BackupEdgeC;
    }

    [Flags]
    public enum YellowByte : byte
    {
        none = 0x00, // nenhuma flag ativa
        disp_info = 0x01,
        A02 = 0x02,
        A04 = 0x04,
        A08 = 0x08,
        A10 = 0x10,
        face1_shared = 0x20,
        face2_shared = 0x40,
        face3_shared = 0x80
    }


}
