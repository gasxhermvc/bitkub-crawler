using GasxherGIS.Application;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Containers.Internal
{
    public interface IApplicationContainer
    {
        IServiceCollection ServiceCollection { get; }

        IServiceProvider ServiceProvider { get; }

        IConsoleBuilder ConsoleBuilder { get; }

        string[] Arguments { get; }

        void BuildCommon();
    }
}
