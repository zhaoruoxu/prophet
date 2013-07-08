using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Prophet.Common;

namespace Prophet.Frontend
{
    public class FrontendCore
    {
        private string _dbpath;
        private const string SQLiteConnString = @"Data Source={0};Version=3;";
        private Disasm _disasm;
        private readonly Dictionary<int, Message> _messages = new Dictionary<int, Message>();

        public Disasm Disasm
        {
            get { return _disasm; }
        }

        public Dictionary<int, Message> Messages { get { return _messages; } } 

        public FrontendCore(string dbpath)
        {
            _dbpath = dbpath;
            using (var conn = new SQLiteConnection(string.Format(SQLiteConnString, dbpath)))
            {
                conn.Open();
                InitDisasm(conn);
                LoadMessages(conn);
            }
            Logger.Debug("Disasm instruction count: {0}", _disasm.Count);
        }

        private void InitDisasm(SQLiteConnection conn)
        {
            _disasm = new Disasm();
            using (var cmd = new SQLiteCommand("SELECT * FROM disasm", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inst = Instruction.Parse(reader);
                        _disasm.Add(inst);
                        if (inst.Text.Contains("xmm"))
                            Logger.Debug("{0:X8} : {1}", inst.Eip, inst.Text);
                    }
                }
            }
        }

        private void LoadMessages(SQLiteConnection conn)
        {
            using (var cmd = new SQLiteCommand("SELECT * FROM messages", conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var msg = new Message(
                            Convert.ToInt32(reader["id"]),
                            Convert.ToInt32(reader["trace_count"]),
                            (UInt32) Convert.ToInt32(reader["addr"]),
                            Convert.ToInt32(reader["length"]));
                        LoadMessage(conn, msg);
                    }
                }
            }
        }

        private void LoadMessage(SQLiteConnection conn, Message msg)
        {
            if (_messages.ContainsKey(msg.Id))
            {
                throw new ApplicationException("Duplicated message id: " + msg.Id);
            }
            _messages.Add(msg.Id, msg);

            using (var cmd = new SQLiteCommand("SELECT * FROM messages_data WHERE id = @msg_id", conn))
            {
                cmd.Parameters.Add(new SQLiteParameter("@msg_id", msg.Id));
                var data = new byte[msg.Length];
                using (var reader = cmd.ExecuteReader())
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        int offset = Convert.ToInt32(reader["offset"]);
                        int val = Convert.ToInt32(reader["data"]);
                        if (offset >= msg.Length)
                        {
                            throw new ApplicationException("Message data offset larger than message length");
                        }
                        if (val < 0 || val > byte.MaxValue)
                        {
                            throw new ApplicationException("Invalid message data");
                        }
                        data[offset] = (byte)val;
                        count++;
                    }
                    if (count != msg.Length)
                    {
                        throw new ApplicationException("Message length doesn't equal bytes count");
                    }
                    msg.Data = data;
                }
            }

            using (var cmd = new SQLiteCommand("SELECT * FROM insts WHERE msg_id = @msg_id ORDER BY id", conn))
            {
                cmd.Parameters.Add(new SQLiteParameter("@msg_id", msg.Id));
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ctx = Context.Parse(reader, _disasm);
                        msg.Add(ctx);
                    }
                }
            }

            Logger.Debug("Message {0} has {1} bytes, {2} contexts", msg.Id, msg.Length, msg.Contexts.Count);
        }

    }
}
