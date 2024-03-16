using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.HttpCommon
{
    public class HttpClientFactory : GasxherGIS.GIS.HttpCommon.Internal.IHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClient;

        private readonly IOptions<HttpClientSetting> _options;

        public HttpClientSetting Options { get => _options?.Value ?? throw new NullReferenceException(nameof(_options)); }

        public HttpClientFactory(IHttpClientFactory httpClient,
            IOptions<HttpClientSetting> options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public GasxherGIS.GIS.HttpCommon.Internal.IHttpClient Create()
        {
            return new HttpClient(_httpClient.CreateClient(), Options);
        }
    }
}
