using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpServerLib
{
    public interface ILogger
    {
        void Log(object message);
    }
}
