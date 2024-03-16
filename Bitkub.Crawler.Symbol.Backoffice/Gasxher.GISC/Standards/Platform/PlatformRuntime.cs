using System.Runtime.InteropServices;
using GasxherGIS.Standards.Platform.Internal;

namespace GasxherGIS.Standards.Platform
{
    public class PlatformRuntime : IPlatformRuntime
    {
        public bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows); ;
        }

        public bool IsOSX()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX); ;
        }
    }
}
