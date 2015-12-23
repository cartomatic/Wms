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

        protected internal Dictionary<string, Action<WmsDriver>> HandleRequestValidationRules = new Dictionary<string, Action<WmsDriver>>()
        {
            //request param should always be present
            {
                "request_param_present", (drv) =>
                {
                    var msg = "Required parameter REQUEST not specified.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if(string.IsNullOrEmpty(drv.GetParam("request")))
                        throw new WmsDriverException(msg, ec);
                }
            },

            //if no version param provided, then request MUST be GetCapabilities
            {
                "no_version_request_must_be_getcaps", (drv) =>
                {
                    var msg = "VERSION paramater not specified.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    if(string.IsNullOrEmpty(drv.GetParam("version")) && string.Compare(drv.GetParam("request"), "GetCapabilities", drv.GetIgnoreCase()) != 0)
                        throw new WmsDriverException(msg, ec);
                }
            },

            //if the version param has been provided, then must match supported versions
            {
                "version_param_matches_supported_versions", (drv) =>
                {
                    var msg = "This service supports only the following version(s): {0}.";
                    var ec = WmsExceptionCode.NotApplicable;
            
                    var p = drv.GetParam("version");
                    if(!string.IsNullOrEmpty(p) && !drv.SupportedVersions.Exists(sv => sv == p))
                        throw new WmsDriverException(string.Format(msg, string.Join(", ", drv.SupportedVersions)), ec);
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
