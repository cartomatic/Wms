using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver
    {
        protected void ApplyServiceCaps()
        {
            SupportedVersions.Add("1.3.0");

            SupportedGetCapabilitiesFormats.Add("1.3.0", new List<string>() {"text/xml"});
            DefaultGetCapabilitiesFormats.Add("1.3.0", "text/xml");

            SupportedGetMapFormats.Add("1.3.0", new List<string>() { "image/png", "image/jpeg", "image/gif" });

            SupportedExceptionFormats.Add("1.3.0", new List<string>() { "XML" });
            DefaultExceptionFormats.Add("1.3.0", "XML");

            SupportedVendorOperations.Add("1.3.0", new List<string>() { "GetLegendGraphic" });
            SupportedVendorOperationFormats.Add(
                "GetLegendGraphic", new Dictionary<string, List<string>>() { { "1.3.0", new List<string>() { "image/png", "image/jpeg", "image/gif" } } }
            );
        }
    }
}
