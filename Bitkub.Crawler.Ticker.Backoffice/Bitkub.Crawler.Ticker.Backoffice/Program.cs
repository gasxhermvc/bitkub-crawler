using GasxherGIS.Application;
using GasxherGIS.Extensions.Application;

namespace Bitkub.Crawler.Ticker.Backoffice
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateAppBuilder(args)
                .Build()
                .Run<CrawlerTicker>();
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
