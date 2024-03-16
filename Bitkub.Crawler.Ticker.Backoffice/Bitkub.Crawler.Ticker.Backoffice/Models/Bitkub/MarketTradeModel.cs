using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitkub.Crawler.Ticker.Backoffice.Models.Bitkub
{
   public class MarketTradeModel
    {
        [JsonProperty("error")]
        public int error { get; set; }
        [JsonProperty("result")]
        public List<dynamic> result { get; set; }
    }
}
