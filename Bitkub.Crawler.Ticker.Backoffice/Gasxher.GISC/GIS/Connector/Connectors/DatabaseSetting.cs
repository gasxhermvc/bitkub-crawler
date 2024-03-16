using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector
{
    public class DatabaseSetting
    {
        /// <summary>
        /// Default 60 sec. unit per second
        /// </summary>
        public int ConnectionTimeout { get; set; } = 60;

        /// <summary>
        /// Default 60 sec. unit per millisecond
        /// </summary>
        public int CommandTimeout { get; set; } = 60000;
    }
}
