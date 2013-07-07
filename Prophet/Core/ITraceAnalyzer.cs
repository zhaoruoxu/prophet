using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prophet.Core
{
    class ExecuteTraceEvent
    {
        
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
