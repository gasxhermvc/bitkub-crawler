using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Platform.Internal
{
    public interface IPlatformRuntime
    {
        bool IsLinux();

        bool IsWindows();

        bool IsOSX();

    }
}
