// <copyright file="AddonModule.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon
{
    using System;
    using AutoMapper;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Prism.Modularity;

    /// <summary>
    /// Configures the module.
    /// </summary>
    /// <seealso cref="Prism.Modularity.IModule" />
    public class AddonModule : IModule
    {
        /// <summary>
        /// The container
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddonModule"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <exception cref="System.ArgumentNullException">container is null</exception>
        public AddonModule(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        /// <summary>
        /// Initialize the module.
        /// </summary>
        public void Initialize()
        {
            // Cross Cutting
            this.container.RegisterType<CrossCutting.ILoggerService, CrossCutting.LoggerService>();

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
