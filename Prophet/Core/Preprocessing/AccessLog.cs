#define FIX_ADJACENT_REVERSE_ORDER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prophet.Common;
using Prophet.Frontend;

namespace Prophet.Core.Preprocessing
{
    public class MessageAccess
    {
        public MessageAccess(int offset, Context ctx, ProcStack ps)
        {
            Offset = offset;
            Context = ctx;
            CallStack = ps.Clone() as ProcStack;
        }

        public int Offset { get; private set; }
        public Context Context { get; private set; }
        public ProcStack CallStack { get; private set; }

        public string ToString(Message msg)
        {
            return string.Format("{0} {1:x2} {3:x8} {4} Hash:{2:x8}", Offset, msg.Data[Offset], 
                CallStack.GetHashCode(), Context.Eip, Context.Inst.Text);
        }
    }

    public class AccessLog : ITraceAnalyzer
    {
        private Message _message;
        private CallStack _callStack;
        private List<MessageAccess> _accesses = new List<MessageAccess>();

        public AccessLog(Message msg, CallStack cs)
        {
            _message = msg;
            _callStack = cs;
        }

        public void Reset()
        {
            _accesses.Clear();
        }

        public void OnComplete()
        {
#if FIX_ADJACENT_REVERSE_ORDER
            
            var cmp = new StackHashComparer();
            int count = 0;
            for (var i = 0; i < _accesses.Count - 1; i++)
            {
                if (_accesses[i].Offset == _accesses[i + 1].Offset + 1 &&
                    cmp.Compare(_accesses[i].CallStack, _accesses[i + 1].CallStack) == 0)
                {
                    var tmp = _accesses[i];
                    _accesses[i] = _accesses[i + 1];
                    _accesses[i + 1] = tmp;
                    count++;
                }
            }
            Logger.Debug("Fixed {0} adjacent reverse order", count);
#endif
        }

        public void OnExecuteTrace(ExecuteTraceEvent e)
        {
            if (e.Context.Mr.Len == 0) return;
            Debug.Assert(e.Context.Mr.Len <= 4);
            for (UInt32 i = 0; i < e.Context.Mr.Len; i++)
            {
                OnMemRead(e.Context, e.Context.Mr.Addr + i, (byte) (e.Context.Mr.Val >> (int)(8 * i)));
            }
        }

        private void OnMemRead(Context ctx, UInt32 addr, byte data)
        {
            if (addr < _message.Base || addr >= _message.End)
                return;
            var offset = addr - _message.Base;
            if (data != _message.Data[offset]) return;
            var acc = new MessageAccess((int) offset, ctx, _callStack.Get());
            _accesses.Add(acc);
        }

        public void OnProcBegin(ExecuteTraceEvent e)
        {
        }

        public void OnProcEnd(ExecuteTraceEvent e)
        {
        }

        public void Dump(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                var q = from p in _accesses select p.ToString(_message);
                foreach (var l in q)
                {
                    writer.WriteLine(l);
                }
            }
        }
    }
}
