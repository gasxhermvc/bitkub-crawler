using Microsoft.Extensions.DependencyInjection;
using GasxherGIS.Application.ServiceProviders.Internal;
using GasxherGIS.Standards.Platform;
using GasxherGIS.Standards.Platform.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GasxherGIS.Application.ServiceProviders.Standard.Platform
{
    public class PlatformServiceProvider : IAppServiceProvider
    {
        public override void RegisterService(IServiceCollection services)
        {
            services.AddSingleton<IPlatformRuntime, PlatformRuntime>();
        }
    }
}
