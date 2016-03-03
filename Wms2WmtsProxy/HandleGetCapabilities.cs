using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wms.Wms_1302;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// Adds some customisations to the generated wms caps doc, so the clients can then call the service through this very proxy
        /// </summary>
        /// <returns></returns>
        protected override WMS_Capabilities GenerateWmsCapabilitiesDocument130()
        {
            var capsDoc = base.GenerateWmsCapabilitiesDocument130();

            //this could be potentially taken off the initial request, but is simply reassembled here
            var urlWithProxy = $"{ProxyUrl}?{ProxyUrlParam}={GetBaseUrl()}";

            capsDoc.Service.OnlineResource.href = urlWithProxy;

            //Note:
            //only mandatory ops for the time being

            foreach (var dcpType in capsDoc.Capability.Request.GetCapabilities.DCPType)
            {
                ModifyDcpTypeUrl(dcpType, urlWithProxy);
            }
            foreach (var dcpType in capsDoc.Capability.Request.GetMap.DCPType)
            {
                ModifyDcpTypeUrl(dcpType, urlWithProxy);
            }

            return capsDoc;
        }

        /// <summary>
        /// Replaces the passed dcpType urls with the provided one
        /// </summary>
        /// <param name="dcpType"></param>
        /// <param name="url"></param>
        protected void ModifyDcpTypeUrl(DCPType dcpType, string url)
        {
            if (dcpType?.HTTP?.Get != null)
            {
                dcpType.HTTP.Get.OnlineResource.href = url;
            }
            if (dcpType?.HTTP?.Post != null)
            {
                dcpType.HTTP.Post.OnlineResource.href = url;
            }
        }

        //get map url - this is important as need to route the wms requests through this very proxy
        //bounding boxes and alike
        //allowed epsg

        //GetMap formats - where resource url resource type == tile
        //same place also gives a request url - per format of course; url has some replacement tokens;  - this can be perhaps extracted by some utils - get by format- some nice extension methods!

        //need to extract available formats of get map
        //ignore get feature info for the time being

        //tile matrix set - this can be perhaps extracted by some utils - get by epsg - some nice extension methods!

        protected override WMS_Capabilities GenerateCapsLayersSection130(WMS_Capabilities capsDoc)
        {


            return capsDoc;
        }
    }
}
