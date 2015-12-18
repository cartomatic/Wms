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


            Request = request; 
            ExtractRequestParams(request);

            try
            {
                PrepareDriver();

                //run all the initial request checkups
                Validate(HandleRequestValidationRules);

                //when ready delegate request handling based on the required operation
                string operation = GetParam("request");
                bool ignoreCase = GetIgnoreCase();

                switch (operation.ToLower())
                {
                    case "getcapabilities" :
                        //make sure the casing is respected
                        if (string.Compare("GetCapabilities", operation, ignoreCase) == 0)
                        {
                            output = HandleGetCapabilities();
                        }
                        else
                        {
                            output = HandleUnsupported(operation);
                        }
                        break;

                    case "getmap":
                        //make sure the casing is respected
                        if (string.Compare("GetMap", operation, ignoreCase) == 0)
                        {
                            output = HandleGetMap();
                        }
                        else
                        {
                            output = HandleUnsupported(operation);
                        }
                        break;

                    case "getfeatureinfo":
                        //make sure the casing is respected
                        if (string.Compare("GetFeatureInfo", operation, ignoreCase) == 0)
                        {
                            output = HandleGetFeatureInfo();
                        }
                        else
                        {
                            output = HandleUnsupported(operation);
                        }
                        break;

                    case "getlegendgraphic":
                        //make sure the casing is respected
                        if (string.Compare("GetLegendGraphic", operation, ignoreCase) == 0)
                        {
                            output = HandleGetLegendGraphic();
                        }
                        else
                        {
                            output = HandleUnsupported(operation);
                        }
                        break;

                    default:
                        output = HandleUnsupported(operation); 
                        break;
                }
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
