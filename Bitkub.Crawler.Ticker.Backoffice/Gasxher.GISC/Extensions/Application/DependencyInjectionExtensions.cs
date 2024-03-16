using System;
using System.Linq;
using GasxherGIS.Containers;
using GasxherGIS.Containers.Internal;
using GasxherGIS.Application;

namespace GasxherGIS.Extensions.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IApplicationContainer CreateAppContainer(this IConsoleBuilder consoleBuilder) =>
            CreateAppContainer(consoleBuilder, Environment.GetCommandLineArgs()?.Skip(1)?.ToArray() ?? new string[] { });

        public static IApplicationContainer CreateAppContainer(this IConsoleBuilder consoleBuilder, string[] args) => new ApplicationContainer(consoleBuilder, args);
    }
}
