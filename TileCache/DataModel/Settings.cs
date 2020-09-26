using Cartomatic.Utils.Serialization;
using System.Collections.Generic;

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
        /// Dev option - whether or not to stamp the tile address
        /// </summary>
        public bool DevModeStampTileAddress { get; set; }

        /// <summary>
        /// Size of the http client pool size; defaults to 4
        /// </summary>
        public int? HttpClientPoolSize { get; set; }

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
