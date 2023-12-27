using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_Extract
{
    public static class OutputFiles
    {
        public static void IDX(FileInfo info, ESatHeader header)
        {
            var text = info.CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");
            text.WriteLine("Magic:" + header.Magic.ToString("X2"));
            text.WriteLine("Count:" + header.Count);
            text.WriteLine("Dummy:" + header.Dummy.ToString("X4"));
            text.Close();
        }

        public static void EsatOBJ(FileInfo info, ESAT esat, bool switchStatus)
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            var text = info.CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            for (int i = 0; i < esat.Vertex_pos_count; i++)
            {
                float x = esat.Positions[i].X;
                float y = esat.Positions[i].Y;
                float z = esat.Positions[i].Z;

                text.WriteLine("v " + (x / 100f).ToString("F9", inv) + " " +
                     (y / 100f).ToString("F9", inv) + " " +
                     (z / 100f).ToString("F9", inv));
            }

            for (int i = 0; i < esat.Face_normal_count; i++)
            {
                float x = esat.Normals[i].X;
                float y = esat.Normals[i].Y;
                float z = esat.Normals[i].Z;

                text.WriteLine("vn " + x.ToString("F9", inv) + " " +
                    y.ToString("F9", inv) + " " +
                    z.ToString("F9", inv));

            }

            Dictionary<Status3Key, List<Face>> facesDic = new Dictionary<Status3Key, List<Face>>();

            for (int i = 0; i < esat.Faces.Length; i++)
            {
                Status3Key key = new Status3Key();
                if (switchStatus)
                {
                    key.s0 = esat.Faces[i].Status2;
                    key.s1 = esat.Faces[i].Status3;
                    key.s2 = esat.Faces[i].Status0;
                }
                else
                {
                    key.s0 = esat.Faces[i].Status0;
                    key.s1 = esat.Faces[i].Status1;
                    key.s2 = esat.Faces[i].Status2;
                }

                if (facesDic.ContainsKey(key))
                {
                    facesDic[key].Add(esat.Faces[i]);
                }
                else
                {
                    facesDic.Add(key, new List<Face>() { esat.Faces[i] });
                }
            }

            foreach (var item in facesDic)
            {
                byte[] status = new byte[3];
                status[0] = item.Key.s0;
                status[1] = item.Key.s1;
                status[2] = item.Key.s2;

                text.WriteLine("g Collision#" + BitConverter.ToString(status) + "#");

                for (int i = 0; i < item.Value.Count; i++)
                {
                    var face = item.Value[i];
                    text.WriteLine("f " +
                        (face.Vertex0 + 1) + "//" +
                        (face.Normal + 1) + " " +
                        (face.Vertex1 + 1) + "//" +
                        (face.Normal + 1) + " " +
                        (face.Vertex2 + 1) + "//" +
                        (face.Normal + 1));
                }

            }

            text.Close();
        }


    }


}
