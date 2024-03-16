using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Abstractions
{
    public interface IStartupConfigureServicesFilter
    {
        /// <summary>
        /// Extends the provided <paramref name="next"/> and returns a modified <see cref="Action"/> action of the same type.
        /// </summary>
        /// <param name="next">The ConfigureServices method to extend.</param>
        /// <returns>A modified <see cref="Action"/>.</returns>
        Action<IServiceCollection> ConfigureServices(Action<IServiceCollection> next);
    }
}
