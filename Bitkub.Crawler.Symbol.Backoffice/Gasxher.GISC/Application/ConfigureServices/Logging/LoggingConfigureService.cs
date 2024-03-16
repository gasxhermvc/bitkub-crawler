using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GasxherGIS.Application.ConfigureServices.Internal;
using GasxherGIS.Standards.Logging.Internal;
using GasxherGIS.Containers.Internal;
using GasxherGIS.Standards.Logging.Manager;
using Microsoft.Extensions.Options;

namespace GasxherGIS.Application.ConfigureServices.Logging
{
    public class LoggingConfigureService : IAppConfigureService
    {
        public override void ConfigureServices(IServiceProvider provider)
        {
            var container = provider.GetService<IApplicationContainer>();
            var _configuration = container.ServiceProvider.GetService<IConfigurationBuilder>();
            var configuration = _configuration.Build();
            container.ServiceCollection.Configure<LoggingProviders>(configuration.GetSection("Logging"));

            container.ServiceCollection.AddLogging(configure =>
            {
                //=>Clear all providers
                configure.ClearProviders();

                //=>Add new provider
                configure.AddConfiguration(configuration.GetSection(AppConsoleDefaults.LoggingKey));
                configure.AddProvider(container.ServiceProvider.GetService<AppLoggerProvider>());
            });
        }
    }
}
