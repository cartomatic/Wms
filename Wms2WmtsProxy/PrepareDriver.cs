using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cartomatic.OgcSchemas.Wmts.Wmts_101;
using Cartomatic.Utils.Serialization;
using Cartomatic.Utils.Web;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// Get caps lock
        /// </summary>
        protected static object WmtsGetCapsLock = new object();

        /// <summary>
        /// Prepares the driver so it can be used for processing the requests
        /// </summary>
        protected override void PrepareDriver()
        {
            var baseUrl = GetBaseUrl();

            var wmtsCaps = GetCachedWmtsCaps(baseUrl);

            if (wmtsCaps == null)
            {
                lock (WmtsGetCapsLock)
                {
                    wmtsCaps = GetCachedWmtsCaps(baseUrl);

                    if (wmtsCaps == null)
                    {
                        PullWmtsCaps(baseUrl);

                        ConfigureWmsDriver(baseUrl);
                    }
                }
            }

            base.PrepareDriver();
        }

        /// <summary>
        /// pulls Wmts capabilities document off the web
        /// </summary>
        protected internal void PullWmtsCaps(string baseUrl)
        {
            var requestUrl = $"{baseUrl}?service=WMTS&request=GetCapabilities&version=1.0.0";

            var request = requestUrl.CreateHttpWebRequest();

            var response = request.ExecuteRequest();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var wmtsCapsCache = GetWmtsServiceCapabilitiesCache();
                wmtsCapsCache[baseUrl] = sr.ReadToEnd().DeserializeFromXml<Capabilities>();
            }
        }

        /// <summary>
        /// Gets a cached wmts caps object
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        protected internal Capabilities GetCachedWmtsCaps(string baseUrl)
        {
            Capabilities caps = null;
            var wmtsCapsCache = GetWmtsServiceCapabilitiesCache();

            if (wmtsCapsCache.ContainsKey(baseUrl))
            {
                caps = wmtsCapsCache[baseUrl];
            }

            return caps;
        }

        /// <summary>
        /// Gets a cache of per proxied service wmts caps docs
        /// </summary>
        /// <returns></returns>
        protected internal Dictionary<string, Capabilities> GetWmtsServiceCapabilitiesCache()
        {
            var wmtsCapsCache = HttpContext.Current.Application["wmts_caps_cache"] as Dictionary<string, Capabilities>;
            if (wmtsCapsCache == null)
            {
                wmtsCapsCache = new Dictionary<string, Capabilities>();
                HttpContext.Current.Application["wmts_caps_cache"] = wmtsCapsCache;
            }
            return wmtsCapsCache;
        }



        /// <summary>
        /// Configures Wms driver based on the Wmts caps document
        /// </summary>
        protected internal void ConfigureWmsDriver(string baseUrl)
        {
            var wmsCfgCache = GetWmsConfigCache(baseUrl);

            if (!wmsCfgCache.ContainsKey(baseUrl))
            {
                var wmsCfg = new Dictionary<string, object>();
                wmsCfgCache[baseUrl] = wmsCfg;

                //wms description
                //abstract, title and such

                //get map url - this is important as need to route the wms requests through this very proxy

                //bounding boxes and alike

                //allowed epsg

                //limit tile size, so does not have to assemble too big tiles

                //limit layers? WMTS has pre-cached tiles, so cannot request many layers. maybe themes, but at this stage it'll be kept simple; limit it to 1

                //GetMap formats - where resource url resource type == tile
                //same place also gives a request url - per format of course; url has some replacement tokens;  - this can be perhaps extracted by some utils - get by format- some nice extension methods!

                //need to extract available formats of get map
                //ignore get feature info for the time being

                //tile matrix set - this can be perhaps extracted by some utils - get by epsg - some nice extension methods!


                //also need to prepare service description object
                //and pass it all the needed data
            }

            //assign proper service description to an internal property
            //ServiceDescription = 
        }

        /// <summary>
        /// Gets a cache of per proxied service wms cfg settings
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        protected internal Dictionary<string, Dictionary<string, object>> GetWmsConfigCache(string baseUrl)
        {
            var wmsConfigCache =
                HttpContext.Current.Application["wms_cfg_cache"] as Dictionary<string, Dictionary<string, object>>;
            if (wmsConfigCache == null)
            {
                wmsConfigCache = new Dictionary<string, Dictionary<string, object>>();
                HttpContext.Current.Application["wms_cfg_cache"] = wmsConfigCache;
            }
            return wmsConfigCache;
        }


    }
}
