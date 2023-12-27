using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_Repack.Vector;

namespace RE4_SAT_EAT_Repack.Group
{
    public class FinalGroupStructure
    {
        public List<FinalGroup> FinalGroupList;

        public FinalGroupStructure() 
        {
            FinalGroupList = new List<FinalGroup>();
        }
    }

    public class FinalGroup 
    {
        public uint BrotherNextPos = 0;
        public uint GroupBytesLenght = 0;

        //----
        public Vector3 Pos = null;
        public Vector3 Dim = null;
        public GroupFlag Flag = GroupFlag.none;
        public List<ushort> FloorTriangles = new List<ushort>();
        public List<ushort> SlopeTriangles = new List<ushort>();
        public List<ushort> WallTriangles = new List<ushort>();

        //-------
        public int StartID = -1;
        public int BrotherNextStartID = -1;

        //---
        public int FinalID = -1;
        public int BrotherNextFinalID = -1;

        // tier apenas informacional
        public int Tier = -1;

        public FinalGroup(int StartID, int Tier)
        {
            this.StartID = StartID;
            this.Tier = Tier;
        }

        public override string ToString()
        {
            return StartID.ToString();
        }


    }
}
