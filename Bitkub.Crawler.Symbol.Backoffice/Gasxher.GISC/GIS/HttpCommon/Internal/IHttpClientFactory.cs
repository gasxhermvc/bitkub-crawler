using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.HttpCommon.Internal
{
    public interface IHttpClientFactory
    {
        IHttpClient Create();
    }
}
