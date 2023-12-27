using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_Repack.Structures;
using RE4_SAT_EAT_Repack.Vector;

namespace RE4_SAT_EAT_Repack.Group
{
    public static class StartGroup
    {
        public static GroupStructure FirstStep(FinalStructure final)
        {
            Dictionary<Limits, float> limits = new Dictionary<Limits, float>();
            GroupStructure groupStructure = new GroupStructure();

            if (final.Positions.Length > 0)
            {
                limits.Add(Limits.MaxX, final.Positions[0].X);
                limits.Add(Limits.MinX, final.Positions[0].X);

                limits.Add(Limits.MaxY, final.Positions[0].Y);
                limits.Add(Limits.MinY, final.Positions[0].Y);

                limits.Add(Limits.MaxZ, final.Positions[0].Z);
                limits.Add(Limits.MinZ, final.Positions[0].Z);
            }
            else
            {
                limits.Add(Limits.MaxX, 0);
                limits.Add(Limits.MinX, 0);

                limits.Add(Limits.MaxY, 0);
                limits.Add(Limits.MinY, 0);

                limits.Add(Limits.MaxZ, 0);
                limits.Add(Limits.MinZ, 0);
            }

            foreach (var item in final.Positions)
            {
                if (item.X < limits[Limits.MinX])
                {
                    limits[Limits.MinX] = item.X;
                }

                if (item.X > limits[Limits.MaxX])
                {
                    limits[Limits.MaxX] = item.X;
                }

                if (item.Y < limits[Limits.MinY])
                {
                    limits[Limits.MinY] = item.Y;
                }

                if (item.Y > limits[Limits.MaxY])
                {
                    limits[Limits.MaxY] = item.Y;
                }

                if (item.Z < limits[Limits.MinZ])
                {
                    limits[Limits.MinZ] = item.Z;
                }

                if (item.Z > limits[Limits.MaxZ])
                {
                    limits[Limits.MaxZ] = item.Z;
                }
            }

            float dimX = Math.Abs(limits[Limits.MinX] - limits[Limits.MaxX]);
            float dimY = Math.Abs(limits[Limits.MinY] - limits[Limits.MaxY]);
            float dimZ = Math.Abs(limits[Limits.MinZ] - limits[Limits.MaxZ]);

            Vector3 pos1 = new Vector3(limits[Limits.MinX], limits[Limits.MinY], limits[Limits.MinZ]);
            Vector3 pos2 = new Vector3(limits[Limits.MaxX], limits[Limits.MaxY], limits[Limits.MaxZ]);
            Vector3 dim1 = new Vector3(dimX, dimY, dimZ);

            Vector2 pmin = new Vector2(limits[Limits.MinX], limits[Limits.MinZ]);
            Vector2 pmax = new Vector2(limits[Limits.MaxX], limits[Limits.MaxZ]);

            groupStructure.Min = pos1;
            groupStructure.Max = pos2;

            GroupTier groupTier0 = new GroupTier(0,0);
            groupTier0.Pos = pos1;
            groupTier0.Dim = dim1;
            groupTier0.Box = new Box2D(pmin, pmax);
            groupTier0.Flag = GroupFlag.none;

            groupStructure.GroupTier0 = groupTier0;
            groupStructure.GroupDic.Add(0, groupTier0);
            groupStructure.GroupByTier.Add(0, new List<GroupTier>() { groupTier0 });

            SecondStep(ref groupStructure);

            ThirdStep(ref groupStructure, ref final);

            FourthStep(ref groupStructure);

            return groupStructure;
        }

        // segunda etapa: cria os grupos;
        private static void SecondStep(ref GroupStructure groupStructure) 
        {
            int index = 0;
            RecursiveSecondStep(ref groupStructure.GroupTier0, 1, ref groupStructure, ref index);
        }

        private static void RecursiveSecondStep(ref GroupTier groupTier, int tier, ref GroupStructure groupStructure, ref int index) 
        {
            Vector2 half = new Vector2(groupTier.Dim.X / 2, groupTier.Dim.Z / 2);
            Vector3 newDim = new Vector3(half.X, groupTier.Dim.Y, half.Z);

            Vector3 point1 = new Vector3(groupTier.Pos.X, groupTier.Pos.Y, groupTier.Pos.Z);
            Vector3 point2 = new Vector3(groupTier.Pos.X + half.X, groupTier.Pos.Y, groupTier.Pos.Z);
            Vector3 point3 = new Vector3(groupTier.Pos.X, groupTier.Pos.Y, groupTier.Pos.Z + half.Z);
            Vector3 point4 = new Vector3(groupTier.Pos.X + half.X, groupTier.Pos.Y, groupTier.Pos.Z + half.Z);

            GroupTier child1 = MakeChild(ref index, tier, point1, newDim);
            GroupTier child2 = MakeChild(ref index, tier, point2, newDim);
            GroupTier child3 = MakeChild(ref index, tier, point3, newDim);
            GroupTier child4 = MakeChild(ref index, tier, point4, newDim);

            child1.Father = groupTier;
            child2.Father = groupTier;
            child3.Father = groupTier;
            child4.Father = groupTier;

            groupTier.Child1 = child1;
            groupTier.Child2 = child2;
            groupTier.Child3 = child3;
            groupTier.Child4 = child4;

            child1.BrotherNext = child2;
            child2.BrotherNext = child3;
            child3.BrotherNext = child4;

            child4.BrotherPrevious = child3;
            child3.BrotherPrevious = child2;
            child2.BrotherPrevious = child1;

            //-------
            groupStructure.GroupDic.Add(child1.ID , child1);
            groupStructure.GroupDic.Add(child2.ID , child2);
            groupStructure.GroupDic.Add(child3.ID , child3);
            groupStructure.GroupDic.Add(child4.ID , child4);

            if (!groupStructure.GroupByTier.ContainsKey(tier))
            {
                groupStructure.GroupByTier.Add(tier, new List<GroupTier>());
            }

            groupStructure.GroupByTier[tier].Add(child1);
            groupStructure.GroupByTier[tier].Add(child2);
            groupStructure.GroupByTier[tier].Add(child3);
            groupStructure.GroupByTier[tier].Add(child4);

            Vector2 nexthalf = new Vector2(half.X / 2, half.Z / 2);
            if (nexthalf.X > 2000 || nexthalf.Z > 2000)
            {
                RecursiveSecondStep(ref child1, tier + 1, ref groupStructure, ref index);
                RecursiveSecondStep(ref child2, tier + 1, ref groupStructure, ref index);
                RecursiveSecondStep(ref child3, tier + 1, ref groupStructure, ref index);
                RecursiveSecondStep(ref child4, tier + 1, ref groupStructure, ref index);
            }
        }

        private static GroupTier MakeChild(ref int index, int tier, Vector3 pos, Vector3 dim) 
        {
            GroupTier child1 = new GroupTier(++index, tier);
            child1.Pos = pos;
            child1.Dim = dim;
            child1.Box = new Box2D(pos, dim);
            child1.Flag = GroupFlag.none;
            return child1;
        }

        // terceira etapa: popula os grupos com os triangulos;
        private static void ThirdStep(ref GroupStructure groupStructure, ref FinalStructure final)
        {
            int lastTier = 0;
            var TierList = groupStructure.GroupByTier.Keys.ToArray();
            foreach (var item in TierList)
            {
                if (item > lastTier)
                {
                    lastTier = item;
                }
            }
            //-------------

            for (int i = 0; i < final.FinalTriangles.Length; i++)
            {
                FinalTriangle tri = final.FinalTriangles[i];

                // usado para saber quais são os limites do triangulo;
                float minX = tri.BackupPositionA.X;
                float maxX = tri.BackupPositionA.X;

                float minZ = tri.BackupPositionA.Z;
                float maxZ = tri.BackupPositionA.Z;

                minX = tri.BackupPositionB.X < minX ? tri.BackupPositionB.X : minX;
                minX = tri.BackupPositionC.X < minX ? tri.BackupPositionC.X : minX;

                maxX = tri.BackupPositionB.X > maxX ? tri.BackupPositionB.X : maxX;
                maxX = tri.BackupPositionC.X > maxX ? tri.BackupPositionC.X : maxX;

                minZ = tri.BackupPositionB.Z < minZ ? tri.BackupPositionB.Z : minZ;
                minZ = tri.BackupPositionC.Z < minZ ? tri.BackupPositionC.Z : minZ;

                maxZ = tri.BackupPositionB.Z > maxZ ? tri.BackupPositionB.Z : maxZ;
                maxZ = tri.BackupPositionC.Z > maxZ ? tri.BackupPositionC.Z : maxZ;

                Vector2 pmin = new Vector2(minX, minZ);
                Vector2 pmax = new Vector2(maxX, maxZ);
                Box2D triBox = new Box2D(pmin, pmax);

                RecursiveCheck(i, tri, triBox, ref groupStructure.GroupTier0, lastTier);
            }

        }

        //verifica se o triangulo esta no grupo e adiciona no grupo, o retorno é true se o triangulo estiver contido no grupo
        private static bool RecursiveCheck(int TriID, FinalTriangle tri, Box2D triBox, ref GroupTier group, int LastTier) 
        {
            bool triangleIsContained = false;

            if (IntersectAABBxAABB(triBox, group.Box)
             || IntersectAABBxAABB(group.Box, triBox)
             ) // caso for true, esse triangulo pode estar contido nesse grupo;
            {

                if (group.Tier == LastTier) // se for verdadeiro, quer dizer que esse é o ultimo tier e tenho que verificar se o triagulo realmente esta contido no grupo
                {
                    var tri_pA = (tri.BackupPositionA.X, tri.BackupPositionA.Z);
                    var tri_pB = (tri.BackupPositionB.X, tri.BackupPositionB.Z);
                    var tri_pC = (tri.BackupPositionC.X, tri.BackupPositionC.Z);
                    var boxMin = group.Box.Min.GetXZ();
                    var boxMax = group.Box.Max.GetXZ();

                    if (kahshiu.SeparatingAxisTheorem.Check(
                        (boxMin, boxMax), (tri_pA, tri_pB, tri_pC) // se retornar true é porque o triangulo realmente esta contido no group; 
                        ))
                    {
                        triangleIsContained = true;
                    }

                }
                // se falso, vai para os grupos dos proximos tiers;
                else 
                {
                    bool c1 = RecursiveCheck(TriID, tri, triBox, ref group.Child1, LastTier);
                    bool c2 = RecursiveCheck(TriID, tri, triBox, ref group.Child2, LastTier);
                    bool c3 = RecursiveCheck(TriID, tri, triBox, ref group.Child3, LastTier);
                    bool c4 = RecursiveCheck(TriID, tri, triBox, ref group.Child4, LastTier);

                    triangleIsContained = triangleIsContained || c1 || c2 || c3 || c4;
                }

                if (triangleIsContained) // se tiver contido, adiciono o id no grupo
                {
                    if (tri.Type == FaceType.Floor)
                    {
                        group.FloorTriangles.Add(TriID);
                    }
                    else if (tri.Type == FaceType.Slope)
                    {
                        group.SlopeTriangles.Add(TriID);
                    }
                    else if (tri.Type == FaceType.Wall)
                    {
                        group.WallTriangles.Add(TriID);
                    }

                }
            }
            // se for false, o triangulo não esta contido no grupo;

            return triangleIsContained;
        }

        //https://developer.mozilla.org/en-US/docs/Games/Techniques/3D_collision_detection
        private static bool IntersectAABBxAABB(Box2D a, Box2D b)
        {
            return (
              a.Min.X <= b.Max.X &&
              a.Max.X >= b.Min.X &&
              a.Min.Z <= b.Max.Z &&
              a.Max.Z >= b.Min.Z
            );
        }

        // quarta etapa: remove os grupos desnecessários;
        private static void FourthStep(ref GroupStructure groupStructure) 
        {
            int lastTier = 0;
            var TierList = groupStructure.GroupByTier.Keys.ToArray();
            foreach (var item in TierList)
            {
                if (item > lastTier)
                {
                    lastTier = item;
                }
            }

            // define flag do ultimo tier
            foreach (var item in groupStructure.GroupByTier[lastTier])
            {
                item.Flag = GroupFlag.endGroup;
            }

            for (int i = lastTier-1; i >= 0; i--)
            {
                List<GroupTier> childtier = new List<GroupTier>();

                foreach (var item in groupStructure.GroupByTier[i])
                {
                    int floor = item.FloorTriangles.Count;
                    int slope = item.SlopeTriangles.Count;
                    int wall = item.WallTriangles.Count;
                    int all = floor + slope + wall;

                    if (all < 10) // se nesse grupo tiver menos de X tantos triangulos, ele não precisa de "childs";
                    {
                        item.Flag = GroupFlag.endGroup;

                        item.Child1 = null;
                        item.Child2 = null;
                        item.Child3 = null;
                        item.Child4 = null;
                    }
                    else 
                    {
                        item.Flag = GroupFlag.haveChildren;

                        childtier.Add(item.Child1);
                        childtier.Add(item.Child2);
                        childtier.Add(item.Child3);
                        childtier.Add(item.Child4);
                    }
                }

                groupStructure.GroupByTier[i + 1] = childtier;
            }

            groupStructure.GroupDic.Clear();
            groupStructure.GroupDic.Add(0, groupStructure.GroupTier0);
            for (int i = 1; i <= lastTier; i++)
            {
                foreach (var item in groupStructure.GroupByTier[i])
                {
                    groupStructure.GroupDic.Add(item.ID, item);
                }
            }

        }

    }
}
