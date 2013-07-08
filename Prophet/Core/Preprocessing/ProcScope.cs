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
    public class Procedure
    {
        private UInt32 _entry;
        private HashSet<UInt32> _addrs = new HashSet<uint>();
        private HashSet<UInt32> _exits = new HashSet<uint>();

        public Procedure(UInt32 eip)
        {
            _entry = eip;
            Extend(_entry);
        }

        public UInt32 Entry { get { return _entry; } }

        public void Extend(UInt32 addr)
        {
            _addrs.Add(addr);
        }

        public void Exit(UInt32 addr)
        {
            Debug.Assert(_addrs.Contains(addr));
            _exits.Add(addr);
        }

        public bool Contains(UInt32 addr)
        {
            return _addrs.Contains(addr);
        }

        public HashSet<UInt32> Addrs
        {
            get { return _addrs; }
        }

        public HashSet<UInt32> Exits
        {
            get { return _exits; }
        }

        public override string ToString()
        {
            return string.Format("Proc_{0:x8}", Entry);
        }
    }
    public class ProcScope : ITraceAnalyzer
    {
        private Dictionary<UInt32, Procedure> _procs = new Dictionary<uint, Procedure>();
        private Stack<Procedure> _callStack = new Stack<Procedure>();
        //private HashSet<Procedure> _entries = new HashSet<Procedure>();

        public bool TryGet(UInt32 addr, out Procedure p)
        {
            return _procs.TryGetValue(addr, out p);
        }

        public void Reset()
        {
            _procs.Clear();
            _callStack.Clear();
            //_entries.Clear();
        }

        public void OnComplete()
        {
            CheckValidity();
        }

        [Conditional("DEBUG")]
        private void CheckValidity()
        {
            var procs = new Dictionary<UInt32, Procedure>();
            foreach (var proc in _procs)
            {
                foreach (var addr in proc.Value.Addrs)
                {
                    if (procs.ContainsKey(addr))
                    {
                        Logger.Error("Procedure overlaps: {0:X8} in both {1} and {2}",
                            addr, proc.Value, procs[addr]);
                    }
                    procs.Add(addr, proc.Value);
                }
            }

            Logger.Debug("ProcScope validity okay: {0} procedures and {1} valid addresses",
                _procs.Count, procs.Count);
        }

        public void OnExecuteTrace(ExecuteTraceEvent e)
        {
            if (_callStack.Count == 0)
            {
                OnProcBegin(e);
                //_entries.Add(_callStack.Peek());
            }
            _callStack.Peek().Extend(e.Context.Eip);
        }

        public void OnProcBegin(ExecuteTraceEvent e)
        {
            var proc = GetOrCreateProc(e.Context);
            _callStack.Push(proc);
        }

        public void OnProcEnd(ExecuteTraceEvent e)
        {
            Debug.Assert(_callStack.Count != 0);
            _callStack.Peek().Exit(e.Context.Eip);
            _callStack.Pop();
        }

        private Procedure GetOrCreateProc(Context ctx)
        {
            Procedure proc;
            if (_procs.TryGetValue(ctx.Eip, out proc))
            {
                return proc;
            }
            proc = new Procedure(ctx.Eip);
            _procs.Add(ctx.Eip, proc);
            return proc;
        }

        public void Dump(Stream stream, Disasm disasm)
        {
            var writer = new StreamWriter(stream);
            foreach (var proc in _procs)
            {
                writer.WriteLine(proc.Value);
                var q = from p in proc.Value.Addrs orderby p select string.Format("{0:x8} {1}", p, disasm[p].Text);
                foreach (var l in q)
                {
                    writer.WriteLine(l);
                }
                writer.WriteLine();
            }
        }
    }
}
