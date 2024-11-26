using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_SAT_EAT_REPACK.Structures;
using RE4_SAT_EAT_REPACK.Group;

namespace RE4_SAT_EAT_REPACK
{
    public static class DebugR
    {
        public static void EsatDebugOBJ(FileInfo info, FinalStructure esat)
        {
            var text = info.CreateText();
            text.WriteLine(Program.HeaderText());
            text.WriteLine("");

            text.WriteLine("#Info:");
            text.WriteLine("");
            text.WriteLine("");
            text.WriteLine("#Vertex_pos_count:" + esat.Positions.Length);
            text.WriteLine("#Face_normal_count:" + esat.Normals.Length);
            text.WriteLine("#Corner_data_count:" + esat.Edges.Length);
            text.WriteLine("");
            text.WriteLine("#Face_total_count:" + esat.FacesCount);
            text.WriteLine("#FaceFloor_count:" + esat.FaceFloorCount);
            text.WriteLine("#FaceSlope_count:" + esat.FaceSlopeCount);
            text.WriteLine("#FaceWall_count:" + esat.FaceWallCount);
            text.WriteLine("");

            text.WriteLine("");
            for (int i = 0; i < esat.Positions.Length; i++)
            {
                float x = esat.Positions[i].X / 100f;
                float y = esat.Positions[i].Y / 100f;
                float z = esat.Positions[i].Z / 100f;

                text.WriteLine("v " +
                    x.ToFloatString() + " " +
                    y.ToFloatString() + " " +
                    z.ToFloatString());
            }

            for (int i = 0; i < esat.Normals.Length; i++)
            {
                float x = esat.Normals[i].X;
                float y = esat.Normals[i].Y;
                float z = esat.Normals[i].Z;

                text.WriteLine("vn " + 
                    x.ToFloatString() + " " +
                    y.ToFloatString() + " " +
                    z.ToFloatString());
            }

            string faceType = "F";

            for (int i = 0; i < esat.FinalTriangles.Length; i++)
            {
                if (i == esat.FaceFloorCount)
                {
                    faceType = "S";
                }

                if (i == esat.FaceSlopeCount + esat.FaceFloorCount)
                {
                    faceType = "W";
                }

                var face = esat.FinalTriangles[i];

                Status4Key key = new Status4Key();
                key.s0 = face.Key.s0;
                key.s1 = face.Key.s1;
                key.s2 = face.Key.s2;
                key.s3 = ((byte)face.SharedFace);

                byte[] status = new byte[4];

                status[0] = key.s0;
                status[1] = key.s1;
                status[2] = key.s2;
                status[3] = key.s3;


                text.WriteLine("g Debug-ID_" + i.ToString("D4") + "-Face" + faceType + "#" + BitConverter.ToString(status) + "#");

                text.WriteLine("f " +
                    (face.IndexPositionA + 1) + "//" +
                    (face.IndexNormal + 1) + " " +
                    (face.IndexPositionB + 1) + "//" +
                    (face.IndexNormal + 1) + " " +
                    (face.IndexPositionC + 1) + "//" +
                    (face.IndexNormal + 1));
            }

            text.Close();
        }

        //----------------

        public static void EsatGroupPlaneOBJ(FileInfo info, GroupStructure esat)
        {
            var text = info.CreateText();
            text.WriteLine(Program.HeaderText());
            text.WriteLine("");

            int index = 1;

            Dictionary<int, float> climbHeightPerTier = new Dictionary<int, float>();

            foreach (var item in esat.GroupByTier)
            {
                foreach (var group in item.Value)
                {

                    float climbHeight = 0;

                    int tier = group.Tier;
                    if (climbHeightPerTier.ContainsKey(tier))
                    {
                        climbHeight = climbHeightPerTier[tier];
                    }
                    else
                    {
                        float tempvalue = 0;
                        for (int j = tier - 1; j > -1; j--)
                        {
                            tempvalue += 500;
                        }
                        climbHeightPerTier.Add(tier, tempvalue);
                        climbHeight = climbHeightPerTier[tier];
                    }

                    climbHeight -= 6000;

                    SetGroupGroupNameP1(ref text, group);
                    text.Write("Tier_" + group.Tier.ToString("D2") + "#");
                    SetGroupGroupNameP2(ref text, group);

                    var bound_pos = group.Pos;
                    var bound_dim = group.Dim;

                    //original -- A
                    text.WriteLine("v " + 
                        (bound_pos.X / 100f).ToFloatString() + " " +
                        ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                        (bound_pos.Z / 100f).ToFloatString());

                    //X -- D
                    text.WriteLine("v " + 
                        ((bound_pos.X + bound_dim.X) / 100f).ToFloatString() + " " +
                        ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                        ((bound_pos.Z) / 100f).ToFloatString());

                    //Z -- B
                    text.WriteLine("v " + 
                        ((bound_pos.X) / 100f).ToFloatString() + " " +
                        ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                        ((bound_pos.Z + bound_dim.Z) / 100f).ToFloatString());

                    //XZ -- C
                    text.WriteLine("v " + 
                        ((bound_pos.X + bound_dim.X) / 100f).ToFloatString() + " " +
                        ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                        ((bound_pos.Z + bound_dim.Z) / 100f).ToFloatString());


                    text.WriteLine("f " + (index + 2) + " " + (index + 3) + " " + (index));
                    text.WriteLine("f " + (index + 3) + " " + (index + 1) + " " + (index));

                    index += 4;
                }
            }


            text.Close();
        }

        private static void SetGroupGroupNameP1(ref StreamWriter text, GroupTier group)
        {
            text.Write("g Group_" + group.ID.ToString("D4") + "#Flag_" + ((ushort)group.Flag).ToString("X4") + "#");

            if (group.BrotherNext != null)
            {
                text.Write("Brother_" + group.BrotherNext.ID.ToString("D4") + "#");
            }
            else
            {
                text.Write("NoBrother#");
            }

        }

        private static void SetGroupGroupNameP2(ref StreamWriter text, GroupTier group)
        {
            if (group.FloorTriangles.Count != 0 || group.SlopeTriangles.Count != 0 || group.WallTriangles.Count != 0)
            {
                if (group.FloorTriangles.Count != 0)
                {
                    text.Write("FaceF: " + string.Join(" ", group.FloorTriangles) + " #");
                }
                if (group.SlopeTriangles.Count != 0)
                {
                    text.Write("FaceS: " + string.Join(" ", group.SlopeTriangles) + " #");
                }
                if (group.WallTriangles.Count != 0)
                {
                    text.Write("FaceW: " + string.Join(" ", group.WallTriangles) + " #");
                }

            }
            text.WriteLine();
        }

        //----------------

        public static void EsatGroupPlaneOBJ(FileInfo info, FinalGroupStructure esat, ushort FacesCount)
        {
            var text = info.CreateText();
            text.WriteLine(Program.HeaderText());
            text.WriteLine("");

            int index = 1;

            Dictionary<int, float> climbHeightPerTier = new Dictionary<int, float>();

            foreach (var group in esat.FinalGroupList)
            {
                float climbHeight = 0;

                int tier = group.Tier;
                if (climbHeightPerTier.ContainsKey(tier))
                {
                    climbHeight = climbHeightPerTier[tier];
                }
                else
                {
                    float tempvalue = 0;
                    for (int j = tier - 1; j > -1; j--)
                    {
                        tempvalue += 500;
                    }
                    climbHeightPerTier.Add(tier, tempvalue);
                    climbHeight = climbHeightPerTier[tier];
                }

                climbHeight -= 6000;

                SetGroupGroupNameP1(ref text, group);
                text.Write("Tier_" + group.Tier.ToString("D2") + "#");
                SetGroupGroupNameP2(ref text, group);

                var bound_pos = group.Pos;
                var bound_dim = group.Dim;

                //original -- A
                text.WriteLine("v " +
                    (bound_pos.X / 100f).ToFloatString() + " " +
                    ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                    (bound_pos.Z / 100f).ToFloatString());

                //X -- D
                text.WriteLine("v " +
                    ((bound_pos.X + bound_dim.X) / 100f).ToFloatString() + " " +
                    ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                    ((bound_pos.Z) / 100f).ToFloatString());

                //Z -- B
                text.WriteLine("v " +
                    ((bound_pos.X) / 100f).ToFloatString() + " " +
                    ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                    ((bound_pos.Z + bound_dim.Z) / 100f).ToFloatString());

                //XZ -- C
                text.WriteLine("v " +
                    ((bound_pos.X + bound_dim.X) / 100f).ToFloatString() + " " +
                    ((bound_pos.Y + climbHeight) / 100f).ToFloatString() + " " +
                    ((bound_pos.Z + bound_dim.Z) / 100f).ToFloatString());


                text.WriteLine("f " + (index + 2) + " " + (index + 3) + " " + (index));
                text.WriteLine("f " + (index + 3) + " " + (index + 1) + " " + (index));

                index += 4;

            }

            text.WriteLine("### Triangle Faces Count: " + FacesCount);
            HashSet<int> contains = new HashSet<int>();
            foreach (var group in esat.FinalGroupList)
            {
                foreach (var tri in group.FloorTriangles)
                {
                    contains.Add(tri);
                }

                foreach (var tri in group.SlopeTriangles)
                {
                    contains.Add(tri);
                }

                foreach (var tri in group.WallTriangles)
                {
                    contains.Add(tri);
                }
            }

            var containsOrder = contains.OrderBy(x => x).ToArray();
            text.WriteLine("# Triangles contained in the groups: " + string.Join(" ", containsOrder) + " #");

            List<int> notContains = new List<int>();
            for (int i = 0; i < FacesCount; i++)
            {
                if (!containsOrder.Contains(i))
                {
                    notContains.Add(i);
                }
            }
            text.WriteLine("# Triangles NOT contained in groups: " + string.Join(" ", notContains) + " #");

            text.Close();
        }

        private static void SetGroupGroupNameP1(ref StreamWriter text, FinalGroup group)
        {
            text.Write("g Group_" + group.FinalID.ToString("D4") + "#Flag_" + ((ushort)group.Flag).ToString("X4") + "#");

            if (group.BrotherNextFinalID > 0)
            {
                text.Write("Brother_" + group.BrotherNextFinalID.ToString("D4") + "#");
            }
            else
            {
                text.Write("NoBrother#");
            }

        }

        private static void SetGroupGroupNameP2(ref StreamWriter text, FinalGroup group)
        {
            if (group.FloorTriangles.Count != 0 || group.SlopeTriangles.Count != 0 || group.WallTriangles.Count != 0)
            {
                if (group.FloorTriangles.Count != 0)
                {
                    text.Write("FaceF: " + string.Join(" ", group.FloorTriangles) + " #");
                }
                if (group.SlopeTriangles.Count != 0)
                {
                    text.Write("FaceS: " + string.Join(" ", group.SlopeTriangles) + " #");
                }
                if (group.WallTriangles.Count != 0)
                {
                    text.Write("FaceW: " + string.Join(" ", group.WallTriangles) + " #");
                }

            }
            text.WriteLine();
        }



    }
}
