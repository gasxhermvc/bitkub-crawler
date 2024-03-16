using Bitkub.Crawler.Ticker.Backoffice.Options;
using GasxherGIS.Application;
using GasxherGIS.Standards.Command.Internal;
using GasxherGIS.Standards.Environment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Bitkub.Crawler.Ticker.Backoffice
{
    public class TemplateApplication : IApplicationConsole
    {
        private readonly ILogger<TemplateApplication> _logger;

        private readonly IConfiguration _configuration;

        private readonly IOptions<TemplateSetting> _options;

        private readonly ICommandParser _commandParser;

        private TemplateSetting Options { get => _options.Value; }

        public TemplateApplication(ILogger<TemplateApplication> logger
            , IConfiguration configuration
            , IOptions<TemplateSetting> options
            , ICommandParser commandParser)
        {
            _logger = logger;
            _configuration = configuration;
            _options = options;
            _commandParser = commandParser;
        }

        public override void Main()
        {
            _logger.LogInformation($"Output --> {Options.Name}");
            try
            {
                var num = _commandParser.GetValue<double>("Data");
                _logger.LogInformation($"Pass Data --> {num}");
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.ToString()}");
            }
        }
    }
}