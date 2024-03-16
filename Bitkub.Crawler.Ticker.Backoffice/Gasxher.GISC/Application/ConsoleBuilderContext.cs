using System;
using Microsoft.Extensions.Configuration;
using GasxherGIS.Standards.Environment;

namespace GasxherGIS.Application
{
    public class ConsoleBuilderContext
    {
        /// <summary>
        /// The <see cref="Arguments"/> initailized by the <see cref="IConsoleBuilder"/>
        /// </summary>
        public string[] Arguments { get; set; } = new string[] { };

        /// <summary>
        /// The <see cref="IServiceProvider"/> initailized by the <see cref="IConsoleBuilder"/>
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; } = default;

        /// <summary>
        /// The <see cref="IAppEnvironment"/> initailized by the <see cref="IConsoleBuilder"/>
        /// </summary>
        public IAppEnvironment AppEnvironment { get; set; } = default!;


        /// <summary>
        /// The <see cref="IConfiguration"/> containing the merged configuration of the application and the <see cref="IConsoleBuilder"/>
        /// </summary>
        public IConfiguration Configuration { get; set; } = default!;
    }
}
