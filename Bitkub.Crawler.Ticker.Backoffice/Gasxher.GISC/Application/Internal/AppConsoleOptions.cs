using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace GasxherGIS.Application.Internal
{
    public class AppConsoleOptions
    {
        public AppConsoleOptions() { }

        public AppConsoleOptions(IConfiguration configuration)
            : this(configuration, string.Empty) { }

        public AppConsoleOptions(IConfiguration configuration, string applicationNameFallback)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ApplicationName = configuration[AppConsoleDefaults.ApplicationKey] ?? applicationNameFallback;
            StartupAssembly = configuration[AppConsoleDefaults.StartupAssemblyKey];
            PreventAppStartup = configuration.GetValue<bool>(AppConsoleDefaults.PreventAppStartupKey);

            CaptureStartupErrors = configuration.GetValue<bool>(AppConsoleDefaults.CaptureStartupErrorsKey);

            Environment = configuration[AppConsoleDefaults.EnvironmentKey];
            AppRoot = configuration[AppConsoleDefaults.AppRootKey];
            ContentRootPath = configuration[AppConsoleDefaults.ContentRootKey];


            // Search the primary assembly and configured assemblies.
            AppStartupAssemblies = Split($"{ApplicationName};{configuration[AppConsoleDefaults.AppStartupAssembliesKey]}");
            AppStartupExcludeAssemblies = Split(configuration[AppConsoleDefaults.AppStartupExcludeAssembliesKey]);

        }

        public string ApplicationName { get; set; }

        public bool PreventAppStartup { get; set; }

        public bool CaptureStartupErrors { get; set; }

        public string Environment { get; set; }

        public string StartupAssembly { get; set; }

        public string AppRoot { get; set; }

        public string ContentRootPath { get; set; }

        public IReadOnlyList<string> AppStartupAssemblies { get; set; }

        public IReadOnlyList<string> AppStartupExcludeAssemblies { get; set; }

        public IEnumerable<string> GetFinalAppStartupAssemblies()
        {
            return AppStartupAssemblies.Except(AppStartupExcludeAssemblies, StringComparer.OrdinalIgnoreCase);
        }
        private static IReadOnlyList<string> Split(string value)
        {
            return value?.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();
        }
    }
}
