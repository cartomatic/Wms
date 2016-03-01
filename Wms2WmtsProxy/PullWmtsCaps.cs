using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.OgcSchemas.Wmts.Wmts_101;
using Cartomatic.Utils.Serialization;
using Cartomatic.Utils.Web;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// pulls Wmts capabilities document off the web
        /// </summary>
        protected internal void PullWmtsCaps()
        {
            if (Request == null)
            {
                throw new WmsDriverException("Unknown WMTS Service URL.");
            }

            //work out the get caps request url
            var baseUrl = Request.RequestUri.AbsoluteUri;
            if (baseUrl.IndexOf("?") > -1)
            {
                baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("?"));
            }
            var requestUrl = $"{baseUrl}?service=WMTS&request=GetCapabilities&version=1.0.0";


            var request = requestUrl.CreateHttpWebRequest();

            var response = request.ExecuteRequest();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                WmtsCaps = sr.ReadToEnd().DeserializeFromXml<Capabilities>();
            }
        }
    }
}
