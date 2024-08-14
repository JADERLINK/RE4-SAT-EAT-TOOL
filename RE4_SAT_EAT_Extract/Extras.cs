using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_SAT_EAT_EXTRACT
{
    public struct Extras
    {
        public int GroupBytesLenght;
        public int BrotherID;
    }

    public struct Status4Key 
    {
        public byte s0;
        public byte s1;
        public byte s2;
        public byte s3;
    }

    public struct Status3Key
    {
        public byte s0;
        public byte s1;
        public byte s2;
    }

    public class GroupTier 
    {
        public int ID = -1;

        public int Tier = -1;

        public GroupTier Father = null;

        public GroupTier BrotherPrevious = null;
        public GroupTier BrotherNext = null;

        public GroupTier child1 = null;
        public GroupTier child2 = null;
        public GroupTier child3 = null;
        public GroupTier child4 = null;

        public GroupTier(int ID, int Tier)
        {
            this.ID = ID;
            this.Tier = Tier;
        }

        public override string ToString()
        {
            return ID.ToString();
        }
    }


}
