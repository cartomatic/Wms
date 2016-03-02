using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public partial class Wms2WmtsProxy
    {
        /// <summary>
        /// Gets a base url for the proxied service; basically this is either a base service url or the actual caps url in a case it's provided as xml
        /// </summary>
        /// <returns></returns>
        protected internal string GetBaseUrl()
        {
            if (Request == null)
            {
                throw new WmsDriverException("Unknown WMTS Service URL.");
            }

            //work out the base service url or the actual get caps url in a case it's provided as xml
            var baseUrl = Request.RequestUri.AbsoluteUri;
            if (baseUrl.IndexOf("?") > -1)
            {
                baseUrl = baseUrl.Substring(0, baseUrl.IndexOf("?"));
            }
            return baseUrl;
        }
    }
}
