using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client
{
    public class ClientModule : IModule
    {
        private IUnityContainer container;

        public ClientModule(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        public void Initialize()
        {
            // Services
            this.container.RegisterType<Services.IInteractionService, Services.InteractionService>();
            this.container.RegisterType<Services.IDispatcherService, Services.DispatcherService>();
            this.container.RegisterType<Services.ILoggerService, Services.LoggerService>();

            // Interception
            this.container.AddNewExtension<Interception>();
            this.container.RegisterType<Interceptions.LoggerInterceptor>(
                "LogInterceptor", 
                new InjectionFactory((container)=>
                {
                    return new Interceptions.LoggerInterceptor()
                    {
                        LoggerService = this.container.Resolve<Services.ILoggerService>()
                    };
                }));

            this.container.RegisterType<Interfaces.IDataSettings>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Interceptions.LoggerInterceptor>("LogInterceptor")
                );
            this.container.RegisterType<Interfaces.INetworkHelper>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Interceptions.LoggerInterceptor>("LogInterceptor")
                );
            this.container.RegisterType<Interfaces.IGoogleRepository>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Interceptions.LoggerInterceptor>("LogInterceptor")
                );
            this.container.RegisterType<Interfaces.IH2Database>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Interceptions.LoggerInterceptor>("LogInterceptor")
                );
            this.container.RegisterType<Interfaces.ICollectorReceiptRepository>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Interceptions.LoggerInterceptor>("LogInterceptor")
                );
            this.container.RegisterType<Interfaces.IGoogleManager>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Interceptions.LoggerInterceptor>("LogInterceptor")
                );
            this.container.RegisterType<Interfaces.ISettingsManager>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<Interceptions.LoggerInterceptor>("LogInterceptor")
                );

            // Views
            this.container.RegisterType<Object, Views.Parameters>("ParametersView");
            this.container.RegisterType<Object, Views.AppBarParameters>("AppBarParametersView");

            this.container.RegisterType<Object, Views.SyncMenu>("SyncMenuView");

            this.container.RegisterType<Object, Views.About>("AboutView");

            this.container.RegisterType<ViewModels.ParametersViewModel>(new ContainerControlledLifetimeManager());
        }
    }
}
