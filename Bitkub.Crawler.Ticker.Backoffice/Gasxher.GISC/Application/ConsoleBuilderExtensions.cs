using GasxherGIS.Application.Abstractions;
using GasxherGIS.Application.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GasxherGIS.Standards.Environment;
using GasxherGIS.Application.Startup;

namespace GasxherGIS.Application
{
    public static class ConsoleBuilderExtensions
    {
        public static IConsoleBuilder UseStartup(this IConsoleBuilder consoleBuilder, [DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType)
        {
            if (startupType == null)
            {
                throw new ArgumentNullException(nameof(startupType));
            }

            var startupAssemblyName = startupType.Assembly.GetName().Name;

            consoleBuilder.UseSetting(AppConsoleDefaults.ApplicationKey, startupAssemblyName);

            return consoleBuilder.ConfigureServices(services =>
            {
                if (typeof(IStartup).IsAssignableFrom(startupType))
                {
                    services.AddSingleton(typeof(IStartup), startupType);
                }
                else
                {
                    services.AddSingleton(typeof(IStartup), sp =>
                    {
                        var appEnvironment = sp.GetRequiredService<IAppEnvironment>();

                        return new ConventionBasedStartup(StartupLoader.LoadMethods(sp, startupType, appEnvironment.GetEnvironment()));
                    });
                }
            });
        }

        public static IConsoleBuilder UseStartup<[DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)]TSartup>(this IConsoleBuilder consoleBuilder) where TSartup : class
        {
            return consoleBuilder.UseStartup(typeof(TSartup));
        }
    }
}
