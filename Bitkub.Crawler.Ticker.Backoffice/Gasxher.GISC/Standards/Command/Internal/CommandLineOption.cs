using CommandLine;

namespace GasxherGIS.Standards.Command.Internal
{
    //=>Default
    public class CommandLineOption
    {
        [Option('d', "debug", Required = false, HelpText = "Debug mode.", Default = false)]
        public bool Debug { get; set; }

        [Option('l', "logging", Required = false, HelpText = "Use Logging.", Default = false)]
        public bool LogKeeper { get; set; }
    }
}
