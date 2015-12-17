using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Web;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver : IWmsDriver
    {
        /// <summary>
        /// Handles WMS request in a form of a url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IWmsDriverResponse HandleRequest(string url)
        {
            return HandleRequest(url.CreateHttpWebRequest());
        }


        public IWmsDriverResponse HandleRequest(HttpWebRequest request)
        {
            IWmsDriverResponse output = null;

            ExtractRequestParams(request);

            try
            {
                PrepareDriver();

                //run all the initial request checkups
                Validate(HandleRequestValidationRules);

                //when ready delegate request handling based on the required operation


                //before defaulting to unsupported operation, try to find a handler by reflection
                //so can easily hook in the extra vendor ops if required

            }
            catch (WmsDriverException drve)
            {
                output = HandleWmsDriverException(drve);
            }
            catch (Exception ex)
            {
                output = HandleWmsDriverException(ex);
            }

            return output;
        }
    }
}
