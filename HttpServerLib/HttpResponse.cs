using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpServerLib
{
    public class HttpResponse
    {
        public int StatusCode = 200;
        public String StatusDescription;
        public String ContentType = "text/html; charset=UTF-8";
        public Stream ResponseStream = new MemoryStream();
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        private ILogger Logger = null;
        private Stream handler;
        private bool isEnd = false;
        private static readonly String[][] s_HTTPStatusDescriptions = new String[][]
        {
            null,
            new String[]
            { 
                /* 100 */"Continue",
                /* 101 */ "Switching Protocols",
                /* 102 */ "Processing"
            },

            new String[]
            { 
                /* 200 */"OK",
                /* 201 */ "Created",
                /* 202 */ "Accepted",
                /* 203 */ "Non-Authoritative Information",
                /* 204 */ "No Content",
                /* 205 */ "Reset Content",
                /* 206 */ "Partial Content",
                /* 207 */ "Multi-Status"
            },

            new String[]
            { 
                /* 300 */"Multiple Choices",
                /* 301 */ "Moved Permanently",
                /* 302 */ "Found",
                /* 303 */ "See Other",
                /* 304 */ "Not Modified",
                /* 305 */ "Use Proxy",
                /* 306 */ String.Empty,
                /* 307 */ "Temporary Redirect"
            },

            new String[]
            { 
                /* 400 */"Bad Request",
                /* 401 */ "Unauthorized",
                /* 402 */ "Payment Required",
                /* 403 */ "Forbidden",
                /* 404 */ "Not Found",
                /* 405 */ "Method Not Allowed",
                /* 406 */ "Not Acceptable",
                /* 407 */ "Proxy Authentication Required",
                /* 408 */ "Request Timeout",
                /* 409 */ "Conflict",
                /* 410 */ "Gone",
                /* 411 */ "Length Required",
                /* 412 */ "Precondition Failed",
                /* 413 */ "Request Entity Too Large",
                /* 414 */ "Request-Uri Too Long",
                /* 415 */ "Unsupported Media Type",
                /* 416 */ "Requested Range Not Satisfiable",
                /* 417 */ "Expectation Failed",
                /* 418 */ String.Empty,
                /* 419 */ String.Empty,
                /* 420 */ String.Empty,
                /* 421 */ String.Empty,
                /* 422 */ "Unprocessable Entity",
                /* 423 */ "Locked",
                /* 424 */ "Failed Dependency"
            },

            new String[]
            { 
                /* 500 */"Internal Server Error",
                /* 501 */ "Not Implemented",
                /* 502 */ "Bad Gateway",
                /* 503 */ "Service Unavailable",
                /* 504 */ "Gateway Timeout",
                /* 505 */ "Http Version Not Supported",
                /* 506 */ String.Empty,
                /* 507 */ "Insufficient Storage"
            }
        };
        internal HttpResponse(Stream stream, ILogger Logger)
        {
            handler = stream;
            this.Logger = Logger;
        }
        public static String GetStatusDescription(int code)
        {
            if (code >= 100 && code < 600)
            {
                int i = code / 100;
                int j = code % 100;

                if (j < s_HTTPStatusDescriptions[i].Length)
                    return s_HTTPStatusDescriptions[i][j];
            }

            return String.Empty;
        }

        public void End()
        {
            if (!isEnd)
            {
                isEnd = true;

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("HTTP/1.1 " + StatusCode + " " + GetStatusDescription(StatusCode));
                if (!Headers.ContainsKey("Content-Type"))
                {
                    builder.AppendLine("Content-Type:" + ContentType);
                }
                foreach (var item in Headers)
                {
                    builder.AppendLine(item.Key + ":" + item.Value);
                }
                builder.AppendLine("Content-Length:" + ResponseStream.Length);
                builder.AppendLine("Connection: close");
                builder.AppendLine("");
                var data = Encoding.UTF8.GetBytes(builder.ToString());
                handler.Write(data, 0, data.Length);
                byte[] contextData = new byte[ResponseStream.Length];
                ResponseStream.Seek(0, SeekOrigin.Begin);
                int contextDatalen = ResponseStream.Read(contextData, 0, (int)ResponseStream.Length);                
                handler.Write(contextData, 0, contextDatalen);
                handler.Close();
            }            
        }

        public void SetError(int statusCode,string ErrorMsg)
        {
            this.StatusCode = statusCode;
            StringBuilder respstr = new StringBuilder();
            respstr.Append("<!DOCTYPE HTML><HTML><HEAD><TITLE>");
            respstr.Append(GetStatusDescription(StatusCode));
            respstr.Append("</TITLE></HEAD><BODY><h2>");
            respstr.Append(GetStatusDescription(StatusCode));
            respstr.Append("</h2><hr /><p>HTTP Error ");
            respstr.Append(StatusCode);
            respstr.Append(". ");
            respstr.Append(ErrorMsg);
            respstr.Append("</p></BODY></HTML>");
            Write(respstr.ToString());
            End();
        }
        public void Write(string str)
        {
            var data = Encoding.UTF8.GetBytes(str.ToString());
            ResponseStream.Write(data, 0, data.Length);
        }
        public void WriteFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                SetError(404, "File No Found");
                return;
            }            
            var data = File.ReadAllBytes(filepath);
            ResponseStream.Write(data, 0, data.Length);
        }
    }
}
