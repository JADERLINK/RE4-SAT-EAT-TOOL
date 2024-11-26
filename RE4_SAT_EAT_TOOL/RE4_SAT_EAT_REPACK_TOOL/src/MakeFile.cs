using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_SAT_EAT_REPACK
{
    public static class MakeFile
    {
        public static void CreateFile(FileInfo info, IdxEsat idx, (Structures.FinalStructure final, Group.FinalGroupStructure group)[] esatObj, SwitchStatus switchStatus, IsVersion isVersion)
        {
            Endianness endian = Endianness.LittleEndian;
            if (isVersion == IsVersion.IsBigEndian)
            {
                endian = Endianness.BigEndian;
            }

            EndianBinaryWriter bw = new EndianBinaryWriter(info.Create(), endian);

            if (idx.Magic == 0x80) // com conteiner
            {
                int headerLength = 4 + (4 * esatObj.Length);
                byte[] header = new byte[headerLength];
                header[0] = 0x80;
                header[1] = (byte)esatObj.Length;
                EndianBitConverter.GetBytes((ushort)idx.Dummy, endian).CopyTo(header, 2);

                bw.Write(header);

                uint OffsetToOffset = 4;
                uint tempOffset = (uint)headerLength;

                for (int i = 0; i < esatObj.Length; i++)
                {
                    bw.BaseStream.Position = OffsetToOffset;
                    bw.Write((uint)tempOffset);
                    bw.BaseStream.Position = tempOffset;

                    long EndOffset;
                    Write0x20Content(ref bw, esatObj[i].final, esatObj[i].group, switchStatus, tempOffset, out EndOffset, isVersion, endian);

                    tempOffset = (uint)EndOffset;
                    OffsetToOffset += 4;
                }
            }
            else // sem conteiner
            {
                Write0x20Content(ref bw, esatObj[0].final, esatObj[0].group, switchStatus, 0, out _, isVersion, endian);
            }

            long fileLength = bw.BaseStream.Position;
            long parts = (fileLength / 32) +1;
            long dif = (parts * 32) - fileLength;
            byte[] padding = new byte[dif];
            for (int i = 0; i < padding.Length; i++)
            {
                padding[i] = 0xCD;
            }
            bw.Write(padding);  

            bw.Close();
        }

        private static void Write0x20Content(ref EndianBinaryWriter bw, Structures.FinalStructure final, Group.FinalGroupStructure group, SwitchStatus switchStatus, long startOffset, out long EndOffset, IsVersion isVersion, Endianness endian)
        {
            bw.BaseStream.Position = startOffset;

            byte magic = 0x20;
            if (isVersion == IsVersion.IsRE4VR)
            {
                magic = 0xFF;
            }

            byte[] header = new byte[20];
            header[0] = magic;
            header[1] = 0x00;
            EndianBitConverter.GetBytes((ushort)final.Positions.Length, endian).CopyTo(header, 2);
            EndianBitConverter.GetBytes((ushort)final.Normals.Length, endian).CopyTo(header, 4);
            EndianBitConverter.GetBytes((ushort)final.Edges.Length, endian).CopyTo(header, 6);
            header[8] = 0x00;
            header[9] = 0x00;
            EndianBitConverter.GetBytes((ushort)final.FacesCount, endian).CopyTo(header, 10);
            EndianBitConverter.GetBytes((ushort)final.FaceFloorCount, endian).CopyTo(header, 12);
            EndianBitConverter.GetBytes((ushort)final.FaceSlopeCount, endian).CopyTo(header, 14);
            EndianBitConverter.GetBytes((ushort)final.FaceWallCount, endian).CopyTo(header, 16);
            EndianBitConverter.GetBytes((ushort)group.FinalGroupList.Count, endian).CopyTo(header, 18);

            bw.Write(header);

            //Positions
            foreach (var item in final.Positions)
            {
                bw.Write((float)item.X);
                bw.Write((float)item.Y);
                bw.Write((float)item.Z);
            }

            //Normals
            foreach (var item in final.Normals)
            {
                bw.Write((float)item.X);
                bw.Write((float)item.Y);
                bw.Write((float)item.Z);
            }

            //Edges 
            foreach (var item in final.Edges)
            {
                bw.Write((float)item.X);
                bw.Write((float)item.Y);
                bw.Write((float)item.Z);
            }

            //faces
            foreach (var item in final.FinalTriangles)
            {
                byte[] line = new byte[20];
                EndianBitConverter.GetBytes((ushort)item.IndexPositionA, endian).CopyTo(line, 0);
                EndianBitConverter.GetBytes((ushort)item.IndexPositionB, endian).CopyTo(line, 2);
                EndianBitConverter.GetBytes((ushort)item.IndexPositionC, endian).CopyTo(line, 4);
                EndianBitConverter.GetBytes((ushort)item.IndexNormal, endian).CopyTo(line, 6);
                EndianBitConverter.GetBytes((ushort)item.IndexEdgeA, endian).CopyTo(line, 8);
                EndianBitConverter.GetBytes((ushort)item.IndexEdgeB, endian).CopyTo(line, 10);
                EndianBitConverter.GetBytes((ushort)item.IndexEdgeC, endian).CopyTo(line, 12);
                line[14] = 0x00;
                line[15] = 0x00;

                if (switchStatus == SwitchStatus.TrueUHD) // inverte posição para UHD
                {
                    line[16] = item.Key.s2;
                    line[17] = (byte)item.SharedFace;
                    line[18] = item.Key.s0;
                    line[19] = item.Key.s1;
                }
                else if (switchStatus == SwitchStatus.BigEndian) // GC/WII/X360
                {
                    line[16] = (byte)item.SharedFace;
                    line[17] = item.Key.s2;
                    line[18] = item.Key.s1;
                    line[19] = item.Key.s0;
                }
                else // para PS2/2007
                {
                    line[16] = item.Key.s0;
                    line[17] = item.Key.s1;
                    line[18] = item.Key.s2;
                    line[19] = (byte)item.SharedFace;
                }

                bw.Write(line);
            }

            //groups
            foreach (var item in group.FinalGroupList)
            {
                byte[] line = new byte[item.GroupBytesLenght];
                EndianBitConverter.GetBytes((float)item.Pos.X, endian).CopyTo(line, 0);
                EndianBitConverter.GetBytes((float)item.Pos.Y, endian).CopyTo(line, 4);
                EndianBitConverter.GetBytes((float)item.Pos.Z, endian).CopyTo(line, 8);
                EndianBitConverter.GetBytes((float)item.Dim.X, endian).CopyTo(line, 12);
                EndianBitConverter.GetBytes((float)item.Dim.Y, endian).CopyTo(line, 16);
                EndianBitConverter.GetBytes((float)item.Dim.Z, endian).CopyTo(line, 20);
                EndianBitConverter.GetBytes((ushort)item.FloorTriangles.Count, endian).CopyTo(line, 24);
                EndianBitConverter.GetBytes((ushort)item.SlopeTriangles.Count, endian).CopyTo(line, 26);
                EndianBitConverter.GetBytes((ushort)item.WallTriangles.Count, endian).CopyTo(line, 28);
                EndianBitConverter.GetBytes((ushort)item.Flag, endian).CopyTo(line, 30);
                EndianBitConverter.GetBytes((uint)item.BrotherNextPos, endian).CopyTo(line, 32);

                int tempPos = isVersion == IsVersion.IsPS4NS ? 40 : 36; // no ps4/ns o campo BrotherNextPos é 64 bits e vez de 32bits

                foreach (var tri in item.FloorTriangles)
                {
                    EndianBitConverter.GetBytes((ushort)tri, endian).CopyTo(line, tempPos);
                    tempPos += 2;
                }

                foreach (var tri in item.SlopeTriangles)
                {
                    EndianBitConverter.GetBytes((ushort)tri, endian).CopyTo(line, tempPos);
                    tempPos += 2;
                }

                foreach (var tri in item.WallTriangles)
                {
                    EndianBitConverter.GetBytes((ushort)tri, endian).CopyTo(line, tempPos);
                    tempPos += 2;
                }

                bw.Write(line);
            }

            EndOffset = bw.BaseStream.Position;
        }


    }
}
