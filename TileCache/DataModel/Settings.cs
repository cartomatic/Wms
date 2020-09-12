using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cartomatic.Utils.Serialization;

namespace Cartomatic.Wms.TileCache
{
    public class Settings
    {
        public Settings() { }

        /// <summary>
        /// Destination folder for the cached files
        /// </summary>
        public string CacheFolder { get; set; }

        /// <summary>
        /// A list of formats that should be cached; if the list is empty, then all formats are treated as supported
        /// </summary>
        public List<string> SupportedFormats { get; set; }

        /// <summary>
        /// Whether or not png output should be size optimised
        /// </summary>
        public bool OptimizePng { get; set; }

        
        /// <summary>
        /// Dev option - whether or not to stamp the tile address
        /// </summary>
        public bool DevModeStampTileAddress { get; set; }


        /// <summary>
        /// Dev option - whether or not to stamp the tile bounds; not used so far
        /// </summary>
        public bool DevModeStampTileBounds { get; set; }


        /// <summary>
        /// Reads the tile cache settings from json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Settings FromJson(string json)
        {
            return json.DeserializeFromJson<Settings>();
        }
    }

}
