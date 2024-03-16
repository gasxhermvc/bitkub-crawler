using System.Collections.Generic;


namespace GasxherGIS.Standards.Logging.Manager
{
    public class LoggingProviders
    {
        public List<LogProvider> Providers { get; set; }
    }

    public class LogProvider : LogTemplateProvider
    {
        private LogHandler handler { get; set; }

        public LogTemplateProvider template { get; set; }

        public void SetHandler(LogHandler handler)
        {
            this.handler = handler;
        }

        public LogHandler GetHandler() => this.handler;
    }

    public class LogTemplateProvider
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public bool Running { get; set; }

        public bool LogNamespace { get; set; }

        public bool Enable { get; set; }

        public bool Console { get; set; }
    }
}
