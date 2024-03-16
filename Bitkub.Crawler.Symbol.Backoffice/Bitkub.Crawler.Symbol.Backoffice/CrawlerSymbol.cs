using Bitkub.Crawler.Symbol.Backoffice.Models.Bitkub;
using Bitkub.Crawler.Symbol.Backoffice.Options;
using GasxherGIS.Application;
using GasxherGIS.GIS.Connector.Internal;
using GasxherGIS.GIS.HttpCommon;
using GasxherGIS.GIS.HttpCommon.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitkub.Crawler.Symbol.Backoffice
{
    public class CrawlerSymbol : IApplicationConsole
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _configuration;

        private readonly IOptions<BitkubApiSetting> _options;

        private readonly ILogger<CrawlerSymbol> _logger;

        private readonly IDbDataAccess _dbDataAccess;

        private BitkubApiSetting Options => _options.Value;

        public CrawlerSymbol(IHttpClientFactory httpClientFactory
            , IConfiguration configuration
            , IOptions<BitkubApiSetting> options
            , ILogger<CrawlerSymbol> logger
            , IDbDataAccess dbDataAccess)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _options = options;
            _logger = logger;
            _dbDataAccess = dbDataAccess;
        }

        public override void Main()
        {
            try
            {
                var symbolUrl = string.Format("{0}{1}", Options.BaseUrl, Options.MarketSymbol);


                var client = _httpClientFactory.Create();
                var response = client.Url(symbolUrl)
                    .Method(HttpMethod.GET)
                    .GetAsync<MarketSymbolModel>()
                    .GetAwaiter()
                    .GetResult();

                var now = DateTime.Now;
                var filesLocalStorage = _configuration.GetValue<string>("FilesLocalStorage");
                var file = System.IO.Path.Combine(filesLocalStorage, now.ToString("yyyyMM"), now.ToString("yyyyMMdd") + ".json");

                var pathFile = System.IO.Path.GetDirectoryName(file);
                if (!System.IO.Directory.Exists(pathFile))
                {
                    System.IO.Directory.CreateDirectory(pathFile);
                }


                if (!System.IO.File.Exists(file))
                {

                    var connect = _dbDataAccess.DbConnection();
                    var colums = this.GetColumns(response.result);
                    var values = this.GetValues(response.result);
                    var SQL = $"REPLACE into symbol {colums} values {values}";
                    var queryResult = connect.ExecuteRawSQLCommand(SQL);

                    if (!queryResult.Success)
                    {
                        _logger.LogError($"Insert Fail, {queryResult.Message}");
                    }

                    System.IO.File.WriteAllText(file,
                        JsonConvert.SerializeObject(response, Formatting.Indented),
                        System.Text.Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception : {ex.ToString()}");
            }
        }

        private string GetColumns(List<Bitkub.Crawler.Symbol.Backoffice.Models.Bitkub.Symbol> symbols)
        {
            var firstItem = symbols.FirstOrDefault();

            if (firstItem == null)
            {
                return string.Empty;
            }

            var names = firstItem.GetType().GetProperties().Select(s => s.Name.Trim()).ToList();

            return names != null && names.Count > 0 ? "(" + string.Join(",", names) + ")" : string.Empty;
        }

        private string GetValues(List<Bitkub.Crawler.Symbol.Backoffice.Models.Bitkub.Symbol> symbols)
        {
            List<string> values = new List<string>();

            symbols.ForEach(f =>
            {
                var names = f.GetType().GetProperties().Select(s => s.GetValue(f)?.ToString() ?? string.Empty).ToList();
                var query = names != null && names.Count > 0 ? string.Join("','", names) : string.Empty;
                if (!string.IsNullOrEmpty(query))
                {
                    query = "('" + query + "')";
                    values.Add(query);
                }
            });

            return values != null && values.Count > 0 ? string.Join(",", values) : string.Empty;
        }
    }
}
