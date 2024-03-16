using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Builder
{
    public class ApplicationBuilderFactory : IApplicationBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationBuilderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IApplicationBuilder CreateBuilder(IFeatureCollection serverFeatures)
        {
            return new ApplicationBuilder(_serviceProvider, serverFeatures);
        }
    }
}
