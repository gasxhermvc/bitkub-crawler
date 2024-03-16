using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.ServiceProviders.Internal
{
    public abstract class IAppServiceProvider
    {
        public abstract void RegisterService(IServiceCollection services);
    }
}
