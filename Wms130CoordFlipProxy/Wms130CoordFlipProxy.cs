using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Cartomatic.OgcSchemas.Wms.Wms_1302;
using Cartomatic.Utils.Web;
using Cartomatic.Utils.Serialization;

namespace Cartomatic.Wms
{
    public partial class Wms130CoordFlipProxy
    {
        public Wms130CoordFlipProxy(string forceCoordSwapParam = null)
        {
        }

        /// <summary>
        /// Handles a wms request and proxies it appropriately
        /// </summary>
        /// <param name="context"></param>
        /// <param name="completeRequest"></param>
        public void HandleRequest(HttpContext context, bool completeRequest = true)
        {
            var proxiedUrl = context.Request.Url.ExtractProxiedUrl();

            //if getmap, adjust the bbox as required
            if (proxiedUrl.IndexOf("GetMap", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                proxiedUrl = SwapBboxCoordsOrder(proxiedUrl);
            }


            if (proxiedUrl.IndexOf("GetCapabilities", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                //need to replace the urls in the caps doc so the requests are routed through the proxy!
                //otherwise, the requests will bypass the proxy

                var request = proxiedUrl.CreateHttpWebRequest(context.Request);

                var response = request.ExecuteRequest(); 

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    var capsDoc = sr.ReadToEnd().DeserializeFromXml<WMS_Capabilities>();
                    
                    //replace urls
                    var baseUrl = $"{context.Request.Url.AbsoluteUri.Substring(0, context.Request.Url.AbsoluteUri.IndexOf("?"))}?{Cartomatic.Utils.Web.ProxyExtensions.DefaultUrlParam}=";

                    capsDoc.Service.OnlineResource.href = baseUrl + capsDoc.Service.OnlineResource.href;
                    AdjustDCPTypeUrls(capsDoc.Capability.Request.GetCapabilities.DCPType, baseUrl);
                    AdjustDCPTypeUrls(capsDoc.Capability.Request.GetMap.DCPType, baseUrl);

                    //get caps and get map are mandatory
                    if (capsDoc.Capability.Request.GetFeatureInfo != null)
                    {
                        AdjustDCPTypeUrls(capsDoc.Capability.Request.GetFeatureInfo.DCPType, baseUrl);
                    }

                    //Note:
                    //could also do the vendor ops, but not needed for now

                    //finally pump the data back to response
                    context.Response.Write(capsDoc.SerializeToXml());
                }

                //finally copy response internals
                response.CopyResponseInternals(context.Response);

                //and finalize request
                if (completeRequest)
                {
                    System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                proxiedUrl.Proxy(context);
            }
        }

        /// <summary>
        /// Adjusts DCPType urls to include proxy url
        /// </summary>
        /// <param name="DCPType"></param>
        /// <param name="proxyUrl"></param>
        protected void AdjustDCPTypeUrls(DCPType[] DCPType, string proxyUrl)
        {
            foreach (var dcpt in DCPType)
            {
                if (dcpt.HTTP.Get != null)
                {
                    dcpt.HTTP.Get.OnlineResource.href = proxyUrl + dcpt.HTTP.Get.OnlineResource.href;
                }
                if (dcpt.HTTP.Post != null)
                {
                    dcpt.HTTP.Post.OnlineResource.href = proxyUrl + dcpt.HTTP.Post.OnlineResource.href;
                }
            }
        }

        /// <summary>
        /// Swaps the bbox coords order for WMS 1.3.0 requests if required; Tests the built in EPSG database for the coord swapping coordinate systems in order to determine whether the coords should be swapped or not; forceSwapCoords param is used to swap the coords without testing the EPSG
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected internal string SwapBboxCoordsOrder(string url)
        {
            var outUrl = url;
            var urlParams = url.Split('?')[1].Split('&');

            if (ShouldSwap(urlParams))
            {
                var rawBbox = ExtractParam(urlParams, "bbox", false);

                var bboxParts = rawBbox.Split('=')[1].Split(',');

                //if bbox swabs coords in wms 1.3.0 it should be specified as miny.minx,maxy,maxx
                //some clients though do not respect this and therefore send the standard minx,miny,maxx,maxy that needs to be swapped to perform a proper request

                var swappedBbox = $"bbox={bboxParts[1]},{bboxParts[0]},{bboxParts[3]},{bboxParts[2]}";

                return outUrl = url.Replace(rawBbox, swappedBbox);
            }
            
            return outUrl;
        }

        /// <summary>
        /// Extracts a param value off the set of params
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="pName"></param>
        /// <param name="onlyParamValue">extracts only a param value, not the full string of pname=pvalue</param>
        /// <returns></returns>
        protected string ExtractParam(IEnumerable<string> parameters, string pName, bool onlyParamValue = true)
        {
            var param = parameters.FirstOrDefault(p => p.StartsWith(pName, StringComparison.InvariantCultureIgnoreCase));
            if (onlyParamValue && !string.IsNullOrEmpty(param))
            {
                param = param.Split('=')[1];
            }
            return param;
        }

        /// <summary>
        /// Whether or not should swap the order of bbox coordinates
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected bool ShouldSwap(IEnumerable<string> urlParams)
        {
            bool swapCoords = ExtractParam(urlParams, "version") == "1.3.0";

            //TODO - could play a bit with some epsgs or something. not sure though if it makes sense. proxy is to be used for cordsys that needs coord swap in version 130 but client does not swap the bbox coords. no point in using this proxy otherwise!!! Could actually always return true here

            return swapCoords;
        }
    }
}
