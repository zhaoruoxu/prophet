using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prophet.Frontend
{
    public class Argument
    {
        public string Mnemonic { get; set; }
        public int Type { get; set; }
        public int Size { get; set; }
        public int Position { get; set; }
        public int BaseReg { get; set; }
        public int IndexReg { get; set; }
        public UInt32 Scale { get; set; }
        public UInt32 Displacement { get; set; }

        public static Argument Parse(SQLiteDataReader reader, string prefix)
        {
            var arg = new Argument();
            arg.Mnemonic = reader[prefix + "mnemonic"].ToString();
            arg.Type = Convert.ToInt32(reader[prefix + "type"]);
            arg.Size = Convert.ToInt32(reader[prefix + "size"]);
            arg.Position = Convert.ToInt32(reader[prefix + "pos"]);
            arg.BaseReg = Convert.ToInt32(reader[prefix + "basereg"]);
            arg.IndexReg = Convert.ToInt32(reader[prefix + "indexreg"]);
            arg.Scale = (UInt32)Convert.ToInt32(reader[prefix + "scale"]);
            arg.Displacement = (UInt32)Convert.ToInt32(reader[prefix + "disp"]);
            return arg;
        }
    }

    public class Instruction
    {
        public string Text { get; set; }
        public UInt32 Eip { get; set; }
        public int Category { get; set; }
        public UInt32 Opcode { get; set; }
        public string Mnemonic { get; set; }
        public int BranchType { get; set; }
        public UInt32 AddrValue { get; set; }
        public UInt32 Immediate { get; set; }
        public Argument Arg1 { get; set; }
        public Argument Arg2 { get; set; }
        public Argument Arg3 { get; set; }
        public bool PrefixOper { get; set; }
        public bool PrefixAddr { get; set; }
        public bool PrefixRepne { get; set; }
        public bool PrefixRep { get; set; }
        public UInt32 AuxOpcode { get; set; }
        public UInt32 AuxModrm { get; set; }
        public UInt32 AuxSib { get; set; }

        public static Instruction Parse(SQLiteDataReader reader)
        {
            var inst = new Instruction();
            inst.Eip = (UInt32)Convert.ToInt32(reader["eip"]);
            inst.Text = reader["instr"].ToString();
            inst.Category = Convert.ToInt32(reader["category"]);
            inst.Opcode = (UInt32)Convert.ToInt32(reader["opcode"]);
            inst.Mnemonic = reader["mnemonic"].ToString();
            inst.BranchType = Convert.ToInt32(reader["branch_type"]);
            inst.AddrValue = (UInt32)Convert.ToInt32(reader["addr_val"]);
            inst.Immediate = (UInt32)Convert.ToInt32(reader["immediate"]);
            inst.Arg1 = Argument.Parse(reader, "a1_");
            inst.Arg2 = Argument.Parse(reader, "a2_");
            inst.Arg3 = Argument.Parse(reader, "a3_");
            inst.PrefixOper = Convert.ToBoolean(reader["p_oper"]);
            inst.PrefixAddr = Convert.ToBoolean(reader["p_addr"]);
            inst.PrefixRepne = Convert.ToBoolean(reader["p_repne"]);
            inst.PrefixRep = Convert.ToBoolean(reader["p_rep"]);
            inst.AuxOpcode = (UInt32)Convert.ToInt32(reader["aux_opcode"]);
            inst.AuxModrm = (UInt32)Convert.ToInt32(reader["aux_modrm"]);
            inst.AuxSib = (UInt32)Convert.ToInt32(reader["aux_sib"]);
            return inst;
        }
    }
}
