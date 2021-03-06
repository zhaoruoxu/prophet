﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prophet.Core.Preprocessing
{
    public class StackHashComparer : IComparer<ProcStack>
    {
        public int Compare(ProcStack x, ProcStack y)
        {
            return x.GetHashCode().CompareTo(y.GetHashCode());
        }
    }

    public class ProcStack : ICloneable
    {
        private LinkedList<Procedure> _stack = new LinkedList<Procedure>();

        public void Clear()
        {
            _stack.Clear();
        }

        public int Count { get { return _stack.Count; } }

        public void AddLast(Procedure p)
        {
            _stack.AddLast(p);
        }

        public void RemoveLast()
        {
            _stack.RemoveLast();
        }

        public override int GetHashCode()
        {
            Debug.Assert(_stack != null);
            return _stack.Aggregate(0, (current, t) => current*13131 + (int) t.Entry);
        }

        public object Clone()
        {
            var r = new ProcStack {_stack = new LinkedList<Procedure>(_stack)};
            //foreach (var v in _stack)
            //    r._stack.AddLast(v);
            return r;
        }
    }

    public class CallStack : ITraceAnalyzer
    {
        private ProcScope _procs;
        private ProcStack _stack = new ProcStack();

        public CallStack(ProcScope ps)
        {
            _procs = ps;
        }

        public ProcStack Get()
        {
            return _stack;
        }

        public void Reset()
        {
            _stack.Clear();
        }

        public void OnComplete()
        {
            Reset();
        }

        public void OnExecuteTrace(ExecuteTraceEvent e)
        {
            if (_stack.Count == 0)
            {
                OnProcBegin(e);
            }
        }

        public void OnProcBegin(ExecuteTraceEvent e)
        {
            Procedure p;
            if (_procs.TryGet(e.Context.Eip, out p))
            {
                _stack.AddLast(p);
            }
        }

        public void OnProcEnd(ExecuteTraceEvent e)
        {
            Debug.Assert(_stack.Count != 0);
            _stack.RemoveLast();
        }
    }
}
