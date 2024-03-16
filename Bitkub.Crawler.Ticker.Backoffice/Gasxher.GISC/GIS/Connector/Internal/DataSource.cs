using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector.Internal
{
    public class DataSource
    {
        public string ConnectionString { get; set; } = default;

        public string UserId { get; set; } = default;

        public string Password { get; set; } = default;

        public ProviderFactory Provider {get;set;}  = default;
    }
}
