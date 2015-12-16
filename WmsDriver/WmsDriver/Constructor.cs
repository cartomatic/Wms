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
            SupportedGetFeatureInfoFormats = new List<string>();
            SupportedGetCapabilitiesFormats = new List<string>();
            SupportedGetMapFormats = new List<string>();
            SupportedExceptionFormats = new List<string>();
            SupportedVersions = new List<string>();
        }
    }
}
