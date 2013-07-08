using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prophet.Common
{
    public class Logger
    {
        private enum Level
        {
            Debug = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        private static ConsoleColor[] ForeColors = new ConsoleColor[]
            {
                ConsoleColor.Cyan,
                ConsoleColor.White, 
                ConsoleColor.Yellow, 
                ConsoleColor.Red, 
            };

        private Logger()
        {

        }

        private static Logger _instance;
        public static Logger Instance
        {
            get { return _instance ?? (_instance = new Logger()); }
        }

        private void DoLog(Level level, string fmt, params object[] args)
        {
            var foreColor = Console.ForegroundColor;
            Console.ForegroundColor = ForeColors[(int)level];
            Console.WriteLine(fmt, args);
            Console.ForegroundColor = foreColor;
        }

        private static void Log(Level level, string fmt, params object[] args)
        {
            Instance.DoLog(level, fmt, args);
        }

        [Conditional("DEBUG")]
        public static void Debug(string fmt, params object[] args)
        {
            Log(Level.Debug, fmt, args);
        }

        public static void Info(string fmt, params object[] args)
        {
            Log(Level.Info, fmt, args);
        }

        public static void Warning(string fmt, params object[] args)
        {
            Log(Level.Warning, fmt, args);
        }

        public static void Error(string fmt, params object[] args)
        {
            Log(Level.Error, fmt, args);
            var str = string.Format(fmt, args);
            throw new Exception(str);
        }
    }
}
