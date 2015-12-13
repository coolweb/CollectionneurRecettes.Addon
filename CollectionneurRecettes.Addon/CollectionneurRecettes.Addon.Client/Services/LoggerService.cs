using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.Services
{
    public class LoggerService : ILoggerService
    {
        TraceSource trace = new TraceSource("CollectionneurRecettes.Addon");

        public void LogError(string message)
        {
            trace.TraceEvent(TraceEventType.Error, 2, message);
        }

        public void LogInformation(string log)
        {
            trace.TraceInformation(log);
        }
    }
}
