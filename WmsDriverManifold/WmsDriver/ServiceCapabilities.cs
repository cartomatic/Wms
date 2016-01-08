using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        protected void ApplyServiceCaps()
        {
            SupportedVersions.Add("1.3.0");

            SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>() {"text/xml"});
            DefaultGetCapabilitiesFormats.Add("1.3.0", "text/xml");

            SupportedGetMapFormats.Add("1.3.0", new List<string>() { "image/png", "image/jpeg", "image/gif" });

            SupportedExceptionFormats.Add("1.3.0", new List<string>() { "XML" });
            DefaultExceptionFormats.Add("1.3.0", "XML");

            //TODO: get legend
        }
    }
}
