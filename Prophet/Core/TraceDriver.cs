using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prophet.Common;
using Prophet.Frontend;

namespace Prophet.Core
{
    class TraceDriver : ITraceAnalyzer
    {
        private IList<Context> _contexts;
        private readonly List<ITraceAnalyzer> _analyzers = new List<ITraceAnalyzer>();
        private Context _prev;


        public TraceDriver(IList<Context> contexts)
        {
            _contexts = contexts;
        }

        public TraceDriver Add(ITraceAnalyzer analyzer)
        {
            _analyzers.Add(analyzer);
            return this;
        }

        public void RunMessage(Message msg)
        {
            for (int i = msg.BeginIndex; i <= msg.EndIndex; i++)
            {
                var e = new ExecuteTraceEvent(i, msg, msg.Contexts[i]);
                OnExecuteTrace(e);
            }
            OnComplete();
        }

        public void Reset()
        {
            _analyzers.Clear();
            _prev = null;
        }

        public void OnComplete()
        {
            foreach (var a in _analyzers)
                a.OnComplete();
            Reset();
        }

        public void OnExecuteTrace(ExecuteTraceEvent e)
        {
            if (e.Seq == 0 || (_prev != null && _prev.Inst.IsCall &&
                               !_prev.HasExecFlag(ExecFlag.WinapiCall) &&
                               !_prev.HasExecFlag(ExecFlag.WinapiJump)) &&
                !IsSpecialCallPop(e))
            {
                foreach (var a in _analyzers)
                {
                    a.OnProcBegin(e);
                }
            }
            foreach (var a in _analyzers)
            {
                a.OnExecuteTrace(e);
            }
            if (e.Context.Inst.IsRet)
            {
                foreach (var a in _analyzers)
                {
                    a.OnProcEnd(e);
                }
            }
            _prev = e.Context;
        }

        public void OnProcBegin(ExecuteTraceEvent e)
        {
            throw new NotImplementedException();
        }

        public void OnProcEnd(ExecuteTraceEvent e)
        {
            throw new NotImplementedException();
        }

        private bool IsSpecialCallPop(ExecuteTraceEvent e)
        {
            if (_prev == null) return false;
            if (!_prev.Inst.IsCall) return false;
            if (!e.Context.Inst.IsPop) return false;
            return true;
        }
    }
}
