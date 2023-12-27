using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_SAT_EAT_Repack.Vector;

namespace RE4_SAT_EAT_Repack.Group
{
    public class GroupStructure
    {
        public Vector3 Min;
        public Vector3 Max;

        public GroupTier GroupTier0;

        // all groups
        public Dictionary<int, GroupTier> GroupDic;

        // groups por tier
        public Dictionary<int, List<GroupTier>> GroupByTier;


        public GroupStructure() 
        {
            GroupDic = new Dictionary<int, GroupTier>();
            GroupByTier = new Dictionary<int, List<GroupTier>>();
        }
    }

    public class GroupTier
    {
        public Vector3 Pos = null;
        public Vector3 Dim = null;

        public Box2D Box = null;

        public GroupFlag Flag = GroupFlag.none;

        public List<int> FloorTriangles = new List<int>();
        public List<int> SlopeTriangles = new List<int>();
        public List<int> WallTriangles = new List<int>();

        //-------
        public int ID = -1;

        public int Tier = -1;

        public GroupTier Father = null;

        public GroupTier BrotherPrevious = null;
        public GroupTier BrotherNext = null;

        public GroupTier Child1 = null;
        public GroupTier Child2 = null;
        public GroupTier Child3 = null;
        public GroupTier Child4 = null;

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

    public enum GroupFlag : ushort
    {
        none = 0, // não usado
        haveChildren = 1,
        endGroup = 2
    }

    public class Box2D
    {
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        public Box2D(Vector2 Min, Vector2 Max)
        {
            this.Min = Min;
            this.Max = Max;
        }
        public Box2D(Vector3 Pos, Vector3 Dim) 
        {
            Min = new Vector2(Pos.X, Pos.Z);
            Max = new Vector2(Pos.X + Dim.X, Pos.Z + Dim.Z);
        }

        //---
        //Plane
        public Vector2 P1 { get { return new Vector2(Min.X, Min.Z); } }
        public Vector2 P2 { get { return new Vector2(Min.X, Max.Z); } }
        public Vector2 P3 { get { return new Vector2(Max.X, Min.Z); } }
        public Vector2 P4 { get { return new Vector2(Max.X, Max.Z); } }
    }

    public enum Limits
    {
        MinX,
        MaxX,
        MinY,
        MaxY,
        MinZ,
        MaxZ
    }


}
