using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_SAT_EAT_EXTRACT
{
    public static class Extractor
    {
        public static ESatHeader Extract(Stream stream, IsVersion isVersion)
        {
            ESatHeader header = new ESatHeader();

            Endianness endian = isVersion == IsVersion.IsBigEndian ? Endianness.BigEndian : Endianness.LittleEndian;

            EndianBinaryReader br = new EndianBinaryReader(stream, endian);

            byte magic = br.ReadByte();
            header.Magic = magic;

            if (magic != 0xFF && isVersion == IsVersion.IsRE4VR)
            {
                throw new ArgumentException("This file is not from the Re4Vr version!");
            }
            else if (magic == 0xFF && isVersion != IsVersion.IsRE4VR)
            {
                throw new ArgumentException("This is a file from the Re4Vr version!");
            }
            else if (magic == 0x80)
            {
                byte count = br.ReadByte();
                ushort dummy = br.ReadUInt16();

                header.Count = count;
                header.Dummy = dummy;
                header.Esat = new ESAT[count];

                uint[] offsets = new uint[count];
                for (int i = 0; i < count; i++)
                {
                    offsets[i] = br.ReadUInt32();
                }

                for (int i = 0; i < count; i++)
                {
                    br.BaseStream.Position = offsets[i];
                    ESAT esat = Extract20(ref br, isVersion);
                    header.Esat[i] = esat;
                }
            }
            else if (magic == 0x20 || magic == 0xFF)
            {
                header.Count = 1;
                header.Dummy = 0;
                header.Esat = new ESAT[1];

                br.BaseStream.Position = 0;
                ESAT esat = Extract20(ref br, isVersion);
                header.Esat[0] = esat;
            }

            return header;
        }


        private static ESAT Extract20(ref EndianBinaryReader br, IsVersion isVersion)
        {
            ESAT esat = new ESAT();

            byte magic = br.ReadByte();
            byte d01 = br.ReadByte(); // sempre 0
            ushort vertex_pos_count = br.ReadUInt16(); // v tag
            ushort face_normal_count = br.ReadUInt16(); // vn tag
            ushort corner_data_count = br.ReadUInt16();
            ushort d02 = br.ReadUInt16(); // padding
            ushort face_total_count = br.ReadUInt16();
            ushort faceF_count = br.ReadUInt16();
            ushort faceS_count = br.ReadUInt16();
            ushort faceW_count = br.ReadUInt16();
            ushort group_count = br.ReadUInt16();

            esat.Magic = magic;
            esat.D01 = d01;
            esat.Vertex_pos_count = vertex_pos_count;
            esat.Face_normal_count = face_normal_count;
            esat.Corner_data_count = corner_data_count;
            esat.D02 = d02;
            esat.Face_total_count = face_total_count;
            esat.FaceFloor_count = faceF_count;
            esat.FaceSlope_count = faceS_count;
            esat.FaceWall_count = faceW_count;
            esat.Group_count = group_count;

            esat.Positions = new Vec3[vertex_pos_count];
            esat.Normals = new Vec3[face_normal_count];
            esat.Edges = new Vec3[corner_data_count];
            esat.Faces = new Face[face_total_count];
            esat.Groups = new Group[group_count];


            for (int i = 0; i < vertex_pos_count; i++)
            {
                float x = br.ReadSingle();
                float y = br.ReadSingle();
                float z = br.ReadSingle();

                esat.Positions[i] = new Vec3(x, y, z);
            }

            for (int i = 0; i < face_normal_count; i++)
            {
                float x = br.ReadSingle();
                float y = br.ReadSingle();
                float z = br.ReadSingle();

                esat.Normals[i] = new Vec3(x, y, z);
            }

            for (int i = 0; i < corner_data_count; i++)
            {
                float x = br.ReadSingle();
                float y = br.ReadSingle();
                float z = br.ReadSingle();

                esat.Edges[i] = new Vec3(x, y, z);
            }

            for (int i = 0; i < face_total_count; i++)
            {

                ushort pos_index1 = br.ReadUInt16();
                ushort pos_index2 = br.ReadUInt16();
                ushort pos_index3 = br.ReadUInt16();
                ushort norm_index = br.ReadUInt16();
                ushort edge_index1 = br.ReadUInt16();
                ushort edge_index2 = br.ReadUInt16();
                ushort edge_index3 = br.ReadUInt16();
                ushort d07 = br.ReadUInt16(); // padding
                byte status0 = br.ReadByte();
                byte status1 = br.ReadByte();
                byte status2 = br.ReadByte();
                byte status3 = br.ReadByte();

                Face face = new Face();
                face.Vertex0 = pos_index1;
                face.Vertex1 = pos_index2;
                face.Vertex2 = pos_index3;
                face.Normal = norm_index;
                face.Edge0 = edge_index1;
                face.Edge1 = edge_index2;
                face.Edge2 = edge_index3;
                face.D07 = d07;
                face.Status0 = status0;
                face.Status1 = status1;
                face.Status2 = status2;
                face.Status3 = status3;
                esat.Faces[i] = face;
            }

            long offsetTemp = br.BaseStream.Position;

            for (int i = 0; i < group_count; i++)
            {
                float bound_posx = br.ReadSingle();
                float bound_posy = br.ReadSingle();
                float bound_posz = br.ReadSingle();

                // dimenção altura largura e profundidade
                float bound_dimx = br.ReadSingle();
                float bound_dimy = br.ReadSingle();
                float bound_dimz = br.ReadSingle();

                ushort count1 = br.ReadUInt16();
                ushort count2 = br.ReadUInt16();
                ushort count3 = br.ReadUInt16();

                ushort flag = br.ReadUInt16();

                uint brotherDistance = br.ReadUInt32();

                if (isVersion == IsVersion.IsPS4NS)
                {
                    uint ps4NsExtra = br.ReadUInt32();
                    // pois o campo acima é um offset, e no ps4/ns offsets são de 64 bytes
                }

                ushort[] face1 = new ushort[count1];
                ushort[] face2 = new ushort[count2];
                ushort[] face3 = new ushort[count3];

                if (isVersion != IsVersion.IsRE4VR || (isVersion == IsVersion.IsRE4VR && flag == 2))
                {
                    for (int f = 0; f < count1; f++)
                    {
                        face1[f] = br.ReadUInt16();
                    }

                    for (int f = 0; f < count2; f++)
                    {
                        face2[f] = br.ReadUInt16();
                    }

                    for (int f = 0; f < count3; f++)
                    {
                        face3[f] = br.ReadUInt16();
                    }
                }

                // padding
                if (isVersion != IsVersion.IsRE4VR)
                {
                    int sum = (count1 + count2 + count3);
                    int rest = sum % 2;

                    for (int p = 0; p < rest; p++)
                    {
                        _ = br.ReadUInt16(); // padding
                    }
                }

                long tempPos = br.BaseStream.Position;

                Group group = new Group();
                group.Bound_Pos = new Vec3(bound_posx, bound_posy, bound_posz);
                group.Bound_Size = new Vec3(bound_dimx, bound_dimy, bound_dimz);
                group.FloorCount = count1;
                group.SlopeCount = count2;
                group.WallCount = count3;
                group.Flag = flag;
                group.BrotherDistance = brotherDistance;
                group.FaceFloor = face1;
                group.FaceSlope = face2;
                group.FaceWall = face3;

                group.Extra.GroupBytesLenght = (int)(tempPos - offsetTemp);
                offsetTemp = tempPos;
                esat.Groups[i] = group;
            }

            //calcula BrotherID

            for (int i = 0; i < group_count; i++)
            {
                if (esat.Groups[i].BrotherDistance != 0)
                {
                    int temp = 0;

                    for (int ii = i; ii < group_count; ii++)
                    {
                        temp += esat.Groups[ii].Extra.GroupBytesLenght;

                        if (temp >= esat.Groups[i].BrotherDistance)
                        {
                            esat.Groups[i].Extra.BrotherID = ii + 1;
                            break;
                        }
                    }
                }
                else 
                {
                    esat.Groups[i].Extra.BrotherID = -1;
                }
            }


            return esat;
        }


    }
}
