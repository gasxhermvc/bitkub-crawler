using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Internal
{
    internal class StartupMethods
    {
        public StartupMethods(object? instance, Action<IApplicationBuilder> configure, Func<IServiceCollection, IServiceProvider> configureServices)
        {
            Debug.Assert(configure != null);
            Debug.Assert(configureServices != null);

            StartupInstance = instance;
            ConfigureDelegate = configure;
            ConfigureServicesDelegate = configureServices;
        }

        public object? StartupInstance { get; }

        public Func<IServiceCollection, IServiceProvider> ConfigureServicesDelegate { get; }

        public Action<IApplicationBuilder> ConfigureDelegate { get; }
    }
}
