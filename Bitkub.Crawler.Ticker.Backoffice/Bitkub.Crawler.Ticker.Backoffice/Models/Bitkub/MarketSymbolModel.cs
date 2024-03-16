using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitkub.Crawler.Ticker.Backoffice.Models.Bitkub
{
    public class MarketSymbolModel
    {
        [JsonProperty("error")]
        public int error { get; set; }
        [JsonProperty("result")]
        public List<Symbol> result { get; set; }
    }

    public class Symbol
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("symbol")]
        public string symbol { get; set; }
        [JsonProperty("info")]
        public string info { get; set; }
    }
}
