using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prophet.Frontend
{
    public class MemAccess
    {
        public UInt32 Addr { get; set; }
        public UInt32 Len { get; set; }
        public UInt32 Val { get; set; }

        public static MemAccess Parse(SQLiteDataReader reader, string prefix)
        {
            var r = new MemAccess();
            r.Addr = (UInt32) Convert.ToInt32(reader[prefix + "addr"]);
            r.Len = (UInt32) Convert.ToInt32(reader[prefix + "len"]);
            r.Val = (UInt32) Convert.ToInt32(reader[prefix + "val"]);
            return r;
        }
    }

    public class Context
    {
        public long Id { get; set; }
        public UInt32 Eax { get; set; }
        public UInt32 Ecx { get; set; }
        public UInt32 Edx { get; set; }
        public UInt32 Ebx { get; set; }
        public UInt32 Esp { get; set; }
        public UInt32 Ebp { get; set; }
        public UInt32 Esi { get; set; }
        public UInt32 Edi { get; set; }
        public UInt32 Eip { get; set; }
        public UInt32 Eflags { get; set; }
        public MemAccess Mr { get; set; }
        public MemAccess Mw { get; set; }
        public bool JumpTaken { get; set; }
        public UInt32 ExecFlag { get; set; }
        public int Tid { get; set; }
        public int ExtTid { get; set; }

        public static Context Parse(SQLiteDataReader reader)
        {
            var r = new Context();
            r.Id = Convert.ToInt64(reader["id"]);
            r.Eax = (UInt32) Convert.ToInt32(reader["eax"]);
            r.Ecx = (UInt32) Convert.ToInt32(reader["ecx"]);
            r.Edx = (UInt32) Convert.ToInt32(reader["edx"]);
            r.Ebx = (UInt32) Convert.ToInt32(reader["ebx"]);
            r.Esp = (UInt32) Convert.ToInt32(reader["esp"]);
            r.Ebp = (UInt32) Convert.ToInt32(reader["ebp"]);
            r.Esi = (UInt32) Convert.ToInt32(reader["esi"]);
            r.Edi = (UInt32) Convert.ToInt32(reader["edi"]);
            r.Eip = (UInt32) Convert.ToInt32(reader["eip"]);
            r.Eflags = (UInt32) Convert.ToInt32(reader["eflags"]);
            r.Mr = MemAccess.Parse(reader, "mr_");
            r.Mw = MemAccess.Parse(reader, "mw_");
            r.JumpTaken = Convert.ToBoolean(reader["jump_taken"]);
            r.ExecFlag = (UInt32) Convert.ToInt32(reader["exec_flag"]);
            r.Tid = Convert.ToInt32(reader["tid"]);
            r.ExtTid = Convert.ToInt32(reader["ext_tid"]);
            return r;
        }
    }
}
