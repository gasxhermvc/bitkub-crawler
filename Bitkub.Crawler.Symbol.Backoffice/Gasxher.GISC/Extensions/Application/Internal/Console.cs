using GasxherGIS.Application;
using GasxherGIS.Application.Abstractions;
using GasxherGIS.Application.Internal;
using GasxherGIS.Standards.Command.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Extensions.Application.Internal
{
    internal sealed partial class Console : IApplicationConsole, IConsole, IDisposable
    {
        private readonly IServiceCollection _applicationServiceCollection;
        private IStartup? _startup;


        private readonly IServiceProvider _appServiceProvider;
        private readonly AppConsoleOptions _options;
        private readonly IConfiguration _configuration;
        private readonly AggregateException? _appStartupErrors;

        private IServiceProvider? _applicationServices;
        private ExceptionDispatchInfo _applicationServicesException;
        private ILogger _logger = NullLogger.Instance;

        internal AppConsoleOptions Options => _options;

        public IServiceProvider Services
        {
            get
            {
                Debug.Assert(_applicationServices != null, "Initialize must be called before accessing services.");

                return _applicationServices;
            }
        }

        public Console(IServiceCollection appServices
            , IServiceProvider appServiceProvider
            , AppConsoleOptions options
            , IConfiguration configuration
            , AggregateException? appStartupErrors)
        {
            if (appServices == null)
            {
                throw new ArgumentNullException(nameof(appServices));
            }

            if (appServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(appServiceProvider));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _configuration = configuration;
            _appStartupErrors = appStartupErrors;
            _options = options;
            _applicationServiceCollection = appServices;
            _appServiceProvider = appServiceProvider;
        }

        public void Initialize()
        {
            try
            {
                EnsureApplicationServices();
            }
            catch (Exception ex)
            {
                if (_applicationServices == null)
                {
                    _applicationServices = _applicationServiceCollection.BuildServiceProvider();
                }

                if (!_options.CaptureStartupErrors)
                {
                    throw;
                }

                _applicationServicesException = ExceptionDispatchInfo.Capture(ex);
            }
        }

        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = _startup.ConfigureServices(_applicationServiceCollection);
            }
        }

        [MemberNotNull(nameof(_startup))]
        private void EnsureStartup()
        {
            if (_startup != null)
            {
                return;
            }

            var startup = _appServiceProvider.GetService<IStartup>();

            if (startup == null)
            {
                throw new InvalidOperationException($"No application configured. Please specify startup via IConsoleBuilder.UseStartup, IConsoleBuilder.Configure, injecting {nameof(IStartup)} or specifying the startup assembly via {nameof(AppConsoleDefaults.StartupAssemblyKey)} in the app console configuration.");
            }

            _startup = startup;
        }

        public override void Main()
        {
            System.Console.WriteLine("{0} -> Application stated...", this.GetType().Name);
        }

        public void Dispose()
        {
        }
    }
}
