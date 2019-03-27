using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpServerLib
{
    public class HttpRequest
    {        
        private ILogger Logger = null;

        public byte[] Content = null;
        public String HttpMethod;
        public string Url;
        public string HttpVersion;
        public string Path;
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        internal HttpRequest(ILogger Logger)
        {
            this.Logger = Logger;
        }
        /// <summary>
        /// 解析请求
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal bool Parse(Stream stream)
        {
            try
            {
                int _contentLength = -1;
                stream.ReadTimeout = 100;
                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                string strLine = sr.ReadLine();
                if (string.IsNullOrEmpty(strLine))
                {
                    return false;
                }
                string[] firstRow = strLine.Split(' ');
                if (firstRow.Length != 3)
                {
                    return false;
                }
                HttpMethod = firstRow[0];
                Url = firstRow[1];
                HttpVersion = firstRow[2];
                while (!string.IsNullOrEmpty(strLine))
                {
                    int spidx = strLine.IndexOf(":");
                    if (spidx > 0)
                    {
                        string key = strLine.Substring(0, spidx);
                        string value = strLine.Substring(spidx + 1);
                        Headers[key] = value;
                        if (key == "Content-Length" && int.TryParse(value, out _contentLength))
                        {

                        }
                    }
                    strLine = sr.ReadLine();
                }
                if (_contentLength > 0)
                {                    
                    char[] ContentChar = new char[_contentLength];
                    sr.Read(ContentChar, 0, _contentLength);
                    Content = sr.CurrentEncoding.GetBytes(new String(ContentChar));
                    Array.Resize(ref Content, _contentLength);
                }
                ParseParams();
            }
            catch(Exception ee) {
                Logger.Log(ee.Message);
                return false;
            }
            return true;
        }

        internal void ParseParams()
        {
            int psidx = Url.IndexOf("?");
            if (psidx == -1)
            {
                Path = Url;
                return;
            }
            Path = Url.Substring(0, psidx);
            string[] strArr = Url.Substring(psidx + 1).Split('&');
            foreach (var item in strArr)
            {
                string[] akey = item.Split('=');
                if (akey.Length >= 2)
                {
                    Params.Add(akey[0], akey[1]);
                }
                else
                {
                    Params.Add(item, "");
                }
            }
        }
    }
}
