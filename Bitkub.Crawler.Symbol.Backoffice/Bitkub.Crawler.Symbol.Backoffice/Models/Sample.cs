using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bitkub.Crawler.Symbol.Backoffice.Models
{
    public class Sample
    {
        [JsonProperty("Fisrt_Name")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LAST_NAME { get; set; }

        [JsonProperty("age")]
        public string AGE { get; set; }

        [JsonProperty("prov_code")]
        public string PROV_CODE { get; set; }
    }
}
