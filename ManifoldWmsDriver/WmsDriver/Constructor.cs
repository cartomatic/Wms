using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Wms;

namespace Cartomatic.Wms
{
    public partial class ManifoldWmsDriver : Cartomatic.Wms.WmsDriver
    {
        /// <summary>
        /// Creates a new WmsDriver instance
        /// </summary>
        /// <param name="mapFile"></param>
        /// <param name="mapComp"></param>
        /// <param name="serviceDescription">Wms service description. If not provided, driver will try to extract them from a map file</param>
        public ManifoldWmsDriver(string mapFile, string mapComp = "Map", WmsServiceDescription serviceDescription = null)
        {
            ApplyServiceCaps();

            ServiceDescription = serviceDescription;
            MapComp = mapComp;
            MapFile = mapFile;
        }

    }
}
