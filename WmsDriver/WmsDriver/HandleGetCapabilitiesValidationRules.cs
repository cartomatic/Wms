using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        protected internal Dictionary<string, IValidationRule> HandleGetCapabilitiesValidationRules = new Dictionary
            <string, IValidationRule>()
        {
            {
                "service_param_presence", new ValidationRule()
                {
                    Message = "Required parameter SERVICE not specified.",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        if(string.IsNullOrEmpty(drv.GetParam("service")))
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                    }
                }
            },

            {
                "service_must_be_wms", new ValidationRule()
                {
                    Message = "Invalid service for GetCapabilities Request. Service parameter must be 'WMS'.",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        if(string.Compare(drv.GetParam("service"), "WMS", drv.GetIgnoreCase()) != 0)
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                    }
                }
            },

            {
                "format_must_be_valid", new ValidationRule()
                {
                    Message = "Version {0} of this service supports only the following GetCapabilities formats: {1}",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        //work out the service version; if version param is present i should have been validatyed by the initial request handler
                        var version = drv.GetDeclaredOrMaxSupportedVersion();

                        var format = drv.GetDeclaredOrDefaultGetCapabilitiesFormatForVersion(version);

                        if (!drv.SupportedGetCapabilitiesFormats[version].Exists(v => v == format))
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                    }
                }
            }

            

            //{
            //    "", new ValidationRule()
            //    {
            //        Message = "",
            //        WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
            //        Action = (drv, a) =>
            //        {

            //        }
            //    }
            //}
        };

    }
}
