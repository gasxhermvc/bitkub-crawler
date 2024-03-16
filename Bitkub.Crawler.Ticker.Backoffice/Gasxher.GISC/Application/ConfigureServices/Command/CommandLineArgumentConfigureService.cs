using System;
using GasxherGIS.Containers.Internal;
using GasxherGIS.Application.ConfigureServices.Internal;
using GasxherGIS.Standards.Command.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.ConfigureServices.Command
{
    public class CommandLineArgumentConfigureService : IAppConfigureService
    {
        public override void ConfigureServices(IServiceProvider provider)
        {
            var container = provider.GetService<IApplicationContainer>();
            var commandLineArgument = provider.GetService<ICommandLineArgument>();

            commandLineArgument.SetArguments(container.Arguments);
        }
    }
}
