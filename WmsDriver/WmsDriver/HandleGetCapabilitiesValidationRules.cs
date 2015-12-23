using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected internal Dictionary<string, Action<WmsDriver>> HandleGetCapabilitiesValidationRules = new Dictionary<string, Action<WmsDriver>>
        {
            {
                "service_param_presence", (drv) =>
                {
                    var msg = "Required parameter SERVICE not specified.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if(string.IsNullOrEmpty(drv.GetParam("service")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            {
                "service_must_be_wms", (drv) =>
                {
                    var msg = "Invalid service for GetCapabilities Request. Service parameter must be 'WMS'.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if(string.Compare(drv.GetParam("service"), "WMS", drv.GetIgnoreCase()) != 0)
                        throw new WmsDriverException(msg, ec);
                }
            },

            {
                "format_must_be_valid", (drv) =>
                {
                    var msg = "Version {0} of this service supports only the following GetCapabilities formats: {1}.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    //work out the service version; if version param is present i should have been validatyed by the initial request handler
                    var version = drv.GetDeclaredOrMaxSupportedVersion();

                    var format = drv.GetDeclaredOrDefaultGetCapabilitiesFormatForVersion(version);

                    if (!drv.SupportedGetCapabilitiesFormats[version].Exists(v => v == format))
                        throw new WmsDriverException(string.Format(msg, version, string.Join(",", drv.SupportedGetCapabilitiesFormats[version])), ec);
                }
            }

            //{
            //    "", (drv) =>
            //        {
            //        }
            //    }
            //}
        };

    }
}
