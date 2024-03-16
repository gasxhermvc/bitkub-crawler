using GasxherGIS.GIS.Connector.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector
{
    public static class DbDataAccessExtensions
    {
        public static IServiceCollection AddDbDataAccess(this IServiceCollection services) => services.AddDbDataAccess(null);

        public static IServiceCollection AddDbDataAccess(this IServiceCollection services, Action<DatabaseSetting> configureDelegate)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var settings = new DatabaseSetting();
            services.TryAddSingleton(settings);

            services.TryAddSingleton<IDbDataAccess, DbDataAccess>();

            if (configureDelegate != null)
            {
                configureDelegate.Invoke(settings);
            }

            services.Configure<DbConfigProvider>(configureOptions: (Setting) =>
            {
                configuration.GetSection("Database").Bind(Setting);
                Setting.ConnectionTimeout = settings.ConnectionTimeout;
                Setting.CommandTimeout = settings.CommandTimeout;
            });

            return services;
        }
    }
}
