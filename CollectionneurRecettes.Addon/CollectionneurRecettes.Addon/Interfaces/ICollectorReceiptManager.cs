using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Interfaces
{
    public interface ICollectorReceiptManager
    {
        Entity.Menu RetrieveMenu();
        bool IsCollectorReceiptAppRunning();
        Entity.CanNotSyncReason CanSync();
    }
}
