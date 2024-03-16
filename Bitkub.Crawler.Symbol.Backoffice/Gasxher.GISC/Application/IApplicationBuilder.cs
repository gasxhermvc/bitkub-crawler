using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application
{
    public interface IApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; set; }

        //=>ทำ Pattern: Chain Responsibility เพื่อกรอง Middleware Application
        //IApplicationMiddleware Use
    }
}
