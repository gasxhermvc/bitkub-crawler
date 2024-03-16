using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GasxherGIS.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GasxherGIS.Application.Startup
{
    public abstract class StartupBase : IStartup
    {
        public abstract void Configure(IApplicationBuilder app);

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            ConfigureServices(services);
            return CreateServiceProvider(services);
        }


        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }

    public abstract class StartupBase<TBuilder> : StartupBase where TBuilder : notnull
    {
        private readonly IServiceProviderFactory<TBuilder> _factory;

        public StartupBase(IServiceProviderFactory<TBuilder> factory)
        {
            _factory = factory;
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var builder = _factory.CreateBuilder(services);
            ConfigureContainer(builder);
            return _factory.CreateServiceProvider(builder);
        }

        public virtual void ConfigureContainer(TBuilder builder)
        {

        }
    }
}
