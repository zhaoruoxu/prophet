using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prophet.Common
{
    public enum ExecFlag : uint
    {
        WinapiCall = 0x1,
        WinapiJump = 0x2,
        PrefixRep = 0x4,
        PrefixRepne = 0x8,
        Callback = 0x16,
        TerminateEip = 0x32,
    }

    class Constants
    {
    }
}
