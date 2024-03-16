using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitkub.Crawler.Symbol.Backoffice.Models.Bitkub
{
    public class MarketTickerModel
    {
        public List<Dictionary<string, Ticker>> tickers { get; set; }
    }

    public class Ticker
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("last")]
        public string last { get; set; }
        [JsonProperty("lowestAsk")]
        public decimal lowestAsk { get; set; }
        [JsonProperty("highestBid")]
        public decimal highestBid { get; set; }
        [JsonProperty("percentChange")]
        public double percentChange { get; set; }
        [JsonProperty("baseVolume")]
        public decimal baseVolume { get; set; }
        [JsonProperty("quoteVolume")]
        public decimal quoteVolume { get; set; }
        [JsonProperty("isFrozen")]
        public byte isFrozen { get; set; }
        [JsonProperty("high24hr")]
        public decimal high24hr { get; set; }
        [JsonProperty("low24hr")]
        public decimal low24hr { get; set; }
    }
}
