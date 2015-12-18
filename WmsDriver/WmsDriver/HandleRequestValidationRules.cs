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
            },

            //request param should always be present
            {
                "request_param_present", new ValidationRule()
                {
                    Message = "Required parameter REQUEST not specified",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        if(string.IsNullOrEmpty(drv.GetParam("request")))
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                    }
                }
            },

            //if no version param provided, then request MUST be GetCapabilities
            {
                "no_version_request_must_be_getcaps", new ValidationRule()
                {
                    Message = "VERSION paramater not specified",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        if(string.IsNullOrEmpty(drv.GetParam("version")) && string.Compare(drv.GetParam("request"), "GetCapabilities", drv.GetIgnoreCase()) != 0)
                            throw new WmsDriverException(a.Message, a.WmsEcExceptionCode);
                    }
                }
            },

            //if the version param has been provided, then must match supported versions
            {
                "version_param_matches_supported_versions", new ValidationRule()
                {
                    Message = "This service supports only the following version(s): {0}",
                    WmsEcExceptionCode = WmsExceptionCode.NotApplicable,
                    Action = (drv, a) =>
                    {
                        var p = drv.GetParam("version");
                        if(!string.IsNullOrEmpty(p) && !drv.SupportedVersions.Exists(sv => sv == p))
                            throw new WmsDriverException(string.Format(a.Message, string.Concat(", ", drv.SupportedVersions)), a.WmsEcExceptionCode);
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
