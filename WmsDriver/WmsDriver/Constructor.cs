using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        static WmsDriver()
        {
            PopulateCoordFlippingSridsDict();
        }

        public WmsDriver()
        {
            //just init some of the data containers
            SupportedGetCapabilitiesFormats = new Dictionary<string, List<string>>();
            DefaultGetCapabilitiesFormats = new Dictionary<string, string>();
            SupportedGetMapFormats = new Dictionary<string, List<string>>();
            SupportedGetFeatureInfoFormats = new Dictionary<string, List<string>>();
            SupportedExceptionFormats = new Dictionary<string, List<string>>();
            DefaultExceptionFormats = new Dictionary<string, string>();
            SupportedVersions = new List<string>();
            SupportedVendorOperations = new Dictionary<string, List<string>>();
            SupportedVendorOperationFormats = new Dictionary<string, Dictionary<string, List<string>>>();
        }
    }
}
