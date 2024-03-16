using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application
{
    public interface IConsole
    {
        /// <summary>
        /// The programs configured services.
        /// </summary>
        IServiceProvider Services { get; }
    }
}
