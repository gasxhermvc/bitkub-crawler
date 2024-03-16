using GasxherGIS.GIS.HttpCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.HttpCommon.Internal
{
    public interface IHttpClient
    {
        string BaseUrl { get; }

        string TemporaryURL { get; set; }

        List<HttpHeader> TemporaryHttpHeaders { get; set; }

        List<HttpParameter> TemporaryHttpParameters { get; set; }

        HttpContent TemporaryContent { get; set; }

        HttpMethod HttpMethod { get; }

        int Timeout { get; }

        List<HttpHeader> HttpHeaders { get; }

        List<HttpParameter> HttpParameters { get; }

        HttpClientSetting Setting { get; }

        IHttpClient Url(string url);

        IHttpClient Method(HttpMethod httpMethod);

        IHttpClient RequestTimeout(int timeout);


        #region Header
        IHttpClient Header(string headerName, string headerValue);

        IHttpClient Header(HttpHeader header);

        IHttpClient Headers(Action<List<HttpHeader>> headesDelegate);
        #endregion

        #region Parameter
        IHttpClient Parameter(string parameterName, string parameterValue);

        IHttpClient Parameter(HttpParameter parameter);

        IHttpClient Parameters(Action<List<HttpParameter>> parametersDelegate);
        #endregion

        #region Call
        TResult Get<TResult>() where TResult : class;

        Task<TResult> GetAsync<TResult>() where TResult : class;

        string GetString();

        Task<string> GetAsyncString();
        #endregion


        #region Send data
        //IHttpClient Upload();

        //IHttpClient Download();
        #endregion
    }
}
