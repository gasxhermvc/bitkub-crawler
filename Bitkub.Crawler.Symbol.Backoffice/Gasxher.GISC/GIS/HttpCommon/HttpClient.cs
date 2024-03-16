using GasxherGIS.GIS.HttpCommon.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.HttpCommon
{
    public class HttpClient : IHttpClient
    {
        private System.Net.Http.HttpClient _client { get; set; }

        public string BaseUrl { get; private set; }

        public HttpMethod HttpMethod { get; private set; }

        public int Timeout { get; private set; }

        public List<HttpHeader> HttpHeaders { get; private set; } = new List<HttpHeader>();

        public List<HttpParameter> HttpParameters { get; private set; } = new List<HttpParameter>();

        public HttpClientSetting Setting { get; private set; }

        public HttpClient(System.Net.Http.HttpClient client, HttpClientSetting setting)
        {
            _client = client;
            Setting = setting;

            if (setting.BypassCertificateValidation)
            {
                System.Net.ServicePointManager.SecurityProtocol |= setting.SecurityProtocol;
                ServicePointManager.SecurityProtocol = setting.SecurityProtocol;
            }

            this.RequestTimeout(Setting.RequestTimeout);

        }

        public IHttpClient Url(string url)
        {
            this.BaseUrl = url;

            return this;
        }

        public IHttpClient Method(HttpMethod httpMethod)
        {
            this.HttpMethod = httpMethod;

            return this;
        }

        public IHttpClient RequestTimeout(int timeout)
        {
            this.Timeout = timeout;

            return this;
        }


        #region Header
        public IHttpClient Header(string headerName, string headerValue)
        {
            this.HttpHeaders.Add(new HttpHeader(headerName, headerValue));

            return this;
        }

        public IHttpClient Header(HttpHeader header)
        {
            this.HttpHeaders.Add(header);

            return this;
        }

        public IHttpClient Headers(Action<List<HttpHeader>> headesDelegate)
        {
            if (headesDelegate == null)
            {
                throw new ArgumentNullException(nameof(headesDelegate));
            }

            headesDelegate.Invoke(this.HttpHeaders);

            return this;
        }
        #endregion

        #region Parameter
        public IHttpClient Parameter(string parameterName, string parameterValue)
        {
            this.HttpParameters.Add(new HttpParameter(parameterName, parameterValue));

            return this;
        }

        public IHttpClient Parameter(HttpParameter parameter)
        {
            this.HttpParameters.Add(parameter);

            return this;
        }

        public IHttpClient Parameters(Action<List<HttpParameter>> parametersDelegate)
        {
            if (parametersDelegate == null)
            {
                throw new ArgumentNullException(nameof(parametersDelegate));
            }

            parametersDelegate.Invoke(this.HttpParameters);

            return this;
        }
        #endregion

        #region Call
        public TResult Get<TResult>()
            where TResult : class
        {
            return _client._Get<TResult>(this);
        }

        public Task<TResult> GetAsync<TResult>()
            where TResult : class
        {
            return _client._GetAsync<TResult>(this);
        }

        public string GetString()
        {
            return _client._GetString(this);
        }

        public Task<string> GetAsyncString()
        {
            return _client._GetAsyncString(this);
        }
        #endregion

        public string TemporaryURL { get; set; }

        public List<HttpHeader> TemporaryHttpHeaders { get; set; } = new List<HttpHeader>();

        public List<HttpParameter> TemporaryHttpParameters { get; set; } = new List<HttpParameter>();

        public HttpContent TemporaryContent { get; set; }
    }
}
