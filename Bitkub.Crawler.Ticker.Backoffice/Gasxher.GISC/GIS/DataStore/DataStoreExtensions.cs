using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.DataStore
{
    public static class DataStoreExtensions
    {
        public static IServiceCollection AddDataStore(this IServiceCollection services)
        {
            services.AddSingleton<IDataStore, DataStore>(_ => new DataStore());

            return services;
        }
    }
}
