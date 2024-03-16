using System;
using GasxherGIS.Application;

namespace GasxherGIS.Extensions.Application
{
    public static class GenericConsoleBuilderExtensions
    {
        public static IConsoleBuilder ConfigureAppConsoleDefaults(this IConsoleBuilder builder, Action<IConsoleBuilder> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure.Invoke(builder);

            return builder;
        }
    }
}
