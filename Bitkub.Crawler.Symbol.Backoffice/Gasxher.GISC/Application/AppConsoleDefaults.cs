using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application
{
    public static class AppConsoleDefaults
    {
        public static readonly string ApplicationKey = "Application:Name";
        public static readonly string StartupAssemblyKey = "startupAssembly";
        public static readonly string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";
        public static readonly string LoggingKey = "Logging";
        public static readonly string LoggingProviderKey = "Logging:Providers";
        public static readonly string PreventAppStartupKey = "preventAppStartup";
        public static readonly string CaptureStartupErrorsKey = "captureStartupErrors";

        /// <summary>
        /// The configuration key associated with the "webRoot" configuration.
        /// </summary>
        public static readonly string AppRootKey = "approot";

        /// <summary>
        /// The configuration key associated with the "ContentRoot" configuration.
        /// </summary>
        public static readonly string ContentRootKey = "contentRoot";


        /// <summary>
        /// The configuration key associated with "hostingStartupAssemblies" configuration.
        /// </summary>
        public static readonly string AppStartupAssembliesKey = "appStartupAssemblies";

        /// <summary>
        /// The configuration key associated with the "hostingStartupExcludeAssemblies" configuration.
        /// </summary>
        public static readonly string AppStartupExcludeAssembliesKey = "appStartupExcludeAssemblies";
    }
}
