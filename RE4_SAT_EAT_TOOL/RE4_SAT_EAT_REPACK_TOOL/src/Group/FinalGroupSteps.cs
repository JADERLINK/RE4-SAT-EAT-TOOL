﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_SAT_EAT_REPACK.Group
{
    public static class FinalGroupSteps
    {
        public static FinalGroupStructure GetFinalGroupStructure(GroupStructure groupStructure, IsVersion isVersion) 
        {
            FinalGroupStructure final = new FinalGroupStructure();

            Step1(ref groupStructure, ref final, isVersion);
            Step2(ref final);

            return final;
        }

        private static void Step1(ref GroupStructure groupStructure, ref FinalGroupStructure final, IsVersion isVersion) 
        {
            Recursive1(ref final, groupStructure.GroupTier0, isVersion);
        }

        // cria os grupos recursivamente e coloca em ordem
        private static void Recursive1(ref FinalGroupStructure final, GroupTier groupTier, IsVersion isVersion) 
        {
            FinalGroup group = new FinalGroup(groupTier.ID, groupTier.Tier);
            group.Pos = groupTier.Pos;
            group.Dim = groupTier.Dim;
            group.Flag = groupTier.Flag;

            if (groupTier.Flag == GroupFlag.endGroup)
            {
                foreach (var item in groupTier.FloorTriangles)
                {
                    group.FloorTriangles.Add((ushort)item);
                }

                foreach (var item in groupTier.SlopeTriangles)
                {
                    group.SlopeTriangles.Add((ushort)item);
                }

                foreach (var item in groupTier.WallTriangles)
                {
                    group.WallTriangles.Add((ushort)item);
                }
            }

            if (groupTier.BrotherNext != null)
            {
                group.BrotherNextStartID = groupTier.BrotherNext.ID;
            }

            int groupBaseLength = isVersion == IsVersion.IsPS4NS ? 40 : 36; //parte de tamanho fixo, no ps4/ns o campo BrotherNextPos é 64 bits e vez de 32bits

            int Bytespadding = ((group.FloorTriangles.Count + group.SlopeTriangles.Count + group.WallTriangles.Count) % 2) * 2;
            if (isVersion == IsVersion.IsRE4VR)
            {
                Bytespadding = 0; // essa versão não tem padding
            }
            group.GroupBytesLenght = (uint)(groupBaseLength + (group.FloorTriangles.Count * 2) + (group.SlopeTriangles.Count * 2) + (group.WallTriangles.Count * 2) + Bytespadding);

            final.FinalGroupList.Add(group);

            if (groupTier.Child1 != null)
            {
                Recursive1(ref final, groupTier.Child1, isVersion);
            }

            if (groupTier.Child2 != null)
            {
                Recursive1(ref final, groupTier.Child2, isVersion);
            }

            if (groupTier.Child3 != null)
            {
                Recursive1(ref final, groupTier.Child3, isVersion);
            }

            if (groupTier.Child4 != null)
            {
                Recursive1(ref final, groupTier.Child4, isVersion);
            }
        }

        // calcula a posição de onde esta BrotherNext, campo "m_pList"
        private static void Step2(ref FinalGroupStructure final) 
        {
            for (int i = 0; i < final.FinalGroupList.Count; i++)
            {
                final.FinalGroupList[i].FinalID = i;

                if (final.FinalGroupList[i].BrotherNextStartID > 0)
                {
                    final.FinalGroupList[i].BrotherNextPos = final.FinalGroupList[i].GroupBytesLenght;

                    for (int j = i + 1; j < final.FinalGroupList.Count; j++)
                    {
                        if (final.FinalGroupList[i].BrotherNextStartID == final.FinalGroupList[j].StartID)
                        {
                            final.FinalGroupList[i].BrotherNextFinalID = j;
                            break;
                        }
                        else 
                        {
                            final.FinalGroupList[i].BrotherNextPos += final.FinalGroupList[j].GroupBytesLenght;
                        }
                    }

                }

            }
        

        }


    }
}
