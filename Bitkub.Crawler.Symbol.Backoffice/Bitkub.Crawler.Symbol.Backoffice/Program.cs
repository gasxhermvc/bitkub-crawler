using GasxherGIS.Application;
using GasxherGIS.Extensions.Application;

namespace Bitkub.Crawler.Symbol.Backoffice
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateAppBuilder(args)
                .Build()
                .Run<CrawlerSymbol>();
        }

        public static IConsoleBuilder CreateAppBuilder(string[] args) =>
           ConsoleApp.CreateDefaultBuilder(args)
               .ConfigureAppConsoleDefaults(consoleBuilder =>
               {
                   //=>Builit-in
                   consoleBuilder.UseStartup<Startup>();
               });
    }
}
