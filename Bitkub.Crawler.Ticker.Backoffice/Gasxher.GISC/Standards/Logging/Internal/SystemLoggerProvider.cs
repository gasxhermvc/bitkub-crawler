using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Command.Internal;
using GasxherGIS.Standards.Logging.Manager;

namespace GasxherGIS.Standards.Logging.Internal
{
    public class SystemLoggerProvider : ILoggerProvider, ILogProvider
    {
        private readonly IDisposable _onChangeToken;

        public bool IsEnable { get; private set; }
        public bool IsLogNamespace { get; private set; }
        public bool IsConsole { get; private set; }
        public LogProvider provider { get; }

        private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

        private ILogger logger { get; set; }

        public SystemLoggerProvider(LoggerManager logManager)
        {
            provider = logManager.GetLogger("System");
            IsLogNamespace = provider.LogNamespace;
            IsEnable = provider.Enable;
            IsConsole = provider.Console;
        }

        public ILogger CreateLogger(string categoryName)
        {
            this.logger = new LoggerHandler(categoryName, this);
            return _loggers.GetOrAdd(categoryName, name => this.logger);
        }

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
