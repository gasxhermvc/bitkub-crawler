using Microsoft.Extensions.DependencyInjection;
using GasxherGIS.Application.ServiceProviders.Internal;
using GasxherGIS.Standards.Command;
using GasxherGIS.Standards.Command.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GasxherGIS.Application.ServiceProviders.Standard.Command
{
    public class CommandServiceProvider : IAppServiceProvider
    {
        public override void RegisterService(IServiceCollection services)
        {
            services.TryAddSingleton<ICommandLineArgument, CommandLineArgument>();
            services.TryAddSingleton<ICommandParser, CommandParser>();
            services.AddSingleton<ICommandOptionParser, CommandOptionParser>();
        }
    }
}
