using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using GasxherGIS.Standards.Environment;
using Microsoft.Extensions.Logging;
using GasxherGIS.Containers.Internal;
using GasxherGIS.Application.Internal;
using GasxherGIS.Application.Abstractions;
using GasxherGIS.Standards.Command.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GasxherGIS.Application
{
    public class ConsoleBuilder : IConsoleBuilder
    {
        //private readonly IAppEnvironment _appEnvironment;
        private IApplicationContainer _container;

        //=>Delegates
        private Action<IConfiguration> _configurationDelegate;
        private Action<ConsoleBuilderContext, IConfiguration> _configureAppConfigurationDelegate;
        private Action<ConsoleBuilderContext, IServiceCollection> _configureAppConfigureServiceDelegate;
        private Action<IServiceCollection> _configureServiceDelegate;

        private IAppEnvironment _appEnvironment;
        private IConfiguration _configuration;
        private ConsoleBuilderContext _context;
        private AppConsoleOptions _options;

        public ConsoleBuilder()
        {
            _context = new ConsoleBuilderContext();
        }

        public void SetContainer(IApplicationContainer container)
        {
            _container = container;
            this.UpdateContext();
        }

        private void UpdateContext()
        {
            _appEnvironment = _container.ServiceProvider.GetService<IAppEnvironment>();
            _configuration = _container.ServiceProvider.GetService<IConfiguration>();

            _context.AppEnvironment = _appEnvironment;
            _context.ServiceProvider = _container.ServiceProvider;
            _context.Configuration = _configuration;
            _context.Arguments = _container.Arguments;
        }

        public IConsole Build()
        {
            var appServices = BuildCommonServices(out var appStartupErrors);
            var applicationService = appServices;
            var appServiceProvider = GetProviderFromFactory(appServices);

            var app = new GasxherGIS.Extensions.Application.Internal.Console(applicationService,
                appServiceProvider,
                _options,
                _configuration,
                appStartupErrors);

            try
            {
                app.Initialize();

                app.Services.GetRequiredService<ICommandLineArgument>().SetArguments(_container.Arguments);
                var _ = app.Services.GetRequiredService<IConfiguration>();

                var logger = app.Services.GetRequiredService<ILogger<GasxherGIS.Extensions.Application.Internal.Console>>();

                var assemblyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var assemblyName in _options.GetFinalAppStartupAssemblies())
                {
                    if (!assemblyNames.Add(assemblyName))
                    {
                        logger.LogWarning($"The assembly {assemblyName} was specified multiple times. App startup assembiles should only be specified onec.");
                    }
                }

                return app;
            }
            catch
            {
                app.Dispose();
                throw;
            }

            IServiceProvider GetProviderFromFactory(IServiceCollection collection)
            {
                var provider = collection.BuildServiceProvider();
                var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

                if (factory != null && !(factory is DefaultServiceProviderFactory))
                {
                    return factory.CreateServiceProvider(factory.CreateBuilder(collection));
                }

                return provider;
            }
        }

        [MemberNotNull(nameof(_options))]
        public IServiceCollection BuildCommonServices(out AggregateException? appStartupErrors)
        {
            appStartupErrors = null;

            _options = new AppConsoleOptions(_configuration, Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty);

            if (!_options.PreventAppStartup)
            {
                var exceptions = new List<Exception>();
                var processed = new HashSet<Assembly>();

                foreach (var assemblyName in _options.GetFinalAppStartupAssemblies())
                {
                    try
                    {
                        var assetmbly = Assembly.Load(new AssemblyName(assemblyName));

                        if ((!processed.Add(assetmbly)))
                        {
                            // Already processed, skip it
                            continue;
                        }

                        foreach (var attribute in assetmbly.GetCustomAttributes<AppStartupAttribute>())
                        {
                            var appStartup = (IAppStartup)Activator.CreateInstance(attribute.AppStartupType);
                            appStartup.Configure(this);
                        }

                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new InvalidOperationException($"Startup assembly {assemblyName} failed to execute. See the inner exception for more details.", ex));
                    }
                }

                if (exceptions.Count > 0)
                {
                    appStartupErrors = new AggregateException(exceptions);
                }
            }

            // Initialize the app environment
            var contentRootPath = ResolveContentRootPath(_options.ContentRootPath, AppContext.BaseDirectory);
            (_appEnvironment as AppEnvironment).Initialize(contentRootPath, _options);

            _container.ServiceCollection.AddSingleton(_appEnvironment);
            #region fix
            ////if (!string.IsNullOrEmpty(_options.StartupAssembly))
            ////{
            ////    try
            ////    {
            ////        var startupType = StartupLoader.FindStartupType(_options.StartupAssembly, _hostingEnvironment.EnvironmentName);

            ////        if (typeof(IStartup).IsAssignableFrom(startupType))
            ////        {
            ////            services.AddSingleton(typeof(IStartup), startupType);
            ////        }
            ////        else
            ////        {
            ////            services.AddSingleton(typeof(IStartup), sp =>
            ////            {
            ////                var hostingEnvironment = sp.GetRequiredService<IHostEnvironment>();
            ////                var methods = StartupLoader.LoadMethods(sp, startupType, hostingEnvironment.EnvironmentName);
            ////                return new ConventionBasedStartup(methods);
            ////            });
            ////        }
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        var capture = ExceptionDispatchInfo.Capture(ex);
            ////        services.AddSingleton<IStartup>(_ =>
            ////        {
            ////            capture.Throw();
            ////            return null;
            ////        });
            ////    }
            ////}
            #endregion

            //var services = _container.ServiceCollection;
            _configureAppConfigureServiceDelegate?.Invoke(_context, _container.ServiceCollection);

            return _container.ServiceCollection;
        }

        public IConsole Build<TStartup>()
        {
            this.UseStartup(typeof(TStartup));

            return this.Build();
        }

        #region Wait Fix
        //public IConsoleBuilder ConfigureAppConfiguration(Action<ConsoleBuilderContext, IConfiguration> configureAppConfigurationDelegate)
        //{
        //    var __context = _context;
        //    var __configuration = new ConfigurationBuilder()
        //                .SetBasePath(AppContext.BaseDirectory)
        //                .AddJsonFile("appSettings.Inhouse.json", optional: false, reloadOnChange: true)
        //                .Build();

        //    _configureAppConfigurationDelegate += configureAppConfigurationDelegate;

        //    _configureAppConfigurationDelegate.Invoke(__context, __configuration);

        //    _context = __context;
        //    _configuration = __configuration;

        //    _container.ServiceCollection.TryAddSingleton(_ => _configuration);
        //    _container.ServiceProvider = _container.ServiceCollection.BuildServiceProvider();

        //    this.UpdateContext();

        //    return this;
        //}

        //public IConsoleBuilder ConfigureAppConfiguration(Action<IConfiguration> configurationDelegate)
        //{
        //    if (configurationDelegate == null)
        //    {
        //        throw new ArgumentNullException(nameof(configurationDelegate));
        //    }

        //    return ConfigureAppConfiguration((_, configure) => configurationDelegate(configure));
        //}
        #endregion

        public IConsoleBuilder ConfigureServices(Action<ConsoleBuilderContext, IServiceCollection> configureAppConfigureServiceDelegate)
        {
            _configureAppConfigureServiceDelegate += configureAppConfigureServiceDelegate;

            return this;
        }

        public IConsoleBuilder ConfigureServices(Action<IServiceCollection> configureServiceDelegate)
        {
            if (configureServiceDelegate == null)
            {
                throw new ArgumentNullException(nameof(configureServiceDelegate));
            }

            return ConfigureServices((_, services) => configureServiceDelegate(services));
        }

        public string GetSetting(string key)
        {
            return _configuration[key];
        }

        public IConsoleBuilder UseSetting(string key, string? value)
        {
            _configuration[key] = value;
            return this;
        }

        private static string ResolveContentRootPath(string contentRootPath, string basePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                return basePath;
            }

            if (Path.IsPathRooted(contentRootPath))
            {
                return contentRootPath;
            }

            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
        }
    }
}
