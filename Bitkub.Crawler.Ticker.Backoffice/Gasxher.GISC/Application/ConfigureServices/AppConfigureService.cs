using System;
using GasxherGIS.Application.ConfigureServices.Internal;

namespace GasxherGIS.Containers.ConfigureServices
{
    public class AppConfigureService : IAppConfigureService
    {
        private Type[] Configures = new Type[]
        {
            typeof(GasxherGIS.Application.ConfigureServices.Command.CommandLineArgumentConfigureService),
            typeof(GasxherGIS.Application.ConfigureServices.Configuration.ConfigurationConfigureService),
            typeof(GasxherGIS.Application.ConfigureServices.Logging.LoggingConfigureService)
        };

        public AppConfigureService(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            this.ConfigureServices(serviceProvider);
        }

        public override void ConfigureServices(IServiceProvider provider)
        {
            foreach (var configure in Configures)
            {
                var instance = (IAppConfigureService)Activator.CreateInstance(configure);
                instance.ConfigureServices(provider);
            }
        }
    }
}
