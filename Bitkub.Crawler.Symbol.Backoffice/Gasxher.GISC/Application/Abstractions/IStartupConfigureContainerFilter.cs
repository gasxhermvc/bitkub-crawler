using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Abstractions
{
    public interface IStartupConfigureContainerFilter<TContainerBuilder>
    {
        /// <summary>
        /// Extends the provided <paramref name="container"/> and returns a modified <see cref="Action"/> action of the same type.
        /// </summary>
        /// <param name="container">The ConfigureContainer method to extend.</param>
        /// <returns>A modified <see cref="Action"/>.</returns>
        Action<TContainerBuilder> ConfigureContainer(Action<TContainerBuilder> container);
    }
}
