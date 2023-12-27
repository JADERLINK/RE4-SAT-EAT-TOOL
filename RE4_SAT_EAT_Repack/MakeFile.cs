using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_Repack
{
    public static class MakeFile
    {
        public static void CreateFile(FileInfo info, IdxEsat idx, (Structures.FinalStructure final, Group.FinalGroupStructure group)[] esatObj, bool switchStatus) 
        {
            BinaryWriter bw = new BinaryWriter(info.Create());

            if (idx.Magic == 0x80) // com conteiner
            {
                int headerLength = 4 + (4 * esatObj.Length);
                byte[] header = new byte[headerLength];
                header[0] = 0x80;
                header[1] = (byte)esatObj.Length;
                BitConverter.GetBytes((ushort)idx.Dummy).CopyTo(header, 2);

                bw.Write(header);

                uint OffsetToOffset = 4;
                uint tempOffset = (uint)headerLength;

                for (int i = 0; i < esatObj.Length; i++)
                {
                    bw.BaseStream.Position = OffsetToOffset;
                    bw.Write((uint)tempOffset);
                    bw.BaseStream.Position = tempOffset;

                    long EndOffset;
                    Write0x20Content(ref bw, esatObj[i].final, esatObj[i].group, switchStatus, tempOffset, out EndOffset);

                    tempOffset = (uint)EndOffset;
                    OffsetToOffset += 4;
                }
            }
            else // sem conteiner
            {
                Write0x20Content(ref bw, esatObj[0].final, esatObj[0].group, switchStatus, 0, out _);
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

        private static void Write0x20Content(ref BinaryWriter bw, Structures.FinalStructure final, Group.FinalGroupStructure group, bool switchStatus, long startOffset, out long EndOffset)
        {
            bw.BaseStream.Position = startOffset;

            byte[] header = new byte[20];
            header[0] = 0x20;
            header[1] = 0x00;
            BitConverter.GetBytes((ushort)final.Positions.Length).CopyTo(header, 2);
            BitConverter.GetBytes((ushort)final.Normals.Length).CopyTo(header, 4);
            BitConverter.GetBytes((ushort)final.Edges.Length).CopyTo(header, 6);
            header[8] = 0x00;
            header[9] = 0x00;
            BitConverter.GetBytes((ushort)final.FacesCount).CopyTo(header, 10);
            BitConverter.GetBytes((ushort)final.FaceFloorCount).CopyTo(header, 12);
            BitConverter.GetBytes((ushort)final.FaceSlopeCount).CopyTo(header, 14);
            BitConverter.GetBytes((ushort)final.FaceWallCount).CopyTo(header, 16);
            BitConverter.GetBytes((ushort)group.FinalGroupList.Count).CopyTo(header, 18);

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
                BitConverter.GetBytes((ushort)item.IndexPositionA).CopyTo(line, 0);
                BitConverter.GetBytes((ushort)item.IndexPositionB).CopyTo(line, 2);
                BitConverter.GetBytes((ushort)item.IndexPositionC).CopyTo(line, 4);
                BitConverter.GetBytes((ushort)item.IndexNormal).CopyTo(line, 6);
                BitConverter.GetBytes((ushort)item.IndexEdgeA).CopyTo(line, 8);
                BitConverter.GetBytes((ushort)item.IndexEdgeB).CopyTo(line, 10);
                BitConverter.GetBytes((ushort)item.IndexEdgeC).CopyTo(line, 12);
                line[14] = 0x00;
                line[15] = 0x00;

                if (switchStatus) // inverte posição para UHD
                {
                    line[16] = item.Key.s2;
                    line[17] = (byte)item.SharedFace;
                    line[18] = item.Key.s0;
                    line[19] = item.Key.s1;
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
                BitConverter.GetBytes((float)item.Pos.X).CopyTo(line, 0);
                BitConverter.GetBytes((float)item.Pos.Y).CopyTo(line, 4);
                BitConverter.GetBytes((float)item.Pos.Z).CopyTo(line, 8);
                BitConverter.GetBytes((float)item.Dim.X).CopyTo(line, 12);
                BitConverter.GetBytes((float)item.Dim.Y).CopyTo(line, 16);
                BitConverter.GetBytes((float)item.Dim.Z).CopyTo(line, 20);
                BitConverter.GetBytes((ushort)item.FloorTriangles.Count).CopyTo(line, 24);
                BitConverter.GetBytes((ushort)item.SlopeTriangles.Count).CopyTo(line, 26);
                BitConverter.GetBytes((ushort)item.WallTriangles.Count).CopyTo(line, 28);
                BitConverter.GetBytes((ushort)item.Flag).CopyTo(line, 30);
                BitConverter.GetBytes((uint)item.BrotherNextPos).CopyTo(line, 32);

                int tempPos = 36;

                foreach (var tri in item.FloorTriangles)
                {
                    BitConverter.GetBytes((ushort)tri).CopyTo(line, tempPos);
                    tempPos += 2;
                }

                foreach (var tri in item.SlopeTriangles)
                {
                    BitConverter.GetBytes((ushort)tri).CopyTo(line, tempPos);
                    tempPos += 2;
                }

                foreach (var tri in item.WallTriangles)
                {
                    BitConverter.GetBytes((ushort)tri).CopyTo(line, tempPos);
                    tempPos += 2;
                }

                bw.Write(line);
            }

            EndOffset = bw.BaseStream.Position;
        }


    }
}
