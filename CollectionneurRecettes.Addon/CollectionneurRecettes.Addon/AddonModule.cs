

namespace CollectionneurRecettes.Addon
{
    using AutoMapper;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddonModule : IModule
    {
        private IUnityContainer container;

        public AddonModule(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        public void Initialize()
        {
            // Data layer
            this.container.RegisterType<Interfaces.IDataSettings, Repository.DataSettings>();
            this.container.RegisterType<Interfaces.IH2Database, Repository.H2Database>();
            this.container.RegisterType<Interfaces.ICollectorReceiptRepository, Repository.CollectorReceiptRepository>();
            this.container.RegisterType<Interfaces.IGoogleRepository, Repository.GoogleRepository>();
            this.container.RegisterType<Interfaces.IProcessRepository, Repository.ProcessRepository>();
            this.container.RegisterType<Interfaces.INetworkHelper, Repository.NetworkHelper>();

            // Business layer
            this.container.RegisterType<Interfaces.ISettingsManager, Business.SettingsManager>();
            this.container.RegisterType<Interfaces.IGoogleManager, Business.GoogleManager>();
            this.container.RegisterType<Interfaces.ICollectorReceiptManager, Business.CollectorReceiptManager>();

            // AutoMapper
            this.container.RegisterType<Entity.Mappings.SelectMenuViewConverter>();

            Mapper.Initialize((cfg) =>
            {
                cfg.ConstructServicesUsing(ServiceLocator.Current.GetInstance);

                cfg.CreateMap<Entity.Data.SelectMenuView, Entity.Menu>().ConvertUsing<Entity.Mappings.SelectMenuViewConverter>();
            });
        }
    }
}
