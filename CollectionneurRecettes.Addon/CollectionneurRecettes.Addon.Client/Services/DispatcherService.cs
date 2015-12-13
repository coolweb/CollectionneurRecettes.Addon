using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.Services
{
    public class DispatcherService : IDispatcherService
    {
        public void Invoke(Action action)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}
