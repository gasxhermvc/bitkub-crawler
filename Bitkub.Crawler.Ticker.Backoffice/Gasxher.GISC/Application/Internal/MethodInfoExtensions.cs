using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Internal
{
    internal static class MethodInfoExtensions
    {
        public static object? InvokeWithoutWrappingExceptions(this MethodInfo methodInfo, object? obj, object[] parameters)
        {
            return methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, binder: null, parameters: parameters, culture: null);
        }
    }
}
