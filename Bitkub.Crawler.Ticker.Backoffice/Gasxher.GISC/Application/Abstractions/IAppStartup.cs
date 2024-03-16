using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Abstractions
{
    public interface IAppStartup
    {
        /// <summary>
        /// Configure the <see cref="IConsoleBuilder"/>.
        /// </summary>
        /// <remarks>
        /// Configure is intended to be called before user code, allowing a user to overwrite any changes made.
        /// </remarks>
        /// <param name="builder"></param>
        void Configure(IConsoleBuilder builder);
    }
}
