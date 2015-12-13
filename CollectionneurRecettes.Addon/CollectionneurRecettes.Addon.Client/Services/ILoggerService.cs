using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.Services
{
    public interface ILoggerService
    {
        void LogInformation(string log);
        void LogError(string message);
    }
}
