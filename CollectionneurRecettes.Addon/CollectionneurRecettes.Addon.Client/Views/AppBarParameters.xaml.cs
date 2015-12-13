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

namespace CollectionneurRecettes.Addon.Client.Views
{
    /// <summary>
    /// Interaction logic for AppBar.xaml
    /// </summary>
    public partial class AppBarParameters : UserControl
    {
        public AppBarParameters()
        {
            InitializeComponent();
        }

        public AppBarParameters(ViewModels.ParametersViewModel viewModel) : this()
        {
            this.DataContext = viewModel;
        }
    }
}
