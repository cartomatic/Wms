using System;
using Cartomatic.Utils.Dto;
using Cartomatic.Utils.Serialization;

namespace Cartomatic.Wms.TileCache
{

    public partial class TileScheme
    {
        public int Precision { get; set; }

        /// <summary>
        /// scheme name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// number of zoom levels
        /// </summary>
        public int NumZoomLevels { get; set; }

        /// <summary>
        /// tileset resolution at 0 level - pixel size in projection units
        /// </summary>
        public decimal? MaxResolution { get; set; }

        /// <summary>
        /// tile size in pixels
        /// </summary>
        public int TileSize { get; set; }

        /// <summary>
        /// level 0 tile matrix width
        /// </summary>
        public int TileSetBaseWidth { get; set; }

        /// <summary>
        /// level 0 tile matrix height
        /// </summary>
        public int TileSetBaseHeight { get; set; }

        /// <summary>
        /// Tileset bounds
        /// </summary>
        public Bounds TileSetBounds { get; set; }


        /// <summary>
        /// reverse y means that the y indexing should start from the top not from the bottom.
        /// </summary>
        public bool ReverseY { get; set; }

        /// <summary>
        /// An array of tileset's resolutions
        /// </summary>
        public decimal[] Resolutions { get; set; }

        /// <summary>
        /// flagged when tilescheme has been prepared for calculations
        /// </summary>
        public bool Ready { get; private set; }
       
    }
}
