using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.HttpCommon
{
    public static class HttpCommonExtensions
    {
        public static IServiceCollection AddCommonHttpClient(this IServiceCollection services) => services.AddCommonHttpClient(null);

        public static IServiceCollection AddCommonHttpClient(this IServiceCollection services, Action<HttpClientSetting> httpSettingDelegate)
        {
            var setting = new HttpClientSetting();

            services.AddHttpClient();
            services.TryAddSingleton<HttpClientSetting>(setting);
            services.AddOptions<HttpClientSetting>();
            services.TryAddSingleton<GasxherGIS.GIS.HttpCommon.Internal.IHttpClientFactory, HttpClientFactory>();

            if (httpSettingDelegate != null)
            {
                httpSettingDelegate.Invoke(setting);
            }

            services.Configure<HttpClientSetting>(configureOptions: (Setting) =>
            {
                Setting.BypassCertificateValidation = setting.BypassCertificateValidation;
                Setting.RequestTimeout = setting.RequestTimeout;
                Setting.SecurityProtocol = setting.SecurityProtocol;
            });

            return services;
        }
    }
}
