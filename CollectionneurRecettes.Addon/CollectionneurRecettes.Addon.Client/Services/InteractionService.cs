using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.Services
{
    public class InteractionService : IInteractionService
    {
        private IEventAggregator eventAggregator;

        public InteractionService(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggrgator");
            }

            this.eventAggregator = eventAggregator;
        }

        public string OpenFileDialog(string filter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = filter;

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        public void ShowErrorDialog(string message)
        {
            this.eventAggregator.GetEvent<Events.DisplayErrorMessageEvent>().Publish(message);
        }

        public void ShowSuccessDialog(string message)
        {
            this.eventAggregator.GetEvent<Events.DisplaySuccessMessageEvent>().Publish(message);
        }
    }
}
