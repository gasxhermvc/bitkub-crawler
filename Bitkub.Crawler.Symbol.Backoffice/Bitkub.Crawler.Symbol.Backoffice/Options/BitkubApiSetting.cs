using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitkub.Crawler.Symbol.Backoffice.Options
{
    public class BitkubApiSetting
    {
        [JsonProperty("BASE_URL")]
        public string BaseUrl { get; set; }

        [JsonProperty("MARKET_SYMBOL")]
        public string MarketSymbol { get; set; }

        [JsonProperty("MARKET_TICKER")]
        public string MarketTicker { get; set; }

        [JsonProperty("MARTKET_TRADE")]
        public string MarketTrade { get; set; }

        [JsonProperty("MARKET_BID")]
        public string MarketBid { get; set; }

        [JsonProperty("MARKET_ASK")]
        public string MarketAsk { get; set; }
    }
}
