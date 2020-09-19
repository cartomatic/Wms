using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Cartomatic.Utils.Serialization;
using Microsoft.Extensions.Configuration;

namespace Cartomatic.Wms.TileCache.DataModel
{
    public class Configuration
    {
        public Settings Settings { get; set; }

        public TileScheme TileScheme { get; set; }


        /// <summary>
        /// Reads a confoguration from appsettings.json or web.config / app.config; config key is TileCacheConfiguration and content should be a json /json string deserializable to <see cref="Configuration"/>
        /// </summary>
        /// <returns></returns>
        public static Configuration Read()
        {
#if NETSTANDARD2_0 || NETCOREAPP3_1
            var cfg = Cartomatic.Utils.NetCoreConfig.GetNetCoreConfig();
            return cfg.GetSection("TileCacheConfiguration").Get<Configuration>();
#endif
#if NETFULL
            return ConfigurationManager.AppSettings["TileCacheConfiguration"].DeserializeFromJson<Configuration>();
#endif
        }
    }
}
