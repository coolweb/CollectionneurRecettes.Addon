using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace CollectionneurRecettes.Addon.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private IEventAggregator eventAggregator;
        private bool isLoaded = false;

        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<Events.DisplayErrorMessageEvent>().Subscribe(new Action<string>((message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (this.Visibility == Visibility.Visible && this.IsLoaded)
                    {
                        this.ShowMessageAsync("Collectionneur recettes Addon", message);
                    }
                    else
                    {
                        this.myNotifyIcon.ShowBalloonTip("Collectionneur recettes Addon", message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
                    }
                });
            }));

            this.eventAggregator.GetEvent<Events.DisplaySuccessMessageEvent>().Subscribe(new Action<string>((message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (this.Visibility == Visibility.Visible && this.IsLoaded)
                    {
                        this.ShowMessageAsync("Collectionneur recettes Addon", message);
                    }
                    else
                    {
                        this.myNotifyIcon.ShowBalloonTip("Collectionneur recettes Addon", message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                    }
                });
            }));

            this.eventAggregator.GetEvent<Events.DisplayNotificationMessageEvent>().Subscribe(new Action<string>((message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.myNotifyIcon.ShowBalloonTip("Collectionneur recettes Addon", message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
                });
            }));

            this.eventAggregator.GetEvent<Events.ActivateWindowEvent>().Subscribe((o) =>
            {
                this.Visibility = Visibility.Visible;
                this.WindowState = WindowState.Maximized;
            });

            this.Loaded += this.MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.isLoaded)
            {
                this.isLoaded = true;

                // Flyout has no visual tree parent, so need to manually set the region manager.
                var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
                RegionManager.SetRegionManager(this.Flyout, regionManager);

                this.Closing += this.MainWindow_Closing;

                this.Visibility = Visibility.Hidden;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
            this.Visibility = Visibility.Hidden;
        }
    }
}
