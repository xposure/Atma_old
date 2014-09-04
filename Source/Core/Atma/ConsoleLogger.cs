using System;

namespace Atma
{
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger()
            : base()
        {

        }

        protected override void log(string type, string module, string message)
        {
            var dt = DateTime.UtcNow;
            Console.WriteLine("({0}) {1, 25} : {2} -> {3}", dt.ToString("MM/dd HH:MM:ss"), type, module, message);
        }
    }
}
