using System;
using GasxherGIS.Standards.Environment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GasxherGIS.Application.ServiceProviders.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GasxherGIS.Application.ServiceProviders.Standard.Configuration
{
    public class ConfigurationServiceProvider : IAppServiceProvider
    {
        public override void RegisterService(IServiceCollection services)
        {
            services.TryAddSingleton<IConfigurationBuilder>(new ConfigurationBuilder());
            services.TryAddSingleton<IConfiguration>(sp => sp.GetService<IConfigurationBuilder>().Build());
        }
    }
}
