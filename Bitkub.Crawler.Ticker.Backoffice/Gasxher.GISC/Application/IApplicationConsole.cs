using GasxherGIS.Application.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application
{
    public abstract class IApplicationConsole : IConsole
    {
        public IServiceProvider Services { get; }

        public abstract void Main();
    }
}
