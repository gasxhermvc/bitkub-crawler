using System;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Logging.Internal;
using GasxherGIS.Standards.Logging.Manager;
using GasxherGIS.Standards.Logging.Extensions;

namespace GasxherGIS.Standards.Logging
{
    public class DatabaseLoggerHandler<T> : ILogger where T : class
    {
        private readonly DatabaseLoggerProvider _databaseLoggerProvider;
        private readonly ILogger _logger;
        private readonly LogHandler _handler;

        public DatabaseLoggerHandler(DatabaseLoggerProvider databaseLoggerProvider)
        {
            if (databaseLoggerProvider.provider == null)
            {
                //=>ตรวจสอบ Configuration มี Provider Database หรือไม่
                throw new ArgumentNullException(nameof(databaseLoggerProvider.provider));
            }

            _databaseLoggerProvider = databaseLoggerProvider;
            var logger = databaseLoggerProvider.CreateLogger("Database");
            _logger = logger;
            _handler = databaseLoggerProvider.provider.GetHandler();
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string messageTemplate = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")} {_databaseLoggerProvider.provider.Name} %level% {_databaseLoggerProvider.RemoveLogNamespace(typeof(T).FullName)}[{eventId}] {formatter(state, exception)}";

            if (_databaseLoggerProvider.IsConsole)
            {
                Console.WriteLine(messageTemplate.Replace("%level%", logLevel.LogLevelElipsis(logLevel.LogLevelColor())));
            }

            if (_databaseLoggerProvider.IsEnable)
            {
                _handler.Append(messageTemplate.Replace("%level%", logLevel.LogLevelElipsis(logLevel.ToString())));
            }
        }
    }
}
