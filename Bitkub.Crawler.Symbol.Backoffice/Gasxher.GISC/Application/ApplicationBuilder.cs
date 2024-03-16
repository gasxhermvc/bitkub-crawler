using GasxherGIS.Extensions.Application.Internal;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        private const string ServerFeaturesKey = "server.Features";
        private const string ApplicationServicesKey = "application.Services";

        public IDictionary<string, object> Properties { get; }

        private readonly IList<Func<object, object>> _components = new List<Func<object, object>>();

        public ApplicationBuilder(IServiceProvider serviceProvider)
        {
            Properties = new Dictionary<string, object>(StringComparer.Ordinal);
            ApplicationServices = serviceProvider;
        }

        public ApplicationBuilder(IServiceProvider serviceProvider, object server)
            : this(serviceProvider)
        {
            SetProperty(ServerFeaturesKey, server);
        }

        private ApplicationBuilder(ApplicationBuilder builder)
        {
            Properties = new CopyOnWriteDictionary<string, object>(builder.Properties, StringComparer.Ordinal);
        }

        public IServiceProvider ApplicationServices
        {
            get => GetProperty<IServiceProvider>(ApplicationServicesKey);
            set => SetProperty<IServiceProvider>(ApplicationServicesKey, value);
        }


        public IFeatureCollection ServerFeatures
        {
            get => GetProperty<IFeatureCollection>(ServerFeaturesKey);
        }

        private T GetProperty<T>(string key)
        {
            object value;

            return Properties.TryGetValue(key, out value) ? (T)value : default(T);
        }

        private void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        //=>Func<object, object> ปรับเป็นของเราเอง
        public IApplicationBuilder Use(Func<object, object> middleware)
        {
            return this;
        }

        public IApplicationBuilder New()
        {
            return new ApplicationBuilder(this);
        }

        //=>ปรับเป็ยของเราเอง
        public object Build()
        {
            var app = new object();

            for (var c = _components.Count - 1; c >= 0; c--)
            {
                app = _components[c](app);
            }

            return app;
        }
    }
}
