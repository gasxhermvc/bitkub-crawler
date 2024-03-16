using GasxherGIS.Standards.Logging.Manager;

namespace GasxherGIS.Standards.Logging.Internal
{
    public interface ILogProvider
    {
        bool IsEnable { get; }

        bool IsConsole { get; }

        bool IsLogNamespace { get; }

        LogProvider provider { get; }
    }
}
