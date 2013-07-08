using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prophet.Common;
using Prophet.Core;
using Prophet.Frontend;

namespace Prophet
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Logger.Info("Usage: {0} sqlite3_db_file", Path.GetFileName(
                    Process.GetCurrentProcess().MainModule.FileName));
                return;
            }
            var frontend = new FrontendCore(args[0]);
            var engine = new Engine(frontend);
            engine.Go();
        }
    }
}
