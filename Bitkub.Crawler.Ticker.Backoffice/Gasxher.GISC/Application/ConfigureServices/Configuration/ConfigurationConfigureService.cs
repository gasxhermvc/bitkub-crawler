using GasxherGIS.Application.ConfigureServices.Internal;
using GasxherGIS.Containers.Internal;
using GasxherGIS.Standards.Environment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace GasxherGIS.Application.ConfigureServices.Configuration
{
    public class ConfigurationConfigureService : IAppConfigureService
    {
        public override void ConfigureServices(IServiceProvider provider)
        {
            var container = provider.GetRequiredService<IApplicationContainer>();
            var appEnvironment = container.ServiceProvider.GetService<IAppEnvironment>();
            var configuration = container.ServiceProvider.GetService<IConfigurationBuilder>();

            //=>Reload Configuration
            configuration.Sources.Clear();

            configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);

            var _jsonFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appSettings.{appEnvironment.GetEnvironment()}.json");

            if (System.IO.File.Exists(_jsonFile))
            {
                configuration.AddJsonFile($"appSettings.{appEnvironment.GetEnvironment()}.json", optional: true);
            }
                    
            var _configuration = configuration.Build();

            container.ServiceCollection.Configure<IConfiguration>(configureOptions: (Configuration) =>
            {
                _configuration.Bind(Configuration);
            });
        }
    }
}
