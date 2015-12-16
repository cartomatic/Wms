using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        /// <summary>
        /// Service description data used to generate GetCapabilities response
        /// </summary>
        public IWmsServiceDescription ServiceDescription { get; set; }

        /// <summary>
        /// A list of supported GetFeatureInfo formats
        /// </summary>
        public List<string> SupportedGetFeatureInfoFormats { get; private set; }

        /// <summary>
        /// A list of supported GetCapabilities formats
        /// </summary>
        public List<string> SupportedGetCapabilitiesFormats { get; private set; }

        /// <summary>
        /// A list of supported GetMap formats
        /// </summary>
        public List<string> SupportedGetMapFormats { get; private set; }

        /// <summary>
        /// A list of supported Exception formats
        /// </summary>
        public List<string> SupportedExceptionFormats { get; private set; }

        /// <summary>
        /// A list of supported WMS versions
        /// </summary>
        public List<string> SupportedVersions { get; protected internal set; }

        /// <summary>
        /// Whether or not the png outpout should be size optimised; Note that this may be heavish operation...
        /// </summary>
        public bool OptimisePng { get; set; }

    }
}
