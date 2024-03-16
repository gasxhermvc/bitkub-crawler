using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector.Internal
{
    public class DbConfigProvider
    {
        public string DataSourceParameter { get; set; } = default;

        public string ProcedureParameter { get; set; } = default;

        public string NotiOutputParameter { get; set; } = default;

        public string UserIdProcedureParameter { get; set; } = default;

        public string DefaultDataSource { get; set; } = default;

        /// <summary>
        /// Default 60 sec. unit per second
        /// </summary>
        public int ConnectionTimeout { get; set; } = 60;

        /// <summary>
        /// Default 60 sec. unit per millisecond
        /// </summary>
        public int CommandTimeout { get; set; } = 60000;

        public Dictionary<string, DataSource> DataSource { get; set; }
    }
}
