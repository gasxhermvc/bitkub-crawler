using GasxherGIS.Application;
using GasxherGIS.GIS.Connector;
using GasxherGIS.GIS.HttpCommon;
using Bitkub.Crawler.Symbol.Backoffice.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bitkub.Crawler.Symbol.Backoffice
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //=>Inection GIS Helper
            services.AddDbDataAccess(config =>
            {
                config.ConnectionTimeout = 1200;
                config.CommandTimeout = 1200000;
            });

            services.AddCommonHttpClient(config =>
            {
                config.BypassCertificateValidation = true;
                config.RequestTimeout = 300; //=>5 min
                config.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            });

            services.AddSingleton<TemplateSetting>();
            services.Configure<TemplateSetting>(options =>
                Configuration.GetSection("Template").Bind(options));

            services.AddSingleton<BitkubApiSetting>();
            services.Configure<BitkubApiSetting>(options =>
                Configuration.GetSection("BitkubApi").Bind(options));
        }

        public void Configure(IApplicationBuilder app)
        {

        }
    }
}
