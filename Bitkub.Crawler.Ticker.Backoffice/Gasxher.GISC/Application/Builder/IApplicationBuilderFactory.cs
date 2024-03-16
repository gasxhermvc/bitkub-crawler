using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Builder
{
    public interface IApplicationBuilderFactory
    {
        IApplicationBuilder CreateBuilder(IFeatureCollection serverFeatures);
    }
}
