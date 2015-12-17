using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {

        protected internal Dictionary<string, IValidationRule> HandleRequestValidationRules = new Dictionary<string, IValidationRule>()
        {
            //Driver must specify at least one service version it supports
            { "service_version_specified", new ValidationRule()
                {
                    Message = "SETUP ERROR: Driver must specify at least one service VERSION it supports.",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        if(drv.SupportedVersions.Count == 0)
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                    }
                }
            },

            //driver must define at least one GetCapabilities format for each supported service version
            {
                "getcaps_format_specified", new ValidationRule()
                {
                    Message = "SETUP ERROR: Driver must specify at least one GetCapabilities format for each supported service version.",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        if (drv.SupportedGetCapabilitiesFormats == null || drv.SupportedGetCapabilitiesFormats.Count == 0)
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);

                        foreach (var version in drv.SupportedGetCapabilitiesFormats)
                        {
                            if (version.Value == null || version.Value.Count == 0)
                                throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                        }
                    }
                }
            },

            //driver must define at least one GetMap format for each supported service version
            {
                "getmap_format_specified", new ValidationRule()
                {
                    Message = "SETUP ERROR: Driver must specify at least one GetMap format for each supported service version.",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        if (drv.SupportedGetMapFormats == null || drv.SupportedGetMapFormats.Count == 0)
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);

                        foreach (var version in drv.SupportedGetMapFormats)
                        {
                            if (version.Value == null || version.Value.Count == 0)
                                throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                        }
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
