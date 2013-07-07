using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prophet.Frontend;

namespace Prophet.Core
{
    class TraceDriver : ITraceAnalyzer
    {
        private IList<Context> _contexts;
        public TraceDriver(IList<Context> contexts)
        {
            _contexts = contexts;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void OnComplete()
        {
            throw new NotImplementedException();
        }

        public void OnExecuteTrace(ExecuteTraceEvent e)
        {
            throw new NotImplementedException();
        }

        public void OnProcBegin(ExecuteTraceEvent e)
        {
            throw new NotImplementedException();
        }

        public void OnProcEnd(ExecuteTraceEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
