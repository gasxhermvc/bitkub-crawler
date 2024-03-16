using Microsoft.Extensions.DependencyInjection;
using GasxherGIS.Application.ServiceProviders.Internal;
using GasxherGIS.Standards.Environment;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using GasxherGIS.Standards.Command.Internal;

namespace GasxherGIS.Application.ServiceProviders.Standard.Environment
{
    public class AppEnvironmentServiceProvider : IAppServiceProvider
    {
        public override void RegisterService(IServiceCollection services)
        {
            services.TryAddSingleton<IAppEnvironment, AppEnvironment>();

            //var provider = services.BuildServiceProvider();

            //var appEnvironment = provider.GetService<IAppEnvironment>();
            //var commandLineArgument = services.BuildServiceProvider().GetService<ICommandLineArgument>();

            //var args = commandLineArgument.SetArguments(System.Environment);
        }
    }
}
