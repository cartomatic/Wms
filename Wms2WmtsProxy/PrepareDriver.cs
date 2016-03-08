using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cartomatic.Utils.Serialization;
using Cartomatic.Utils.Web;

using Wmts_101 = Cartomatic.OgcSchemas.Wmts.Wmts_101;
using Wms_1302 = Cartomatic.OgcSchemas.Wms.Wms_1302;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// Get caps lock
        /// </summary>
        protected static object WmtsGetCapsLock = new object();

        /// <summary>
        /// Url of the proxy handler that passed the request here
        /// </summary>
        protected string ProxyUrl { get; set; }

        /// <summary>
        /// Param used to specify the url of the actual service
        /// </summary>
        protected string ProxyUrlParam => "wmtscapsurl";

        /// <summary>
        /// param describing whether or not bbox coords should be flipped or not before parsing it;
        /// used to allow clients shuch as manifold 8 to use the service even though they send invalid bbox for wms 130
        /// </summary>
        protected string FlipBboxParam => "flipbboxcoords";

        /// <summary>
        /// Whether or not requested bbox coords should be flipped or not
        /// </summary>
        protected bool FlipBbox { get; set; }

        /// <summary>
        /// Make sure the incoming full url as passed to the proxy is actually disassembled here!
        /// </summary>
        /// <param name="request"></param>
        protected override void ExtractRequestParams(HttpWebRequest request)
        {
            if (request.Address.AbsoluteUri.IndexOf('?') > -1)
            {
                ProxyUrl = request.Address.AbsoluteUri.Substring(0, request.Address.AbsoluteUri.IndexOf('?'));
            }

            var flipParam = request.Address.Query.Replace("?", "").Split('&')
                .FirstOrDefault(p => p.StartsWith(FlipBboxParam))?
                .Replace($"{FlipBboxParam}=", "");

            if (!string.IsNullOrEmpty(flipParam))
            {
                bool flipBbox;
                bool.TryParse(flipParam, out flipBbox);
                FlipBbox = flipBbox;
            }

            //Use proxy utils to get the url that should be called
            var proxiedUrl = request.Address.AbsoluteUri.ExtractProxiedUrl(ProxyUrlParam);

            base.ExtractRequestParams(proxiedUrl.CreateHttpWebRequest());
        }


        /// <summary>
        /// Prepares the driver so it can be used for processing the requests
        /// </summary>
        protected override void PrepareDriver()
        {
            //assuming the incoming url is actually what has been sent to the proxy handler
            //

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

                        PrepareBasicDriverConfiguration(baseUrl); 
                    }
                }
            }

            //this is the actual bit that configures the driver
            ApplyServiceCaps(baseUrl);

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
                wmtsCapsCache[baseUrl] = sr.ReadToEnd().DeserializeFromXml<Wmts_101.Capabilities>();
            }
        }

        /// <summary>
        /// Gets a cached wmts caps object
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        protected internal Wmts_101.Capabilities GetCachedWmtsCaps(string baseUrl)
        {
            Wmts_101.Capabilities caps = null;
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
        protected internal Dictionary<string, Wmts_101.Capabilities> GetWmtsServiceCapabilitiesCache()
        {
            var wmtsCapsCache = HttpContext.Current.Application["wmts_caps_cache"] as Dictionary<string, Wmts_101.Capabilities>;
            if (wmtsCapsCache == null)
            {
                wmtsCapsCache = new Dictionary<string, Wmts_101.Capabilities>();
                HttpContext.Current.Application["wmts_caps_cache"] = wmtsCapsCache;
            }
            return wmtsCapsCache;
        }



        /// <summary>
        /// Applies wmts service dependant WMS service capabilities for current request
        /// </summary>
        /// <param name="baseUrl"></param>
        private void ApplyServiceCaps(string baseUrl)
        {
            var wmsCfg = GetWmsConfigCache(baseUrl)[baseUrl];


            //this is standard for this driver and does not depend on the backend WMTS
            SupportedVersions.Add("1.3.0");

            SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>() { "text/xml" });
            DefaultGetCapabilitiesFormats.Add("1.3.0", "text/xml");

            SupportedExceptionFormats.Add("1.3.0", new List<string>() { "XML" });
            DefaultExceptionFormats.Add("1.3.0", "XML");

            //service description and image formats though do depend on the WMTS backend
            ServiceDescription = (WmsServiceDescription) wmsCfg["WmsServiceDescription"];


            SupportedGetMapFormats.Add("1.3.0", (List<string>)wmsCfg["SupportedGetMapFormats"]);
        }


        /// <summary>
        /// Configures Wms driver based on the Wmts caps document
        /// </summary>
        protected internal void PrepareBasicDriverConfiguration(string baseUrl)
        {
            var wmtsCaps = GetCachedWmtsCaps(baseUrl);

            var wmsCfgCache = GetWmsConfigCache(baseUrl);

            if (!wmsCfgCache.ContainsKey(baseUrl))
            {
                var wmsCfg = new Dictionary<string, object>(); 
                wmsCfgCache[baseUrl] = wmsCfg;

                //wms description
                //abstract, title and such
                var wmsDesc = new WmsServiceDescription();
                //Note:
                //Not in a standard new obj(){p1=xxx,p2=...} so can spot errors easily as the code below is a bit verbose ;)
                wmsDesc.Abstract = wmtsCaps.Contents.LayerSet.Aggregate(string.Empty,
                    (agg, ls) =>
                        agg +=
                            (agg.Length > 0 ? " | " : "") + ls.Title.FirstOrDefault()?.Value + ": " +
                            ls.Abstract.FirstOrDefault()?.Value);
                wmsDesc.AccessConstraints = string.Join(", ", wmtsCaps.ServiceIdentification?.AccessConstraints ?? new[] { "" });
                wmsDesc.Address = string.Join(", ",
                    wmtsCaps.ServiceProvider?.ServiceContact?.ContactInfo?.Address?.DeliveryPoint ?? new[] {""});
                wmsDesc.AddressType = "";
                wmsDesc.City = wmtsCaps?.ServiceProvider?.ServiceContact?.ContactInfo?.Address?.City;
                wmsDesc.ContactElectronicMailAddress = string.Join(", ",
                    wmtsCaps?.ServiceProvider?.ServiceContact?.ContactInfo?.Address?.ElectronicMailAddress ?? new[] {""});
                wmsDesc.ContactOrganization = wmtsCaps.ServiceProvider?.ProviderName;
                wmsDesc.ContactPerson = wmtsCaps.ServiceProvider?.ServiceContact?.IndividualName;
                wmsDesc.ContactPosition = wmtsCaps.ServiceProvider?.ServiceContact?.PositionName;
                wmsDesc.ContactVoiceTelephone = string.Join(", ",
                    wmtsCaps?.ServiceProvider?.ServiceContact?.ContactInfo?.Phone?.Voice ?? new[] {""});
                wmsDesc.Country = wmtsCaps.ServiceProvider?.ServiceContact?.ContactInfo?.Address?.Country;
                wmsDesc.Fees = wmtsCaps.ServiceIdentification?.Fees;
                wmsDesc.Keywords =
                    wmtsCaps.ServiceIdentification?.Keywords?.FirstOrDefault()?.Keyword?.Select(k => k.Value).ToList();
                wmsDesc.LayerLimit = 1; //limit layers - after all it is a prerendered data
                wmsDesc.MaxHeight = 256; //limit tile size, so does not have to assemble too big tiles
                wmsDesc.MaxWidth = 256;
                wmsDesc.PostCode = wmtsCaps.ServiceProvider?.ServiceContact?.ContactInfo?.Address?.PostalCode;
                wmsDesc.StateOrProvince =
                    wmtsCaps.ServiceProvider?.ServiceContact?.ContactInfo?.Address?.AdministrativeArea;
                wmsDesc.Title = wmtsCaps.ServiceIdentification?.Title?.FirstOrDefault()?.Value;

                wmsCfg["WmsServiceDescription"] = wmsDesc;


                
                //extract and cache image formats the wmts get tile lists for all layers
                wmsCfg["SupportedGetMapFormats"] = wmtsCaps.Contents.LayerSet.Aggregate(new List<string>(), (agg, ls) =>
                {
                    agg.AddRange(
                        ls.ResourceURL.Where(resurl => resurl.resourceType == Wmts_101.URLTemplateTypeResourceType.tile)
                            .Select(resurl => resurl.format));
                    return agg;
                }).Distinct().ToList();


                //other stuff depends on the actual request, so it'll be ok to extract it from the cached wmts caps object as needed for both getcaps and getmap requests
            }
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
