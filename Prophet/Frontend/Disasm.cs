using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prophet.Common;

namespace Prophet.Frontend
{
    public class Disasm
    {
        private readonly Dictionary<UInt32, Instruction> _instrs = new Dictionary<uint, Instruction>();
 
        public void Add(Instruction inst)
        {
            if (_instrs.ContainsKey(inst.Eip))
            {
                Logger.Warning("Instruction {0:X8} is already in Disasm", inst.Eip);
            }
            _instrs.Add(inst.Eip, inst);
        }

        public Instruction this[UInt32 offset]
        {
            get
            {
                if (!_instrs.ContainsKey(offset))
                    throw new ArgumentException(string.Format("No disasm for instruction {0:X8}", offset));
                return _instrs[offset];
            }
        }
    }
}
