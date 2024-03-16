using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Internal
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection Clone(this IServiceCollection serviceCollection)
        {
            IServiceCollection clone = new ServiceCollection();
            foreach (var service in serviceCollection)
            {
                clone.Add(service);
            }
            return clone;
        }
    }
}
