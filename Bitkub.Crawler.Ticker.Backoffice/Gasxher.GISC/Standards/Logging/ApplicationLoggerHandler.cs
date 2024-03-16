using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Logging.Internal;
using GasxherGIS.Standards.Logging.Manager;
using GasxherGIS.Standards.Logging.Extensions;

namespace GasxherGIS.Standards.Logging
{
    public class ApplicationLoggerHandler : ILogger
    {
        private readonly string _name;
        private readonly ILogProvider _appLoggerProvider;
        private readonly LogHandler _handler;

        public ApplicationLoggerHandler(string name, ILogProvider loggerProvider)
        {
            if (loggerProvider.provider == null)
            {
                //=>ตรวจสอบ Configuration มี Provider Application หรือไม่
                throw new ArgumentNullException(nameof(loggerProvider.provider));
            }
            _name = name;
            _appLoggerProvider = loggerProvider;
            _handler = loggerProvider.provider.GetHandler();
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string messageTemplate = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")} {_appLoggerProvider.provider.Name} %level% {_appLoggerProvider.RemoveLogNamespace(_name)}[{eventId}] {formatter(state, exception)}";

            if (_appLoggerProvider.IsConsole)
            {
                Console.WriteLine(messageTemplate.Replace("%level%", logLevel.LogLevelElipsis(logLevel.LogLevelColor())));
            }

            if (_appLoggerProvider.IsEnable)
            {
                _handler.Append(messageTemplate.Replace("%level%", logLevel.LogLevelElipsis(logLevel.ToString())));
            }
        }
    }
}
