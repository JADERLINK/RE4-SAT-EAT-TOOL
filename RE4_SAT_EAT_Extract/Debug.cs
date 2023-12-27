using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_SAT_EAT_Extract
{
    public static class Debug
    {
        public static void EsatDebugOBJ(FileInfo info, ESAT esat, bool switchStatus)
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            var text = info.CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            text.WriteLine("#Info:");
            text.WriteLine("#Magic:" + esat.Magic.ToString("X2"));
            text.WriteLine("#D01:" + esat.D01.ToString("X2"));
            text.WriteLine("#Vertex_pos_count:" + esat.Vertex_pos_count);
            text.WriteLine("#Face_normal_count:" + esat.Face_normal_count);
            text.WriteLine("#Corner_data_count:" + esat.Corner_data_count);
            text.WriteLine("#D02:" + esat.D02.ToString("X4"));
            text.WriteLine("#Face_total_count:" + esat.Face_total_count);
            text.WriteLine("#FaceFloor_count:" + esat.FaceFloor_count);
            text.WriteLine("#FaceSlope_count:" + esat.FaceSlope_count);
            text.WriteLine("#FaceWall_count:" + esat.FaceWall_count);
            text.WriteLine("#Group_count:" + esat.Group_count);

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

            string faceType = "F";

            for (int i = 0; i < esat.Faces.Length; i++)
            {
                if (i == esat.FaceFloor_count)
                {
                    faceType = "S";
                }

                if (i == esat.FaceSlope_count + esat.FaceFloor_count)
                {
                    faceType = "W";
                }

                var face = esat.Faces[i];

                Status4Key key = new Status4Key();
                key.s0 = face.Status0;
                key.s1 = face.Status1;
                key.s2 = face.Status2;
                key.s3 = face.Status3;

                byte[] status = new byte[4];
                if (switchStatus)
                {
                    status[0] = key.s2;
                    status[1] = key.s3;
                    status[2] = key.s0;
                    status[3] = key.s1;
                }
                else
                {
                    status[0] = key.s0;
                    status[1] = key.s1;
                    status[2] = key.s2;
                    status[3] = key.s3;
                }

                text.WriteLine("g Debug-ID_" + i.ToString("D4") + "-Face" + faceType + "#" + BitConverter.ToString(status) + "#");

                text.WriteLine("f " +
                    (face.Vertex0 + 1) + "//" +
                    (face.Normal + 1) + " " +
                    (face.Vertex1 + 1) + "//" +
                    (face.Normal + 1) + " " +
                    (face.Vertex2 + 1) + "//" +
                    (face.Normal + 1));
            }

            text.Close();
        }

        public static void EsatLinesOBJ(FileInfo info, ESAT esat, bool switchStatus) 
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            var text = info.CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            text.WriteLine("#Info:");
            text.WriteLine("#Magic:" + esat.Magic.ToString("X2"));
            text.WriteLine("#D01:" + esat.D01.ToString("X2"));
            text.WriteLine("#Vertex_pos_count:" + esat.Vertex_pos_count);
            text.WriteLine("#Face_normal_count:" + esat.Face_normal_count);
            text.WriteLine("#Corner_data_count:" + esat.Corner_data_count);
            text.WriteLine("#D02:" + esat.D02.ToString("X4"));
            text.WriteLine("#Face_total_count:" + esat.Face_total_count);
            text.WriteLine("#FaceFloor_count:" + esat.FaceFloor_count);
            text.WriteLine("#FaceSlope_count:" + esat.FaceSlope_count);
            text.WriteLine("#FaceWall_count:" + esat.FaceWall_count);
            text.WriteLine("#Group_count:" + esat.Group_count);

            text.WriteLine("");
            for (int i = 0; i < esat.Vertex_pos_count; i++)
            {
                float x = esat.Positions[i].X;
                float y = esat.Positions[i].Y;
                float z = esat.Positions[i].Z;

                text.WriteLine("#Vertice ID: " + (i+1));
                text.WriteLine("v " + (x / 100f).ToString("F9", inv) + " " +
                     (y / 100f).ToString("F9", inv) + " " +
                     (z / 100f).ToString("F9", inv));
            }

            for (int i = 0; i < esat.Face_normal_count; i++)
            {
                float x = esat.Normals[i].X;
                float y = esat.Normals[i].Y;
                float z = esat.Normals[i].Z;

                text.WriteLine("#Normal ID: " + (i + 1));
                text.WriteLine("vn " + x.ToString("F9", inv) + " " +
                    y.ToString("F9", inv) + " " +
                    z.ToString("F9", inv));
            }

            for (int i = 0; i < esat.Corner_data_count; i++)
            {
                float x = esat.Edges[i].X;
                float y = esat.Edges[i].Y;
                float z = esat.Edges[i].Z;

                text.WriteLine("#Edge ID: " + (i + 1));
                text.WriteLine("# " + (x / 100f).ToString("F9", inv) + " " +
                     (y / 100f).ToString("F9", inv) + " " +
                     (z / 100f).ToString("F9", inv));
            }

            string faceType = "F";

            for (int i = 0; i < esat.Faces.Length; i++)
            {
                if (i == esat.FaceFloor_count)
                {
                    faceType = "S";
                }

                if (i == esat.FaceSlope_count + esat.FaceFloor_count)
                {
                    faceType = "W";
                }

                var face = esat.Faces[i];

                Status4Key key = new Status4Key();
                key.s0 = face.Status0;
                key.s1 = face.Status1;
                key.s2 = face.Status2;
                key.s3 = face.Status3;

                byte[] status = new byte[4];
                if (switchStatus)
                {
                    status[0] = key.s2;
                    status[1] = key.s3;
                    status[2] = key.s0;
                    status[3] = key.s1;
                }
                else
                {
                    status[0] = key.s0;
                    status[1] = key.s1;
                    status[2] = key.s2;
                    status[3] = key.s3;
                }

                text.WriteLine("#face: " + (face.Vertex0 + 1) 
                    + " " + (face.Vertex1 + 1) 
                    + " " + (face.Vertex2 + 1) 
                    + " | " + (face.Normal + 1)
                    + " | " + (face.Edge0 + 1)
                    + " " + (face.Edge1 + 1)
                    + " " + (face.Edge2 + 1)
                    );

                string ab = (face.Vertex0 + 1) + " " + (face.Vertex1 + 1);
                string ac = (face.Vertex0 + 1) + " " + (face.Vertex2 + 1);
                string bc = (face.Vertex1 + 1) + " " + (face.Vertex2 + 1);

                byte face1 = (byte)(status[3] & 0x20);
                byte face2 = (byte)(status[3] & 0x40);
                byte face3 = (byte)(status[3] & 0x80);

                string sface1 = face1 == 0x20 ? "Enable" : "";
                string sface2 = face2 == 0x40 ? "Enable" : "";
                string sface3 = face3 == 0x80 ? "Enable" : "";


                text.WriteLine("g Lines-ID_" + i.ToString("D4") + "-Face" + faceType + "#" + BitConverter.ToString(status) + "#AB: " + ab + " " + sface1);
                text.WriteLine("l " + ab);

                text.WriteLine("g Lines-ID_" + i.ToString("D4") + "-Face" + faceType + "#" + BitConverter.ToString(status) + "#BC: " + bc + " " + sface2);
                text.WriteLine("l " + bc);

                text.WriteLine("g Lines-ID_" + i.ToString("D4") + "-Face" + faceType + "#" + BitConverter.ToString(status) + "#AC: " + ac + " " + sface3);
                text.WriteLine("l " + ac);

            }

            text.Close();
        }


        private static void SetGroupGroupNameP1(ref StreamWriter text, Group group, int ID)
        {
            text.Write("g Group_" + ID.ToString("D4") + "#Flag_" + group.Flag.ToString("X4") + "#");

            if (group.Extra.BrotherID != -1)
            {
                text.Write("Brother_" + group.Extra.BrotherID.ToString("D4") + "#");
            }
            else
            {
                text.Write("NoBrother#");
            }

            
        }

        private static void SetGroupGroupNameP2(ref StreamWriter text, Group group, int ID)
        {
            if (group.FaceFloor.Length != 0 || group.FaceSlope.Length != 0 || group.FaceWall.Length != 0)
            {
                if (group.FaceFloor.Length != 0)
                {
                    text.Write("FaceF: " + string.Join(" ", group.FaceFloor) + " #");
                }
                if (group.FaceSlope.Length != 0)
                {
                    text.Write("FaceS: " + string.Join(" ", group.FaceSlope) + " #");
                }
                if (group.FaceWall.Length != 0)
                {
                    text.Write("FaceW: " + string.Join(" ", group.FaceWall) + " #");
                }

            }
            text.WriteLine();
        }


        public static void EsatGroupBoxOBJ(FileInfo info, ESAT esat) 
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            var text = info.CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            int index = 1;

            for (int i = 0; i < esat.Groups.Length; i++)
            {
                var group = esat.Groups[i];

                SetGroupGroupNameP1(ref text, group, i);
                SetGroupGroupNameP2(ref text, group, i);

                var bound_pos = esat.Groups[i].Bound_Pos;
                var bound_dim = esat.Groups[i].Bound_Size;

                //original -- A
                text.WriteLine("v " + (bound_pos.X / 100f).ToString("F9", inv) + " " +
                   (bound_pos.Y / 100f).ToString("F9", inv) + " " +
                    (bound_pos.Z / 100f).ToString("F9", inv));

                //soma dos 3 -- G
                text.WriteLine("v " + ((bound_pos.X + bound_dim.X) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Y + bound_dim.Y) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Z + bound_dim.Z) / 100f).ToString("F9", inv));

                //X -- D
                text.WriteLine("v " + ((bound_pos.X + bound_dim.X) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Y) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Z) / 100f).ToString("F9", inv));

                //Y -- E
                text.WriteLine("v " + ((bound_pos.X) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y + bound_dim.Y) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Z) / 100f).ToString("F9", inv));

                //Z -- B
                text.WriteLine("v " + ((bound_pos.X) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Z + bound_dim.Z) / 100f).ToString("F9", inv));

                //XY -- H
                text.WriteLine("v " + ((bound_pos.X + bound_dim.X) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y + bound_dim.Y) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Z) / 100f).ToString("F9", inv));

                //XZ -- C
                text.WriteLine("v " + ((bound_pos.X + bound_dim.X) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Z + bound_dim.Z) / 100f).ToString("F9", inv));

                //YZ -- F
                text.WriteLine("v " + ((bound_pos.X) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y + bound_dim.Y) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Z + bound_dim.Z) / 100f).ToString("F9", inv));


                int A = 0 + index;
                int B = 4 + index;
                int C = 6 + index;
                int D = 2 + index;
                int E = 3 + index;
                int F = 7 + index;
                int G = 1 + index;
                int H = 5 + index;

                //LINHA DE FORA DO CUBO
                text.WriteLine("l " + A + " " + B);
                text.WriteLine("l " + C + " " + B);
                text.WriteLine("l " + C + " " + D);
                text.WriteLine("l " + A + " " + D);
                text.WriteLine("l " + E + " " + F);
                text.WriteLine("l " + F + " " + G);
                text.WriteLine("l " + G + " " + H);
                text.WriteLine("l " + H + " " + E);
                text.WriteLine("l " + A + " " + E);
                text.WriteLine("l " + F + " " + B);
                text.WriteLine("l " + C + " " + G);
                text.WriteLine("l " + D + " " + H);

                //LINHAS DIAGONAIS DO CUBO
                text.WriteLine("l " + A + " " + C);
                text.WriteLine("l " + E + " " + G);
                text.WriteLine("l " + A + " " + F);
                text.WriteLine("l " + D + " " + G);
                text.WriteLine("l " + C + " " + F);
                text.WriteLine("l " + D + " " + E);

                index += 8;
            }

            text.Close();
        }

        public static void EsatGroupArrowOBJ(FileInfo info, ESAT esat) 
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            var text = info.CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            int index = 1;

            Dictionary<int, float> climbHeightPerTier = new Dictionary<int, float>();

            Dictionary<int, GroupTier> TierDic = null;
            var main = CreateGroupTierList(esat.Groups, out TierDic);

            for (int i = 0; i < esat.Groups.Length; i++)
            {
                var group = esat.Groups[i];

                var bound_pos = esat.Groups[i].Bound_Pos;
                var bound_dim = esat.Groups[i].Bound_Size;


                float climbHeight = 0;

                int tier = TierDic[i].Tier;
                if (climbHeightPerTier.ContainsKey(tier))
                {
                    climbHeight = climbHeightPerTier[tier];
                }
                else
                {
                    float tempvalue = 0;
                    for (int j = tier - 1; j > -1; j--)
                    {
                        tempvalue += 1000;
                    }
                    climbHeightPerTier.Add(tier, tempvalue);
                    climbHeight = climbHeightPerTier[tier];
                }

                climbHeight += bound_dim.Y;

                SetGroupGroupNameP1(ref text, group, i);
                text.Write("Tier_" + TierDic[i].Tier.ToString("D2") + "#");
                SetGroupGroupNameP2(ref text, group, i);

   
                text.WriteLine("v " + (bound_pos.X / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y + climbHeight) / 100f).ToString("F9", inv) + " " +
                 (bound_pos.Z / 100f).ToString("F9", inv));

                text.WriteLine("v " + ((bound_pos.X + 1000) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Y + climbHeight + 1000) / 100f).ToString("F9", inv) + " " +
                  (bound_pos.Z / 100f).ToString("F9", inv));

                text.WriteLine("v " + ((bound_pos.X - 1000) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Y + climbHeight + 1000) / 100f).ToString("F9", inv) + " " +
                  (bound_pos.Z / 100f).ToString("F9", inv));

                text.WriteLine("f " + index + " " + (index + 1) + " " + (index + 2));

                index += 3;
            }

            text.Close();
        }

        public static void EsatGroupPlaneOBJ(FileInfo info, ESAT esat) 
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            var text = info.CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            int index = 1;

            Dictionary<int, float> climbHeightPerTier = new Dictionary<int, float>();

            Dictionary<int, GroupTier> TierDic = null;
            var main = CreateGroupTierList(esat.Groups, out TierDic);

            for (int i = 0; i < esat.Groups.Length; i++)
            {
                var group = esat.Groups[i];

                float climbHeight = 0;

                int tier = TierDic[i].Tier;
                if (climbHeightPerTier.ContainsKey(tier))
                {
                    climbHeight = climbHeightPerTier[tier];
                }
                else 
                {
                    float tempvalue = 0;
                    for (int j = tier -1; j > -1; j--)
                    {
                        tempvalue += 500;
                    }
                    climbHeightPerTier.Add(tier, tempvalue);
                    climbHeight = climbHeightPerTier[tier];
                }

                climbHeight -= 6000;

                SetGroupGroupNameP1(ref text, group, i);
                text.Write("Tier_" + TierDic[i].Tier.ToString("D2") + "#");
                SetGroupGroupNameP2(ref text, group, i);

                text.WriteLine("#BrotherDistance:" + group.BrotherDistance.ToString("X8"));
                text.WriteLine("#Extra.GroupBytesLenght:" + group.Extra.GroupBytesLenght.ToString("X8"));

                var bound_pos = esat.Groups[i].Bound_Pos;
                var bound_dim = esat.Groups[i].Bound_Size;

                //original -- A
                text.WriteLine("v " + (bound_pos.X / 100f).ToString("F9", inv) + " " +
                   ((bound_pos.Y + climbHeight) / 100f).ToString("F9", inv) + " " +
                    (bound_pos.Z / 100f).ToString("F9", inv));

                //X -- D
                text.WriteLine("v " + ((bound_pos.X + bound_dim.X) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Y + climbHeight) / 100f).ToString("F9", inv) + " " +
                 ((bound_pos.Z) / 100f).ToString("F9", inv));

                //Z -- B
                text.WriteLine("v " + ((bound_pos.X) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y + climbHeight) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Z + bound_dim.Z) / 100f).ToString("F9", inv));

                //XZ -- C
                text.WriteLine("v " + ((bound_pos.X + bound_dim.X) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Y + climbHeight) / 100f).ToString("F9", inv) + " " +
                ((bound_pos.Z + bound_dim.Z) / 100f).ToString("F9", inv));


                text.WriteLine("f " + (index + 2) + " " + (index + 3) + " " + (index));
                text.WriteLine("f "  + (index + 3) + " " + (index + 1) + " " + (index));

                index += 4;
            }


            text.Close();
        }


        private static GroupTier CreateGroupTierList(Group[] Groups, out Dictionary<int, GroupTier> TierDic) 
        {
            TierDic = new Dictionary<int, GroupTier>();

            // Group Id 0 pai de todos
            GroupTier main = new GroupTier(0, 0);
            TierDic.Add(0, main);

            if (Groups.Length > 1)
            {
                int tempID = 1;
                int tempTier = 1;
                GroupTierRecursive(Groups, ref main, tempID, tempTier, ref TierDic);
            }
            return main;
        }

        private static void GroupTierRecursive(Group[] Groups, ref GroupTier temp, int StartID, int tier, ref Dictionary<int, GroupTier> TierDic) 
        {
            Group group1 = Groups[StartID];
            GroupTier inner1 = new GroupTier(StartID, tier);
            inner1.Father = temp;
            temp.child1 = inner1;

            Group group2 = null;
            GroupTier inner2 = null;
            if (group1.Extra.BrotherID != -1)
            {
                group2 = Groups[group1.Extra.BrotherID];
                inner2 = new GroupTier(group1.Extra.BrotherID, tier);
                inner2.Father = temp;
                temp.child2 = inner2;
            }

            Group group3 = null;
            GroupTier inner3 = null;
            if (group2 != null && group2.Extra.BrotherID != -1)
            {
                group3 = Groups[group2.Extra.BrotherID];
                inner3 = new GroupTier(group2.Extra.BrotherID, tier);
                inner3.Father = temp;
                temp.child3 = inner3;
            }

            Group group4 = null;
            GroupTier inner4 = null;
            if (group3 != null && group3.Extra.BrotherID != -1)
            {
                group4 = Groups[group3.Extra.BrotherID];
                inner4 = new GroupTier(group3.Extra.BrotherID, tier);
                inner4.Father = temp;
                temp.child4 = inner4;
            }

            inner1.BrotherNext = inner2;
            TierDic.Add(inner1.ID, inner1);

            if (inner2 != null)
            {
                inner2.BrotherNext = inner3;
                inner2.BrotherPrevious = inner1;
                TierDic.Add(inner2.ID, inner2);
            }

            if (inner3 != null)
            {
                inner3.BrotherNext = inner4;
                inner3.BrotherPrevious = inner2;
                TierDic.Add(inner3.ID, inner3);
            }

            if (inner4 != null)
            {
                inner4.BrotherPrevious = inner3;
                TierDic.Add(inner4.ID, inner4);
            }
            

            if (group1.Flag == 0x0001)
            {
                GroupTierRecursive(Groups, ref inner1, StartID + 1, tier + 1, ref TierDic);
            }

            if (group2 != null && group2.Flag == 0x0001)
            {
                GroupTierRecursive(Groups, ref inner2, group1.Extra.BrotherID + 1, tier + 1, ref TierDic);
            }

            if (group3 != null && group3.Flag == 0x0001)
            {
                GroupTierRecursive(Groups, ref inner3, group2.Extra.BrotherID + 1, tier + 1, ref TierDic);
            }

            if (group4 != null && group4.Flag == 0x0001)
            {
                GroupTierRecursive(Groups, ref inner4, group3.Extra.BrotherID + 1, tier + 1, ref TierDic);
            }
            
        }


    }


}

