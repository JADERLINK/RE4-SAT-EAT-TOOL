using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_SAT_EAT_REPACK.Structures;
using RE4_SAT_EAT_REPACK.Vector;

namespace RE4_SAT_EAT_REPACK
{
    public static class RepackOBJ
    {
        public static FinalStructure Repack(StreamReader streamReader) 
        {
            string pattern = "^(COLLISION#)([0-9|A-F]{1,2})(-)([0-9|A-F]{1,2})(-)([0-9|A-F]{1,2})(#|-).*$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant);
            //--------

            // load .obj file
            ObjLoader.Loader.Loaders.LoadResult arqObj = null;
            try
            {
                var objLoaderFactory = new ObjLoader.Loader.Loaders.ObjLoaderFactory();
                var objLoader = objLoaderFactory.Create();
                arqObj = objLoader.Load(streamReader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                streamReader.Close();
            }
 
            //conjunto de faces
            StartStructure ObjList = new StartStructure();

            for (int iG = 0; iG < arqObj.Groups.Count; iG++)
            {
                string GroupName = arqObj.Groups[iG].GroupName.ToUpperInvariant().Trim();

                if (GroupName.StartsWith("COLLISION"))
                {
                    //FIX NAME
                    GroupName = GroupName.Replace("_", "-").Replace("COLLISION-", "COLLISION#");

                    //REGEX
                    if (regex.IsMatch(GroupName))
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName);
                    }
                    else
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName + "  The group name is wrong;");
                    }

                    Status3Key info = getGroupInfo(GroupName);

                    List<List<StartVertex>> facesList = new List<List<StartVertex>>();

                    for (int iF = 0; iF < arqObj.Groups[iG].Faces.Count; iF++)
                    {
                        List<StartVertex> verticeListInObjFace = new List<StartVertex>();

                        for (int iI = 0; iI < arqObj.Groups[iG].Faces[iF].Count; iI++)
                        {
                            StartVertex vertice = new StartVertex();

                            if (arqObj.Groups[iG].Faces[iF][iI].VertexIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1 >= arqObj.Vertices.Count)
                            {
                                throw new ArgumentException("Vertex Position Index is invalid! Value: " + arqObj.Groups[iG].Faces[iF][iI].VertexIndex);
                            }

                            Vector3 position = new Vector3(
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].X * 100,
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Y * 100,
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Z * 100
                                );

                            vertice.Position = position;

                            if (arqObj.Groups[iG].Faces[iF][iI].NormalIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1 >= arqObj.Normals.Count)
                            {
                                vertice.Normal = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                Vector3 normal = new Vector3(
                                arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].X,
                                arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Y,
                                arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Z
                                );

                                vertice.Normal = normal;
                            }

                            verticeListInObjFace.Add(vertice);

                        }

                        if (verticeListInObjFace.Count >= 3)
                        {
                            for (int i = 2; i < verticeListInObjFace.Count; i++)
                            {
                                List<StartVertex> face = new List<StartVertex>();
                                face.Add(verticeListInObjFace[0]);
                                face.Add(verticeListInObjFace[i - 1]);
                                face.Add(verticeListInObjFace[i]);
                                facesList.Add(face);
                            }
                        }

                    }




                    for (int i = 0; i < facesList.Count; i++)
                    {
                        var face = facesList[i];
                        StartFace startFace = new StartFace();
                        startFace.Key = info;
                        startFace.PositionA = face[0].Position;
                        startFace.PositionB = face[1].Position;
                        startFace.PositionC = face[2].Position;

                        float nX = (face[0].Normal.X + face[1].Normal.X + face[2].Normal.X) / 3;
                        float nY = (face[0].Normal.Y + face[1].Normal.Y + face[2].Normal.Y) / 3;
                        float nZ = (face[0].Normal.Z + face[1].Normal.Z + face[2].Normal.Z) / 3;

                        startFace.Normal = new Vector3(nX, nY, nZ);

                        ObjList.SetOfFaces.Add(startFace);
                    }

                }
                else
                {
                    Console.WriteLine("Loading in Obj: " + GroupName + "   Warning: Group not used;");
                }
            }

            // comprime os vertices
            ObjList.CompressAllFaces();

            var intermediary = MakeIntermediary.GetIntermediaryStructure(ObjList);
            var final = MakeFinalTriangleList.GetFinalStructure(intermediary);
            return final;

        }

        private static Status3Key getGroupInfo(string GroupName)
        {
            Status3Key line = new Status3Key();
            line.s0 = 0;
            line.s1 = 0;
            line.s2 = 0;

            var split = GroupName.Split('#');
            try
            {
                var subSplit = split[1].Split('-');

                byte s0 = byte.Parse(Utils.ReturnValidHexValue(subSplit[0].ToUpperInvariant().Replace("0X", "")), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                byte s1 = byte.Parse(Utils.ReturnValidHexValue(subSplit[1].ToUpperInvariant().Replace("0X", "")), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                byte s2 = byte.Parse(Utils.ReturnValidHexValue(subSplit[2].ToUpperInvariant().Replace("0X", "")), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);

                line.s0 = s0;
                line.s1 = s1;
                line.s2 = s2;
            }
            catch (Exception)
            {
            }

            return line;
        }

    }
}
