using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms.TileCache
{
    public class Tile
    {
        public Tile() {}

        /// <summary>
        /// whether or not a tile fits within a tile scheme flag used by the tileset checkup procedures
        /// </summary>
        public bool Valid { get; set; }

        public Bounds Bounds { get; set; }
        public TileAddress Address { get; set; }
    }
}
