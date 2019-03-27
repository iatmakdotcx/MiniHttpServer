using HttpServerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniHttpServer
{
    public class ConsoleLogger : ILogger
    {
        public void Log(object message)
        {
            Console.WriteLine(message);
        }
    }
}
