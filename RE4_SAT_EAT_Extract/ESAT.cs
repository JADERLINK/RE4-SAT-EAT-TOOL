using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_SAT_EAT_EXTRACT
{
    public struct Vec3 
    {
        public float X;
        public float Y;
        public float Z;

        public Vec3(float x, float y, float z) 
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    public class ESatHeader 
    {
        public byte Magic;
        public byte Count;
        public ushort Dummy;
        public ESAT[] Esat;
    }

    public class ESAT
    {
        public byte Magic;
        public byte D01;
        public ushort Vertex_pos_count;
        public ushort Face_normal_count;
        public ushort Corner_data_count;
        public ushort D02;
        public ushort Face_total_count;
        public ushort FaceFloor_count;
        public ushort FaceSlope_count;
        public ushort FaceWall_count;
        public ushort Group_count;

        public Vec3[] Positions;
        public Vec3[] Normals;
        public Vec3[] Edges;
        public Face[] Faces;
        public Group[] Groups;
    }

    public class Face 
    {
        public ushort Vertex0;
        public ushort Vertex1;
        public ushort Vertex2;
        public ushort Normal;
        public ushort Edge0;
        public ushort Edge1;
        public ushort Edge2;
        public ushort D07;
        public byte Status0;
        public byte Status1;
        public byte Status2;
        public byte Status3;
        
    }

    public class Group 
    {
        public Vec3 Bound_Pos;
        public Vec3 Bound_Size;
        public ushort FloorCount;
        public ushort SlopeCount;
        public ushort WallCount;
        public ushort Flag;
        public uint BrotherDistance;
        public ushort[] FaceFloor;
        public ushort[] FaceSlope;
        public ushort[] FaceWall;

        //extras
        public Extras Extra;
    }

}
