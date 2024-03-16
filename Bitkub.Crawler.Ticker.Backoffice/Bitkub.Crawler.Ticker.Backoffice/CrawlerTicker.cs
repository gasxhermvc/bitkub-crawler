using Bitkub.Crawler.Ticker.Backoffice.Options;
using GasxherGIS.Application;
using GasxherGIS.GIS.HttpCommon.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitkub.Crawler.Ticker.Backoffice
{
    public class CrawlerTicker : IApplicationConsole
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IOptions<BitkubApiSetting> _options;

        private BitkubApiSetting Options => _options.Value;

        public CrawlerTicker(IHttpClientFactory httpClientFactory
            , IOptions<BitkubApiSetting> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
        }

        public override void Main()
        {
            var symbolUrl = string.Format("{0}{1}", Options.BaseUrl, Options.MarketSymbol);
            var tickerUrl = string.Format("{0}{1}", Options.BaseUrl, Options.MarketTicker);

            Console.WriteLine(symbolUrl);
            Console.WriteLine(tickerUrl);
        }
    }
}
