using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.Services
{
    public interface IInteractionService
    {
        string OpenFileDialog(string filter);
        void ShowErrorDialog(string message);
        void ShowSuccessDialog(string message);
    }
}
