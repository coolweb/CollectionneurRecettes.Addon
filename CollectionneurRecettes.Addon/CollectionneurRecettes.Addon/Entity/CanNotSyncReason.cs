using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Entity
{
    public enum CanNotSyncReason
    {
        None = 0,
        NoNetwork = 1,
        CollectorAppIsRunning = 2,
        BadSettings = 3
    }
}
