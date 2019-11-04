using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Veganko.Services.Logging
{
    public class Logger : ILogger
    {
        public void LogException(Exception ex)
        {
            Debug.WriteLine($"{ex.Message}\t{ex.StackTrace}");
#if !DEBUG
            Crashes.TrackError(ex);
#endif
        }

        public void WriteLine<TClass>(string message)
        {
            string callingClassName = typeof(TClass).FullName;
            message = $"[{callingClassName}]: {message}";
            Debug.WriteLine(message);
#if !DEBUG
            Analytics.TrackEvent(message);
#endif
        }
    }
}
