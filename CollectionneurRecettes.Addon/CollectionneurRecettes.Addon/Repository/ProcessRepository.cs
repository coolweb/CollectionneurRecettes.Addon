using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Repository
{
    public class ProcessRepository : Interfaces.IProcessRepository
    {
        public Process RunningProcess(string name)
        {
            var foundProcesses = Process.GetProcessesByName(name).FirstOrDefault();
            
            return foundProcesses;
        }
    }
}
