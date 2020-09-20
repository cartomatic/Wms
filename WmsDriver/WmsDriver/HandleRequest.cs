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
        /// Handles a WMS request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<IWmsDriverResponse> HandleRequestAsync(string url)
        {
            return await HandleRequestAsync(url.CreateHttpWebRequest());
        }

        /// <summary>
        /// Handles a WMS request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IWmsDriverResponse> HandleRequestAsync(HttpWebRequest request)
        {
            IWmsDriverResponse output = null;

            ExtractRequestParams(request);

            try
            {
                PrepareDriver();

                //run the driver setup checkup
                Validate(DriverSetupValidationRules);

                //run all the initial request checkups
                Validate(HandleRequestValidationRules);

                //when ready delegate request handling based on the required operation
                var operation = GetParam<string>("request");
                var ignoreCase = GetIgnoreCase();

                switch (operation.ToLower())
                {
                    case "getcapabilities" :
                        //make sure the casing is respected
                        if (string.Compare("GetCapabilities", operation, ignoreCase) == 0)
                        {
                            output = await HandleGetCapabilitiesAsync();
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
                            output = await HandleGetMapAsync();
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
                            output = await HandleGetFeatureInfoAsync();
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
