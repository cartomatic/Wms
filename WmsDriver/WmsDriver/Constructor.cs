using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        public WmsDriver()
        {
            //just init some of the data containers
            SupportedGetFeatureInfoFormats = new Dictionary<string, List<string>>();
            SupportedGetCapabilitiesFormats = new Dictionary<string, List<string>>();
            SupportedGetMapFormats = new Dictionary<string, List<string>>();
            SupportedExceptionFormats = new Dictionary<string, List<string>>();
            SupportedVersions = new List<string>();
        }
    }
}
