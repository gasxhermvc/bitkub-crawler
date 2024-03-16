using GasxherGIS.GIS.HttpCommon.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Http = System.Net.Http;

namespace GasxherGIS.GIS.HttpCommon
{
    public static class HttpClientExtensions
    {
        public static TResult _Get<TResult>(this Http.HttpClient httpClient, IHttpClient instance) where TResult : class
        {
            httpClient.SetupRequest(instance);

            return instance.HttpMethod switch
            {
                HttpMethod.GET => httpClient.GetAsync(instance.TemporaryURL)
                        .GetAwaiter()
                        .GetResult()
                        .ToObject<TResult>(),
                HttpMethod.POST => httpClient.PostAsync(instance.TemporaryURL, instance.TemporaryHttpParameters.BuildContent())
                        .GetAwaiter()
                        .GetResult()
                        .ToObject<TResult>(),
                HttpMethod.PATCH => default(TResult),
                HttpMethod.PUT => default(TResult),
                HttpMethod.DELETE => default(TResult),
                HttpMethod.OPTIONS => default(TResult),
                _ => throw new InvalidOperationException($"ไม่รองรับ Http Method : {instance.HttpMethod}")
            };
        }

        public static Task<TResult> _GetAsync<TResult>(this Http.HttpClient httpClient, IHttpClient instance)
            where TResult : class
        {
            httpClient.SetupRequest(instance);

            return instance.HttpMethod switch
            {
                HttpMethod.GET => Task.FromResult(
                    httpClient.GetAsync(instance.TemporaryURL)
                        .GetAwaiter()
                        .GetResult()
                        .ToObject<TResult>()),
                HttpMethod.POST => Task.FromResult(
                    httpClient.PostAsync(instance.TemporaryURL, instance.TemporaryHttpParameters.BuildContent())
                        .GetAwaiter()
                        .GetResult()
                        .ToObject<TResult>()),
                HttpMethod.PATCH => Task.FromResult(default(TResult)),
                HttpMethod.PUT => Task.FromResult(default(TResult)),
                HttpMethod.DELETE => Task.FromResult(default(TResult)),
                HttpMethod.OPTIONS => Task.FromResult(default(TResult)),
                _ => throw new InvalidOperationException($"ไม่รองรับ Http Method : {instance.HttpMethod}")
            };
        }

        public static Task<string> _GetAsyncString(this Http.HttpClient httpClient, IHttpClient instance)
        {
            httpClient.SetupRequest(instance);

            return instance.HttpMethod switch
            {
                HttpMethod.GET => Task.FromResult(
                    httpClient.GetStringAsync(instance.TemporaryURL)
                        .GetAwaiter()
                        .GetResult()),
                HttpMethod.POST => Task.FromResult(
                    httpClient.PostAsync(instance.TemporaryURL, instance.TemporaryHttpParameters.BuildContent())
                        .GetAwaiter()
                        .GetResult()
                        .Content
                        .ReadAsStringAsync()
                        .GetAwaiter()
                        .GetResult()),
                HttpMethod.PATCH => Task.FromResult(string.Empty),
                HttpMethod.PUT => Task.FromResult(string.Empty),
                HttpMethod.DELETE => Task.FromResult(string.Empty),
                HttpMethod.OPTIONS => Task.FromResult(string.Empty),
                _ => throw new InvalidOperationException($"ไม่รองรับ Http Method : {instance.HttpMethod}")
            };
        }

        public static string _GetString(this Http.HttpClient httpClient, IHttpClient instance)
        {
            httpClient.SetupRequest(instance);

            return instance.HttpMethod switch
            {
                HttpMethod.GET => httpClient.GetStringAsync(instance.TemporaryURL)
                    .GetAwaiter()
                    .GetResult(),
                HttpMethod.POST => httpClient.PostAsync(instance.TemporaryURL, instance.TemporaryHttpParameters.BuildContent())
                    .GetAwaiter()
                    .GetResult()
                    .Content
                    .ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult(),
                HttpMethod.PATCH => string.Empty,
                HttpMethod.PUT => string.Empty,
                HttpMethod.DELETE => string.Empty,
                HttpMethod.OPTIONS => string.Empty,
                _ => throw new InvalidOperationException($"ไม่รองรับ Http Method : {instance.HttpMethod}")
            };
        }

        #region Support
        internal static void SetupRequest(this Http.HttpClient httpClient, IHttpClient instance)
        {
            string[] urlSpliting = instance.BaseUrl.Trim('/').Split('?');
            string url = instance.BaseUrl.Trim('/');
            string temporary = url;

            if (instance.TemporaryHttpHeaders == null)
            {
                instance.TemporaryHttpHeaders = new List<HttpHeader>();
            }

            if (instance.TemporaryHttpParameters == null)
            {
                instance.TemporaryHttpParameters = new List<HttpParameter>();
            }

            instance.TemporaryURL = string.Empty;
            instance.TemporaryHttpHeaders.Clear();
            instance.TemporaryHttpParameters.Clear();

            switch (instance.HttpMethod)
            {
                case HttpMethod.GET:
                    if (url.Contains("?"))
                    {
                        //=>Get Base URL
                        url = urlSpliting[0];

                        //=>Parser Url Parameter to List<HttpParameter>
                        var parameters = instance.BaseUrl.UrlParamToDictionary().Select(s => new HttpParameter
                        {
                            Name = s.Key,
                            Value = s.Value?.ToString() ?? string.Empty
                        }).ToList();

                        //=>Mixin UrlParameter [a from url, b from url] + [c from parameters] = [a,b,c]
                        parameters = MixinParams(parameters, instance.HttpParameters);

                        if (parameters != null && parameters.Count > 0)
                        {
                            temporary = url + "?" + parameters.ToQueryString();
                        }

                        instance.TemporaryURL = temporary;
                    }
                    else
                    {
                        instance.TemporaryURL = url;
                    }
                    break;
                case HttpMethod.POST:
                    instance.TemporaryURL = temporary;
                    break;
                case HttpMethod.PATCH:
                    instance.TemporaryURL = temporary;
                    break;
                case HttpMethod.PUT:
                    instance.TemporaryURL = temporary;
                    break;
                case HttpMethod.DELETE:
                    instance.TemporaryURL = temporary;
                    break;
                case HttpMethod.OPTIONS:
                    instance.TemporaryURL = temporary;
                    break;
                default:
                    throw new InvalidOperationException("รูปแบบ Http Method ไม่ถูกต้อง");
            }

            if (instance.HttpHeaders != null && instance.HttpHeaders.Count > 0)
            {
                httpClient.DefaultRequestHeaders.Clear();

                foreach (var header in instance.HttpHeaders)
                {
                    try
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Name, header.Value);
                    }
                    catch
                    {
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Name, header.Value);
                    }
                }
            }

            httpClient.Timeout = TimeSpan.FromSeconds(instance.Timeout);
            instance.TemporaryHttpHeaders = instance.HttpHeaders.ToList();
            instance.TemporaryHttpParameters = instance.HttpParameters.ToList();
        }

        public static List<HttpHeader> Set(this List<HttpHeader> Headers, string headerName, string headerValue)
        {
            Headers.Add(new HttpHeader(headerName, headerValue));

            return Headers;
        }

        public static List<HttpParameter> Set(this List<HttpParameter> Parameters, string parameterName, string parameterValue)
        {
            Parameters.Add(new HttpParameter(parameterName, parameterValue));

            return Parameters;
        }

        internal static HttpContent BuildContent(this List<HttpParameter> parameters)
        {
            var dict = parameters.ToDictionary(k => k.Name, v => v.Value);
            HttpContent contents = new FormUrlEncodedContent(dict);

            return contents;
        }

        internal static object ToObject(this HttpResponseMessage response, Type convertType)
        {
            string result = null;

            result = response.Content.ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult()
                    .Trim();

            if (response.IsSuccessStatusCode)
            {

                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject(result, convertType);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Cannot convert to type {convertType.FullName}, {ex.ToString()}");
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new InvalidOperationException($"Http status code is {response.StatusCode}, {result}");
            }
        }

        internal static TResult ToObject<TResult>(this HttpResponseMessage response)
            where TResult : class => (TResult)response.ToObject(typeof(TResult));

        internal static Dictionary<string, object> UrlParamToDictionary(this string url)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            var splitUrl = url.Split('?')[1];
            var splitParams = splitUrl.Split('&').Where(w => !string.IsNullOrEmpty(w)).ToList();

            splitParams.ForEach(f =>
            {
                var keyPair = f.Split('=');

                if (keyPair != null && keyPair.Length > 0)
                {
                    parameters.Add(keyPair[0], keyPair[1] ?? string.Empty);
                }
            });

            return parameters;
        }

        internal static List<HttpParameter> MixinParams(List<HttpParameter> parameters, List<HttpParameter> defaultParameters)
        {
            defaultParameters.ForEach(f =>
            {
                var find = parameters.FirstOrDefault(ff => ff.Name == f.Name);

                if (find != null)
                {
                    f.Value = find.Value;
                }
            });

            return defaultParameters;
        }

        internal static string ToQueryString(this List<HttpParameter> model)
        {
            var result = model.Select((kvp) => kvp.Name.ToString() + "=" + Uri.EscapeDataString(kvp.Value)).Aggregate((p1, p2) => p1 + "&" + p2);
            return result;
        }
        #endregion
    }
}
