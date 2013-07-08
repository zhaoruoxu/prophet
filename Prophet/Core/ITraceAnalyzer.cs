using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prophet.Frontend;

namespace Prophet.Core
{
    class ExecuteTraceEvent
    {
        public int Seq { get; set; }
        public Message Message { get; set; }
        public Context Context { get; set; }

        public ExecuteTraceEvent(int seq, Message msg, Context ctx)
        {
            Seq = seq;
            Message = msg;
            Context = ctx;
        }
    }

    interface ITraceAnalyzer
    {
        void Reset();
        void OnComplete();
        void OnExecuteTrace(ExecuteTraceEvent e);
        void OnProcBegin(ExecuteTraceEvent e);
        void OnProcEnd(ExecuteTraceEvent e);
    }
}
