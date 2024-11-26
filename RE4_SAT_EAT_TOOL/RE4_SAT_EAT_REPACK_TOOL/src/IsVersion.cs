using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_SAT_EAT_REPACK
{
    public enum IsVersion : byte
    {
        Null,
        IsPS2,
        IsUHD,
        IsPS4NS,
        IsBigEndian, //GC/WII/X360
        IsRE4VR
    }
}
