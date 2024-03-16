using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Environment;
using Microsoft.Extensions.Configuration;

namespace GasxherGIS.Application
{
    public static class AppConsoleAbstractionsExtensions
    {
        internal static void LaunchApplication(this Type consoleType, IConsole console, Action<IConsole, Type> actionDelegate)
        {
            if (actionDelegate == null)
            {
                throw new ArgumentNullException(nameof(actionDelegate));
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //=>Generic Get LoggerProvider from Generic Type
            var consoleTypeParameters = new Type[] { consoleType };
            var loggerType = typeof(ILogger<>).MakeGenericType(consoleTypeParameters);

            //=>GetService ILogger from Generic type
            ILogger logging = (ILogger)console.Services.GetService(loggerType);

            //=>Get Config and Env
            var Configuration = console.Services.GetService<IConfiguration>();
            var Env = console.Services.GetService<IAppEnvironment>();

            //=>Log begin
            logging.LogInformation("Application start...");
            logging.LogInformation("Application name : {0}", Configuration[AppConsoleDefaults.ApplicationKey]);
            logging.LogInformation("Environment name : {0}", Env.GetEnvironment());

            //=>App run
            actionDelegate.Invoke(console, consoleType);

            //=>Log end begin
            logging.LogInformation("Duration time : {0} ms.", sw.Elapsed.ToString());
            logging.LogInformation("Done.");

            sw.Stop();
        }
        /// <summary>
        /// IConsole = Application, IConsole != Console
        /// </summary>
        /// <param name="console"></param>
        internal static void Run(this IConsole console)
        {
            IApplicationConsole app = console as IApplicationConsole;

            app.Main();
        }

        public static void Run(this IConsole console, Type consoleType) =>
            consoleType.LaunchApplication(console, (IConsole app, Type t) =>
               Run((IConsole)ActivatorUtilities.CreateInstance(console.Services, t)));

        public static void Run<TAppConsole>(this IConsole console) =>
            typeof(TAppConsole).LaunchApplication(console, (IConsole app, Type t) =>
                Run((IConsole)ActivatorUtilities.CreateInstance(console.Services, t)));


        /// <summary>
        /// สั่งทำงาน Application Flow แบบใช้สร้าง Class ควบคุม
        /// เพื่อใช้งานคุณสมบัติของ Injection ได้มากขึ้น
        /// </summary>
        /// <typeparam name="TAppConsole"></typeparam>
        /// <param name="console"></param>
        /// <param name="appFlowBuilder"></param>
        public static void ControlFlow<TAppConsole>(this IConsole console)
        {

        }

        /// <summary>
        /// สั่งทำงาน Application Flow แบบ Built-in
        /// </summary>
        /// <typeparam name="TAppConsole"></typeparam>
        /// <param name="console"></param>
        /// <param name="appFlowBuilder"></param>
        public static void ControlFlow(this IConsole console, Action<ApplicationFlowBuilder> appFlowBuilder)
        {
            if (appFlowBuilder == null)
            {
                throw new ArgumentNullException(nameof(appFlowBuilder));
            }

            typeof(ApplicationFlowBuilder).LaunchApplication(console, (IConsole app, Type t) =>
            {
                var appFlow = (ApplicationFlowBuilder)ActivatorUtilities.CreateInstance(console.Services, t);
                appFlowBuilder.Invoke(appFlow);
            });
        }
    }
}
