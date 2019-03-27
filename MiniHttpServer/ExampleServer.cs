using HttpServerLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MiniHttpServer
{
    public class ExampleServer: HttpServerLib.HttpServer
    {
        public ExampleServer(string ipAddress, int port): base(ipAddress, port)
        {

        }

        public override void OnPost(HttpRequest request, HttpResponse response)
        {
            if (request.Headers.ContainsKey("Content-Type"))
            {
                response.Headers["Content-Type"] = request.Headers["Content-Type"];
            }
            response.Write("ResponseData:");
            response.ResponseStream.Write(request.Content, 0, request.Content.Length);
        }

        public override void OnGet(HttpRequest request, HttpResponse response)
        {
            string requestFile = Path.Combine(ServerRoot, request.Path.TrimStart('/'));
            if (!requestFile.StartsWith(ServerRoot))
            {
                response.SetError(400, "The request URL is invalid");
                return;
            }
            if (string.IsNullOrEmpty(Path.GetExtension(requestFile)))
            {
                //path
                requestFile = Path.Combine(requestFile, "index.html");
            }
            response.WriteFile(requestFile);
        }

        public override void OnDefault(HttpRequest request, HttpResponse response)
        {

        }
    }
}
