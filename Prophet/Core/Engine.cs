using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prophet.Core.Preprocessing;
using Prophet.Frontend;

namespace Prophet.Core
{
    public class Engine
    {
        private FrontendCore _frontend;

        public Engine(FrontendCore frontend)
        {
            _frontend = frontend;
        }

        public void Go()
        {
            foreach (var message in _frontend.Messages)
            {
                OnMessage(message.Value);
            }
        }

        private void OnMessage(Message msg)
        {
            var driver = new TraceDriver();
            var procScope = new ProcScope();
            driver.Add(procScope);

            driver.RunMessage(msg);

            using (var f = new FileStream(@"d:\procscope.txt", FileMode.OpenOrCreate))
            {
                procScope.Dump(f, _frontend.Disasm);
            }
            

            var callStack = new CallStack(procScope);
            var accessLog = new AccessLog(msg, callStack);
            driver.Add(callStack).Add(accessLog);
            driver.RunMessage(msg);


            using (var f = new FileStream(@"d:\accesslog.txt", FileMode.OpenOrCreate))
            {
                accessLog.Dump(f);
            }
        }
    }
}
