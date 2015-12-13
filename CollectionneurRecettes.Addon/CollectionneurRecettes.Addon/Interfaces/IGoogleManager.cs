using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Interfaces
{
    public interface IGoogleManager
    {
        bool IsAccountAlreadyConfigured();
        bool SecretFileExists();
        void ClearConfiguredAccount();
        Task<bool> ConfigureAccount();
        Task<IEnumerable<Entity.Calendar>> LoadCalendars();
        Task<int> CreateMenus(Entity.Menu menu, IProgress<Entity.ProgressCreateMenus> progress);
    }
}
