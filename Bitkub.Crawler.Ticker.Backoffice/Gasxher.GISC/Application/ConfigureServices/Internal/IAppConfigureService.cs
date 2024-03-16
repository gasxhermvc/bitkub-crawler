using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.ConfigureServices.Internal
{
    public abstract class IAppConfigureService
    {
        public abstract void ConfigureServices(IServiceProvider provider);
    }
}
