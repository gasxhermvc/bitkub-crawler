using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GasxherGIS.Application.ServiceProviders.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace GasxherGIS.Application.ServiceProviders
{
    public class AppServiceProvider : IAppServiceProvider
    {
        public AppServiceProvider(IServiceCollection serviceCollection, IServiceProvider provider)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            this.RegisterService(serviceCollection);
        }

        private Type[] Providers = new Type[]
        {
            typeof(GasxherGIS.Application.ServiceProviders.Standard.Platform.PlatformServiceProvider),
            typeof(GasxherGIS.Application.ServiceProviders.Standard.Command.CommandServiceProvider),
            typeof(GasxherGIS.Application.ServiceProviders.Standard.Environment.AppEnvironmentServiceProvider),
            typeof(GasxherGIS.Application.ServiceProviders.Standard.Configuration.ConfigurationServiceProvider),
            typeof(GasxherGIS.Application.ServiceProviders.Standard.Logging.LoggingServiceProvider),
        };

        public override void RegisterService(IServiceCollection services)
        {
            foreach (var provider in this.Providers)
            {
                //=>Dynamic create provider from type
                IAppServiceProvider instance = (IAppServiceProvider)Activator.CreateInstance(provider);
                instance.RegisterService(services);
            }
        }
    }
}
