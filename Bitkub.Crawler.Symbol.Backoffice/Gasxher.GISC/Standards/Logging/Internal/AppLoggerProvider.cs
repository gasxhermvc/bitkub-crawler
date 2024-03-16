using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Logging.Manager;
using GasxherGIS.Standards.Command.Internal;

namespace GasxherGIS.Standards.Logging.Internal
{
    public class AppLoggerProvider : ILoggerProvider, ILogProvider
    {
        private readonly IDisposable _onChangeToken;

        public bool IsEnable { get; private set; }
        public bool IsLogNamespace { get; private set; }
        public bool IsConsole { get; private set; }
        public LogProvider provider { get; }

        private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

        private ILogger logger { get; set; }

        public AppLoggerProvider(LoggerManager logManager)
        {
            provider = logManager.GetLogger("Application");
            IsConsole = provider.Console;
            IsEnable = provider.Enable;
            IsLogNamespace = provider.LogNamespace;
        }

        public ILogger CreateLogger(string categoryName)
        {
            this.logger = new ApplicationLoggerHandler(categoryName, this);
            return _loggers.GetOrAdd(categoryName, name => this.logger);
        }

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }

    }
}
