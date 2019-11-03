using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Logging
{
    public interface ILogger
    {
        void LogException(Exception ex);

        void WriteLine<TClass>(string message);
    }
}
