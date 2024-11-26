using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_SAT_EAT_EXTRACT
{
    public enum SwitchStatus : byte
    {
        Null,
        FalsePs2, // ps2/2007 não muda a ordem // Status0, Status1, Status2, Status3
        TrueUHD, // UHD/PS4/NS/RE4VR           // Status2, Status3, Status0, Status1
        BigEndian // GC/WII/X360               // Status3, Status2, Status1, Status0
    }
}
