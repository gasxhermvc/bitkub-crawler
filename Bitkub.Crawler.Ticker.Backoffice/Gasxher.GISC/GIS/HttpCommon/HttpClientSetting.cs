using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Security;

namespace GasxherGIS.GIS.HttpCommon
{
    public class HttpClientSetting
    {
        public int RequestTimeout { get; set; } = 60;

        public bool BypassCertificateValidation { get; set; }

        public System.Net.SecurityProtocolType SecurityProtocol { get; set; }
    }
}
