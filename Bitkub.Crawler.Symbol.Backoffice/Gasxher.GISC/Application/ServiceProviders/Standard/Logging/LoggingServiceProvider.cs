using System;
using Microsoft.Extensions.DependencyInjection;
using GasxherGIS.Application.ServiceProviders.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Logging;
using GasxherGIS.Standards.Logging.Internal;
using GasxherGIS.Standards.Logging.Manager;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GasxherGIS.Application.ServiceProviders.Standard.Logging
{
    public class LoggingServiceProvider : IAppServiceProvider
    {
        public override void RegisterService(IServiceCollection services)
        {

            //=>Register Logger Manager
            services.TryAddSingleton(typeof(LoggerManager));

            //=>Register Provider
            services.TryAddSingleton(typeof(SystemLoggerProvider));
            services.TryAddSingleton(typeof(DatabaseLoggerProvider));
            services.TryAddScoped(typeof(AppLoggerProvider));

            //=>Register Handler
            services.AddScoped(typeof(LoggerHandler));
            services.AddSingleton(typeof(SystemLoggerHandler<>));
            services.AddSingleton(typeof(DatabaseLoggerHandler<>));
            services.AddSingleton(typeof(ApplicationLoggerHandler));

            //services.AddSingleton<LoggingProviders>(new LoggingProviders());
            //services.AddOptions<LoggingProviders>();

        }
    }
}
