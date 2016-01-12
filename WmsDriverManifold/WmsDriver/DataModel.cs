using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using M = Manifold.Interop;

namespace Cartomatic.Manifold
{
    public partial class WmsDriver
    {
        /// <summary>
        /// Name of the comments component containing the wms settings
        /// </summary>
        protected internal string WmsSettingsComp = "wms.settings";

        /// <summary>
        /// Map file path
        /// </summary>
        public string MapFile { get; set; }

        /// <summary>
        /// Served map component name
        /// </summary>
        public string MapComp { get; set; }

        /// <summary>
        /// Manifold MapServer object
        /// </summary>
        protected M.MapServer MapServer { get; set; }


        /// <summary>
        /// pixel size in metres; resolution is assumed to be 96ppi -> 0.0254 = 2.54 cm in metres
        /// </summary>
        const double PIXEL_SIZE = 0.0254 / 96;

        /// <summary>
        /// default mapserver render quality; applicability of this params varies between formats
        /// </summary>
        protected const int DEFAULT_RENDER_QUALITY = 100;

        /// <summary>
        /// Manifold WmsDriver specific settings
        /// </summary>
        protected internal WmsDriverSettings MSettings { get; set; }
    }
}
