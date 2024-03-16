using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitkub.Crawler.Symbol.Backoffice.Extensions
{
    public static class MixinObject
    {
        public static object Mixin<TSource>(this TSource source, params object[] args) where TSource : class
        {

            JObject jsonObj = JObject.FromObject(source);

            if (args != null && args.Length > 0)
            {
                foreach (var arg in args)
                {
                    jsonObj.Merge(JObject.FromObject(arg));
                }
            }

            return jsonObj.ToObject<object>();
        }

        public static TResult CopyTo<TSource, TResult>(this TSource source)
            where TSource : class
            where TResult : class
        {
            object result = Activator.CreateInstance(typeof(TResult));
            var resultType = result.GetType();
            var resultProps = resultType.GetProperties();

            if (source == null)
            {
                return default(TResult);
            }

            var sourceType = source.GetType();
            var sourceProps = sourceType.GetProperties();

            if (sourceProps == null)
            {
                return default(TResult);
            }

            foreach (var prop in sourceProps)
            {
                var resultProperty = resultProps.FirstOrDefault(f => f.Name.ToLower() == prop.Name.ToLower());

                if (resultProperty != null)
                {
                    var sourcePropertyValue = prop.GetValue(source);

                    if (sourcePropertyValue == null)
                    {
                        var defaultValue = MixinObject.GetDefaultValue(resultProperty.PropertyType);
                        //Console.WriteLine(defaultValue);
                        //resultPropertyName.SetValue(default(resultPropertyValue), resultProps);
                    }
                    else
                    {

                        resultProperty.SetValue(result, sourcePropertyValue);
                    }
                }
            }

            return (TResult)result;
        }

        public static TResult CopyTo<TSource, TResult>(this TSource source, object mixin)
            where TSource : class
            where TResult : class
        {
            object result = Activator.CreateInstance(typeof(TResult));
            var resultType = result.GetType();
            var resultProps = resultType.GetProperties();

            if (source == null)
            {
                return default(TResult);
            }

            var sourceType = source.GetType();
            var sourceProps = sourceType.GetProperties();

            if (sourceProps == null)
            {
                return default(TResult);
            }

            foreach (var prop in sourceProps)
            {
                var resultProperty = resultProps.FirstOrDefault(f => f.Name.ToLower() == prop.Name.ToLower());

                if (resultProperty != null)
                {
                    var sourcePropertyValue = prop.GetValue(source);

                    if (sourcePropertyValue == null)
                    {
                        var defaultValue = MixinObject.GetDefaultValue(resultProperty.PropertyType);
                        Console.WriteLine(defaultValue);
                        //resultPropertyName.SetValue(default(resultPropertyValue), resultProps);
                    }
                    else
                    {

                        resultProperty.SetValue(result, sourcePropertyValue);
                    }
                }
            }


            var mixinType = mixin.GetType();
            var mixinProps = mixinType.GetProperties();

            foreach (var prop in mixinProps)
            {
                var mixinProperty = mixinProps.FirstOrDefault(f => f.Name.ToLower() == prop.Name.ToLower());
                if (mixinProperty == null) continue;

                var mixinValue = prop.GetValue(mixin);

                //switch (mixinValue.GetType().FullName)
                //{
                //    case "System.Int":
                //    case "System.Int8":
                //    case "System.Int16":
                //    case "System.Int32":
                //    case "System.Int64":
                //        if (mixinValue == 0)
                //            break;
                //}

                if (mixinValue == null || (string.IsNullOrEmpty(mixinValue?.ToString()))) continue;
                var resultProperty = resultProps.FirstOrDefault(f => f.Name.ToLower() == prop.Name.ToLower());

                if (resultProperty != null)
                {
                    resultProperty.SetValue(result, mixinValue);
                }
            }

            return (TResult)result;
        }

        public static object GetDefaultValue(Type type)
        {
            //switch(type.)

            return null;
        }
    }
}
