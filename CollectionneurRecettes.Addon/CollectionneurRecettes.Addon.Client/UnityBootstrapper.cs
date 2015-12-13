

namespace CollectionneurRecettes.Addon.Client
{
    using Microsoft.Practices.Unity;
    using Prism.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    internal class UnityBootstrapper : Prism.Unity.UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            base.InitializeModules();
            return new Views.MainWindow(this.Container.Resolve<IEventAggregator>());
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            Type moduleAddon = typeof(CollectionneurRecettes.Addon.AddonModule);
            ModuleCatalog.AddModule(
                new Prism.Modularity.ModuleInfo()
                {
                    ModuleName = moduleAddon.Name,
                    ModuleType = moduleAddon.AssemblyQualifiedName
                });

            Type moduleClient = typeof(ClientModule);
            ModuleCatalog.AddModule(
                new Prism.Modularity.ModuleInfo()
                {
                    ModuleName = moduleClient.Name,
                    ModuleType = moduleClient.AssemblyQualifiedName
                });
        }
    }
}
