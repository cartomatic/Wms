using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace Cartomatic.Wms.ManifoldWmsDriverWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.RegisterSwaggerConfig();

            config.MapHttpAttributeRoutes();


            //newtonsoft json serializer settings! so customised json is returned
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                //make the json props be camel case!
                ContractResolver = new CamelCasePropertyNamesContractResolver(),

                //ignore nulls, no point in outputting them!
                NullValueHandling = NullValueHandling.Ignore,

                //ensure dates are handled as ISO 8601
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };



            //make the debug mode output be a bit more readable...
#if DEBUG
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
#endif

            
            config.EnableCors(new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*"
            ));

            app.UseWebApi(config);

            //this will make the api output json in most cases, unless explicitly asked for xml
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));


        }
    }
}