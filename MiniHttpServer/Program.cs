using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniHttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ExampleServer server = new ExampleServer("0.0.0.0", 4050);
           // server.SetSSL(SelfSignedCertificateMgr.GetCertificateForHost("127.0.0.1"));
            server.SetRoot(@"D:\Hexo\public");
            server.Logger = new ConsoleLogger();
            server.Start();
        }
    }
}
