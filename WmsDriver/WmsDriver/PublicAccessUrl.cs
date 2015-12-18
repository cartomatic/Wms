using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {

        protected internal string GetPublicAccessURL()
        {
            string url;

            if (!string.IsNullOrEmpty(ServiceDescription.PublicAccessURL))
            {
                url = ServiceDescription.PublicAccessURL;
            }
            else
            {
                string port = Request.RequestUri.IsDefaultPort
                    ? ""
                    : ":" + Request.RequestUri.Port;

                url = "http://" + Request.RequestUri.Host + port + Request.RequestUri.AbsolutePath;
            }

            return url;
        }
    }
}
