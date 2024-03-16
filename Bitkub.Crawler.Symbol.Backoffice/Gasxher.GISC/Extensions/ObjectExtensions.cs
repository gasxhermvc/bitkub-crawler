using System;
using Newtonsoft.Json;

namespace GasxherGIS.Extensions
{
    public static class ObjectExtensions
    {
        public static TSource Clone<TSource>(this TSource source)
        {
            return (TSource)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source),typeof(TSource));
        }

        public static TResult Clone<TSource,TResult>(this TSource source)
        {
            return (TResult)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(source),typeof(TResult));
        }
    }
}
