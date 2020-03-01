using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Veganko.Extensions
{
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task task)
        {
            task.ContinueWith(
                (t) => 
                {
                    Debug.WriteLine($"{t.Exception.Message}\n{t.Exception.StackTrace}");
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
