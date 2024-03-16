using System;
using Microsoft.Extensions.DependencyInjection;
using GasxherGIS.Application;
using GasxherGIS.Application.Builder;
using GasxherGIS.Application.ServiceProviders;
using GasxherGIS.Containers.Internal;
using System.Linq;
using GasxherGIS.Containers.ConfigureServices;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Logging;
using GasxherGIS.Application.Internal;
using Microsoft.Extensions.Configuration;

namespace GasxherGIS.Containers
{
    public class ApplicationContainer : IApplicationContainer
    {
        private IServiceCollection initializeServices { get; set; }

        private ILogger _logger { get; set; }

        public IServiceCollection ServiceCollection { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public IConsoleBuilder ConsoleBuilder { get; private set; }

        public string[] Arguments { get; private set; }

        public ApplicationContainer(IConsoleBuilder consoleBuilder, string[] args)
        {
            if (consoleBuilder == null) throw new ArgumentNullException(nameof(consoleBuilder));

            if (args == null) throw new ArgumentNullException(nameof(args));

            ConsoleBuilder = consoleBuilder;
            Arguments = args;

            this.Boot();
        }

        private void Boot()
        {
            //=>Pre instance
            initializeServices = new ServiceCollection();
            initializeServices.AddSingleton<IServiceCollection, ServiceCollection>(_ => new ServiceCollection());

            var provider = initializeServices.BuildServiceProvider(true);
            //_logger = this.ServiceProvider.GetService<SystemLogger>();

            ServiceCollection = initializeServices.Clone();

            this.Container(configureServiceDelegate: configureServiceDelegate =>
            {
                //=>DI
                ServiceCollection.AddSingleton<IServiceProvider, ServiceProvider>();


                //=>container
                configureServiceDelegate.AddTransient<IApplicationContainer, ApplicationContainer>(_ => this);


                //=>Boot internal services (ConsoleBuilder, Stanard Service)
                configureServiceDelegate.AddTransient<IApplicationBuilderFactory, ApplicationBuilderFactory>();

                configureServiceDelegate.AddOptions();
                //configureServiceDelegate.AddLogging();

                configureServiceDelegate.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();
            }).Build(() =>
            {
                this.RegisterServices(ServiceCollection, this.ServiceProvider);
                this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();
                this.ConfigureServices(this.ServiceProvider);
            }).Run(() =>
            {
                //var config = this.ServiceProvider.GetRequiredService<IConfiguration>();
            });
        }

        private ApplicationContainer Container(Action<IServiceCollection> configureServiceDelegate)
        {
            if (configureServiceDelegate == null)
            {
                throw new ArgumentNullException(nameof(configureServiceDelegate));
            }

            configureServiceDelegate.Invoke(ServiceCollection);
            this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();

            return this;
        }

        private ApplicationContainer Build(Action buildDeletegate)
        {
            if (buildDeletegate == null)
            {
                throw new ArgumentNullException(nameof(buildDeletegate));
            }

            buildDeletegate.Invoke();

            return this;
        }

        private void Run(Action runDeletegate)
        {
            if (runDeletegate == null)
            {
                throw new ArgumentNullException(nameof(runDeletegate));
            }

            runDeletegate.Invoke();
        }

        private void RegisterServices(IServiceCollection services, IServiceProvider providers)
        {
            new AppServiceProvider(services, providers);
        }

        private void ConfigureServices(IServiceProvider providers)
        {
            new AppConfigureService(providers);
        }

        public void BuildCommon()
        {
            this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();
        }
    }
}
