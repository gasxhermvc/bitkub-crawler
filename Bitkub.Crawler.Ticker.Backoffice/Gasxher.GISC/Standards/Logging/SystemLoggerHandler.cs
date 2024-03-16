using System;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Logging.Internal;
using GasxherGIS.Standards.Logging.Manager;
using GasxherGIS.Standards.Logging.Extensions;

namespace GasxherGIS.Standards.Logging
{
    public class SystemLoggerHandler<T> : ILogger where T : class
    {
        private readonly SystemLoggerProvider _systemLoggerProvider;
        private readonly ILogger _logger;
        private readonly LogHandler _handler;

        public SystemLoggerHandler(SystemLoggerProvider systemLoggerProvider)
        {
            if (systemLoggerProvider.provider == null)
            {
                //=>ตรวจสอบ Configuration มี Provider System หรือไม่
                throw new ArgumentNullException(nameof(systemLoggerProvider.provider));
            }

            _systemLoggerProvider = systemLoggerProvider;
            var logger = systemLoggerProvider.CreateLogger("SystemLoggerHandler");
            _logger = logger;
            _handler = systemLoggerProvider.provider.GetHandler();
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string messageTemplate = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")} {_systemLoggerProvider.provider.Name} %level% {_systemLoggerProvider.RemoveLogNamespace(typeof(T).FullName)}[{eventId}] {formatter(state, exception)}";

            if (_systemLoggerProvider.IsConsole)
            {
                Console.WriteLine(messageTemplate.Replace("%level%", logLevel.LogLevelElipsis(logLevel.LogLevelColor())));
            }

            if (_systemLoggerProvider.IsEnable)
            {
                _handler.Append(messageTemplate.Replace("%level%", logLevel.LogLevelElipsis(logLevel.ToString())));
            }
        }
    }
}
